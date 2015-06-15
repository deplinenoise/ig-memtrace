using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemTrace
{
  // Event codes - keep in sync with native code side.
  public enum EventCode
  {
    BeginStream = 1,
    EndStream,
    ModuleDump,
    Mark,

    AddressAllocate = 10,
    AddressFree,
    VirtualCommit,
    VirtualDecommit,

    PhysicalAllocate,
    PhysicalFree,
    PhysicalMap,
    PhysicalUnmap,

    HeapCreate,
    HeapDestroy,
    HeapAddCore,
    HeapRemoveCore,
    HeapAllocate,
    HeapReallocate,
    HeapFree,
  }
}
