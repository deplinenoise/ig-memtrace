using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Diagnostics;

namespace MemTrace
{
  public enum TraceStatus
  {
    Ready,
    Recording
  }

  public struct AddressRange : IEquatable<AddressRange>
  {
    public ulong BaseAddress;
    public ulong SizeBytes;

    public override int GetHashCode()
    {
      return (int) ((BaseAddress * 13) ^ SizeBytes);
    }

    public bool Equals(AddressRange other)
    {
      return other.BaseAddress == this.BaseAddress && other.SizeBytes == this.SizeBytes;
    }
  };

  public sealed class HeapInfo
  {
    private List<AddressRange> m_Ranges = new List<AddressRange>();

    public string Name { get; private set; }
    public IReadOnlyCollection<AddressRange> CoreRanges { get { return m_Ranges; } }

    public HeapInfo(string name)
    {
      Name = name;
    }

    internal void AddCore(ulong start, ulong size)
    {
      m_Ranges.Add(new AddressRange { BaseAddress = start, SizeBytes = size });
    }

    internal void RemoveCore(ulong start, ulong size)
    {
      // This is lame and doesn't handle partial ranges.
      for (int i = 0; i < m_Ranges.Count; ++i)
      {
        if (m_Ranges[i].BaseAddress == start)
        {
          if (m_Ranges[i].SizeBytes == size)
          {
            m_Ranges.RemoveAt(i);
            return;
          }
        }
      }

      throw new IOException(String.Format("Couldn't find Core range {0}->{1} in heap {2}", start, size, Name));
    }

    public bool Contains(ulong addr)
    {
      // TODO: Use binary search lower bound
      for (int i = 0; i < m_Ranges.Count; ++i)
      {
        ulong begin = m_Ranges[i].BaseAddress;
        ulong end = begin + m_Ranges[i].SizeBytes;

        if (addr >= begin && addr < end)
        {
          return true;
        }
      }

      return false;
    }
  }

  public sealed class StackBackTrace
  {
    public IReadOnlyList<SymbolInfo> Frames { get { return m_Frames; } }

    private List<SymbolInfo> m_Frames;

    internal StackBackTrace(List<SymbolInfo> frames)
    {
      m_Frames = frames;
    }
  }

  internal struct EventHeader
  {
    // These fields are assigned through a memory map read, so we suppress the warning about them never being assigned.
#pragma warning disable 649
    private int _code;
    public int BackLink;
    public double TimeStamp;
    public int Scope;
    public int ScopeStringIndex;
    public int BackTraceIndex;
#pragma warning restore 649

    public EventCode Code { get { return (EventCode) _code; } }

    public const int SizeBytes = 28;

    public static void Decode(out EventHeader h, MemoryMappedViewAccessor view, ref long pos_ref)
    {
      long pos = pos_ref;
      view.Read<EventHeader>(pos, out h);
      pos_ref = pos + SizeBytes;
    }
  }

  internal struct AllocateAddressEvent
  {
    public ulong Address;
    public ulong Size;
    public int NameStringIndex;

    public static void Decode(out AllocateAddressEvent ev, MemoryMappedViewAccessor view, ref long pos)
    {
      ev.Address = view.ReadUInt64(pos);
      ev.Size = view.ReadUInt64(pos + 8);
      ev.NameStringIndex = view.ReadInt32(pos + 16);
      pos += 20;
    }
  }

  internal struct FreeAddressEvent
  {
    public ulong Address;
    public long SourceEventPos;

    public static void Decode(out FreeAddressEvent ev, MemoryMappedViewAccessor view, ref long pos)
    {
      ev.Address = view.ReadUInt64(pos);
      ev.SourceEventPos = view.ReadInt64(pos + 8);
      pos += 16;
    }
  }

  internal struct VirtualCommitEvent
  {
    public ulong Address;
    public ulong Size;

    public static void Decode(out VirtualCommitEvent ev, MemoryMappedViewAccessor view, ref long pos)
    {
      ev.Address = view.ReadUInt64(pos);
      ev.Size = view.ReadUInt64(pos + 8);
      pos += 16;
    }
  }

