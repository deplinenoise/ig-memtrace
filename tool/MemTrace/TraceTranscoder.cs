using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemTrace
{
  public struct AllocationKey : IEquatable<AllocationKey>
  {
    public ulong HeapId;
    public ulong Address;

    public override int GetHashCode()
    {
      return (int) ((HeapId * 13) ^ Address);
    }

    public bool Equals(AllocationKey other)
    {
      return other.HeapId == this.HeapId && other.Address == this.Address;
    }
  }

  // Analyzes a trace stream as it's coming in off the network, collecting information such as:
  // - The OS it came from
  // - What marks (points of interest) are in the stream
  // - What modules must be loaded for symbol resolution
  //
  // If saves a trimmed version of the data to an output stream.
  class TraceTranscoder
  {
    public const uint StreamMagic = 0xbfaf0003;

    public TraceMeta MetaData { get; private set; }

    private const uint BufferSize = 128 * 1024;
    private const uint RingMask = BufferSize - 1;

    private byte[] m_InputRing = new byte[BufferSize];
    private ulong m_ReadPos = 0;
    private ulong m_WritePos = 0;

    private List<string> m_SeenStrings = new List<string>();
    private List<ulong[]> m_SeenStacks = new List<ulong[]>();

    private Dictionary<string,string> m_StringCache = new Dictionary<string,string>();

    // Stash output locations of create events so delete/destroy events can be linked up for backwards stream replaying
    private Dictionary<ulong, long> m_AddressCreateEvents = new Dictionary<ulong, long>();
    private Dictionary<ulong, long> m_HeapCreateEvents = new Dictionary<ulong, long>();
    private Dictionary<AllocationKey, long> m_AllocCreateEvents = new Dictionary<AllocationKey, long>();

    // Items we need to roll back speculatively if a full event could not be read.
    private int m_SeenStackRollback = 0;
    private int m_SeenStringRollback = 0;

    private BinaryWriter m_Out;

    // Data about current event being decoded.
    private ulong m_CurrCode;
    private int m_CurrScope;
    private int m_CurrScopeDataIndex;
    private int m_CurrBackTraceIndex;
    private ulong m_CurrTimestamp;
    private long m_PrevEventOffset = 0;

    public TraceTranscoder(Stream outStream)
    {
      MetaData = new TraceMeta();

      m_Out = new BinaryWriter(outStream, Encoding.UTF8, /*leaveOpen:*/ true);

      // Reserve space for two offsets used when decoding.
      m_Out.Write((ulong)0);
      m_Out.Write((ulong)0);
    }

    public void Update(byte[] data, int length)
    {
      MetaData.WireSizeBytes += length;

      ulong prev_read_pos = m_ReadPos;
      int data_offset = 0;
      byte[] ring = m_InputRing;

      do
      {
        // Copy as much as possible in to the ring buffer.
        uint ring_space = (uint)(BufferSize - (m_WritePos - m_ReadPos));

        ring_space = Math.Min((uint)(length - data_offset), ring_space);

        ulong wpos = m_WritePos;

        // TODO: Optimize
        while (ring_space > 0)
        {
          ring[wpos & RingMask] = data[data_offset++];
          --ring_space;
          ++wpos;
        }

        m_WritePos = wpos;

        StepDecoder();

        MetaData.EncodedDataSizeBytes = m_Out.BaseStream.Position;

      } while (data_offset < length);
    }

    private byte GetByte(ulong pos)
    {
      return m_InputRing[pos & RingMask];
    }

    private bool ReadUnsigned(out int code, ref ulong pos)
    {
      ulong val;
      if (ReadUnsigned(out val, ref pos))
      {
        code = (int)val;
        return true;
      }
      else
      {
        code = -1;
        return false;
      }
    }

    private bool ReadUnsigned(out ulong code, ref ulong pos)
    {
      ulong val = 0;
      ulong mul = 1;
      ulong b;
      ulong i = pos;
      do
      {
        if (i == m_WritePos)
        {
          code = 0;
          return false;
        }
        b = GetByte(i++);
        val |= b * mul;
        mul <<= 7;
      } while (b < 0x80);

      val &= ~mul;

      pos = i;
      code = val;
      return true;
    }

    private bool SkipUnsigned(ref ulong pos)
    {
      ulong ignored;
      return ReadUnsigned(out ignored, ref pos);
    }

    private bool SkipString(ref ulong pos)
    {
      string ignored;
      return ReadString(out ignored, ref pos);
    }

    private void StepDecoder()
    {
      ulong pos = m_ReadPos;
      ulong maxpos = m_WritePos;

      if (m_SeenStringRollback > 0)
      {
        m_SeenStrings.RemoveRange(m_SeenStrings.Count - m_SeenStringRollback, m_SeenStringRollback);
        m_SeenStringRollback = 0;
      }

      if (m_SeenStackRollback > 0)
      {
        m_SeenStacks.RemoveRange(m_SeenStacks.Count - m_SeenStackRollback, m_SeenStackRollback);
        m_SeenStackRollback = 0;
      }

      while (pos < maxpos)
      {
        if (!ReadUnsigned(out m_CurrCode, ref pos)) return;

        if (!Enum.IsDefined(typeof(EventCode), (EventCode) m_CurrCode))
        {
          throw new IOException(String.Format("Bad event code {0} at pos {1}\n", m_CurrCode, pos - 1));
        }

        if (!ReadUnsigned(out m_CurrScope, ref pos)) return;

        if (0 != m_CurrScope)
        {
          if (!ReadStringIndex(out m_CurrScopeDataIndex, ref pos)) return;
        }
        else
        {
          m_CurrScopeDataIndex = -1;
        }

        ulong timestamp;
        if (!ReadUnsigned(out timestamp, ref pos))
          return;

        // Only commit after the whole thing read successfully, as the UI thread could sample the
        // timestamp at any time to put in a trace marker.
        m_CurrTimestamp = timestamp;

        if (!ReadBackTraceIndex(out m_CurrBackTraceIndex, ref pos)) return;

        switch ((EventCode)m_CurrCode)
        {
          case EventCode.BeginStream:
            string pname;
            ulong ptrsize;
            ulong magic;
            ulong timer_freq;
            ulong well_known_address;
            if (!ReadUnsigned(out magic, ref pos)) return;
            if (magic != StreamMagic)
              throw new IOException(String.Format("Bad stream magic {0:x8}, expected {1:x8}", magic, StreamMagic));

            if (!ReadString(out pname, ref pos)) return;
            if (!ReadUnsigned(out ptrsize, ref pos)) return;
            if (!ReadUnsigned(out timer_freq, ref pos)) return;
            if (!ReadUnsigned(out well_known_address, ref pos)) return;
            MetaData.PlatformName = pname;
            MetaData.PointerSizeBytes = (int)ptrsize;
            MetaData.TimerFrequency = timer_freq;
            MetaData.MemTraceInitCommonAddress = well_known_address;
            // Nothing written.
            break;

          case EventCode.EndStream:
            // Nothing written.
            break;

          case EventCode.ModuleDump:
            {
              List<ModuleInfo> tmp = new List<ModuleInfo>();
              for (; ; )
              {
                ulong ctrl;
                if (!ReadUnsigned(out ctrl, ref pos)) return;
                if (0 == ctrl)
                  break;

                string modname;
                ulong modbase;
                ulong modsize;
                if (!ReadString(out modname, ref pos)) return;
                if (!ReadUnsigned(out modbase, ref pos)) return;
                if (!ReadUnsigned(out modsize, ref pos)) return;

                if (modname.EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase))
                {
                  MetaData.ExecutableName = Path.GetFileNameWithoutExtension(modname);
                }

                tmp.Add(new ModuleInfo { BaseAddress = modbase, SizeBytes = modsize, Name = modname });
              }

              // We got all of it. Commit.
              MetaData.Modules = tmp;
            }
            // Nothing written.
            break;

          case EventCode.Mark:
            {
              string name;
              if (!ReadString(out name, ref pos)) return;
              MetaData.AddMark(new TraceMark { Name = name, TimeStamp = m_CurrTimestamp });
            }
            // Nothing written.
            break;

          case EventCode.AddressAllocate:
            {
              ulong addr, size;
              int name_index;

              if (!ReadUnsigned(out addr, ref pos))
                return;
              if (!ReadUnsigned(out size, ref pos))
                return;
              if (!ReadStringIndex(out name_index, ref pos))
                return;

              if (!m_AddressCreateEvents.ContainsKey(addr))
              {
                m_AddressCreateEvents[addr] = m_Out.BaseStream.Position;
                BeginOutEvent();
                m_Out.Write(addr);
                m_Out.Write(size);
                m_Out.Write(name_index);
              }
              else
              {
                MetaData.AddWarning("Attempt to allocate already allocated virtual address {0:x16}", addr);
              }
            }
            break;

          case EventCode.AddressFree:
            {
              ulong addr;
              if (!ReadUnsigned(out addr, ref pos))
                return;

              // Handle sloppy runtime code reporting frees multiple times :(
              if (m_AddressCreateEvents.ContainsKey(addr))
              {
                BeginOutEvent();
                m_Out.Write(addr);
                m_Out.Write(m_AddressCreateEvents[addr]);
                m_AddressCreateEvents.Remove(addr);
              }
              else
              {
                MetaData.AddWarning("Attempt to free non-allocated virtual address {0:x16}", addr);
              }
            }
            break;

          case EventCode.VirtualCommit:
          case EventCode.VirtualDecommit:
            {
              ulong addr, size;
              if (!ReadUnsigned(out addr, ref pos)) return;
              if (!ReadUnsigned(out size, ref pos)) return;
              BeginOutEvent();
              m_Out.Write(addr);
              m_Out.Write(size);
            }
            break;

          case EventCode.HeapCreate:
            {
              ulong id;
              int str_index;
              if (!ReadUnsigned(out id, ref pos)) return;
              if (!ReadStringIndex(out str_index, ref pos)) return;
              m_HeapCreateEvents[id] = m_Out.BaseStream.Position;
              BeginOutEvent();
              m_Out.Write(id);
              m_Out.Write(str_index);
            }
            break;

          case EventCode.HeapDestroy:
            {
              ulong id;
              if (!ReadUnsigned(out id, ref pos)) return;
              if (m_HeapCreateEvents.ContainsKey(id))
              {
                BeginOutEvent();
                m_Out.Write(id);
                m_Out.Write(m_HeapCreateEvents[id]);
                m_HeapCreateEvents.Remove(id);
              }
              else
              {
                MetaData.AddWarning("Attempt to destroy non-existing heap id={0:x16}", id);
              }
            }
            break;

          case EventCode.HeapAddCore:
          case EventCode.HeapRemoveCore:
            {
              ulong id, addr, size;
              if (!ReadUnsigned(out id, ref pos)) return;
              if (!ReadUnsigned(out addr, ref pos)) return;
              if (!ReadUnsigned(out size, ref pos)) return;
              BeginOutEvent();
              m_Out.Write(id);
              m_Out.Write(addr);
              m_Out.Write(size);
            }
            break;

          case EventCode.HeapAllocate:
            {
              ulong addr, size, heap_id;
              if (!ReadUnsigned(out heap_id, ref pos)) return;
              if (!ReadUnsigned(out addr, ref pos)) return;
              if (!ReadUnsigned(out size, ref pos)) return;
              var key = new AllocationKey { HeapId = heap_id, Address = addr };
              if (!m_AllocCreateEvents.ContainsKey(key))
              {
                m_AllocCreateEvents[key] = m_Out.BaseStream.Position;
                BeginOutEvent();
                m_Out.Write((uint) heap_id);
                m_Out.Write(addr);
                m_Out.Write(size);
              }
              else
              {
                MetaData.AddWarning("Attempt to allocate already-allocated address {0:x16}", addr);
              }
            }
            break;

          case EventCode.HeapReallocate:
            {
              ulong heap_id, old_addr, new_addr, new_size;
              if (!ReadUnsigned(out heap_id, ref pos)) return;
              if (!ReadUnsigned(out old_addr, ref pos)) return;
              if (!ReadUnsigned(out new_addr, ref pos)) return;
              if (!ReadUnsigned(out new_size, ref pos)) return;
              long npos = m_Out.BaseStream.Position;
              var key = new AllocationKey { HeapId = heap_id, Address = old_addr };
              if (m_AllocCreateEvents.ContainsKey(key))
              {
                BeginOutEvent();
                m_Out.Write((uint) heap_id);
                m_Out.Write(old_addr);
                m_Out.Write(new_addr);
                m_Out.Write(new_size);
                m_Out.Write(m_AllocCreateEvents[key]);
                m_AllocCreateEvents.Remove(key);
                var new_key = new AllocationKey { HeapId = heap_id, Address = new_addr };
                m_AllocCreateEvents[new_key] = npos;
              }
              else
              {
                MetaData.AddWarning("Attempt to reallocate non-allocated address {0:x16}", old_addr);
              }
              break;
            }

          case EventCode.HeapFree:
            {
              ulong addr, heap_id;
              if (!ReadUnsigned(out heap_id, ref pos)) return;
              if (!ReadUnsigned(out addr, ref pos)) return;
              if (addr != 0)
              {
                var key = new AllocationKey { HeapId = heap_id, Address = addr };
                if (m_AllocCreateEvents.ContainsKey(key))
                {
                  BeginOutEvent();
                  m_Out.Write((uint)heap_id);
                  m_Out.Write(addr);
                  m_Out.Write(m_AllocCreateEvents[key]);
                  m_AllocCreateEvents.Remove(key);
                }
                else
                {
                  MetaData.AddWarning("Attempt to free non-allocated address {0:x16}", addr);
                }
              }
            }
            break;

          default:
            throw new IOException(String.Format("Unexpected event code {0}", m_CurrCode));
        }

        // Commit data decoded so far
        m_ReadPos = pos;
        m_SeenStringRollback = 0;
        m_SeenStackRollback = 0;
        MetaData.MaxTimeStamp = m_CurrTimestamp;
        ++MetaData.EventCount;
      }
    }

    private void BeginOutEvent()
    {
      Debug.Assert(MetaData.TimerFrequency != 0);
      double t = m_CurrTimestamp / (double) MetaData.TimerFrequency;

      long pos = m_Out.BaseStream.Position;

      m_Out.Write((int)m_CurrCode);
      m_Out.Write((int)(pos - m_PrevEventOffset));
      m_Out.Write(t);
      m_Out.Write(m_CurrScope);
      m_Out.Write(m_CurrScopeDataIndex);
      m_Out.Write(m_CurrBackTraceIndex);

      m_PrevEventOffset = pos;
    }

    private const int IndexInterval = 4096; // Must be power of two.

    private bool ReadBackTraceIndex(out int out_index, ref ulong pos)
    {
      int index;
      if (!ReadUnsigned(out index, ref pos))
      {
        out_index = -1;
        return false;
      }

      if (index < m_SeenStacks.Count)
      {
        out_index = (int)index;
        return true;
      }

      if (index != m_SeenStacks.Count)
        throw new IOException("Unexpected stack index");

      int frame_count;
      if (!ReadUnsigned(out frame_count, ref pos))
      {
        out_index = -1;
        return false;
      }

      ulong[] frames = new ulong[frame_count];

      for (int i = 0; i < frame_count; ++i)
      {
        if (!ReadUnsigned(out frames[i], ref pos))
        {
          out_index = -1;
          return false;
        }

        if (!MetaData.Symbols.Contains(frames[i]))
          MetaData.Symbols.Add(frames[i]);
      }

      ++m_SeenStackRollback;
      m_SeenStacks.Add(frames);
      out_index = index;
      return true;
    }

    private bool ReadString(out string data, ref ulong pos)
    {
      data = null;

      ulong index;
      if (!ReadUnsigned(out index, ref pos)) return false;

      if ((int) index < m_SeenStrings.Count)
      {
        data = m_SeenStrings[(int) index];
        return true;
      }

      ulong length;
      if (!ReadUnsigned(out length, ref pos)) return false;

      byte[] buf = new byte[(int) length];
      for (ulong i = 0; i < length; ++i)
      {
        if (pos + i == m_WritePos)
          return false;

        buf[i] = GetByte(pos + i);
      }

      data = Encoding.UTF8.GetString(buf);
      ++m_SeenStringRollback;
      string shared;
      if (!m_StringCache.TryGetValue(data, out shared))
      {
        shared = data;
        m_StringCache.Add(data, data);
      }
      m_SeenStrings.Add(data);
      pos += length;
      return true;
    }

    private bool ReadStringIndex(out int out_index, ref ulong pos)
    {
      out_index = -1;

      ulong index;
      if (!ReadUnsigned(out index, ref pos)) return false;

      if ((int) index < m_SeenStrings.Count)
      {
        out_index = (int)index;
        return true;
      }

      ulong length;
      if (!ReadUnsigned(out length, ref pos)) return false;

      byte[] buf = new byte[(int) length];
      for (ulong i = 0; i < length; ++i)
      {
        if (pos + i == m_WritePos)
          return false;

        buf[i] = GetByte(pos + i);
      }

      string data = Encoding.UTF8.GetString(buf);
      ++m_SeenStringRollback;
      m_SeenStrings.Add(data);
      pos += length;
      out_index = m_SeenStrings.Count - 1;
      return true;
    }

    internal void Flush()
    {
      // Write strings and callstacks.

      long strpos = m_Out.BaseStream.Position;
      long strtab_sz = 8 * m_SeenStrings.Count;

      {
        var buf = new MemoryStream();
        foreach (var s in m_SeenStrings)
        {
          var bytes = Encoding.UTF8.GetBytes(s);
          m_Out.Write((uint)(strtab_sz + buf.Length));
          m_Out.Write((int)bytes.Length);
          buf.Write(bytes, 0, bytes.Length);
        }

        buf.Position = 0;
        buf.CopyTo(m_Out.BaseStream);
      }

      // Call stacks.
      long stkpos = m_Out.BaseStream.Position;
      long stktab_sz = 8 * m_SeenStacks.Count;
      {
        var buf = new MemoryStream();
        var bufh = new BinaryWriter(buf);
        foreach (var s in m_SeenStacks)
        {
          int cnt = s.Length;
          m_Out.Write((uint)(stktab_sz + buf.Length));
          m_Out.Write(cnt);
          foreach (ulong addr in s)
          {
            bufh.Write(addr);
          }
        }
        buf.Position = 0;
        buf.CopyTo(m_Out.BaseStream);
      }

      // Patch up offsets
      m_Out.BaseStream.Position = 0;
      m_Out.Write(strpos);
      m_Out.Write(stkpos);
      m_Out.BaseStream.Position = m_Out.BaseStream.Length;

      MetaData.Save(m_Out);
      MetaData.Status = TraceStatus.Ready;
    }

    public ulong CurrentTimeStamp
    {
      get
      {
        return m_CurrTimestamp;
      }
    }
  }

}
