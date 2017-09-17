using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using MemTrace;
using System.Threading;
using MemTrace.Widgets;
using System.Diagnostics;

namespace MemTraceTool
{
  public partial class HeapWindow : Form, IFragmentationData
  {
    private TraceReplayStateful m_Trace;
    private Options m_Options;

    internal HeapWindow(TraceListItem trace_info, Options options)
    {
      m_Options = options;
      m_Trace = new TraceReplayStateful(trace_info.FileName);

      InitializeComponent();

      InitTree();

      InitFragmentationView();

      // Set window title.
      Text = String.Format("{0} ({2} - {1:n0} events) - MemTrace Heap View", Path.GetFileName(m_Trace.FileName), m_Trace.MetaData.EventCount, m_Trace.MetaData.PlatformName);
    }

    private void InitFragmentationView()
    {
      m_FragWidget.FragmentationData = this;
      m_FragPanel.VerticalScroll.Visible = true;
      m_FragAddressColumn.AspectGetter = (object o) => { return String.Format("{0:x16}", ((FragAllocData)o).Alloc.Address); };
      m_FragSizeColumn.AspectGetter = (object o) => { return String.Format("{0:n0}", ((FragAllocData)o).Alloc.SizeBytes); };
      m_FragColorColumn.AspectGetter = (object o) => { return ""; };
      m_FragAgeColumn.AspectGetter = (object o) => { return String.Format("{0:n2}s", m_TimeControl.CurrentTime - ((FragAllocData)o).Alloc.TimeCreated); };
    }

    private void InitTree()
    {
      // Init perspective list.
      m_Perspective.Items.AddRange(TreePerspective.AllItems);
      m_Perspective.SelectedIndex = 0;

      TimeControlHelpers.CreateTimeControlMarks(m_TimeControl, m_Trace);
    }

    private void UpdateTree()
    {
      m_RefreshButton.Enabled = false;
      m_TimeControl.Enabled = false;

      var perspective = (TreePerspective) m_Perspective.SelectedItem;
      var time = m_TimeControl.CurrentTime;

      Task.Run(() =>
      {
        try
        {
          TraceReplayStateful.StatusDelegate replay_callback = (double ratio) =>
          {
            this.Invoke((Action)delegate()
            {
              m_ReplayProgress.Value = (int)Math.Round(1000.0 * ratio);
            });
          };

          this.Invoke((Action)delegate() { m_StripStatus.Text = "Replaying events.."; });

          TreeBuilderBase tree_builder;
          tree_builder = new SnapshotTreeBuilder(m_Trace, time, replay_callback, perspective, m_Options);

          this.Invoke((Action)delegate() { m_StripStatus.Text = "Building report tree.."; });

          var root = tree_builder.BuildReportTree();

          this.Invoke((Action)delegate()
          {
            OnTreeUpdated(root);
          });
        }
        catch (Exception ex)
        {
          this.Invoke((Action)delegate()
          {
            // Display an error and give up on this replay.
            MessageBox.Show(this, ex.Message, "Error");
            m_ReplayProgress.Value = 0;
            m_StripStatus.Text = "Error - badly structured stream. Fix the client application!";
          });
        }
      });
    }

    private class HeapComboItem
    {
      public string Name;
      public ulong StartAddress;
      public ulong EndAddress;

      public override string ToString()
      {
        return Name;
      }
    }

    private void OnTreeUpdated(MemTreeNode root)
    {
      m_HeapTree.SetRoot(root);
      m_StripStatus.Text = "Ready";
      m_ReplayProgress.Value = 0;
      m_RefreshButton.Enabled = true;
      m_TimeControl.Enabled = true;

      string selected_name = null;
      if (m_HeapCombo.SelectedItem != null)
      {
        selected_name = ((HeapComboItem)m_HeapCombo.SelectedItem).Name;
      }

      // Update heap combo box.
      m_HeapCombo.Items.Clear();

      object new_sel_item = null;

      foreach (var heap in m_Trace.Heaps.Values)
      {
        int r = 1;
        foreach (var range in heap.CoreRanges)
        {
          var item = new HeapComboItem
          {
            Name = String.Format("{0} (R:{1})", heap.Name, r++),
            StartAddress = range.BaseAddress,
            EndAddress = range.BaseAddress + range.SizeBytes,
          };
          m_HeapCombo.Items.Add(item);

          if (null != selected_name && item.Name == selected_name)
          {
            new_sel_item = item;
          }
        }
      }

      if (new_sel_item != null)
      {
        m_HeapCombo.SelectedItem = new_sel_item;
      }

      m_FragWidget.OnRangeUpdated();
    }

    private ContextMenuStrip GetRightClickMenu(MemTreeNode node)
    {
      if (node == null || node.Icon != "symbol")
        return null;

      return m_SymbolContextMenu;
    }

    private void OnCellRightClick(object sender, BrightIdeasSoftware.CellRightClickEventArgs e)
    {
      e.MenuStrip = GetRightClickMenu((MemTreeNode)e.Model);
    }

