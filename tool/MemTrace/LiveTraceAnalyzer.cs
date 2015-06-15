using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemTrace
{
  // Analyzes a trace stream as it's coming in off the network, collecting information such as:
  // - The OS it came from
  // - What marks (points of interest) are in the stream
  // - What modules must be loaded for symbol resolution
  class LiveTraceAnalyzer
  {
    public const uint StreamMagic = 0xbfaf0001;

    private TraceMeta m_MetaData;

    private const uint BufferSize = 128 * 1024;
    private const uint RingMask = BufferSize - 1;

    private byte[] m_InputRing = new byte[BufferSize];                 // FIXME: Switch to ring buffer to get rid of all the data shuffling.
    private ulong m_ReadPos = BufferSize-128;
    private ulong m_WritePos = BufferSize-128;

    private ulong m_SeenStacks = 0;

    private List<string> m_SeenStrings = new List<string>();      // FIXME: Optimize to only keep window size. Transmit window size in header.

    // Items we need to roll back speculatively if a full event could not be read.
    private ulong m_SeenStackRollback = 0;
    private int m_SeenStringRollback = 0;

    public LiveTraceAnalyzer(TraceMeta meta)
    {
      m_MetaData = meta;
    }

    public void Update(byte[] data, int length)
    {
      ulong prev_read_pos = m_ReadPos;
      int data_offset = 0;
      byte[] ring = m_InputRing;

      while (data_offset < length)
      {
        // Copy as much as possible in to the ring buffer.
        uint ring_space = (uint)(BufferSize - (m_WritePos - m_ReadPos));

        ring_space = Math.Min((uint) (length - data_offset), ring_space);

        ulong wpos = m_WritePos;

        while (ring_space > 0)
        {
          ring[wpos & RingMask] = data[data_offset++];
          --ring_space;
          ++wpos;
        }

        m_WritePos = wpos;

        StepDecoder();

        if (prev_read_pos == m_ReadPos)
        {
          throw new IOException(String.Format("Can't make progress past offset {0} in stream", prev_read_pos));
        }

      }
    }

    private byte GetByte(ulong pos)
    {
      return m_InputRing[pos & RingMask];
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

      while (m_SeenStringRollback > 0)
      {
        m_SeenStrings.RemoveAt(m_SeenStrings.Count - 1);
        --m_SeenStringRollback;
      }

      m_SeenStacks -= m_SeenStackRollback;
      m_SeenStackRollback = 0;

      while (pos < maxpos)
      {
        ulong code;
        ulong scope;
        string scope_data;
        ulong timestamp;

        if (!ReadUnsigned(out code, ref pos)) return;

        if (!Enum.IsDefined(typeof(EventCode), (EventCode) code))
        {
          throw new IOException(String.Format("Bad event code {0} at pos {1}\n", code, pos - 1));
        }

        if (!ReadUnsigned(out scope, ref pos)) return;

        if (0 != scope)
        {
          if (!ReadString(out scope_data, ref pos)) return;
        }

        if (!ReadUnsigned(out timestamp, ref pos)) return;
        if (!SkipBackTrace(ref pos)) return;

        switch ((EventCode)code)
        {
          case EventCode.BeginStream:
            string pname;
            ulong ptrsize;
            ulong magic;
            ulong timer_freq;
            if (!ReadUnsigned(out magic, ref pos)) return;
            if (magic != StreamMagic)
              throw new IOException(String.Format("Bad stream magic {0:x8}, expected {1:x8}", magic, StreamMagic));

            if (!ReadString(out pname, ref pos)) return;
            if (!ReadUnsigned(out ptrsize, ref pos)) return;
            if (!ReadUnsigned(out timer_freq, ref pos)) return;
            m_MetaData.PlatformName = pname;
            m_MetaData.PointerSizeBytes = (int)ptrsize;
            m_MetaData.TimerFrequency = timer_freq;
            break;

          case EventCode.EndStream:
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

                tmp.Add(new ModuleInfo { BaseAddress = modbase, SizeBytes = modsize, Name = modname });
              }

              // We got all of it. Commit.
              m_MetaData.Modules = tmp;
            }
            break;

          case EventCode.Mark:
            {
              string name;
              if (!ReadString(out name, ref pos)) return;
              m_MetaData.Marks.Add(new TraceMark { Name = name, TimeStamp = timestamp });
            }
            break;

          case EventCode.AddressAllocate:
            if (!SkipUnsigned(ref pos)) return;
            if (!SkipUnsigned(ref pos)) return;
            if (!SkipString(ref pos)) return;
            break;

          case EventCode.AddressFree:
            if (!SkipUnsigned(ref pos)) return;
            break;

          case EventCode.VirtualCommit:
            if (!SkipUnsigned(ref pos)) return;
            if (!SkipUnsigned(ref pos)) return;
            break;

          case EventCode.VirtualDecommit:
            if (!SkipUnsigned(ref pos)) return;
            if (!SkipUnsigned(ref pos)) return;
            break;

          case EventCode.HeapCreate:
            if (!SkipUnsigned(ref pos)) return;
            if (!SkipString(ref pos)) return;
            break;

          case EventCode.HeapDestroy:
            if (!SkipUnsigned(ref pos)) return;
            break;

          case EventCode.HeapAddCore:
            if (!SkipUnsigned(ref pos)) return;
            if (!SkipUnsigned(ref pos)) return;
            if (!SkipUnsigned(ref pos)) return;
            break;

          case EventCode.HeapRemoveCore:
            if (!SkipUnsigned(ref pos)) return;
            if (!SkipUnsigned(ref pos)) return;
            if (!SkipUnsigned(ref pos)) return;
            break;

          case EventCode.HeapAllocate:
            if (!SkipUnsigned(ref pos)) return;
            if (!SkipUnsigned(ref pos)) return;
            break;

          case EventCode.HeapReallocate:
            if (!SkipUnsigned(ref pos)) return;
            if (!SkipUnsigned(ref pos)) return;
            if (!SkipUnsigned(ref pos)) return;
            break;

          case EventCode.HeapFree:
            if (!SkipUnsigned(ref pos)) return;
            break;

          default:
            throw new IOException(String.Format("Unexpected event code {0}", code));
        }

        // Commit data decoded so far
        m_ReadPos = pos;
        m_SeenStringRollback = 0;
        m_SeenStackRollback = 0;
        m_MetaData.MaxTimeStamp = timestamp;
        ++m_MetaData.EventCount;
      }
    }

    private bool SkipBackTrace(ref ulong pos)
    {
      ulong index;
      if (!ReadUnsigned(out index, ref pos)) return false;

      if (index < m_SeenStacks)
        return true;

      if (index != m_SeenStacks)
        throw new IOException("Unexpected stack index");

      ulong frame_count;
      if (!ReadUnsigned(out frame_count, ref pos)) return false;

      for (ulong i = 0; i < frame_count; ++i)
      {
        ulong addr;
        if (!ReadUnsigned(out addr, ref pos))
          return false;

        if (!m_MetaData.Symbols.Contains(addr))
          m_MetaData.Symbols.Add(addr);
      }

      ++m_SeenStackRollback;
      ++m_SeenStacks;
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
      m_SeenStrings.Add(data);
      pos += length;
      return true;
    }
  }

}
