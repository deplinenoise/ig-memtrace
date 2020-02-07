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

#pragma once

#include <stdint.h>
#include <stddef.h>

// You can control this with a build macro if desired to compile MemTrace out.
#define MEMTRACE_ENABLE 1

namespace MemTrace
{
  typedef uint32_t HeapId;

  enum ScopeKind
  {
    kScopeNone,
    kScopeAsset,
    kScopeComponent
  };

#if MEMTRACE_ENABLE

  // Get connection parameters specified on the command line, if any (returns true)
  // Useful to forward memtrace configuration along to spawned child processes.
  bool    GetSocketData(char (&ip_addr_out)[128], int* port_out);

  void    InitFile(const char *trace_temp_file);

  void    InitSocket(const char *server_ip_address, int server_port);
  bool    SendSocket(const void* block, size_t size);

  void    Shutdown();

  void    UserMark(const char* label, ...);

  void    Flush();

  void    AddressAllocate(const void* base, size_t size_bytes, const char* name);
  void    AddressFree(const void* base);

  void    VirtualCommit(const void* base, size_t size_bytes);
  void    VirtualDecommit(const void* base, size_t size_bytes);

  HeapId  HeapCreate(const char* name);
  void    HeapDestroy(HeapId heap_id);
  void    HeapAddCore(HeapId heap_id, const void* base, size_t size_bytes);
  void    HeapRemoveCore(HeapId heap_id, const void* base, size_t size_bytes);
  void    HeapAllocate(HeapId heap_id, const void* ptr, size_t size_bytes);
  void    HeapReallocate(HeapId heap_id, const void* ptr_in, const void* ptr_out, size_t new_size_bytes);
  void    HeapFree(HeapId heap_id, const void* ptr);

  void    PushScope(ScopeKind kind, const char* str, ScopeKind* old_kind, const char** old_str);
  void    RestoreScope(ScopeKind kind, const char* str);

  class ScopeHelper
  {
    ScopeKind m_Kind;
    const char *m_String;

  public:
    ScopeHelper(ScopeKind kind, const char* str)
    {
      PushScope(kind, str, &m_Kind, &m_String);
    }

    ~ScopeHelper()
    {
      RestoreScope(m_Kind, m_String);
    }
  private:
    ScopeHelper(const ScopeHelper&);
    ScopeHelper& operator=(const ScopeHelper&);
  };

  void DummyInitFunction(char dummy);

#define MEMTRACE_SCOPE(kind, text) \
  ::MemTrace::ScopeHelper memtrace_scope_(kind, text)

#else

#define MEMTRACE_SCOPE(kind, text) \
  do {} while(0)

#endif

}