    private void ExcludeSymbolMenuItem_Click(object sender, EventArgs e)
    {
      var node = m_HeapTree.SelectedNode;

      if (node != null)
      {
        throw new ApplicationException("Implement me again");
        //m_Settings.SuppressedSymbols.Add(new SuppressedSymbol(node.Key));
        //m_SettingsChanged = true;
        //UpdateTree();
      }
    }

    private void RefreshMenuItem_Click(object sender, EventArgs e)
    {
      UpdateTree();
    }

    private void RefreshButton_Click(object sender, EventArgs e)
    {
      UpdateTree();
    }

    private void OnFormatRow(object sender, BrightIdeasSoftware.FormatRowEventArgs e)
    {
      var item = (MemTreeNode)e.Model;
      if (item.SizeBytes < 0)
      {
        e.Item.BackColor = Color.LightPink;
      }
    }

    private void OnFormClosed(object sender, FormClosedEventArgs e)
    {
      m_Trace.Dispose();
    }

    private void label2_Click(object sender, EventArgs e)
    {

    }

    private void m_Perspective_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    private void OnTimeChanged(object sender, TimeChangedEvent e)
    {
      UpdateTree();
    }

    private void HeapTree_CellDblClick(object sender, EventArgs e)
    {
        MemTreeNode selectedNode = m_HeapTree.SelectedNode;

        if (selectedNode == null)
            return;              

        String filename = selectedNode.FileName;        
        int fileline = selectedNode.LineNumber;
        
        VisualStudioHelper.OpenVisualStudioByFileLine(filename, fileline);
    }

    public void GetAllocations(List<FragAllocData> data, ulong addr_lo, ulong addr_hi)
    {
      // Find lower bound.
      var sa = m_Trace.AllocationsByAddress;

      if (sa.Count == 0)
        return;

      var key_lo = HeapAllocationInfo.CreateSearchKey(addr_lo);
      var key_hi = HeapAllocationInfo.CreateSearchKey(addr_hi);
      int index_lo = sa.BinarySearch(key_lo, new HeapAllocationInfo.EndComparer());
      int index_hi = sa.BinarySearch(key_hi, new HeapAllocationInfo.StartComparer());

      if (index_lo < 0)
      {
        index_lo = (~index_lo);
      }

      if (index_hi < 0)
      {
        index_hi = (~index_hi);
      }

      index_lo = Math.Min(Math.Max(0, index_lo), sa.Count - 1);
      index_hi = Math.Min(Math.Max(0, index_hi), sa.Count);

      for (int i = index_lo; i < index_hi; ++i)
      {
        ulong col = 0xefefefef;
        var a = sa[i];
        foreach (var frame in a.BackTrace.Frames)
        {
          col = ((col << 60) | (col >> 4)) ^ frame.Address;
        }
        int r = Math.Min((int) (col & 0xff), 0xc0);
        int g = Math.Min((int) (col & 0xff00) >> 8, 0xc0);
        int b = Math.Min((int) (col & 0xff0000) >> 16, 0xc0);
        data.Add(new FragAllocData { Alloc = a, Color = Color.FromArgb(255, (int) col&0xff, (int) (col >>8) & 0xff, (int) (col >> 16) & 0xff) });
      }
    }

    private void OnValidateHex(object sender, CancelEventArgs e)
    {
      ulong ignored;
      e.Cancel = !TryConvertHex64(((TextBox)sender).Text, out ignored);
    }

    private bool TryConvertHex64(string s, out ulong val)
    {

      if (s.StartsWith("0x"))
        s = s.Substring(2);

      try
      {
        val = Convert.ToUInt64(s, 16);
        return true;
      }
      catch (Exception)
      {
        val = 0;
        return false;
      }
    }

    private void OnHeapComboCommit(object sender, EventArgs e)
    {
      var item = (HeapComboItem) m_HeapCombo.SelectedItem;

      if (item != null)
      {
        m_FragWidget.MinAddress = item.StartAddress;
        m_FragWidget.MaxAddress = item.EndAddress;
        m_FragWidget.OnRangeUpdated();
      }
    }

    private void OnFragBlockSelected(object sender, ulong start, ulong end)
    {
      m_BlockVisualizerLabel.Text = String.Format("Block Visualizer ({0:x16} - {1:x16})", start, end);

      var allocs = new List<FragAllocData>();
      GetAllocations(allocs, start, end);
      m_BlockView.SetBlock(start, end, allocs);
      m_AllocInfo.SetObjects(allocs);
    }

    private void OnFormatFragCell(object sender, BrightIdeasSoftware.FormatCellEventArgs e)
    {
      if (e.ColumnIndex == m_FragColorColumn.Index)
      {
        e.SubItem.BackColor = ((FragAllocData)e.Model).Color;
      }
    }

    private void OnAllocSelectionChanged(object sender, EventArgs e)
    {
      if (-1 == m_AllocInfo.SelectedIndex)
      {
        m_CallstackList.ClearObjects();
      }
      else
      {
        var data = (FragAllocData)m_AllocInfo.SelectedObject;
        m_CallstackList.SetObjects(data.Alloc.BackTrace.Frames);
      }
    }

    private void OnFormShown(object sender, EventArgs e)
    {
      // Generate initial tree view.
      UpdateTree();
    }
  }
}
