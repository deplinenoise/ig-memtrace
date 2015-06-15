using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemTrace
{
  /// <summary>
  /// Metadata stored after the compressed event stream.
  /// </summary>
  public sealed class TraceMeta
  {
    public const uint V0Magic = 0xfbfae11c;
    public const uint V1Magic = 0xfbfae11d;
    public const uint V2Magic = 0xfbfae11e;
    public const uint V3Magic = 0xfbfae11f;
    public const uint V4Magic = 0xfbfae120;

    private static readonly uint[] AllMagics = new uint[] { V0Magic, V1Magic, V2Magic, V3Magic, V4Magic };

    /// <summary>
    ///  Name of the platform where the trace file originated.
    /// </summary>
    public string PlatformName { get; internal set; }

    public int Version { get; private set; }

    /// <summary>
    /// The ticks/second frequency of time values in the stream.
    /// </summary>
    public ulong TimerFrequency { get; set; }

    /// <summary>
    /// Size in bytes of a pointer on the platform where the trace file was generated.
    /// </summary>
    public int PointerSizeBytes { get; internal set; }
    /// <summary>
    /// List of modules loaded while dump was recorded (currently the first list of modules dumped by the client).
    /// </summary>
    public List<ModuleInfo> Modules { get; internal set; }
    /// <summary>
    /// Symbols that appear in the trace file.
    /// </summary>
    public List<ulong> Symbols { get; internal set; }
    /// <summary>
    /// Position in the file where the footer lives
    /// </summary>
    public long FooterPosition { get; internal set; }
    /// <summary>
    /// Number of bytes of event mmap data in the file.
    /// </summary>
    public long EncodedDataSizeBytes { get; internal set; }
    /// <summary>
    /// The number of events that can be decoded.
    /// </summary>
    public long EventCount { get; internal set; }
    /// <summary>
    /// Source machine IP this was captured from.
    /// </summary>
    public string SourceMachine { get; internal set; }
    /// <summary>
    /// Highest timestamp of any event.
    /// </summary>
    public ulong MaxTimeStamp { get; internal set; }
    /// <summary>
    /// Possibly empty dictionary of resolved symbols.
    /// </summary>
    public Dictionary<ulong, SymbolInfo> ResolvedSymbols { get; internal set; }

    /// <summary>
    /// Address of the symbol MemTrace::InitCommon - to handle randomized base addressing where needed.
    /// </summary>
    public ulong MemTraceInitCommonAddress { get; internal set; }

    public TraceStatus Status { get; internal set; }

    public long FileSize { get; internal set; }

    public int WarningCount { get; private set; }
    public List<string> Warnings { get; internal set; }

    public string ExecutableName { get; set; }

    private List<TraceMark> m_Marks;

    /// <summary>
    /// Raw size of the wire event stream when the trace was captured.
    /// </summary>
    public long WireSizeBytes { get; internal set; }

    internal TraceMeta()
    {
      Status = TraceStatus.Ready;
      Symbols = new List<ulong>();
      ResolvedSymbols = new Dictionary<ulong, SymbolInfo>();
      m_Marks = new List<TraceMark>();
      PlatformName = "Unknown";
      ExecutableName = "";
      Warnings = new List<string>();
    }

    public void Load(string fn)
    {
      using (var stream = File.Open(fn, FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        FileSize = stream.Length;

        // Figure out what the footer looks like
        using (var br = new BinaryReader(stream, Encoding.UTF8, /*leaveOpen:*/ true))
        {
          // Seek to last 8 bytes.
          stream.Position = stream.Length - 8;

          // Read position of footer.
          long footer_pos = br.ReadInt64();

          // Stash length of compressed data preceding the footer.
          FooterPosition = footer_pos;

          stream.Position = footer_pos;

          // Slurp in the serialized footer.
          uint magic = br.ReadUInt32();
          Version = Array.IndexOf(AllMagics, magic);
          if (Version == -1)
            throw new IOException(String.Format("Expected decorated stream magic {0:x8}, got {1:x8}. Old file?", V2Magic, magic));

          PlatformName = br.ReadString();
          PointerSizeBytes = br.ReadInt32();
          EventCount = br.ReadInt64();
          WireSizeBytes = br.ReadInt64();
          SourceMachine = br.ReadString();
          MaxTimeStamp = br.ReadUInt64();
          TimerFrequency = br.ReadUInt64();
          EncodedDataSizeBytes = br.ReadInt64();

          {
            int modcount = br.ReadInt32();
            Modules = new List<ModuleInfo>(modcount);
            for (int i = 0; i < modcount; ++i)
            {
              var name = br.ReadString();
              var baseaddr = br.ReadUInt64();
              var size = br.ReadUInt64();
              Modules.Add(new ModuleInfo { Name = name, BaseAddress = baseaddr, SizeBytes = size });
            }
          }

          {
            int markcount = br.ReadInt32();
            m_Marks.Clear();
            for (int i = 0; i < markcount; ++i)
            {
              var name = br.ReadString();
              var time = br.ReadUInt64();
              AddMark(new TraceMark { Name = name, TimeStamp = time });
            }
          }

          {
            int symcount = br.ReadInt32();
            Symbols = new List<ulong>(symcount);
            for (int i = 0; i < symcount; ++i)
            {
              var addr = br.ReadUInt64();
              Symbols.Add(addr);
            }
          }

          {
            ResolvedSymbols.Clear();
            int rsymcount = br.ReadInt32();
            for (int i = 0; i < rsymcount; ++i)
            {
              var sym = new SymbolInfo();
              ulong addr = br.ReadUInt64();
              sym.FileName = br.ReadString();
              sym.LineNumber = br.ReadInt32();
              sym.Symbol = br.ReadString();
              ResolvedSymbols[addr] = sym;
            }
          }

          if (Version > 0)
          {
            MemTraceInitCommonAddress = br.ReadUInt64();
          }

          if (Version > 1)
          {
            ExecutableName = br.ReadString();
            WarningCount = br.ReadInt32();
          }

          if (Version > 3)
          {
            int warnTextCount = br.ReadInt32();
            for (int i = 0; i < warnTextCount; ++i)
            {
              Warnings.Add(br.ReadString());
            }
          }
        }
      }
    }

    public void Save(BinaryWriter b)
    {
      long trailer_pos = b.BaseStream.Position;

      b.Write(AllMagics[AllMagics.Length-1]);

      b.Write(PlatformName);
      b.Write(PointerSizeBytes);
      b.Write(EventCount);
      b.Write(WireSizeBytes);
      b.Write(SourceMachine);
      b.Write(MaxTimeStamp);
      b.Write(TimerFrequency);
      b.Write(EncodedDataSizeBytes);

      if (Modules != null)
      {
        b.Write(Modules.Count);
        foreach (var mod in Modules)
        {
          b.Write(mod.Name);
          b.Write(mod.BaseAddress);
          b.Write(mod.SizeBytes);
        }
      }
      else
      {
        b.Write(0u);
      }

      lock (this)
      {
        b.Write(m_Marks.Count);
        foreach (var mark in m_Marks)
        {
          b.Write(mark.Name);
          b.Write(mark.TimeStamp);
        }
      }

      if (Symbols != null)
      {
        b.Write(Symbols.Count);
        foreach (var sym in Symbols)
        {
          b.Write(sym);
        }
      }
      else
      {
        b.Write(0u);
      }

      b.Write(ResolvedSymbols.Count);
      foreach (var sym in ResolvedSymbols)
      {
        b.Write(sym.Key);
        b.Write(sym.Value.FileName);
        b.Write(sym.Value.LineNumber);
        b.Write(sym.Value.Symbol);
      }

      b.Write(MemTraceInitCommonAddress);

      b.Write(ExecutableName);
      b.Write(WarningCount);

      b.Write((int)Warnings.Count);
      foreach (var w in Warnings)
      {
        b.Write(w);
      }

      b.Write(trailer_pos);
    }

    public void Update(string fn)
    {
      using (var fh = File.Open(fn, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
      {
        long trailer_pos;

        // Figure out where the old metadata lived.
        using (BinaryReader br = new BinaryReader(fh, Encoding.UTF8, /*leaveOpen:*/ true))
        {
          fh.Position = fh.Length - 8;
          trailer_pos = br.ReadInt64();
        }

        // Truncate file.
        fh.SetLength(trailer_pos);

        // Append new trailer.
        fh.Position = trailer_pos;
        using (var b = new BinaryWriter(fh, Encoding.UTF8, /* leaveOpen: */ true))
        {
          Save(b);
        }
      }
    }

    internal void AddMark(TraceMark traceMark)
    {
      lock (this)
      {
        m_Marks.Add(traceMark);
      }
    }

    /// <summary>
    /// Points of interest in the trace file put by the user.
    /// </summary>
    public void GetTraceMarks(List<TraceMark> output)
    {
      lock (this)
      {
        output.AddRange(m_Marks);
      }
    }

    public void AddWarning(string fmt, params object[] args)
    {
      WarningCount++;
      Warnings.Add(String.Format(fmt, args));
    }
  }
}
