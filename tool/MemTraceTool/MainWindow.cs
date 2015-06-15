using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MemTrace;
using System.IO;
using System.Net;

namespace MemTraceTool
{
  public partial class MainWindow : Form, ITraceFileHandler
  {
    private readonly List<TraceListItem> m_TraceItems = new List<TraceListItem>();
    private readonly TraceListener m_Listener;
    private Options m_Options = new Options();

    public MainWindow()
    {
      InitializeComponent();

      SetupListView();

      if (!m_Options.LoadFromRegistry())
      {
        // Load default settings.
        string dir = Path.GetDirectoryName(typeof(MainWindow).Assembly.Location);
        string defaults = Path.Combine(dir, @"DefaultOptions.json");

        m_Options = Options.LoadFromFile(defaults);

        // Show options to allow user to set directory.
        using (var dlg = new OptionsDialog(m_Options))
        {
          if (DialogResult.OK == dlg.ShowDialog(this))
          {
            m_Options = dlg.Options;
          }
        }
      }

      m_Listener = new TraceListener(this);
      m_Listener.BindAddress = IPAddress.Parse(m_Options.BindAddress);
      m_Listener.BindPort = m_Options.BindPort;
      m_Listener.TraceDirectory = m_Options.TraceDirectory;

      m_Listener.Start();

      ScanTraceDirectory();
    }

    private static string FormatSize(long size)
    {
        if (size >= 1024L * 1024L * 1024L)
          return String.Format("{0:n2} GB", size / (1024.0 * 1024.0 * 1024.0));
        if (size >= 1024L * 1024L)
          return String.Format("{0:n2} MB", size / (1024.0 * 1024.0));
        if (size >= 1024L)
          return String.Format("{0:n2} KB", size / (1024.0));

        return String.Format("{0:n0} B", size);
    }

    private void SetupListView()
    {
      m_FileNameColumn.AspectGetter = (object o) =>
      {
        return Path.GetFileName(((TraceListItem)o).FileName);
      };
      m_FileNameColumn.AspectPutter = (object o, object value) =>
      {
        var item = (TraceListItem)o;
        var name = value.ToString();

        if (!name.EndsWith(".mtrace"))
          name += ".mtrace";

        var dir = Path.GetDirectoryName(item.FileName);
        var newPath = Path.Combine(dir, name);

        try
        {
          File.Move(item.FileName, newPath);
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message, "Failed to rename file");
          return;
        }
        item.FileName = newPath;

        m_TraceList.UpdateObject(item);
      };
      m_WireSizeColumn.AspectGetter = (object o) =>
      {
        return FormatSize(((TraceListItem)o).WireSize);
      };
      m_OutputSizeColumn.AspectGetter = (object o) =>
      {
        return FormatSize(((TraceListItem)o).OutputSize);
      };

      m_TraceList.SetObjects(m_TraceItems);
    }

    object ITraceFileHandler.OnRecordingStarted(string filename, TraceMeta meta, TraceRecorder recorder)
    {
      var item = new TraceListItem(filename, meta, recorder);
      this.Invoke((Action)delegate()
        {
          m_TraceItems.Add(item);
          m_TraceList.SetObjects(m_TraceItems);
        });
      return item;
    }

    void ITraceFileHandler.OnRecordingProgress(object context, TraceMeta meta)
    {
      this.BeginInvoke((Action)delegate()
      {
        m_TraceList.UpdateObject(context);
      });
    }

    void ITraceFileHandler.OnRecordingEnded(object context)
    {
      this.BeginInvoke((Action)delegate()
      {
        var item = (TraceListItem)context;
        item.Recorder = null;
        m_TraceList.UpdateObject(context);
        if (m_TraceList.SelectedObject == item)
        {
          TraceList_ItemSelectionChanged(m_TraceList, null);
        }
      });
    }

    private void ChangeOptionsMenuItem_Click(object sender, EventArgs e)
    {
      using (var dlg = new OptionsDialog(m_Options))
      {
        if (DialogResult.OK == dlg.ShowDialog(this))
        {
          m_Options = dlg.Options;
          m_Listener.TraceDirectory = m_Options.TraceDirectory;
          ScanTraceDirectory();
        }
      }
    }