  internal struct HeapAllocEvent
  {
    public ulong Address;
    public ulong Size;
    public int HeapId;

    private static void DecodeCommon(out HeapAllocEvent ev, MemoryMappedViewAccessor view, long pos)
    {
      ev.HeapId = -1;
      ev.Address = view.ReadUInt64(pos);
      ev.Size = view.ReadUInt64(pos + 8);
    }

    public static void DecodeV3(out HeapAllocEvent ev, MemoryMappedViewAccessor view, ref long pos)
    {
      int heapId = view.ReadInt32(pos);
      DecodeCommon(out ev, view, pos + 4);
      ev.HeapId = heapId;
      pos += 20;
    }

    public static void DecodePreV3(out HeapAllocEvent ev, MemoryMappedViewAccessor view, ref long pos)
    {
      DecodeCommon(out ev, view, pos);
      pos += 16;
    }
  }

  internal struct HeapReallocEvent
  {
    public ulong OldAddress;
    public ulong NewAddress;
    public ulong NewSize;
    public long SourceEventPos;
    public int HeapId;

    private static void DecodeCommon(out HeapReallocEvent ev, MemoryMappedViewAccessor view, long pos)
    {
      ev.HeapId = -1;
      ev.OldAddress = view.ReadUInt64(pos);
      ev.NewAddress = view.ReadUInt64(pos + 8);
      ev.NewSize = view.ReadUInt64(pos + 16);
      ev.SourceEventPos = view.ReadInt64(pos + 8);
    }

    public static void DecodeV3(out HeapReallocEvent ev, MemoryMappedViewAccessor view, ref long pos)
    {
      int heapId = view.ReadInt32(pos);
      DecodeCommon(out ev, view, pos + 4);
      ev.HeapId = heapId;
      pos += 36;
    }

    public static void DecodePreV3(out HeapReallocEvent ev, MemoryMappedViewAccessor view, ref long pos)
    {
      DecodeCommon(out ev, view, pos);
      pos += 32;
    }
  }

  internal struct HeapFreeEvent
  {
    public ulong Address;
    public long SourceEventPos;
    public int HeapId;

    private static void DecodeCommon(out HeapFreeEvent ev, MemoryMappedViewAccessor view, long pos)
    {
      ev.HeapId = -1;
      ev.Address = view.ReadUInt64(pos);
      ev.SourceEventPos = view.ReadInt64(pos + 8);
    }

    public static void DecodeV3(out HeapFreeEvent ev, MemoryMappedViewAccessor view, ref long pos)
    {
      int heapId = view.ReadInt32(pos);
      DecodeCommon(out ev, view, pos + 4);
      ev.HeapId = heapId;
      pos += 20;
    }

    public static void DecodePreV3(out HeapFreeEvent ev, MemoryMappedViewAccessor view, ref long pos)
    {
      DecodeCommon(out ev, view, pos);
      pos += 16;
    }
  }

  internal struct HeapCreateEvent
  {
    public ulong Id;
    public int NameStringIndex;

    public static void Decode(out HeapCreateEvent ev, MemoryMappedViewAccessor view, ref long pos)
    {
      ev.Id = view.ReadUInt64(pos);
      ev.NameStringIndex = view.ReadInt32(pos + 8);
      pos += 12;
    }
  }

  internal struct HeapDestroyEvent
  {
    public ulong Id;
    public long SourceEventPos;

    public static void Decode(out HeapDestroyEvent ev, MemoryMappedViewAccessor view, ref long pos)
    {
      ev.Id = view.ReadUInt64(pos);
      ev.SourceEventPos = view.ReadInt64(pos + 8);
      pos += 16;
    }
  }

  internal struct HeapCoreEvent
  {
    public ulong Id;
    public ulong Address;
    public ulong Size;

    public static void Decode(out HeapCoreEvent ev, MemoryMappedViewAccessor view, ref long pos)
    {
      ev.Id = view.ReadUInt64(pos);
      ev.Address = view.ReadUInt64(pos + 8);
      ev.Size = view.ReadUInt64(pos + 16);
      pos += 24;
    }
  }


