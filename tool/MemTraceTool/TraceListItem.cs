using MemTrace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemTraceTool
{
  class TraceListItem
  {
    private string m_FileName;
    public TraceMeta Meta { get; internal set; }
    public TraceRecorder Recorder { get; internal set; }

    public string FileName { get { return m_FileName; } set { m_FileName = value; } }
    public string SourceMachine { get { return Meta.SourceMachine; } }
    public long EventCount { get { return Meta.EventCount; } }
    public long WireSize { get { return Meta.WireSizeBytes; } }
    public long OutputSize { get { return Meta.EncodedDataSizeBytes; } }
    public string PlatformName { get { return Meta.PlatformName; } }
    public string IsResolved { get { return Meta.ResolvedSymbols.Count > 0 ? "Yes" : "No"; } }
    public TraceStatus Status { get { return Meta.Status; } }
    public string ExecutableName { get { return Meta.ExecutableName; } }
    public int WarningCount { get { return Meta.WarningCount; } }

    internal TraceListItem(string fn, TraceMeta meta, TraceRecorder recorder)
    {
      m_FileName = fn;
      Meta = meta;
      Recorder = recorder;
    }

    public ICollection<string> Warnings { get { return Meta.Warnings; } }
  }
}
