using BrightIdeasSoftware;
using MemTrace;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MemTraceTool
{
  public partial class OptionsDialog : Form
  {
    internal Options Options { get; private set; }

    private void StringListPutHelper(ObjectListView list_view, List<string> l, object row_object, object value)
    {
      for (int i = 0, count = l.Count; i < count; ++i)
      {
        if (Object.ReferenceEquals(row_object, l[i]))
        {
          l[i] = (string)value;
          break;
        }
      }
      list_view.SetObjects(l, true);
    }

    internal OptionsDialog(Options options)
    {
      Options = options.Clone();

      InitializeComponent();

      m_EnableSuppression.Checked = Options.EnableSuppression;
      m_BindAddress.Text = Options.BindAddress.ToString();
      m_BindPort.Text = Options.BindPort.ToString();

      if (null != Options.TraceDirectory)
      {
        m_TraceDirectory.Text = Options.TraceDirectory;
      }
      else
      {
        // Move to trace directory tab.
        m_Tabs.SelectTab(2);
      }

      m_SymbolColumn.AspectGetter = (object o) => { return o; };
      m_SymbolColumn.AspectPutter = (object row_object, object value) => { StringListPutHelper(m_SuppressedSymbolList, Options.SuppressedSymbols, row_object, value); };

      m_SymPathColumn.AspectGetter = (object o) => { return o; };
      m_SymPathColumn.AspectPutter = (object row_object, object value) => { StringListPutHelper(m_SymbolPathsList, Options.SymbolPaths, row_object, value); };

      m_SuppressedSymbolList.SetObjects(Options.SuppressedSymbols);
      m_SymbolMappingList.SetObjects(Options.ModulePathRemappings);
      m_SymbolPathsList.SetObjects(Options.SymbolPaths);
    }

    private void AddSymbolButton_Click(object sender, EventArgs e)
    {
      Options.SuppressedSymbols.Add("(new symbol)");
      m_SuppressedSymbolList.SetObjects(Options.SuppressedSymbols);
    }

    private void SuppressedSymbolList_SelectedIndexChanged(object sender, EventArgs e)
    {
      m_RemoveSymbolButton.Enabled = m_SuppressedSymbolList.SelectedIndex != -1;
    }

    private void RemoveSymbolButton_Click(object sender, EventArgs e)
    {
      Options.SuppressedSymbols.RemoveAt(m_SuppressedSymbolList.SelectedIndex);
      m_SuppressedSymbolList.SetObjects(Options.SuppressedSymbols);
    }

    private void EnableSuppression_CheckedChanged(object sender, EventArgs e)
    {
      Options.EnableSuppression = m_EnableSuppression.Checked;
    }

    private void OnBindAddressValidating(object sender, CancelEventArgs e)
    {
      try
      {
        IPAddress.Parse(m_BindAddress.Text);
      }
      catch (Exception)
      {
        e.Cancel = true;
      }
      HighlightValidation(!e.Cancel, m_BindAddress);
    }

    private void OnBindPortValidating(object sender, CancelEventArgs e)
    {
      try
      {
        int val = Int32.Parse(m_BindPort.Text);

        if (val < 1 || val > 65535)
          e.Cancel = true;
      }
      catch (Exception)
      {
        e.Cancel = true;
      }
      HighlightValidation(!e.Cancel, m_BindPort);
    }

    private void OnFormClosing(object sender, FormClosingEventArgs e)
    {
      try
      {
        IPAddress.Parse(m_BindAddress.Text);
        Options.BindAddress = m_BindAddress.Text;
      }
      catch (Exception)
      {
        MessageBox.Show("Bind address is not valid.");
        e.Cancel = true;
        return;
      }

      try
      {
        Options.BindPort = Int32.Parse(m_BindPort.Text);
      }
      catch (Exception)
      {
        MessageBox.Show("Bind port is not valid.");
        e.Cancel = true;
        return;
      }

      try
      {
        Options.TraceDirectory = m_TraceDirectory.Text;
        if (Options.TraceDirectory == null || !Directory.Exists(Options.TraceDirectory))
          throw new ApplicationException("blah");
      }
      catch (Exception)
      {
        MessageBox.Show("Trace directory is not valid");
        e.Cancel = true;
        return;
      }
    }

    private void BrowseButton_Click(object sender, EventArgs e)
    {
      using (var dlg = new FolderBrowserDialog())
      {
        dlg.Description = "Select Trace File Directory";
        if (DialogResult.OK == dlg.ShowDialog(this))
        {
          m_TraceDirectory.Text = dlg.SelectedPath;
        }
      }
    }

    private void HighlightValidation(bool valid, Control control)
    {
      if (!valid)
      {
        control.BackColor = Color.LightPink;
      }
      else
      {
        control.BackColor = Color.White;
      }
    }

    private void OnTraceDirectoryValidating(object sender, CancelEventArgs e)
    {
      e.Cancel = !Directory.Exists(m_TraceDirectory.Text);
      HighlightValidation(!e.Cancel, m_TraceDirectory);
    }

    private void OnAddSymbolMapping(object sender, EventArgs e)
    {
      Options.ModulePathRemappings.Add(new ModulePathRemapping
      {
        Platform = "(edit me)",
        Path = "(edit me)",
        ReplacementPath = "(edit me)",
      });
      m_SymbolMappingList.SetObjects(Options.ModulePathRemappings);
    }

    private void OnRemoveMapping(object sender, EventArgs e)
    {
      Options.ModulePathRemappings.RemoveAt(m_SymbolMappingList.SelectedIndex);
      m_SymbolMappingList.SetObjects(Options.ModulePathRemappings);
    }

    private void OnRemapSelectionChanged(object sender, EventArgs e)
    {
      m_RemoveMappingButton.Enabled = m_SymbolMappingList.SelectedIndex != -1;
    }

    private void OnAddSymbolPath(object sender, EventArgs e)
    {
      Options.SymbolPaths.Add("(edit me)");
      m_SymbolPathsList.SetObjects(Options.SymbolPaths);
    }

    private void OnSymbolPathSelectionChanged(object sender, EventArgs e)
    {
      m_RemovePathButton.Enabled = m_SymbolPathsList.SelectedIndex != -1;
    }

    private void OnRemoveSymbolPath(object sender, EventArgs e)
    {
      Options.SymbolPaths.RemoveAt(m_SymbolPathsList.SelectedIndex);
      m_SymbolPathsList.SetObjects(Options.SymbolPaths);
    }
  }
}