  public interface IMemEventHandler
  {
    void OnProgress(double ratio);
    void OnHeapAllocate(ulong ptr, ulong size, int scope_type, string scope_data_str, StackBackTrace backtrace, double time);
    void OnHeapFree(ulong ptr, StackBackTrace backtrace, double time);
  }

  public struct HeapAllocationInfo
  {
    public sealed class StartComparer : IComparer<HeapAllocationInfo>
    {
      public int Compare(HeapAllocationInfo x, HeapAllocationInfo y)
      {
        if (x.Address < y.Address)
          return -1;
        else if (x.Address > y.Address)
          return 1;
        return 0;
      }
    }

    public sealed class EndComparer : IComparer<HeapAllocationInfo>
    {
      public int Compare(HeapAllocationInfo x, HeapAllocationInfo y)
      {
        ulong xa = x.Address + x.SizeBytes;
        ulong ya = y.Address + y.SizeBytes;
        if (xa < ya)
          return -1;
        else if (xa > ya)
          return 1;
        return 0;
      }
    }

    public int ScopeType { get; internal set; }
    public string ScopeData { get; internal set; }
    public HeapInfo Heap { get; internal set; }
    public ulong Address { get; internal set; }
    public ulong SizeBytes { get; internal set; }
    public StackBackTrace BackTrace { get; internal set; }
    public double TimeCreated { get; internal set; }

    public static HeapAllocationInfo CreateSearchKey(ulong addr)
    {
      return new HeapAllocationInfo { Address = addr };
    }
  }

  /// <summary>
  /// Replay a trace file, exposing its data in an event-by-event fashion to some higher-level system.
  /// </summary>
  public abstract class TraceProcessorBase : IDisposable
  {
    public delegate void StatusDelegate(double ratio);

    /// <summary>
    ///  The file being played back.
    /// </summary>
    public string FileName { get; private set; }

    /// <summary>
    /// Metadata about the trace file.
    /// </summary>
    public TraceMeta MetaData { get; private set; }

    private MemoryMappedFile m_MmapFile;
    private MemoryMappedViewAccessor m_MmapView;

    // Current offset in the memory mapped file.
    private long m_MmapPos;
    // Offset of last valid decoded event.
    private long m_MmapPrevPos;
    private long m_MmapStringTablePos;
    private long m_MmapStackFrameTablePos;

    public double CurrentTime { get { return m_CurrentTime; } }

    private double m_CurrentTime;

    /// <summary>
    /// Construct a trace replay.
    /// </summary>
    /// <param name="fn">The filename of the trace to play back</param>
    /// <exception cref="IOException">If anything is wrong with the file.</exception>
    protected TraceProcessorBase(string fn)
    {
      FileName = fn;
      MetaData = new TraceMeta();
      MetaData.Load(fn);

      // Don't mmap the file right away, do so lazily to avoid locking the file handle for updates in other parts of the UI.
    }

    static int mapNo = 0;

    private void EnsureMappedFile()
    {
      if (m_MmapFile != null)
        return;

      FileStream f = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
      m_MmapFile = MemoryMappedFile.CreateFromFile(f, String.Format("Mmap{0}", mapNo++), 0, MemoryMappedFileAccess.Read, null, HandleInheritability.None, false);
      m_MmapView = m_MmapFile.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);

      // Decode all strings and call stacks
      m_MmapStringTablePos = m_MmapView.ReadInt64(0);
      m_MmapStackFrameTablePos = m_MmapView.ReadInt64(8);
      m_MmapPos = 16;
      m_MmapPrevPos = -1;
    }

    ~TraceProcessorBase()
    {
      Dispose(false);
    }

    public void Dispose()
    {
      Dispose(true);
    }

    private void Dispose(bool disposing)
    {
      if (disposing)
        GC.SuppressFinalize(this);

      if (m_MmapView != null)
        m_MmapView.Dispose();

      if (m_MmapFile != null)
        m_MmapFile.Dispose();
    }

