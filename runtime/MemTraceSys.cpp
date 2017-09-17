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

#include "MemTraceSys.h"

#if defined(MEMTRACE_UNIX)
#include <sys/time.h>
#include <sys/stat.h>
#include <arpa/inet.h>
#include <pthread.h>
#include <fcntl.h>
#include <unistd.h>
#include <stdio.h>
#include <string.h>
#endif

#if defined(MEMTRACE_WINDOWS)
#include <stdio.h>
#endif

#if defined(MEMTRACE_MAC)
#include <execinfo.h>   // for backtrace(3)
#endif

#if defined(MEMTRACE_MAC)
const char MemTrace::kPlatformName[] = "MacOS X";
#elif defined(MEMTRACE_FREEBSD)
const char MemTrace::kPlatformName[] = "FreeBSD";
#elif defined(MEMTRACE_LINUX)
const char MemTrace::kPlatformName[] = "Linux";
#elif defined(MEMTRACE_WINDOWS)
const char MemTrace::kPlatformName[] = "Windows";
#endif

void MemTrace::Strcpy(char* dest, size_t dest_size, const char* src)
{
  size_t capacity = dest_size - 1;
  while (capacity--)
  {
    char ch = *src++;
    if (!ch)
      break;
    *dest++ = ch;
  }
  *dest++ = '\0';
}

#if defined(MEMTRACE_UNIX)
int64_t MemTrace::TimerGetSystemCounter()
{
  struct timeval tv;
  gettimeofday(&tv, nullptr);
  return int64_t(tv.tv_sec) * 1000000 + tv.tv_usec;
}

int64_t MemTrace::TimerGetSystemFrequencyInt()
{
  return 1000000;
}

void MemTrace::closesocket(SOCKET s)
{
  close(s);
}

MemTrace::FileHandle MemTrace::FileOpenForReadWrite(const char* fn)
{
  return open(fn, O_RDWR|O_CREAT, 0666);
}

void MemTrace::FileWrite(FileHandle fh, const void* data, size_t size)
{
  write(fh, data, size);
}

void MemTrace::FileClose(FileHandle fh)
{
  close(fh);
}

void MemTrace::FileSeekTo(FileHandle fh, int64_t pos)
{
  lseek(fh, pos, SEEK_SET);
}

int64_t MemTrace::FileSize(FileHandle fh)
{
  struct stat st;
  fstat(fh, &st);
  return st.st_size;
}

void MemTrace::FileRead(FileHandle fh, void* buffer, size_t size)
{
  read(fh, buffer, size);
}

int MemTrace::Vsnprintf(char* buffer, size_t buffer_size, const char* fmt, va_list args)
{
  int rc = vsnprintf(buffer, buffer_size, fmt, args);

  if (rc > buffer_size)
    return (int) buffer_size;
  else
    return rc;
}

void MemTrace::OutputDebugStringA(const char* str)
{
  write(1, str, strlen(str));
}
#endif

#if defined(MEMTRACE_WINDOWS)
int64_t MemTrace::TimerGetSystemFrequencyInt()
{
  static int64_t s_TimerFrequency = 0;

  if (s_TimerFrequency != 0)
  {
    return s_TimerFrequency;
  }

  // Query the performance of the Windows high resolution timer.
  QueryPerformanceFrequency((LARGE_INTEGER*)&s_TimerFrequency);

  return s_TimerFrequency;
}

int64_t MemTrace::TimerGetSystemCounter()
{
  int64_t ticks;
  QueryPerformanceCounter((LARGE_INTEGER*)&ticks);
  return ticks;
}

MemTrace::FileHandle MemTrace::FileOpenForReadWrite(const char* fn)
{
  return CreateFileA(fn, FILE_GENERIC_WRITE, FILE_SHARE_READ, NULL, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
}

void MemTrace::FileWrite(FileHandle fh, const void* data, size_t size)
{
  ::WriteFile(fh, data, (DWORD) size, NULL, NULL);
}

void MemTrace::FileClose(FileHandle fh)
{
  ::CloseHandle(fh);
}

void MemTrace::FileSeekTo(FileHandle fh, int64_t pos)
{
  LARGE_INTEGER l;
  l.QuadPart = pos;
  SetFilePointerEx(fh, l, NULL, FILE_BEGIN);
}

int64_t MemTrace::FileSize(FileHandle fh)
{
  LARGE_INTEGER val;
  GetFileSizeEx(fh, &val);
  return val.QuadPart;
}

void MemTrace::FileRead(FileHandle fh, void* buffer, size_t size)
{
  DWORD bytes_read;
  ::ReadFile(fh, buffer, (DWORD) size, &bytes_read, NULL);
}

int MemTrace::Vsnprintf(char* buffer, size_t buffer_size, const char* fmt, va_list args)
{
  int rc = _vsnprintf(buffer, buffer_size, fmt, args);

  if (rc < 0)
    return (int) strlen(buffer);
  else
    return rc;
}

#endif
