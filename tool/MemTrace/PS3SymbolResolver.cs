using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemTrace
{
  public sealed class PS3SymbolResolver : ISymbolResolver
  {
    private static readonly string s_MangledWellKnownSymbol = "_ZN8MemTraceL10InitCommonEPFvPKvmE";

    public bool NeedsExePath { get { return true; } }
    public string ExeExtension { get { return ".elf;*.self"; } }

    public void BeginResolve(string exePath, ISymbolProgressListener listener, TraceReplayStateful replay, ICollection<string> symbolPaths, ICollection<ModulePathRemapping> remappings)
    {
      Task.Run(() =>
      {
        try
        {
          string sdkDir = Environment.GetEnvironmentVariable("SCE_PS3_ROOT");
          sdkDir = sdkDir.Replace('/', '\\');

          if (null == sdkDir)
            throw new ApplicationException("Environment variable SCE_PS3_ROOT not set");

          ResolveSymbols(listener, replay, exePath, sdkDir);

          listener.Done();
        }
        catch (Exception ex)
        {
          listener.UpdateError(ex.Message);
        }
      });
    }

    private void ResolveSymbols(ISymbolProgressListener listener, TraceReplayStateful replay, string elfFile, string sdkDir)
    {
      var result = new Dictionary<ulong, SymbolInfo>();

      var procInfo = new ProcessStartInfo
      {
        // Need to merge stdout and stderr together
        Arguments = String.Format("-i \"{0}\" -a2l", elfFile),
        UseShellExecute = false,
        WindowStyle = ProcessWindowStyle.Hidden,
        CreateNoWindow = true,
        FileName = Path.Combine(sdkDir, @"host-win32\sn\bin\ps3bin.exe"),
        RedirectStandardInput = true,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
      };

      var stdout = new List<string>();

      using (var proc = Process.Start(procInfo))
      {
        proc.OutputDataReceived += (object sender, DataReceivedEventArgs args) =>
        {
          if (!String.IsNullOrEmpty(args.Data) && !args.Data.StartsWith("WARNING:"))
            stdout.Add(args.Data);
        };

        proc.ErrorDataReceived += (object sender, DataReceivedEventArgs args) =>
        {
          // Throw it on the floor.
        };

        proc.BeginOutputReadLine();
        proc.BeginErrorReadLine();

        int x = 0;
        var lastUpdate = DateTime.Now; 
        var updateThres = new TimeSpan(0, 0, 0, 0, 30);

        foreach (ulong address in replay.MetaData.Symbols)
        {
          if (x % 10 == 0 || DateTime.Now - lastUpdate > updateThres)
          {
            listener.UpdateProgress(String.Format("Resolving symbol {0}/{1}..", x + 1, replay.MetaData.Symbols.Count), (x + 1) / (double)replay.MetaData.Symbols.Count);
            lastUpdate = DateTime.Now; 
          }

          if (address != 0)
          {
            proc.StandardInput.WriteLine("0x{0:x16}", address);
          }

          ++x;
        }

        proc.StandardInput.Close();
        proc.WaitForExit();
      }

      // Now deal with orbis-bin's wonky output format
      for (int k = 0; k < stdout.Count; )
      {
        var addrStr = MatchPrefix(stdout[k++], "Address:");
        ulong addr = UInt64.Parse(addrStr.Substring(2), NumberStyles.HexNumber);
        string directory = MatchPrefix(stdout[k++], "Directory:").Replace('/', Path.DirectorySeparatorChar);
        string filename = MatchPrefix(stdout[k++], "File Name:");

        var sym = new SymbolInfo();

        sym.Address = addr;
        if (filename != "??")
          sym.FileName = Path.Combine(directory, filename);
        else
          sym.FileName = "Unknown";
        sym.LineNumber = Int32.Parse(MatchPrefix(stdout[k++], "Line Number:"));
        sym.Symbol = MatchPrefix(stdout[k++], "Symbol:");

        // Try to discard function signature data.
        int firstParenPos = sym.Symbol.IndexOf('(');
        if (-1 != firstParenPos)
        {
          sym.Symbol = sym.Symbol.Substring(0, firstParenPos);
        }

        result[addr] = sym;
      }

      listener.UpdateProgress("Saving..", 1.0);
      listener.UpdateMessage("Writing resolved symbols back to trace file");

      replay.UpdateResolvedSymbols(result);

      listener.UpdateProgress("Done", 1.0);
      listener.UpdateMessage("Finished!");

    }

    private string MatchPrefix(string line, string prefix)
    {
      if (!line.StartsWith(prefix))
        throw new ApplicationException("Expected '" + line + "' to start with " + prefix);
      var data = line.Substring(prefix.Length);
      return data.Trim();
    }
  }
}
