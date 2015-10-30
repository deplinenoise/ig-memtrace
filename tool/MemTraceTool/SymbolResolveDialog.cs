using MemTrace;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MemTraceTool
{
  public partial class SymbolResolveDialog : Form, ISymbolProgressListener
  {
    private TraceListItem m_Item;
    private TraceReplayStateful m_Replay;
    private Options m_Options;
    private volatile bool m_Done;

    internal SymbolResolveDialog(TraceListItem trace, Options options)
    {
      InitializeComponent();

      m_Options = options;
      m_Item = trace;
      m_Replay = new TraceReplayStateful(trace.FileName);

    }

    private ISymbolResolver CreateResolver(TraceReplayStateful m_Replay)
    {
      var platformName = m_Replay.MetaData.PlatformName;
      switch (platformName)
      {
        case "Windows":
        case "Durango":
          return new DbgHelpSymbolResolver();

        case "Orbis":
          return new OrbisSymbolResolver();
        case "PS3":
          return new PS3SymbolResolver();
        default:
          return null;
      }
    }

    private List<string> m_LogLines = new List<string>();

    private void OnFormClosed(object sender, FormClosedEventArgs e)
    {
      m_Replay.Dispose();
    }

    private void OnFormClosing(object sender, FormClosingEventArgs e)
    {
      if (!m_Done)
      {
        e.Cancel = true;
        return;
      }
    }

    public TraceMeta UpdatedMeta { get { return m_Replay.MetaData; } }

    public void UpdateProgress(string status, double pct)
    {
      this.Invoke((Action)delegate() {
        m_ProgressBar.Value = (int) Math.Round(pct * m_ProgressBar.Maximum);
        m_StatusLabel.Text = status;
      });
    }

    public void UpdateMessage(string status, params object[] args)
    {
      this.Invoke((Action)delegate() {
        m_LogLines.Add(String.Format(status, args));
        m_LogText.Lines = m_LogLines.ToArray();
        m_LogText.SelectionStart = m_LogText.Text.Length;
        m_LogText.ScrollToCaret();
        m_LogText.Refresh();
      });
    }

    public void Done()
    {
      m_Done = true;
      UpdateProgress("Done", 1.0);
    }


    public void UpdateError(string p)
    {
      UpdateMessage("ERROR: " + p);
      Done();
    }

    private void OnFormShown(object sender, EventArgs e)
    {
      ISymbolResolver resolver = CreateResolver(m_Replay);
      if (resolver != null)
      {
        if (resolver.NeedsExePath)
        {
          using (var fd = new OpenFileDialog())
          {
            fd.Title = "Select Executable File";
            fd.CheckFileExists = true;
            var ext = resolver.ExeExtension;
            fd.Filter = "Executable Files (*" + ext + ")|*" + ext + "|All files(*.*)|*.*";

            switch (fd.ShowDialog())
            {
              case DialogResult.OK:
                resolver.BeginResolve(fd.FileName, this, m_Replay, m_Options.SymbolPaths, m_Options.ModulePathRemappings);
                break;
              default:
                m_Done = true;
                this.Close();
                break;
            }
          }
        }
        else
        {
          resolver.BeginResolve(null, this, m_Replay, m_Options.SymbolPaths, m_Options.ModulePathRemappings);
        }
      }
      else
      {
        MessageBox.Show("Sorry, don't know how to resolve symbols for this platform yet");
        m_Done = true;
      }
    }
  }
}