    protected virtual void OnBeginSeek(double secs)
    {
    }

    protected virtual void OnEndSeek(double secs)
    {
    }

    protected void SeekImpl(double secs, StatusDelegate status_callback)
    {
      EnsureMappedFile();

      double start = m_CurrentTime;
      double end = secs;

      int i = 0;

      OnBeginSeek(secs);

      while (secs > m_CurrentTime)
      {
        if (0 == (i & 1023) && status_callback != null)
        {
          double pct = (m_CurrentTime - start) / (end - start);
          status_callback(Math.Min(pct, 1.0));
        }
        if (!NextEvent())
          break;
        ++i;
      }
      while (secs < m_CurrentTime)
      {
        if (0 == (i & 1023) && status_callback != null)
        {
          double pct = (m_CurrentTime - start) / (end - start);
          status_callback(Math.Min(pct, 1.0));
        }
        if (!PrevEvent())
          break;
        ++i;
      }

      OnEndSeek(secs);
    }

    protected bool NextEvent()
    {
      var v = m_MmapView;
      var pos = m_MmapPos;

      if (pos == MetaData.EncodedDataSizeBytes)
        return false;

      m_MmapPrevPos = pos;

      EventHeader header;
      EventHeader.Decode(out header, v, ref pos);

      bool hasHeapIds = MetaData.Version >= 3;

      m_CurrentTime = header.TimeStamp;

      switch (header.Code)
      {
        case EventCode.HeapCreate:
          {
            HeapCreateEvent ev;
            HeapCreateEvent.Decode(out ev, v, ref pos);
            OnHeapCreate(ev.Id, ev.NameStringIndex);
          }
          break;

        case EventCode.HeapDestroy:
          {
            HeapDestroyEvent ev;
            HeapDestroyEvent.Decode(out ev, v, ref pos);
            OnHeapDestroy(ev.Id);
          }
          break;

        case EventCode.HeapAddCore:
        case EventCode.HeapRemoveCore:
          {
            HeapCoreEvent ev;
            HeapCoreEvent.Decode(out ev, v, ref pos);
            if (EventCode.HeapAddCore == header.Code)
              OnHeapAddCore(ev.Id, ev.Address, ev.Size);
            else
              OnHeapRemoveCore(ev.Id, ev.Address, ev.Size);
          }
          break;

        case EventCode.HeapAllocate:
          {
            HeapAllocEvent ev;
            if (hasHeapIds)
              HeapAllocEvent.DecodeV3(out ev, v, ref pos);
            else
              HeapAllocEvent.DecodePreV3(out ev, v, ref pos);
            OnHeapAllocate(ev.HeapId, ev.Address, ev.Size, header.Scope, header.ScopeStringIndex, header.BackTraceIndex, header.TimeStamp);
          }
          break;

        case EventCode.HeapFree:
          {
            HeapFreeEvent ev;
            if (hasHeapIds)
              HeapFreeEvent.DecodeV3(out ev, v, ref pos);
            else
              HeapFreeEvent.DecodePreV3(out ev, v, ref pos);
            OnHeapFree(ev.HeapId, ev.Address, header.BackTraceIndex, header.TimeStamp);
          }
          break;

        case EventCode.HeapReallocate:
          {
            HeapReallocEvent ev;
            if (hasHeapIds)
              HeapReallocEvent.DecodeV3(out ev, v, ref pos);
            else
              HeapReallocEvent.DecodePreV3(out ev, v, ref pos);
            OnHeapFree(ev.HeapId, ev.OldAddress, header.BackTraceIndex, header.TimeStamp);
            OnHeapAllocate(ev.HeapId, ev.NewAddress, ev.NewSize, header.Scope, header.ScopeStringIndex, header.BackTraceIndex, header.TimeStamp);
          }
          break;

        case EventCode.AddressAllocate:
          {
            AllocateAddressEvent ev;
            AllocateAddressEvent.Decode(out ev, v, ref pos);
            OnAddressAllocate(ev.Address, ev.Size, ev.NameStringIndex);
          }
          break;

        case EventCode.AddressFree:
          {
            FreeAddressEvent ev;
            FreeAddressEvent.Decode(out ev, v, ref pos);
            OnAddressFree(ev.Address);
          }
          break;

        case EventCode.VirtualCommit:
        case EventCode.VirtualDecommit:
          {
            VirtualCommitEvent ev;
            VirtualCommitEvent.Decode(out ev, v, ref pos);
            if (header.Code == EventCode.VirtualCommit)
              OnVirtualCommit(ev.Address, ev.Size);
            else
              OnVirtualDecommit(ev.Address, ev.Size);
          }
          break;

        default:
          throw new IOException(String.Format("Unexpected event code {0} in stream at position {1}", header.Code, m_MmapPos));
      }

      m_MmapPos = pos;
      return true;
    }

