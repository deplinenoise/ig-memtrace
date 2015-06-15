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
  public sealed class OrbisSymbolResolver : ISymbolResolver
  {
    private static readonly string s_MangledWellKnownSymbol = "_ZN8MemTraceL10InitCommonEPFvPKvmE";

    public bool NeedsExePath { get { return true; } }
    public string ExeExtension { get { return ".elf"; } }

    public void BeginResolve(string exePath, ISymbolProgressListener listener, TraceReplayStateful replay, ICollection<string> symbolPaths, ICollection<ModulePathRemapping> remappings)
    {
      Task.Run(() =>
      {
        try
        {
          string sdkDir = Environment.GetEnvironmentVariable("SCE_ORBIS_SDK_DIR");

          if (null == sdkDir)
            throw new ApplicationException("Environment variable SCE_ORBIS_SDK_DIR not set");

          // Figure out the base address of the ELF
          ulong baseAddress = GetElfBaseAddress(listener, replay, exePath, sdkDir);

          ResolveSymbols(listener, replay, exePath, sdkDir, baseAddress);

          listener.Done();
        }
        catch (Exception ex)
        {
          listener.UpdateError(ex.Message);
        }
      });
    }

    private static ulong GetElfBaseAddress(ISymbolProgressListener listener, TraceReplayStateful replay, string elfFile, string sdkDir)
    {
      ulong wellKnownAddress = 0;

      listener.UpdateMessage("Finding symbol MemTrace::InitCommon in the ELF..");

      var procInfo = new ProcessStartInfo
      {
        Arguments = String.Format("\"{0}\"", elfFile),
        UseShellExecute = false,
        WindowStyle = ProcessWindowStyle.Hidden,
        CreateNoWindow = true,
        FileName = Path.Combine(sdkDir, @"host_tools\bin\orbis-nm.exe"),
        RedirectStandardOutput = true
      };

      using (var nm = Process.Start(procInfo))
      {
        for (; ; )
        {
          string line = nm.StandardOutput.ReadLine();

          if (null == line)
            break;

          if (line.EndsWith(s_MangledWellKnownSymbol))
          {
            wellKnownAddress = UInt64.Parse(line.Split()[0], NumberStyles.HexNumber);
            nm.Kill();
          }
        }
      }

      listener.UpdateMessage("MemTrace::InitCommon at ELF address 0x{0:x16}, runtime 0x{1:x16}", wellKnownAddress, replay.MetaData.MemTraceInitCommonAddress);

      ulong baseAddress = replay.MetaData.MemTraceInitCommonAddress - wellKnownAddress;
      listener.UpdateMessage("ELF base address set to 0x{0:x16}", baseAddress);

      return baseAddress;
    }

    private void ResolveSymbols(ISymbolProgressListener listener, TraceReplayStateful replay, string elfFile, string sdkDir, ulong elfBase)
    {
      var result = new Dictionary<ulong, SymbolInfo>();

      var procInfo = new ProcessStartInfo
      {
        // Need to merge stdout and stderr together
        Arguments = String.Format("--infile=\"{0}\" -a2l", elfFile),
        UseShellExecute = false,
        WindowStyle = ProcessWindowStyle.Hidden,
        CreateNoWindow = true,
        FileName = Path.Combine(sdkDir, @"host_tools\bin\orbis-bin.exe"),
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
        var updateThres = new TimeSpan(0, 0, 0, 0, 100);

        foreach (ulong address in replay.MetaData.Symbols)
        {
          if (x % 100 == 0 || DateTime.Now - lastUpdate > updateThres)
          {
            listener.UpdateProgress(String.Format("Resolving symbol {0}/{1}..", x + 1, replay.MetaData.Symbols.Count), (x + 1) / (double)replay.MetaData.Symbols.Count);
            lastUpdate = DateTime.Now; 
          }

          if (address != 0)
          {
            proc.StandardInput.WriteLine("0x{0:x16}", address - elfBase);
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

        sym.Address = addr + elfBase;
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

        result[addr + elfBase] = sym;
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
