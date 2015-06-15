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

//-----------------------------------------------------------------------------
// Try to auto-configure platform stuff as much as possible to reduce build
// system complexity for this public release.

#if defined(__APPLE__)
#define MEMTRACE_UNIX 1
#define MEMTRACE_MAC 1
#endif

#if defined(__FreeBSD__)
#define MEMTRACE_UNIX 1
#define MEMTRACE_FREEBSD 1
#endif

#if defined(linux)
#define MEMTRACE_UNIX 1
#define MEMTRACE_LINUX 1
#endif

#if defined(_WIN32) || defined(_MSC_VER)
#define MEMTRACE_WINDOWS 1
#ifndef WIN32_LEAN_AND_MEAN 
#define WIN32_LEAN_AND_MEAN 1
#endif
#ifndef _CRT_SECURE_NO_WARNINGS
#define _CRT_SECURE_NO_WARNINGS 1
#endif
#endif

//-----------------------------------------------------------------------------
// Common includes

#include <stdint.h>
#include <assert.h>
#include <stdarg.h>

//-----------------------------------------------------------------------------
// MemTraceSys.h
//
// Wrappers, helpers etc that support MemTrace. These are a "just what's
// needed" version of some of Insomniac's Foundation libraries.


//-----------------------------------------------------------------------------
// Macros for thread local storage, asserts and other misc stuff

#if defined(MEMTRACE_UNIX)
#define THREAD_LOCAL_STORAGE __thread
#define ALIGN_TO(n) __attribute__((aligned(n)))
#define ASSERT_FATAL(expr, msg, ...) assert(expr && msg)
#endif

#if defined(MEMTRACE_WINDOWS)
#define THREAD_LOCAL_STORAGE __declspec(thread)
#define ALIGN_TO(n) __declspec((align(n)))
#define ASSERT_FATAL(expr, msg, ...) assert(expr && msg)
#endif

#define ARRAY_SIZE(expr) ((sizeof(expr))/sizeof(expr[0]))

//-----------------------------------------------------------------------------
// System includes needed for wrappers in this file.

#if defined(MEMTRACE_WINDOWS)
#include <Windows.h>
#endif

#if defined(MEMTRACE_UNIX)
#include <pthread.h>
#endif

namespace MemTrace
{
  extern const char kPlatformName[];

  int64_t TimerGetSystemCounter();
  int64_t TimerGetSystemFrequencyInt();

  inline double TimerGetSystemDeltaInSeconds(int64_t t0)
  {
    int64_t t1 = TimerGetSystemCounter();
    int64_t diff = t1 - t0;
    return diff / double(TimerGetSystemFrequencyInt());
  }

  void Strcpy(char* dest, size_t dest_size, const char* src);

  // Critical section wrapper
#if defined(MEMTRACE_UNIX)
  class CriticalSection
  {
    pthread_mutex_t m_Mutex;

  public:
    void Init()
    {
      pthread_mutex_init(&m_Mutex, nullptr);
    }

    void Destroy()
    {
      pthread_mutex_destroy(&m_Mutex);
    }

    void Enter()
    {
      pthread_mutex_lock(&m_Mutex);
    }

    void Leave()
    {
      pthread_mutex_unlock(&m_Mutex);
    }
  };
#elif defined(MEMTRACE_WINDOWS)
  class CriticalSection
  {
    CRITICAL_SECTION m_Mutex;

  public:
    void Init()
    {
      InitializeCriticalSection(&m_Mutex);
    }

    void Destroy()
    {
      DeleteCriticalSection(&m_Mutex);
    }

    void Enter()
    {
      EnterCriticalSection(&m_Mutex);
    }

    void Leave()
    {
      LeaveCriticalSection(&m_Mutex);
    }
  };

#endif

  class CSAutoLock
  {
    CriticalSection& m_CS;

  public:
    explicit CSAutoLock(CriticalSection& cs) : m_CS(cs)
    {
      cs.Enter();
    }

    ~CSAutoLock()
    {
      m_CS.Leave();
    }

  private:
    CSAutoLock(const CSAutoLock&);
    CSAutoLock& operator=(const CSAutoLock&);
  };


#if defined(MEMTRACE_UNIX)
  typedef int SOCKET;
  typedef int FileHandle;

  static const FileHandle kInvalidFileHandle = -1;
  static const SOCKET INVALID_SOCKET = -1;

  // For API compatibility with WinSock
  void closesocket(SOCKET s);

  // For API compatibility
  void OutputDebugStringA(const char* str);
#endif

#if defined(MEMTRACE_WINDOWS)
  // Use WinSock's idea of SOCKET and INVALID_SOCKET
  typedef HANDLE FileHandle;
  static const FileHandle kInvalidFileHandle = INVALID_HANDLE_VALUE;
#endif

  //-----------------------------------------------------------------------------
  // File handling. Note that these HAVE to be implemented in terms of OS
  // primitives that don't allocate any user memory, or MemTrace will end up in
  // endless recursion.

  FileHandle FileOpenForReadWrite(const char* fn);

  void FileWrite(FileHandle fh, const void* data, size_t size);

  void FileClose(FileHandle fh);

  //-----------------------------------------------------------------------------
  // String formatting with sane return value.
  int Vsnprintf(char* buffer, size_t buffer_size, const char* fmt, va_list args);
}
