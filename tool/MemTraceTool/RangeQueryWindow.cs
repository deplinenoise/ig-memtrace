using MemTrace;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MemTraceTool
{
  public partial class RangeQueryWindow : Form, IMemEventHandler
  {
    public RangeQueryWindow(string trace_fn)
    {
      m_FileName = trace_fn;

      InitializeComponent();
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (components != null)
        {
          components.Dispose();
        }
      }

      base.Dispose(disposing);
    }

    private ulong m_Start;
    private ulong m_End;

    private void SearchButton_Click(object sender, EventArgs e)
    {
      try
      {
        var sat = m_StartAddress.Text;
        var szt = m_SizeBytes.Text;

        var size = 0ul;

        if (sat.StartsWith("0x", true, CultureInfo.InvariantCulture))
        {
          sat = sat.Substring(2);
        }

        m_Start = Convert.ToUInt64(sat, 16);

        if (szt.StartsWith("0x", true, CultureInfo.InvariantCulture))
        {
          size = Convert.ToUInt64(szt.Substring(2), 16);
        }
        else
        {
          size = Convert.ToUInt64(szt, 10);
        }

        m_End = m_Start + size;
      }
      catch (Exception)
      {
        MessageBox.Show("Invalid number format");
        return;
      }

      m_StatusText.Text = "Replaying trace file..";
      m_SearchButton.Enabled = false;
      m_SearchResults.SetObjects(new Result[0]);
      m_SearchTask = Task.Run(() => {
        try
        {
          using (TraceReplayEventBased r = new TraceReplayEventBased(m_FileName))
          {
            r.StreamEvents(this);
            this.OnSearchComplete();
          }
        }
        catch (Exception ex)
        {
          this.OnSearchError(ex.Message);
        }
      });
    }

    private void OnSearchError(string p)
    {
      this.Invoke(new Action(() =>
      {
        m_StatusText.Text = String.Format("Error: {0}", p);
        m_ProgressBar.Value = 0;
        m_TrackedAddresses.Clear();
        m_SearchTask = null;
        m_SearchButton.Enabled = true;
      }));
    }

    private void OnSearchComplete()
    {
      this.Invoke(new Action(() =>
      {
        m_StatusText.Text = "Done";
        m_ProgressBar.Value = 0;
        m_TrackedAddresses.Clear();
        m_SearchTask = null;
        m_SearchButton.Enabled = true;
      }));
    }

    private sealed class Result
    {
      public string EventType { get; set; }
      public ulong Address { get; set; }
      public ulong Size { get; set; }
      public StackBackTrace BackTrace { get; set; }
      public double Time { get; set; }
    }

    private Dictionary<ulong, ulong> m_TrackedAddresses = new Dictionary<ulong, ulong>();
    private Task m_SearchTask;
    private string m_FileName;

    void IMemEventHandler.OnHeapAllocate(ulong ptr, ulong size, int scope_type, string scope_data_str, StackBackTrace backtrace, double time)
    {
      if (ptr + size > m_Start && ptr < m_End)
      {
        m_SearchResults.AddObject(new Result { EventType = "Allocate", Address = ptr, Size = size, BackTrace = backtrace, Time = time });
        m_TrackedAddresses.Add(ptr, size);
      }
    }

    void IMemEventHandler.OnHeapFree(ulong ptr, StackBackTrace backtrace, double time)
    {
      ulong size;
      if (m_TrackedAddresses.TryGetValue(ptr, out size))
      {
        m_SearchResults.AddObject(new Result { EventType = "Free", Address = ptr, Size = size, BackTrace = backtrace, Time = time });
        m_TrackedAddresses.Remove(ptr);
      }
    }

    void IMemEventHandler.OnProgress(double ratio)
    {
      this.Invoke(new Action(()=> {
        this.m_ProgressBar.Value = (int)Math.Max(Math.Min(ratio * this.m_ProgressBar.Maximum, this.m_ProgressBar.Maximum), 0.0);
      }));
    }

    private void OnToolTipShowing(object sender, BrightIdeasSoftware.ToolTipShowingEventArgs e)
    {
      var b = new StringBuilder();
      var f = (Result) e.Model;
      foreach (var frame in f.BackTrace.Frames)
      {
        b.AppendFormat("  {0} ({1}:{2})\r\n", frame.Symbol, frame.FileName, frame.LineNumber);
      }
      e.Title = "Stack Backtrace";
      e.Text = b.ToString();
    }

    private void OnCancel(object sender, EventArgs e)
    {

    }
  }
}
