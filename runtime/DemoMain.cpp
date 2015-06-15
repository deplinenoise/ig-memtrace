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
#include "MemTraceInit.h"

#include <vector>
#include <string>
#include <map>
#include <assert.h>

static void TestCRTAllocs()
{
  // This is ridiculous, but shows CRT allocating grabbing working.

  MemTrace::UserMark("CRT Test Starting");

  std::map<std::string, std::vector<std::string> > blah;    // Wee!!

  for (int i = 0; i < 512; ++i)
  {
    std::string key = "k";
    for (int k = 0; k < 10; ++k)
    {
      blah[key].push_back("Gah!");
      blah[key].push_back("This is for demo purposes to make sure we're capturing CRT allocations");

      key += "a";
    }
  }

  MemTrace::UserMark("CRT Test Ending");
}

class BlockAllocator
{
  struct Hdr
  {
    Hdr *m_Next;
  };

  char*             m_MemRange;
  Hdr*              m_FreeList;
  size_t            m_ElemSize;
  size_t            m_ElemCount;
  size_t            m_MemSize;
  MemTrace::HeapId  m_HeapId;

private:
  Hdr* GetHeader(size_t index)
  {
    char* ptr = m_MemRange + index * m_ElemSize;
    assert(ptr >= m_MemRange && ptr < m_MemRange + m_MemSize);
    return (Hdr*) ptr;
  }

public:
  BlockAllocator(size_t elem_size, size_t elem_count, const char* name)
  {
    assert(elem_size >= sizeof(char*));
    size_t mem_size = elem_size * elem_count;
    m_MemSize = mem_size;
    m_MemRange = (char*) malloc(mem_size);
    m_ElemSize = elem_size;
    m_ElemCount = elem_count;

    m_HeapId = MemTrace::HeapCreate(name);
    MemTrace::HeapAddCore(m_HeapId, m_MemRange, mem_size);

    for (size_t i = 0; i < elem_count - 1; ++i)
    {
      GetHeader(i)->m_Next = GetHeader(i + 1);
    }

    GetHeader(elem_count-1)->m_Next = nullptr;

    m_FreeList = GetHeader(0);
  }

  ~BlockAllocator()
  {
    MemTrace::HeapRemoveCore(m_HeapId, m_MemRange, m_ElemSize * m_ElemCount);
    MemTrace::HeapDestroy(m_HeapId);
    free(m_MemRange);
  }

  void* Alloc()
  {
    if (Hdr* hdr = m_FreeList)
    {
      m_FreeList = hdr->m_Next;
      MemTrace::HeapAllocate(m_HeapId, hdr, m_ElemSize);
      return hdr;
    }
    return nullptr;
  }

  void Free(void *ptr_)
  {
    MemTrace::HeapFree(m_HeapId, ptr_);
    Hdr* hdr = (Hdr*) ptr_;
    hdr->m_Next = m_FreeList;
    m_FreeList = hdr;
  }
};

static void TestCustomAllocator()
{
  MemTrace::UserMark("Custom Allocator Test Starting");

  BlockAllocator a(16, 256, "Allocator A");
  BlockAllocator b(32, 256, "Allocator B");

  for (int i = 0; i < 200; ++i)
  {
    if (i & 2) a.Alloc();
    if (i & 3) b.Alloc();
  }

  MemTrace::UserMark("Custom Allocator Test Ending");
}

int main(int argc, char* argv[])
{
  TestCRTAllocs();

  TestCustomAllocator();

  return 0;
}
