using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemTrace;
using System.IO;

namespace MemTrace
{
  public sealed class DbgHelpSymbolResolver : ISymbolResolver
  {
    public bool NeedsExePath { get { return false; } }

    public string ExeExtension { get { return ".exe"; } }

    public void BeginResolve(string ignored, ISymbolProgressListener listener, TraceReplayStateful replay, ICollection<string> symbolPaths, ICollection<ModulePathRemapping> remappings)
    {
      Task.Run(() =>
      {
        var result = new Dictionary<ulong, SymbolInfo>();
        var metadata = replay.MetaData;
        var platform = metadata.PlatformName;

        using (var dbghelp = new DbgHelp(symbolPaths))
        {
          for (int i = 0, count = metadata.Modules.Count; i < count; ++i)
          {
            var mod = metadata.Modules[i];

            listener.UpdateProgress(String.Format("Loading module {0}/{1}..", i + 1, count), (i + 1) / (double)count);

            // TEMP! Ignore empty module from Durango.
            if (mod.Name == "")
              continue;

            var fn = mod.Name;
            foreach (var rm in remappings)
            {
              if (0 != StringComparer.InvariantCultureIgnoreCase.Compare(rm.Platform, platform))
                continue;

              if (fn.ToLower().StartsWith(rm.Path.ToLower()))
              {
                var oldfn = fn;
                fn = rm.ReplacementPath + fn.Substring(rm.Path.Length);
                listener.UpdateMessage(String.Format("Remapped {0} -> {1}", oldfn, fn));
              }
            }

            listener.UpdateMessage(String.Format("Loading {0} at {1:x16}, size={2:n0} bytes", fn, mod.BaseAddress, mod.SizeBytes));
            dbghelp.LoadModule(fn, mod.BaseAddress, mod.SizeBytes);
          }

          int x = 0;
          foreach (ulong address in metadata.Symbols)
          {
            if (x % 100 == 0)
            {
              listener.UpdateProgress(String.Format("Resolving symbol {0}/{1}..", x + 1, metadata.Symbols.Count), (x + 1) / (double)metadata.Symbols.Count);
            }

            SymbolInfo sym;
            if (dbghelp.LookupSymbol(address, out sym))
            {
              result[address] = sym;
            }
            else
            {
              sym.Symbol = "(unknown)";
              foreach (var mod in metadata.Modules)
              {
                if (address >= mod.BaseAddress && address <= mod.BaseAddress + mod.SizeBytes)
                {
                  sym.Symbol = String.Format("{0}!0x{1:x16}", Path.GetFileNameWithoutExtension(mod.Name), address);
                  result[address] = sym;
                  break;
                }
              }
              //UpdateMessage(String.Format("Failed to resolve address {0:x}\n", address));
            }

            ++x;
          }
        }

        listener.UpdateProgress("Saving..", 1.0);
        listener.UpdateMessage("Writing resolved symbols back to trace file");

        replay.UpdateResolvedSymbols(result);

        listener.UpdateProgress("Done", 1.0);
        listener.UpdateMessage("Finished!");

        listener.Done();
      });
    }
  }
}