    protected bool PrevEvent()
    {
      var v = m_MmapView;

      if (m_MmapPos == MetaData.EncodedDataSizeBytes)
        m_MmapPos = m_MmapPrevPos;

      var pos = m_MmapPos;
      var inpos = pos;

      if (pos == 16)
        return false;

      bool isV3 = MetaData.Version >= 3;

      EventHeader header;
      EventHeader.Decode(out header, v, ref pos);

      m_CurrentTime = header.TimeStamp;

      switch (header.Code)
      {
        case EventCode.HeapCreate:
          {
            HeapCreateEvent ev;
            HeapCreateEvent.Decode(out ev, v, ref pos);
            OnHeapDestroy(ev.Id);
          }
          break;

        case EventCode.HeapDestroy:
          {
            HeapDestroyEvent ev;
            HeapDestroyEvent.Decode(out ev, v, ref pos);

            HeapCreateEvent create_ev;
            long create_pos = ev.SourceEventPos + EventHeader.SizeBytes;
            HeapCreateEvent.Decode(out create_ev, v, ref create_pos);
            OnHeapCreate(create_ev.Id, create_ev.NameStringIndex);
          }
          break;

        case EventCode.HeapAddCore:
        case EventCode.HeapRemoveCore:
          {
            HeapCoreEvent ev;
            HeapCoreEvent.Decode(out ev, v, ref pos);
            if (EventCode.HeapRemoveCore == header.Code)
              OnHeapAddCore(ev.Id, ev.Address, ev.Size);
            else
              OnHeapRemoveCore(ev.Id, ev.Address, ev.Size);
          }
          break;

        case EventCode.HeapAllocate:
          {
            HeapAllocEvent ev;
            if (isV3)
              HeapAllocEvent.DecodeV3(out ev, v, ref pos);
            else
              HeapAllocEvent.DecodePreV3(out ev, v, ref pos);
            OnHeapFree(ev.HeapId, ev.Address, -1, 0.0); // args are bogus, we don't have that data
          }
          break;

        case EventCode.HeapFree:
          {
            HeapFreeEvent ev;
            if (isV3)
              HeapFreeEvent.DecodeV3(out ev, v, ref pos);
            else
              HeapFreeEvent.DecodePreV3(out ev, v, ref pos);

            long alloc_pos = ev.SourceEventPos;
            EventHeader alloc_hdr;
            EventHeader.Decode(out alloc_hdr, v, ref alloc_pos);

            HeapAllocEvent alloc_ev;
            if (isV3)
              HeapAllocEvent.DecodeV3(out alloc_ev, v, ref alloc_pos);
            else
              HeapAllocEvent.DecodePreV3(out alloc_ev, v, ref alloc_pos);


            OnHeapAllocate(alloc_ev.HeapId, alloc_ev.Address, alloc_ev.Size, alloc_hdr.Scope, alloc_hdr.ScopeStringIndex, alloc_hdr.BackTraceIndex, alloc_hdr.TimeStamp);
          }
          break;

        case EventCode.HeapReallocate:
          {
            HeapReallocEvent ev;
            if (isV3)
              HeapReallocEvent.DecodeV3(out ev, v, ref pos);
            else
              HeapReallocEvent.DecodePreV3(out ev, v, ref pos);

            // Restore preceding alloc which can have been either an alloc or another reallocation
            long old_pos = ev.SourceEventPos;
            EventHeader old_hdr;
            EventHeader.Decode(out old_hdr, v, ref old_pos);

            // Undo realloc allocation part
            OnHeapFree(ev.HeapId, ev.NewAddress, old_hdr.BackTraceIndex, old_hdr.TimeStamp);

            switch (old_hdr.Code)
            {
              case EventCode.HeapAllocate:
                {
                  HeapAllocEvent alloc_ev;
                  if (isV3)
                    HeapAllocEvent.DecodeV3(out alloc_ev, v, ref old_pos);
                  else
                    HeapAllocEvent.DecodePreV3(out alloc_ev, v, ref old_pos);
                  OnHeapAllocate(alloc_ev.HeapId, alloc_ev.Address, alloc_ev.Size, old_hdr.Scope, old_hdr.ScopeStringIndex, old_hdr.BackTraceIndex, old_hdr.TimeStamp);
                }
                break;

              case EventCode.HeapReallocate:
                {
                  HeapReallocEvent realloc_ev;
                  if (isV3)
                    HeapReallocEvent.DecodeV3(out realloc_ev, v, ref old_pos);
                  else
                    HeapReallocEvent.DecodePreV3(out realloc_ev, v, ref old_pos);
                  OnHeapAllocate(realloc_ev.HeapId, realloc_ev.NewAddress, realloc_ev.NewSize, old_hdr.Scope, old_hdr.ScopeStringIndex, old_hdr.BackTraceIndex, old_hdr.TimeStamp);
                }
                break;

              default:
                throw new IOException("Unexpected source event for reallocation " + old_hdr.Code);
            }
          }
          break;

        case EventCode.AddressAllocate:
          {
            AllocateAddressEvent ev;
            AllocateAddressEvent.Decode(out ev, v, ref pos);
            OnAddressFree(ev.Address);
          }
          break;

        case EventCode.AddressFree:
          {
            FreeAddressEvent ev;
            FreeAddressEvent.Decode(out ev, v, ref pos);

            long alloc_pos = ev.SourceEventPos + EventHeader.SizeBytes;
            AllocateAddressEvent alloc_ev;
            AllocateAddressEvent.Decode(out alloc_ev, v, ref alloc_pos);

            OnAddressAllocate(alloc_ev.Address, alloc_ev.Size, alloc_ev.NameStringIndex);
            break;
          }
        case EventCode.VirtualCommit:
          {
            VirtualCommitEvent ev;
            VirtualCommitEvent.Decode(out ev, v, ref pos);
            OnVirtualDecommit(ev.Address, ev.Size);
          }
          break;

        case EventCode.VirtualDecommit:
          {
            VirtualCommitEvent ev;
            VirtualCommitEvent.Decode(out ev, v, ref pos);
            OnVirtualCommit(ev.Address, ev.Size);
          }
          break;

        default:
          throw new IOException(String.Format("Unexpected event code {0} in stream at position {1}", header.Code, m_MmapPos));
      }

      m_MmapPos = inpos - header.BackLink;
      return true;
    }