    private void ScanTraceDirectory()
    {
      HashSet<string> paths = new HashSet<string>();

      for (int i = 0; i < m_TraceItems.Count; )
      {
        var item = m_TraceItems[i];
        if (item.Status == TraceStatus.Ready && !Path.Equals(Path.GetDirectoryName(item.FileName), m_Options.TraceDirectory))
        {
          m_TraceItems.RemoveAt(i);
        }
        else
        {
          paths.Add(item.FileName.ToLowerInvariant());
          ++i;
        }
      }

      foreach (var fn in Directory.GetFiles(m_Options.TraceDirectory, "*.mtrace", SearchOption.TopDirectoryOnly))
      {
        if (paths.Contains(fn.ToLowerInvariant()))
          continue; // Skip files being recorded already

        try
        {
          using (var replay = new TraceReplayStateful(fn))
          {
            var item = new TraceListItem(fn, replay.MetaData, null);
            m_TraceItems.Add(item);
          }
        }
        catch (Exception ex)
        {
          if (DialogResult.Yes == MessageBox.Show(this, "Failed to open " + fn + "\n\n" + ex.Message + "\n\nDelete it?", "Error", MessageBoxButtons.YesNo))
          {
            try
            {
              File.Delete(fn);
            }
            catch (IOException)
            {
            }
          }
        }

      }

      m_TraceList.SetObjects(m_TraceItems);
      m_TraceFileLabel.Text = "Trace Files in " + m_Options.TraceDirectory;
    }

    private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
    {
      m_Options.SaveToRegistry();
    }

    private void HeapViewButton_Click(object sender, EventArgs e)
    {
      var trace = m_TraceItems[m_TraceList.SelectedIndex];
      var window = new HeapWindow(trace, m_Options);
      window.Show();
    }

    private void TraceList_ItemSelectionChanged(object sender, EventArgs e)
    {
      int index = m_TraceList.SelectedIndex;
      var item = index >= 0 ? m_TraceItems[index] : null;
      m_HeapViewButton.Enabled = item != null && item.Status == TraceStatus.Ready;
      m_ResolveSymbolsButton.Enabled = item != null && item.Status == TraceStatus.Ready;
      m_SearchButton.Enabled = item != null && item.Status == TraceStatus.Ready;
      m_DeltaButton.Enabled = item != null && item.Status == TraceStatus.Ready;
    }

    private void ResolveSymbolsButton_Click(object sender, EventArgs e)
    {
      int index = m_TraceList.SelectedIndex;
      var item = m_TraceItems[index];

      using (var dlg = new SymbolResolveDialog(item, m_Options))
      {
        dlg.ShowDialog();
        item.Meta = dlg.UpdatedMeta;
        m_TraceList.UpdateObject(item);
      }
    }

    private void OnCellRightClick(object sender, BrightIdeasSoftware.CellRightClickEventArgs e)
    {
      if (e.Model != null)
      {
        var item = (TraceListItem) e.Model;
        m_StopRecordingMenuItem.Enabled = item.Status == TraceStatus.Recording;
        m_AddUserTraceMarkMenuItem.Enabled = item.Status == TraceStatus.Recording;
        m_ShowWarningsMenuItem.Enabled = item.Status == TraceStatus.Ready && item.WarningCount > 0;
        e.MenuStrip = m_NodeContextMenu;
      }
    }

    private void StopRecordingMenuItem_Click(object sender, EventArgs e)
    {
      var item = m_TraceItems[m_TraceList.SelectedIndex];
      item.Recorder.Cancel();
    }

    private void SearchButton_Click(object sender, EventArgs e)
    {
      var o = (TraceListItem)m_TraceList.SelectedObject;
      var w = new RangeQueryWindow(o.FileName);
      w.Show();
    }

    private void m_QuitMenuItem_Click(object sender, EventArgs e)
    {
      Application.Exit();
    }

    private void DeltaButton_Click(object sender, EventArgs e)
    {
      var o = (TraceListItem)m_TraceList.SelectedObject;
      var w = new DeltaWindow(o.FileName, m_Options);
      w.Show();
    }

    private void AddUserTraceMarkMenuItem_Click(object sender, EventArgs e)
    {
      var item = m_TraceItems[m_TraceList.SelectedIndex];
      item.Recorder.AddTraceMarkFromUI();
    }

    private void ShowWarningsMenuItem_Click(object sender, EventArgs e)
    {
      var form = new WarningWindow();
      var o = (TraceListItem)m_TraceList.SelectedObject;
      form.SetWarnings(o.Warnings);
      form.Show();
    }
  }
}
