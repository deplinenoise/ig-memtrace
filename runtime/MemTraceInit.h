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

// MemTraceInit.h
//
// Initialization code to start MemTrace as early as possible, before any CRT
// allocations are done ideally. This is highly compiler specific. Consider
// these two examples as inspiration.

#include "MemTrace.h"

#if defined(MEMTRACE_ENABLE)

#include <string.h>

#if defined(MEMTRACE_USE_TEMP_FILE)
// Platform agnostic and uses a temporary file.
// Always starts tracing to the temporary file.
// Application code should check command line options itself and either call
// MemTrace::InitSocket() (to upgrade to a socket connection) or 
// MemTrace::Shutdown() to abandon the temporary file
struct MemTraceInitObject
{
  MemTraceInitObject()
  {
    MemTrace::InitFile("memtrace.tmp");
  }

  ~MemTraceInitObject()
  {
    MemTrace::Shutdown();
  }
};

MemTraceInitObject __attribute__((init_priority(101))) g_MemTraceInit;

#elif defined(__ORBIS__)
// Example startup handler for Orbis. Checks for -memtrace <ip> on the command line.
struct MemTraceInitObject
{
  MemTraceInitObject()
  {
    int argc = getargc();
    char** argv= getargv();
    for (int i = 0; i < argc; ++i)
    {
      if (0 == strcmp("-memtrace", argv[i]) && i + 1 < argc)
      {
        MemTrace::InitSocket(argv[i + 1], 9811);
        break;
      }
    }
  }

  ~MemTraceInitObject()
  {
    MemTrace::Shutdown();
  }
};
MemTraceInitObject __attribute__((init_priority(101))) g_MemTraceInit;

#elif defined(_MSC_VER)

#include <Windows.h>
#include <string.h>

// Example startup handler for Windows. Checks for -memtrace <ip> on the command line.
struct MemTraceInitObject
{
  MemTraceInitObject()
  {
    const wchar_t* cmdline = GetCommandLineW();
    static const wchar_t prefix[] = L"-memtrace ";
    if (const wchar_t* arg = wcsstr(cmdline, prefix))
    {
      wchar_t ip_addr_u[64];
      wcsncpy_s(ip_addr_u, sizeof(ip_addr_u)/sizeof(ip_addr_u[0]), arg + (sizeof(prefix)/sizeof(prefix[0])) - 1, _TRUNCATE);
      if (wchar_t* space = wcschr(ip_addr_u, L' '))
        *space = L'\0';

      char ip_addr[64];
      wcstombs_s( NULL, ip_addr, sizeof(ip_addr), ip_addr_u, _TRUNCATE );

      MemTrace::InitSocket(ip_addr, 9811);
    }
  }

  ~MemTraceInitObject()
  {
    MemTrace::Shutdown();
  }
};

#pragma warning(disable:4074) 
#pragma init_seg(compiler)
MemTraceInitObject g_MemTraceInit;

#elif defined(MEMTRACE_USE_ARGC_ARGV)
// Platform agnostic, but assumes we can get to __argc and __argv before static initializers.
// Checks for -memtrace <ip> on the command line.
struct MemTraceInitObject
{
  MemTraceInitObject()
  {
    extern char **__argv;
    extern int __argc;
    int argc = __argc;
    char** argv= __argv;
    for (int i = 0; i < argc; ++i)
    {
      if (0 == strcmp("-memtrace", argv[i]) && i + 1 < argc)
      {
        MemTrace::InitSocket(argv[i + 1], 9811);
        break;
      }
    }
  }

  ~MemTraceInitObject()
  {
    MemTrace::Shutdown();
  }
};

MemTraceInitObject __attribute__((init_priority(101))) g_MemTraceInit;

#else
#error Sorry, no compatible init protocol for this platform. Make your own!
#endif

#endif