    protected abstract void OnVirtualDecommit(ulong ptr, ulong size);
    protected abstract void OnVirtualCommit(ulong ptr, ulong size);
    protected abstract void OnAddressFree(ulong ptr);
    protected abstract void OnAddressAllocate(ulong ptr, ulong size, int name_index);

    protected abstract void OnHeapFree(int heapId, ulong ptr, int stack_index, double time);
    protected abstract void OnHeapAllocate(int heapId, ulong ptr, ulong size, int scope_type, int scope_data_str, int stack_index, double time);

    protected abstract void OnHeapRemoveCore(ulong heap_id, ulong addr, ulong size);
    protected abstract void OnHeapAddCore(ulong heap_id, ulong addr, ulong size);

    protected abstract void OnHeapCreate(ulong id, int name_si);
    protected abstract void OnHeapDestroy(ulong id);


    private List<string> m_StringCache = new List<string>();
    private List<StackBackTrace> m_BackTraceCache = new List<StackBackTrace>();

    public string GetStringByIndex(int string_index)
    {
      if (string_index == -1)
        return null;

      if (string_index < m_StringCache.Count)
      {
        var s = m_StringCache[string_index];
        if (s != null)
          return s;
      }

      while (m_StringCache.Count < string_index + 1)
        m_StringCache.Add(null);

      long tab_idx = m_MmapStringTablePos + (long)string_index * 8;

      long str_data_pos = m_MmapStringTablePos + m_MmapView.ReadUInt32(tab_idx);
      int str_len = m_MmapView.ReadInt32(tab_idx + 4);

      byte[] data = new byte[str_len];
      m_MmapView.ReadArray(str_data_pos, data, 0, str_len);

      var result = Encoding.UTF8.GetString(data);

      m_StringCache[string_index] = result;

      return result;
    }

