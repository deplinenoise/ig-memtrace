/*
Copyright (c) 2015, Insomniac Games All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

Redistributions of source code must retain the above copyright notice, this
list of conditions and the following disclaimer.

Redistributions in binary form must reproduce the above copyright notice, this
list of conditions and the following disclaimer in the documentation and/or
other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

#include "MemTrace.h"
#include "MemTraceSys.h"
#include <assert.h>
#include <string.h>
#include <stdarg.h>
#include <stdio.h>

#if defined(MEMTRACE_UNIX)
#include <sys/time.h>
#include <arpa/inet.h>
#include <pthread.h>
#include <fcntl.h>
#include <unistd.h>
#endif

#if defined(MEMTRACE_MAC)
#include <execinfo.h>   // for backtrace(3)
#endif

#if defined(MEMTRACE_WINDOWS)
#include <psapi.h>
#include <minhook.h>
#include <winsock2.h>
#include <ws2tcpip.h>
#endif

#if MEMTRACE_ENABLE

#if defined(MEMTRACE_WINDOWS)
#pragma comment(lib, "ws2_32")
#endif

// Choice of hash function is arbitrary, nothing is assumed across the wire
// about this function.  It's used purely to avoid resending data on the client
// side. But it should be a good hash function, we don't store string data to be
// able to re-verify a conflict for performance/memory reasons.
static uint64_t Fnv1A_64(const char* str)
{
  uint64_t hash = 14695981039346656037ull;
  while (uint8_t ch = (uint8_t) *str++)
  {
    hash ^= ch;
    hash *= 1099511628211;
  }
  return hash;
}

static const uint32_t kCrtHeapId = 0;

// We can't use Printf/printf() in general because they're not initialized yet.
// Vsnprintf() is OK because it doesn't allocate.
static void MemTracePrint(const char* fmt, ...)
{
  using namespace MemTrace;

  char msg[1024];
  va_list args;
  va_start(args, fmt);
  Vsnprintf(msg, sizeof msg, fmt, args);
  va_end(args);
  OutputDebugStringA(msg);
}

namespace MemTrace
{
  //-----------------------------------------------------------------------------
  // Various limits
  enum
  {
    kBufferSize     = 32768,
    kMaxStrings     = 1024,   // Max string hashes to keep around
    kMaxStacks      = 1024,   // Max call stack hashes to keep around
    kMaxFrames      = 256,    // Max frames in single callstack - needs to be large to capture all of it
  };

  // Start of stream protocol value - to handle version changes without crashing decoder.
  static const uint32_t kStreamMagic = 0xbfaf0003;

  //-----------------------------------------------------------------------------
  // Event codes that can be sent to the output stream.
  enum EventCode : uint8_t
  {
    kBeginStream     = 1,
    kEndStream,
    kModuleDump,
    kMark,

    kAddressAllocate = 10,
    kAddressFree,
    kVirtualCommit,
    kVirtualDecommit,

    kPhysicalAllocate,
    kPhysicalFree,
    kPhysicalMap,
    kPhysicalUnmap,

    kHeapCreate,
    kHeapDestroy,
    kHeapAddCore,
    kHeapRemoveCore,
    kHeapAllocate,
    kHeapReallocate,
    kHeapFree,
  };

  //-----------------------------------------------------------------------------
  // Statistics
  static struct {
    uint32_t        m_StringCount;
    uint32_t        m_ReusedStringCount;
    uint32_t        m_StackCount;
    uint32_t        m_ReusedStackCount;
  } s_Stats;

  //-----------------------------------------------------------------------------
  // Per-thread current scope tracker.
  THREAD_LOCAL_STORAGE static struct
  {
    ScopeKind m_Kind;
    const char* m_String;
  } s_Scope;

  //-----------------------------------------------------------------------------
  // Platform-dependent routine to walk stack and generate a backtrace
  static int GetBackTrace(uint64_t frames[], uint64_t *hash_out, int skip_levels);

  //-----------------------------------------------------------------------------
  // Minimal hash table mapping 64-bit numbers to 64-bit numbers.
  // Used exclusively by the slot cache. 
  //
  template <size_t MaxCount>
  struct FixedHash64
  {
    enum
    {
      kArraySize = MaxCount * 2,
      kArrayMask = kArraySize - 1   // Power of 2
    };

    int       m_Count;

    uint64_t  m_Keys[kArraySize];
    uint64_t  m_Values[kArraySize];

    void Init()
    {
      m_Count = 0;
      memset(m_Keys, 0, sizeof m_Keys);
    }

    uint64_t* Find(uint64_t key)
    {
      uint32_t index = uint32_t(key & kArrayMask);
      // Because we never fill the table entirely this loop is guaranteed to terminate.
      for (;;)
      {
        const uint64_t candidate = m_Keys[index];

        if (candidate == key)
        {
          return &m_Values[index];
        }
        else if (candidate == 0)
        {
          return nullptr;
        }

        index = (index + 1) & kArrayMask;
      }
    }

    void Insert(uint64_t key, uint64_t value)
    {
      // Client code guarantees that the thing is not in the table, so no need
      // to look. Just insert it into the first open slot.

      uint32_t index = uint32_t(key & kArrayMask);

      for (;;)
      {
        if (0 == m_Keys[index])
        {
          m_Keys[index] = key;
          m_Values[index] = value;
          return;
        }

        index = (index + 1) & kArrayMask;
      }
    }

    void Remove(uint64_t key)
    {
      uint32_t index = uint32_t(key & kArrayMask);

      while (m_Keys[index] && m_Keys[index] != key)
      {
        index = (index + 1) & kArrayMask;
      }

      if (m_Keys[index] != key)
      {
        return;
      }

      m_Keys[index] = 0;
      m_Values[index] = 0;

      // Move following items that may have landed there due to collisions.
      uint32_t src_index = (index + 1) & kArrayMask;

      for (;;)
      {
        uint64_t k = m_Keys[src_index];

        if (!k)
        {
          // Stop moving if the slot is unused
          break;
        }
        else if ((k & kArrayMask) != src_index)
        {
          // Skip things that are in the right place.

          uint64_t bv = m_Values[src_index];

          m_Keys[src_index]   = 0;
          m_Values[src_index] = 0;

          Insert(k, bv);

          index               = src_index;
        }
        src_index           = (src_index + 1) & kArrayMask;
      }
    }
  };

  //-----------------------------------------------------------------------------
  // An O(1) windowing lookup structure for compression purposes.
  // The idea is to keep a window of hashes of previously seen things and their indices.
  // This is used for both strings and stack backtraces.
  template <size_t MaxCount>
  class SlotCache
  {
  private:
    uint64_t              m_SequenceNumber;   // Current sequence number
    uint64_t              m_Hashes[MaxCount]; // Cyclic buffer of previous hashes
    FixedHash64<MaxCount> m_Table;

  public:
    void Init()
    {
      m_SequenceNumber = 0;
      m_Table.Init();
    }

  public:
    bool Find(uint64_t hash, uint64_t* seqno_out)
    {
      if (const uint64_t *seqno = m_Table.Find(hash))
      {
        // We know about this thing; just emit its old sequence number.
        // The decoder will reuse the old value.
        *seqno_out = *seqno;
        return true;
      }

      // Pick next sequence number
      const uint64_t seqno = m_SequenceNumber++;

      // Compute lookup index.
      const uint64_t index = seqno & (MaxCount - 1);

      // Drop the oldest thing from the lookup (if we're reusing it)
      if (uint64_t old_hash = m_Hashes[index])
      {
        m_Table.Remove(old_hash);
      }

      // Insert new hash
      m_Hashes[index] = hash;
      m_Table.Insert(hash, seqno);
      *seqno_out = seqno;
      return false;
    }
  };

  //-----------------------------------------------------------------------------
  // Type of callback to transmit an encoded block of event data to output stream
  typedef void (TransmitBlockFn)(const void* block, size_t size_bytes);

  //-----------------------------------------------------------------------------
  // Local functions.

 // Refresh loaded modules, send a module dump event if changed.
  void    RefreshLoadedModules();
  // As above, but avoids taking the lock.
  void    RefreshLoadedModulesUnlocked();

  // Common init routine.
  static void InitCommon(TransmitBlockFn* fn);
  // Panic shutdown
  static void ErrorShutdown();
  // Hook CRT allocators.
  static void HookCrt();

  //-----------------------------------------------------------------------------
  // Encodes integers and strings using variable-length encoding and windowing
  // to compress the outgoing data.
  class Encoder
  {
  private:
    size_t                        m_WriteOffset;                    // Write offset within current buffer
    TransmitBlockFn*              m_TransmitFn;                     // Function to transmit (partially) filled blocks
    uint64_t                      m_StartTime;                      // System timer for initial event. We use a delta to generate smaller numbers.

    SlotCache<kMaxStrings>        m_Strings;                        // Window of recently sent strings
    SlotCache<kMaxStacks>         m_Stacks;                         // Window of recently sent backtraces
    int                           m_CurBuffer;                      // Index of current encoding buffer
    uint8_t                       m_Buffers[2][kBufferSize];        // Raw encoding buffers

  private:
    //-----------------------------------------------------------------------------
    // Flush current buffer and flip buffers.
    void TransmitCurrentBuffer()
    {
      (*m_TransmitFn)(m_Buffers[m_CurBuffer], m_WriteOffset);

      // Flip buffers.
      m_WriteOffset = 0;
      m_CurBuffer  ^= 1;
    }

    //-----------------------------------------------------------------------------
    // Reserve space for <size> bytes. Can cause a buffer flipflush.
    uint8_t* Reserve(size_t size)
    {
      ASSERT_FATAL(size < kBufferSize, "Block size too small for reservation");
      uint8_t *base = m_Buffers[m_CurBuffer];

      if (size + m_WriteOffset > kBufferSize)
      {
        TransmitCurrentBuffer();

        base          = m_Buffers[m_CurBuffer];
      }

      return base + m_WriteOffset;
    }

    //-----------------------------------------------------------------------------
    // Commit <size> bytes. At least <size> bytes must have been previously reserved via Reserve()
    void Commit(size_t size)
    {
      m_WriteOffset += size;
    }

    //-----------------------------------------------------------------------------
    // Emit a relative time stamp to the stream.
    void EmitTimeStamp()
    {
      uint64_t t = TimerGetSystemCounter();
      uint64_t delta = t - m_StartTime;
      EmitUnsigned(delta);
    }

  public:
    //-----------------------------------------------------------------------------
    // Initialize the encoder with a function that writes encoded blocks to some output device.
    void Init(TransmitBlockFn *transmit_fn)
    {
      m_WriteOffset  = 0;
      m_TransmitFn   = transmit_fn;
      m_StartTime    = TimerGetSystemCounter();

      m_Strings.Init();
      m_Stacks.Init();
    }

    //-----------------------------------------------------------------------------
    // Swap out the transmit function (for file->socket switcharoo)
    void SetTransmitFn(TransmitBlockFn* fn)
    {
      m_TransmitFn = fn;
    }

    //-----------------------------------------------------------------------------
    // Flush any pending data to the output device.
    void Flush()
    {
      TransmitCurrentBuffer();
      // Immediately sync async write.
      (*m_TransmitFn)(NULL, 0);
    }

  public:
    //-----------------------------------------------------------------------------
    // Encode a 64-bit integer to the output stream using an encoding that favors small numbers.
    //
    // Encoding used:
    // - There is 7 bits of payload per byte, leading to a worst case of 10 bytes to store a 64-bit number
    // - Decoding proceeds as long as the MSB is zero.
    void EmitUnsigned(uint64_t val)
    {
      uint8_t* out  = Reserve(10);
      uint8_t byte;
      size_t   i    = 0;

      do
      {
        byte     = (uint8_t) (val & 0x7f);
        out[i++] = byte;
        val    >>= 7;
      } while (val);

      out[i-1] = byte | 0x80;

      Commit(i);
    }

    //-----------------------------------------------------------------------------
    // Emit a pointer to the output stream.
    void EmitPointer(const void* ptr)
    {
      EmitUnsigned(uintptr_t(ptr));
    }

    //-----------------------------------------------------------------------------
    // Emit a string to the output stream.
    void EmitString(const char* str)
    {
      if (!str)
        str = "(null)";

      s_Stats.m_StringCount++;

      const uint64_t hash  = Fnv1A_64(str);
      uint64_t       seqno;

      const bool     reuse = m_Strings.Find(hash, &seqno);

      EmitUnsigned(seqno);

      if (reuse)
      {
        s_Stats.m_ReusedStringCount++;
        return;
      }

      const size_t   len   = strlen(str);
      EmitUnsigned(len);
      memcpy(Reserve(len), str, len);
      Commit(len);
    }

    //-----------------------------------------------------------------------------
    // Emit a stack back trace to the output stream
    void EmitCallStack(int levels_to_skip)
    {
      s_Stats.m_StackCount++;

      uint64_t frames[kMaxFrames];
      uint64_t stack_hash;
      uint64_t seqno;

      const int  count = GetBackTrace(frames, &stack_hash, levels_to_skip);
      const bool reuse = m_Stacks.Find(stack_hash, &seqno);

      if (count == kMaxFrames)
      {
        MemTracePrint("MemTrace: Error: Call stack too deep; bump kMaxFrames\n");
        ErrorShutdown();
      }

      EmitUnsigned(seqno);

      if (reuse)
      {
        s_Stats.m_ReusedStackCount++;
        return;
      }

      EmitUnsigned(count);

      for (int i = 0; i < count; ++i)
      {
        EmitUnsigned(frames[i]);
      }
    }

    //-----------------------------------------------------------------------------
    // Emit common data that goes with every event.  
    void BeginEvent(EventCode code)
    {
      EmitUnsigned(code);
      EmitUnsigned(s_Scope.m_Kind);
      if (s_Scope.m_Kind != kScopeNone)
        EmitString(s_Scope.m_String);
      EmitTimeStamp();
      EmitCallStack(2);
    }
  };

  //-----------------------------------------------------------------------------
  // Subsystem global state
  struct
  {
    int             m_Active;                     // Non-zero if the system is active
    Encoder         m_Encoder;                    // Encoder
    uint32_t        m_NextHeapId;                 // Next free heap ID
    CriticalSection m_Lock;                       // Synchronizes access to encoder/stream
    int64_t         m_LastModuleRefreshTime;      // Timer used to periodically refresh list of modules and send a module dump
    uint64_t        m_ModuleChecksum;             // Checksum of previous module dump to avoid retransmitting redundant information

    FileHandle      m_BootFile;
    SOCKET          m_Socket;                     // Output socket during normal operation
    char            m_BootFileName[128];

  } S;
}

#if defined(MEMTRACE_WINDOWS)
//-----------------------------------------------------------------------------
// Wrapper functions for malloc-like functions MS likes to use in their own DLLs.

static void* (*Original_calloc)(size_t _Count, size_t _Size);
static void* Wrapped_calloc(size_t _Count, size_t _Size)
{
  void* result = (*Original_calloc)(_Count, _Size);
  MemTrace::HeapAllocate(kCrtHeapId, result, _Count * _Size);
  return result;
}

static void (*Original_free)(void* _Memory);
static void Wrapped_free(void* _Memory)
{
  MemTrace::HeapFree(kCrtHeapId, _Memory);
  (*Original_free)(_Memory);
}

static void* (*Original_malloc)(size_t _Size);
static void* Wrapped_malloc(size_t _Size)
{
  void* result = (*Original_malloc)(_Size);
  MemTrace::HeapAllocate(kCrtHeapId, result, _Size);
  return result;
}

static void TraceCrtRealloc(void* oldp, void* newp, size_t newsize)
{
  if (newp)
  {
    if (oldp)
      MemTrace::HeapReallocate(kCrtHeapId, oldp, newp, newsize);
    else
      MemTrace::HeapAllocate(kCrtHeapId, newp, newsize);
  }
  else if (0 == newsize)
  {
    MemTrace::HeapFree(kCrtHeapId, oldp);
  }
}

static void* (*Original_realloc)(void* _Memory, size_t _NewSize);
static void* Wrapped_realloc(void* _Memory, size_t _NewSize)
{
  void *result = (*Original_realloc)(_Memory, _NewSize);
  TraceCrtRealloc(_Memory, result, _NewSize);
  return result;
}

static void* (*Original__recalloc)(void * _Memory, size_t _Count, size_t _Size);
static void* Wrapped__recalloc(void * _Memory, size_t _Count, size_t _Size)
{
  void* result = (*Original__recalloc)(_Memory, _Count, _Size);
  TraceCrtRealloc(_Memory, result, _Count * _Size);
  return result;
}

static void (*Original__aligned_free)(void * _Memory);
static void Wrapped__aligned_free(void * _Memory)
{
  MemTrace::HeapFree(kCrtHeapId, _Memory);
  (*Original__aligned_free)(_Memory);
}

static void* (*Original__aligned_malloc)(size_t _Size, size_t _Alignment);
static void* Wrapped__aligned_malloc(size_t _Size, size_t _Alignment)
{
  void* result = (*Original__aligned_malloc)(_Size, _Alignment);
  MemTrace::HeapAllocate(kCrtHeapId, result, _Size);
  return result;
}

static void* (*Original__aligned_offset_malloc)(size_t _Size, size_t _Alignment, size_t _Offset);
static void* Wrapped__aligned_offset_malloc(size_t _Size, size_t _Alignment, size_t _Offset)
{
  void* result = (*Original__aligned_offset_malloc)(_Size, _Alignment, _Offset);
  MemTrace::HeapAllocate(kCrtHeapId, result, _Size);
  return result;
}

static void* (*Original__aligned_realloc)(void* _Memory, size_t _NewSize, size_t _Alignment);
static void* Wrapped__aligned_realloc(void* _Memory, size_t _NewSize, size_t _Alignment)
{
  void* result = (*Original__aligned_realloc)(_Memory, _NewSize, _Alignment);
  TraceCrtRealloc(_Memory, result, _NewSize);
  return result;
}

static void* (*Original__aligned_recalloc)(void* _Memory, size_t _Count, size_t _Size, size_t _Alignment);
static void* Wrapped__aligned_recalloc(void* _Memory, size_t _Count, size_t _Size, size_t _Alignment)
{
  void* result = (*Original__aligned_recalloc)(_Memory, _Count, _Size, _Alignment);
  TraceCrtRealloc(_Memory, result, _Count * _Size);
  return result;
}

static void* (*Original__aligned_offset_realloc)(void* _Memory, size_t _NewSize, size_t _Alignment, size_t _Offset);
static void* Wrapped__aligned_offset_realloc(void* _Memory, size_t _NewSize, size_t _Alignment, size_t _Offset)
{
  void* result = (*Original__aligned_offset_realloc)(_Memory, _NewSize, _Alignment, _Offset);
  TraceCrtRealloc(_Memory, result, _NewSize);
  return result;
}

static void* (*Original__aligned_offset_recalloc)(void* _Memory, size_t _Count, size_t _Size, size_t _Alignment, size_t _Offset);
static void* Wrapped__aligned_offset_recalloc(void* _Memory, size_t _Count, size_t _Size, size_t _Alignment, size_t _Offset)
{
  void* result = (*Original__aligned_offset_recalloc)(_Memory, _Count, _Size, _Alignment, _Offset);
  TraceCrtRealloc(_Memory, result, _Count * _Size);
  return result;
}

#endif

//-----------------------------------------------------------------------------
// Dummy to force initialization object to exist in top-level executable
  void MemTrace::DummyInitFunction(char)
  {
  }

//-----------------------------------------------------------------------------
// Common init routine for first time setup

static void MemTrace::InitCommon(TransmitBlockFn* write_block_fn)
{
  if (S.m_Active)
  {
#if defined(MEMTRACE_WINDOWS)
    DebugBreak();
#else
    abort();
#endif
  }

  S.m_Active     = 1;
  S.m_Socket     = INVALID_SOCKET;
  S.m_NextHeapId = kCrtHeapId;

  S.m_Lock.Init();
  S.m_Encoder.Init(write_block_fn);

  S.m_Encoder.BeginEvent(kBeginStream);
  S.m_Encoder.EmitUnsigned(kStreamMagic);
  S.m_Encoder.EmitString(kPlatformName);
  S.m_Encoder.EmitUnsigned(sizeof(void*));
  S.m_Encoder.EmitUnsigned(TimerGetSystemFrequencyInt());
  S.m_Encoder.EmitPointer((const void*) MemTrace::InitCommon);

  MemTrace::HeapCreate("CRT Heap"); // Will get id 0 in a side effecty kind of way.

  HookCrt();

  RefreshLoadedModules();
}

//-----------------------------------------------------------------------------
static void MemTrace::HookCrt()
{
  // @@@ If you are a licensed Durango dev, get in touch with us and we can
  // share a way to hook the CRT for Durango. NDA material.

#if defined(MEMTRACE_WINDOWS)
  // On Windows, dynamically hook the CRT allocation functions to route through memtrace.

  // Load minhook DLL
  if (HMODULE minhook_module = LoadLibraryA("MinHook.x64.dll"))
  {
    auto MH_Initialize_Func = (decltype(&MH_Initialize)) GetProcAddress(minhook_module, "MH_Initialize");
    auto MH_CreateHook_Func = (decltype(&MH_CreateHook)) GetProcAddress(minhook_module, "MH_CreateHook");
    auto MH_EnableHook_Func = (decltype(&MH_EnableHook)) GetProcAddress(minhook_module, "MH_EnableHook");

    if (!MH_Initialize_Func || !MH_CreateHook_Func || !MH_EnableHook_Func || MH_OK != (*MH_Initialize_Func)())
    {
      DebugBreak();
    }

#if _MSC_VER != 1700
#error This needs updating for the new CRT version. Talk to Andreas.
#endif

#if !defined(_DEBUG)

    if (HMODULE crt_module = GetModuleHandleA("msvcr110.dll"))
    {
#define IG_WRAP_FN(symbol) { #symbol, (void*) Wrapped_##symbol, (void**) &Original_##symbol }
      static const struct
      {
        const char* m_FunctionName;
        void*       m_Wrapper;
        void**      m_StockFunctionPtr;
      }
      wrapped_functions[] =
      {
        IG_WRAP_FN(calloc),
        IG_WRAP_FN(malloc),
        IG_WRAP_FN(free),
        IG_WRAP_FN(realloc),
        IG_WRAP_FN(_recalloc),
        IG_WRAP_FN(_aligned_malloc),
        IG_WRAP_FN(_aligned_free),
        IG_WRAP_FN(_aligned_realloc),
        IG_WRAP_FN(_aligned_recalloc),
        IG_WRAP_FN(_aligned_offset_malloc),
        IG_WRAP_FN(_aligned_offset_realloc),
        IG_WRAP_FN(_aligned_offset_recalloc),
      };
#undef IG_WRAP_FN

      for (int i = 0; i < ARRAY_SIZE(wrapped_functions); ++i)
      {
        if (void* target = GetProcAddress(crt_module, wrapped_functions[i].m_FunctionName))
        {
          (*MH_CreateHook_Func)(target, wrapped_functions[i].m_Wrapper, wrapped_functions[i].m_StockFunctionPtr);
        }
        else
        {
          MemTracePrint("Failed to hook '%s' - entry point not found\n", wrapped_functions[i].m_FunctionName);
        }
      }

      MH_STATUS status;

      if (MH_OK != (status = (*MH_EnableHook_Func)(MH_ALL_HOOKS)))
      {
        MemTracePrint("CRT hooking failed: %08x\n", (uint32_t) status);
      }
    }
#else
    MemTracePrint("WARNING: CRT hooking in Debug builds not yet supported\n");
#endif

    // NOTE: minhook.x64.dll left mapped on purpose.
  }
  else
  {
    MemTracePrint("WARNING: Failed to load MinHook.x64.dll - CRT allocations will not be captured\n");
  }
#endif
}

//-----------------------------------------------------------------------------
void MemTrace::InitFile(const char* trace_temp_file)
{
  FileHandle hf = FileOpenForReadWrite(trace_temp_file);

  if (hf == kInvalidFileHandle)
  {
    MemTracePrint("MemTrace: Failed to open %s for writing, disabling system\n", trace_temp_file);
    return;
  }

  // Stash the boot filename so we can delete it later.
  Strcpy(S.m_BootFileName, ARRAY_SIZE(S.m_BootFileName), trace_temp_file);

  S.m_BootFile = hf;

  // Callback that dumps event buffer data using async writes to our socket.
  auto write_block_fn = [](const void* block, size_t size) -> void
  {
    FileWrite(S.m_BootFile, block, size);
  };

  InitCommon(write_block_fn);
}

//-----------------------------------------------------------------------------
void MemTrace::InitSocket(const char *server_ip_address, int server_port)
{
  bool error = false;

  // Remember if we were already active; if we were we need to protect against memory allocations
  // on other threads trying to trace while we're switching protocols.
  const int was_active = S.m_Active;

  if (was_active)
  {
    S.m_Lock.Enter();
  }

#if defined(MEMTRACE_WINDOWS)
  WSADATA wsa_data;
  WSAStartup(MAKEWORD(2, 2), &wsa_data);
#endif

  sockaddr_in address;
  memset(&address, 0, sizeof address);

#if defined(MEMTRACE_WINDOWS)
  InetPtonA(AF_INET, server_ip_address, &address.sin_addr);
#else
  inet_pton(AF_INET, server_ip_address, &address.sin_addr);
#endif


  address.sin_family = AF_INET;
  address.sin_port = htons((u_short) server_port);

  // Connect to the server.
  SOCKET sock = socket(address.sin_family, SOCK_STREAM, IPPROTO_TCP);
  if (INVALID_SOCKET == sock)
  {
    MemTracePrint("MemTrace: Failed to create socket\n");
    ErrorShutdown();
    return;
  }

  if (0 != connect(sock, (struct sockaddr*) &address, sizeof address))
  {
    MemTracePrint("MemTrace: Failed to connect to %s (port %d)- is the server running?\n", server_ip_address, ntohs(address.sin_port));
    ErrorShutdown();
    return;
  }

  // Set send buffer size appropriately to avoid blocking needlessly.
  int sndbufsize = 4 * kBufferSize;
  if (0 != setsockopt(sock, SOL_SOCKET, SO_SNDBUF, (char*) &sndbufsize, sizeof sndbufsize))
  {
    MemTracePrint("MemTrace: Warning: Couldn't set send buffer size to %d bytes\n", sndbufsize);
  }

  auto write_block_fn = [](const void* block, size_t size) -> void
  {
    // If we don't have a socket, we drop everything on the floor.
    if (INVALID_SOCKET == S.m_Socket)
      return;

    if (size != send(S.m_Socket, (const char*) block, (int) size, 0))
    {
      MemTracePrint("MemTrace: send() failed - shutting down\n");
      MemTrace::ErrorShutdown();
    }
  };

  if (!was_active)
  {
    InitCommon(write_block_fn);
    S.m_Socket = sock;
  }
  else
  {
    S.m_Socket = sock;
    MemTracePrint("MemTrace: Switching to socket transport\n");
    S.m_Encoder.Flush();

    FileHandle fh = S.m_BootFile;

    if (int64_t sz = FileSize(fh))
    {
      FileSeekTo(fh, 0);

      char buf[1024];
      size_t remain = (size_t) sz;
      while (remain)
      {
        size_t copy_size = remain;
        if (copy_size > ARRAY_SIZE(buf))
          copy_size = ARRAY_SIZE(buf);

        FileRead(fh, buf, copy_size);
        if (copy_size != send(sock, buf, (int) copy_size, 0))
        {
          MemTracePrint("send() failed while uploading trace file, shutting down.\n");
          error = true;
        }

        remain -= copy_size;
      }
    }

    FileClose(fh);
    S.m_BootFile = kInvalidFileHandle;

    // Clean up the temporary file.
#if defined(MEMTRACE_WINDOWS)
    DeleteFileA(S.m_BootFileName);
#else
    remove(S.m_BootFileName);
#endif

    // Switch to socket transmit method
    S.m_Encoder.SetTransmitFn(write_block_fn);
  }

  if (was_active)
  {
    S.m_Lock.Leave();
  }

  if (error)
  {
    ErrorShutdown();
  }
}

void MemTrace::ErrorShutdown()
{
  int was_active = S.m_Active;

  if (was_active)
    S.m_Lock.Enter();

  if (S.m_BootFile != kInvalidFileHandle)
  {
    FileClose(S.m_BootFile);
    S.m_BootFile = kInvalidFileHandle;

#if defined(MEMTRACE_WINDOWS)
    if (S.m_BootFileName[0])
    {
      DeleteFileA(S.m_BootFileName);
    }
#endif
  }

  if (S.m_Socket != INVALID_SOCKET)
  {
    closesocket(S.m_Socket);
    S.m_Socket = INVALID_SOCKET;
  }

  S.m_Active = 0;

  if (was_active)
    S.m_Lock.Leave();
}

void MemTrace::Flush()
{
  if (!S.m_Active)
    return;

  CSAutoLock lock(S.m_Lock);

  S.m_Encoder.Flush();
}

void MemTrace::Shutdown()
{
  if (!S.m_Active)
    return;

  S.m_Lock.Enter();

  S.m_Encoder.BeginEvent(kEndStream);

  MemTracePrint("MemTrace: Shutting down..\n");

  // There's a tiny chance of a race on shutdown here, but it's small enough
  // that it shouldn't be a real problem. The race is:
  // 1. Thread checks m_Active, finds 1, is timesliced before taking the lock
  // 2. Main thread calls Shutdown(), destroying everything
  // 3. Thread resumes
  S.m_Active = 0;

  // Flush and shut down writer.
  S.m_Encoder.Flush();

  closesocket(S.m_Socket);

  MemTracePrint("MemTrace: %u strings written, of which %u were reused\n", s_Stats.m_StringCount, s_Stats.m_ReusedStringCount);
  MemTracePrint("MemTrace: %u stacks written, of which %u were reused\n", s_Stats.m_StackCount, s_Stats.m_ReusedStackCount);

  S.m_Lock.Leave();
  S.m_Lock.Destroy();
}


void MemTrace::UserMark(const char* label, ...)
{
  if (!S.m_Active)
    return;

  va_list args;
  char buffer[256];
  va_start(args, label);
  Vsnprintf(buffer, sizeof buffer, label, args);
  va_end(args);

  CSAutoLock lock(S.m_Lock);

  S.m_Encoder.BeginEvent(kMark);
  S.m_Encoder.EmitString(buffer);
}

void MemTrace::AddressAllocate(
    const void* base, size_t size_bytes, const char* name)
{
  if (!S.m_Active)
    return;

  CSAutoLock lock(S.m_Lock);

  S.m_Encoder.BeginEvent(kAddressAllocate);
  S.m_Encoder.EmitPointer(base);
  S.m_Encoder.EmitUnsigned(size_bytes);
  S.m_Encoder.EmitString(name);
}

void MemTrace::AddressFree(const void* base)
{
  if (!S.m_Active || base == NULL)
    return;

  CSAutoLock lock(S.m_Lock);

  S.m_Encoder.BeginEvent(kAddressFree);
  S.m_Encoder.EmitPointer(base);
}

void MemTrace::VirtualCommit(const void* base, size_t size_bytes)
{
  if (!S.m_Active)
    return;

  CSAutoLock lock(S.m_Lock);

  S.m_Encoder.BeginEvent(kVirtualCommit);
  S.m_Encoder.EmitPointer(base);
  S.m_Encoder.EmitUnsigned(size_bytes);
}

void MemTrace::VirtualDecommit(const void* base, size_t size_bytes)
{
  if (!S.m_Active)
    return;

  CSAutoLock lock(S.m_Lock);

  S.m_Encoder.BeginEvent(kVirtualDecommit);
  S.m_Encoder.EmitPointer(base);
  S.m_Encoder.EmitUnsigned(size_bytes);
}

MemTrace::HeapId MemTrace::HeapCreate(const char* name)
{
  if (!S.m_Active)
    return ~0u;

  CSAutoLock lock(S.m_Lock);

  HeapId id = S.m_NextHeapId++;

  S.m_Encoder.BeginEvent(kHeapCreate);
  S.m_Encoder.EmitUnsigned(id);
  S.m_Encoder.EmitString(name);

  return id;
}

void MemTrace::HeapDestroy(HeapId heap_id)
{
  if (!S.m_Active)
    return;

  CSAutoLock lock(S.m_Lock);

  S.m_Encoder.BeginEvent(kHeapDestroy);
  S.m_Encoder.EmitUnsigned(heap_id);
}

void MemTrace::HeapAddCore(HeapId heap_id, const void* base, size_t size_bytes)
{
  if (!S.m_Active)
    return;

  CSAutoLock lock(S.m_Lock);

  S.m_Encoder.BeginEvent(kHeapAddCore);
  S.m_Encoder.EmitUnsigned(heap_id);
  S.m_Encoder.EmitPointer(base);
  S.m_Encoder.EmitUnsigned(size_bytes);
}

void MemTrace::HeapRemoveCore(HeapId heap_id, const void* base, size_t size_bytes)
{
  if (!S.m_Active)
    return;

  CSAutoLock lock(S.m_Lock);

  S.m_Encoder.BeginEvent(kHeapRemoveCore);
  S.m_Encoder.EmitUnsigned(heap_id);
  S.m_Encoder.EmitPointer(base);
  S.m_Encoder.EmitUnsigned(size_bytes);
}

void MemTrace::HeapAllocate(HeapId id, const void* ptr, size_t size_bytes)
{
  if (!S.m_Active)
    return;

  CSAutoLock lock(S.m_Lock);

  RefreshLoadedModulesUnlocked();

  S.m_Encoder.BeginEvent(kHeapAllocate);
  S.m_Encoder.EmitUnsigned(id);
  S.m_Encoder.EmitPointer(ptr);
  S.m_Encoder.EmitUnsigned(size_bytes);
}

void MemTrace::HeapReallocate(HeapId id, const void* ptr_in, const void* ptr_out, size_t new_size_bytes)
{
  if (!S.m_Active)
    return;

  CSAutoLock lock(S.m_Lock);

  RefreshLoadedModulesUnlocked();

  S.m_Encoder.BeginEvent(kHeapReallocate);
  S.m_Encoder.EmitUnsigned(id);
  S.m_Encoder.EmitPointer(ptr_in);
  S.m_Encoder.EmitPointer(ptr_out);
  S.m_Encoder.EmitUnsigned(new_size_bytes);
}

void MemTrace::HeapFree(HeapId id, const void* ptr)
{
  if (!S.m_Active)
    return;

  CSAutoLock lock(S.m_Lock);

  RefreshLoadedModulesUnlocked();

  S.m_Encoder.BeginEvent(kHeapFree);
  S.m_Encoder.EmitUnsigned(id);
  S.m_Encoder.EmitPointer(ptr);
}

void MemTrace::PushScope(ScopeKind kind, const char* str, ScopeKind* old_kind, const char** old_str)
{
  *old_kind = s_Scope.m_Kind;
  *old_str = s_Scope.m_String;
  s_Scope.m_Kind = kind;
  s_Scope.m_String = str;
}

void MemTrace::RestoreScope(ScopeKind kind, const char* str)
{
  s_Scope.m_Kind = kind;
  s_Scope.m_String = str;
}

void MemTrace::RefreshLoadedModulesUnlocked()
{
  if (TimerGetSystemDeltaInSeconds(S.m_LastModuleRefreshTime) < 10.0)
    return;

  S.m_LastModuleRefreshTime = TimerGetSystemCounter();

#if defined(MEMTRACE_WINDOWS)

  S.m_Encoder.BeginEvent(kModuleDump);

  HMODULE modules[1024];
  DWORD bytes_needed = 0;
  if (EnumProcessModules(GetCurrentProcess(), modules, sizeof modules, &bytes_needed))
  {
    size_t bytes = sizeof modules;
    if (bytes > bytes_needed)
      bytes = bytes_needed;
    size_t nmods = bytes / sizeof modules[0];
    for (size_t i = 0; i < nmods; ++i)
    {
      HMODULE mod = modules[i];
      MODULEINFO modinfo;
      if (GetModuleInformation(GetCurrentProcess(), mod, &modinfo, sizeof modinfo))
      {
        char modname[256];
        if (DWORD namelen = GetModuleFileNameExA(GetCurrentProcess(), mod, modname, sizeof modname))
        {
          S.m_Encoder.EmitUnsigned(1);
          S.m_Encoder.EmitString(modname);
          S.m_Encoder.EmitPointer(mod);
          S.m_Encoder.EmitUnsigned(modinfo.SizeOfImage);
        }
      }
    }
  }
  S.m_Encoder.EmitUnsigned(0);
#endif

  // @@@ If you're a licensed Durango dev we can provide module walking code.
  // NDA material.
}

void MemTrace::RefreshLoadedModules()
{
  if (!S.m_Active)
    return;

  CSAutoLock lock(S.m_Lock);

  RefreshLoadedModulesUnlocked();
}

#if defined(MEMTRACE_WINDOWS)
int MemTrace::GetBackTrace(uint64_t frames[], uint64_t *hash_out, int skip_levels)
{
  DWORD hash = 0;

  int count = RtlCaptureStackBackTrace(skip_levels, kMaxFrames, (void**) frames, &hash);

  // Make sure we don't hash to zero.
  *hash_out = (uint64_t(hash) << 1) | 1;

  return count;
}

#elif defined(MEMTRACE_MAC)

int MemTrace::GetBackTrace(uint64_t frames[], uint64_t *hash_out, int skip_levels)
{
  int count = backtrace((void**)frames, kMaxFrames);
  int skip_count = skip_levels > count ? count : skip_levels;
  uint64_t         hash = 0x0123456789abcdef;

  memmove(&frames[skip_levels], &frames[0], sizeof(frames[0]) * count - skip_count);
  count -= skip_count;

  // Compute a hash.
  for (int i = 0; i < count; ++i)
  {
    hash = ((hash << 51) | (hash >> 13)) ^ frames[i];
  }

  // Make sure we don't hash to zero.
  *hash_out = hash ? hash : 1;

  return count;
}

#else

// Manually walk the linked list of frames on the stack.  This depends on frame
// pointer omission not being disabled. (Which is the default on most
// compilers & platforms. You might want to do something different.)

int __attribute__((noinline)) MemTrace::GetBackTrace(uint64_t frames[], uint64_t *hash_out, int skip_levels)
{
  // Grab address of current frame.
  const uintptr_t* fp   = (const uintptr_t*) __builtin_frame_address(0);
  int              i    = -skip_levels;
  uint64_t         hash = 0x0123456789abcdef;

  // Deref current frame pointer (we'll start with the caller's frame)
  while (fp)
  {
    // The parent frame link is the first 64-bit word in the frame in memory order
    // The return address is the second 64-bit word in the frame in memory order (after the link).
    const uintptr_t parent_frame = fp[0];
    const uintptr_t ret_addr     = fp[1];

    // (If we're still skipping levels, skip over this part)
    if (i >= 0) {
      // Write return address to output array and update running hash
      frames[i] = ret_addr;
      hash      = ((hash << 51) | (hash >> 13)) ^ ret_addr;
    }

    // Bail if we've filled the output array completely (shouldn't happen ideally)
    if (++i == kMaxFrames)
      break;

    // Deref link to parent frame and loop.
    fp = (uintptr_t*) parent_frame;
  }

  // Make sure we don't hash to zero.
  *hash_out = hash ? hash : 1;

  return i > 0 ? i - 1 : 0;
}
#endif

#endif // MEMTRACE_ENABLE
