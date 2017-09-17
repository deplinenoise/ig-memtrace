using MemTrace;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MemTraceTool
{
  public partial class DeltaWindow : Form
  {
    private TraceReplayStateful m_Trace;
    private Options m_Options;

    internal DeltaWindow(string fn, Options options)
    {
      InitializeComponent();

      m_Trace = new TraceReplayStateful(fn);
      m_Options = options;

      TimeControlHelpers.CreateTimeControlMarks(m_StartTimeControl, m_Trace);
      TimeControlHelpers.CreateTimeControlMarks(m_EndTimeControl, m_Trace);

      m_Perspective.Items.AddRange(TreePerspective.AllItems);
      m_Perspective.SelectedIndex = 0;
    }

    protected override void OnClosed(EventArgs e)
    {
      m_Trace.Dispose();
    }

    private void OnRefreshButtonClick(object sender, EventArgs e)
    {
      m_RefreshButton.Enabled = false;
      m_StartTimeControl.Enabled = false;
      m_EndTimeControl.Enabled = false;

      var perspective = (TreePerspective) m_Perspective.SelectedItem;
      var startTime = m_StartTimeControl.CurrentTime;
      var endTime = m_EndTimeControl.CurrentTime;

      Task.Run(() => { this.UpdateDeltaViewAsync(startTime, endTime, perspective); });
    }

    private void UpdateDeltaViewAsync(double startTime, double endTime, TreePerspective perspective)
    {
      this.Invoke((Action)delegate() { m_StripStatus.Text = "Replaying events.."; });

      TraceReplayStateful.StatusDelegate replayCallback = (double ratio) =>
      {
        this.Invoke((Action)delegate()
        {
          m_ReplayProgress.Value = (int)Math.Round(1000.0 * ratio);
        });
      };

      TreeBuilderBase tree_builder;
      tree_builder = new DeltaTreeBuilder(m_Trace, startTime, endTime, replayCallback, perspective, m_Options);

      this.Invoke((Action)delegate() { m_StripStatus.Text = "Building report tree.."; });

      var root = tree_builder.BuildReportTree();

      this.Invoke((Action)delegate()
      {
        OnTreeUpdated(root);
      });
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

    private void OnTreeUpdated(MemTreeNode root)
    {
      m_HeapTree.SetRoot(root);
      m_StripStatus.Text = "Ready";
      m_ReplayProgress.Value = 0;
      m_RefreshButton.Enabled = true;
      m_StartTimeControl.Enabled = true;
      m_EndTimeControl.Enabled = true;
    }
  }
}