    public StackBackTrace GetBackTraceByIndex(int stack_index)
    {
      if (stack_index < m_BackTraceCache.Count)
      {
        var s = m_BackTraceCache[stack_index];
        if (s != null)
          return s;
      }

      // Set up a new back trace stack.
      while (m_BackTraceCache.Count < stack_index + 1)
        m_BackTraceCache.Add(null);

      long tab_idx = m_MmapStackFrameTablePos + (long)stack_index * 8;
      long stk_pos = m_MmapStackFrameTablePos + m_MmapView.ReadUInt32(tab_idx);
      int stk_count = m_MmapView.ReadInt32(tab_idx + 4);

      ulong[] frames = new ulong[stk_count];
      m_MmapView.ReadArray(stk_pos, frames, 0, stk_count);

      var syms = MetaData.ResolvedSymbols;

      var result = new List<SymbolInfo>();
      foreach (var addr in frames)
      {
        SymbolInfo s;
        if (syms.TryGetValue(addr, out s))
        {
          result.Add(s);
        }
        else
        {
          result.Add(new SymbolInfo { Address = addr, FileName = "", LineNumber = -1, Symbol = String.Format("{0:x16}", addr) });
        }
      }

      var wrap = new StackBackTrace(result);
      m_BackTraceCache[stack_index] = wrap;

      return wrap;
    }

    public void UpdateResolvedSymbols(Dictionary<ulong, SymbolInfo> result)
    {
      MetaData.ResolvedSymbols = result;
      MetaData.Update(FileName);
    }

