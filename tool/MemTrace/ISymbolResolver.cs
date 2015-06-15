using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemTrace
{
  public sealed class ModulePathRemapping
  {
    public string Platform { get; set; }
    public string Path { get; set;  }
    public string ReplacementPath { get; set; }

    public ModulePathRemapping Clone()
    {
      return new ModulePathRemapping
      {
        Platform = Platform,
        Path = Path,
        ReplacementPath = ReplacementPath
      };
    }
  }

  public interface ISymbolProgressListener
  {
    void UpdateProgress(string status, double pct);
    void UpdateMessage(string status, params object[] args);
    void Done();
    void UpdateError(string p);
  }

  public interface ISymbolResolver
  {
    bool NeedsExePath { get; }
    string ExeExtension { get; }

    void BeginResolve(string exePath, ISymbolProgressListener listener, TraceReplayStateful replay, ICollection<string> symbolPaths, ICollection<ModulePathRemapping> remappings);
  }
}
