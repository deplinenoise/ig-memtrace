using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using MemTrace;

namespace MemTraceTool
{
  sealed class Options
  {
    private static readonly string RegKeyBase = @"Software\Insomniac Games\Trace Viewer";

    public List<string> SuppressedSymbols { get; private set; }
    public bool EnableSuppression { get; set; }
    public string BindAddress { get; set; }
    public int BindPort { get; set; }
    public string TraceDirectory { get; set; }
    public List<string> SymbolPaths { get; private set; }
    public List<ModulePathRemapping> ModulePathRemappings { get; private set; }

    public Options()
    {
      SuppressedSymbols = new List<string>();
      ModulePathRemappings = new List<ModulePathRemapping>();
      SymbolPaths = new List<string>();
      ModulePathRemappings = new List<ModulePathRemapping>();
      SymbolPaths = new List<string>();
      BindAddress = "0.0.0.0";
      BindPort = 9811;
    }

    public Options Clone()
    {
      var result = new Options();
      result.SuppressedSymbols.AddRange(SuppressedSymbols);
      result.EnableSuppression = EnableSuppression;
      result.BindAddress = BindAddress;
      result.BindPort = BindPort;
      result.TraceDirectory = TraceDirectory;
      result.SymbolPaths.AddRange(SymbolPaths);
      foreach (var m in ModulePathRemappings)
      {
        result.ModulePathRemappings.Add(m.Clone());
      }
      return result;
    }
    
    private static string GetSingle(RegistryKey key, string name, string default_value)
    {
      object val = key.GetValue(name);
      string sval = val as string;

      if (sval != null)
      {
        return sval;
      }
      return default_value;
    }

    private static string[] GetMulti(RegistryKey key, string name)
    {
      object vals = key.GetValue(name);
      string[] array = vals as string[];

      if (array != null)
      {
        return array;
      }
      return new string[] { };
    }

    public bool LoadFromRegistry()
    {
      using (var key = Registry.CurrentUser.OpenSubKey(RegKeyBase))
      {
        if (null == key)
          return false;

        SuppressedSymbols.AddRange(GetMulti(key, "SuppressedSymbols"));
        EnableSuppression = Boolean.Parse(GetSingle(key, "EnableSuppression", "false"));
        BindAddress = GetSingle(key, "BindAddress", "0.0.0.0");
        BindPort = Int32.Parse(GetSingle(key, "BindPort", "9811"));
        TraceDirectory = GetSingle(key, "TraceDirectory", null);
        foreach (var s in GetMulti(key, "ModulePathRemappings"))
        {
          var elems = s.Split('|');
          ModulePathRemappings.Add(new ModulePathRemapping
          {
            Platform = elems[0],
            Path = elems[1],
            ReplacementPath = elems[2]
          });
        }
        SymbolPaths.AddRange(GetMulti(key, "SymbolPaths"));
        return true;
      }
    }

    public void SaveToRegistry()
    {
      using (var key = Registry.CurrentUser.CreateSubKey(RegKeyBase))
      {
        key.SetValue("SuppressedSymbols", SuppressedSymbols.ToArray());
        key.SetValue("EnableSuppression", EnableSuppression.ToString());
        key.SetValue("BindAddress", BindAddress.ToString());
        key.SetValue("BindPort", BindPort.ToString());
        key.SetValue("TraceDirectory", TraceDirectory.ToString());
        var vals = new List<string>();
        foreach (var rm in ModulePathRemappings)
        {
          vals.Add(String.Format("{0}|{1}|{2}", rm.Platform, rm.Path, rm.ReplacementPath));
        }
        key.SetValue("ModulePathRemappings", vals.ToArray());
        key.SetValue("SymbolPaths", SymbolPaths.ToArray());
      }
    }

    public static Options LoadFromFile(string fn)
    {
      return (Options) JsonConvert.DeserializeObject(File.ReadAllText(fn), typeof(Options));
    }

    public void SaveToFile(string fn)
    {
      string data = JsonConvert.SerializeObject(this, typeof(Options), new JsonSerializerSettings { Formatting = Formatting.Indented } );
      File.WriteAllText(fn, data, Encoding.UTF8);
    }
  }
}