    public void ReplayEvents(IMemEventHandler handler)
    {
      throw new NotImplementedException();
    }
  }

  public sealed class TraceReplayStateful : TraceProcessorBase
  {
    private Dictionary<ulong, HeapInfo> m_Heaps = new Dictionary<ulong, HeapInfo>();
    private Dictionary<AllocationKey, HeapAllocationInfo> m_Allocs = new Dictionary<AllocationKey, HeapAllocationInfo>();

    public IReadOnlyDictionary<ulong, HeapInfo> Heaps { get { return m_Heaps; } }
    public IReadOnlyDictionary<AllocationKey, HeapAllocationInfo> HeapAllocations { get { return m_Allocs; } }

    public List<HeapAllocationInfo> AllocationsByAddress { get { return m_SortedAllocs; } }
    public List<HeapAllocationInfo> m_SortedAllocs = new List<HeapAllocationInfo>();

    public TraceReplayStateful(string fn)
      : base(fn)
    {
    }

    protected override void OnVirtualDecommit(ulong ptr, ulong size)
    {
    }

    protected override void OnVirtualCommit(ulong ptr, ulong size)
    {
    }

    protected override void OnAddressFree(ulong ptr)
    {
    }

    protected override void OnAddressAllocate(ulong ptr, ulong size, int name_index)
    {
    }

    protected override void OnHeapFree(int heapId, ulong ptr, int stack_index, double time)
    {
      m_Allocs.Remove(new AllocationKey { HeapId = (ulong) heapId, Address = ptr });
    }

    protected override void OnHeapAllocate(int heapId, ulong ptr, ulong size, int scope_type, int scope_data_str, int stack_index, double time)
    {
      var heap = FindHeap(heapId, ptr);
      var key = new AllocationKey { HeapId = (ulong)heapId, Address = ptr };
      m_Allocs[key] = new HeapAllocationInfo
      {
        ScopeType = scope_type,
        ScopeData = GetStringByIndex(scope_data_str),
        Heap = heap,
        Address = ptr,
        SizeBytes = size,
        BackTrace = GetBackTraceByIndex(stack_index),
        TimeCreated = time,
      };
    }

    private HeapInfo FindHeap(int heap_id, ulong ptr)
    {
      HeapInfo heap;
      if (m_Heaps.TryGetValue((ulong)heap_id, out heap))
      {
        return heap;
      }

      // Speed this up.
      foreach (var h in m_Heaps.Values)
      {
        if (h.Contains(ptr))
          return h;
      }

      return null;
    }

    protected override void OnHeapRemoveCore(ulong heap_id, ulong addr, ulong size)
    {
      m_Heaps[heap_id].RemoveCore(addr, size);
    }

    protected override void OnHeapAddCore(ulong heap_id, ulong addr, ulong size)
    {
      m_Heaps[heap_id].AddCore(addr, size);
    }

    protected override void OnHeapCreate(ulong id, int name_si)
    {
      m_Heaps[id] = new HeapInfo(GetStringByIndex(name_si));
    }

    protected override void OnHeapDestroy(ulong id)
    {
      m_Heaps.Remove(id);
    }

    protected override void OnBeginSeek(double secs)
    {
      // Nuke old sorted list to save some RAM.
      m_SortedAllocs.Clear();
    }

    protected override void OnEndSeek(double secs)
    {
      // Update sorted allocations
      m_SortedAllocs.AddRange(m_Allocs.Values);
      m_SortedAllocs.Sort(new HeapAllocationInfo.StartComparer());
    }

    public void SeekTo(double secs, StatusDelegate status_callback)
    {
      SeekImpl(secs, status_callback);
    }
  }

  public sealed class TraceReplayEventBased : TraceProcessorBase
  {
    private IMemEventHandler m_Handler;
    private volatile bool m_Cancel = false;

    public TraceReplayEventBased(string fn)
      : base(fn)
    {
    }

    public void StreamEvents(IMemEventHandler handler)
    {
      m_Handler = handler;

      SeekImpl(0.0, null);

      int i = 0;

      double max_time = this.MetaData.MaxTimeStamp / (double)this.MetaData.TimerFrequency;

      while (NextEvent())
      {
        if (0 == (i & 1023))
        {
          if (m_Cancel)
            break;
          handler.OnProgress(this.CurrentTime / max_time);
        }
        ++i;
      }

      m_Cancel = false;
      m_Handler = null;
    }

    public void Cancel()
    {
      m_Cancel = true;
    }

    protected override void OnVirtualDecommit(ulong ptr, ulong size)
    {
    }

    protected override void OnVirtualCommit(ulong ptr, ulong size)
    {
    }

    protected override void OnAddressFree(ulong ptr)
    {
    }

    protected override void OnAddressAllocate(ulong ptr, ulong size, int name_index)
    {
    }

    protected override void OnHeapFree(int heapId, ulong ptr, int stack_index, double time)
    {
      m_Handler.OnHeapFree(ptr, GetBackTraceByIndex(stack_index), time);
    }

    protected override void OnHeapAllocate(int heapId, ulong ptr, ulong size, int scope_type, int scope_data_str, int stack_index, double time)
    {
      m_Handler.OnHeapAllocate(ptr, size, scope_type, GetStringByIndex(scope_data_str), GetBackTraceByIndex(stack_index), time);
    }

    protected override void OnHeapRemoveCore(ulong heap_id, ulong addr, ulong size)
    {
    }

    protected override void OnHeapAddCore(ulong heap_id, ulong addr, ulong size)
    {
    }

    protected override void OnHeapCreate(ulong id, int name_si)
    {
    }

    protected override void OnHeapDestroy(ulong id)
    {
    }
  }
}
