using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MemTrace.Widgets
{
  public partial class TimeControl : UserControl
  {
    public double MinTime { get; set; }
    public double MaxTime { get; set; }

    public string TimeLabel
    {
      get { return m_Label.Text; }
      set { m_Label.Text = value; }
    }

    public double CurrentTime
    {
      get; private set;
    }

    public delegate void TimeChangedDelegate(object sender, TimeChangedEvent e);
    public event TimeChangedDelegate TimeChanged;

    public sealed class Mark
    {
      public string Name;
      public double Time;

      public override string ToString()
      {
        return String.Format("{1:0.000}s ({0})", Name, Time);
      }
    }

    public void SetMarks(ICollection<Mark> marks)
    {
      m_Time.Items.Clear();
      m_Time.Items.AddRange(marks.ToArray());
    }

    public TimeControl()
    {
      InitializeComponent();
      m_Time.Text = "0.000";
    }

    private void OnRev2Button_Click(object sender, EventArgs e)
    {
      CurrentTime = Math.Max(0, CurrentTime - 1.0);
      UpdateTime();
    }

    private void OnRev1Button_Click(object sender, EventArgs e)
    {
      CurrentTime = Math.Max(0, CurrentTime - 0.033);
      UpdateTime();
    }

    private void OnFwd1Button_Click(object sender, EventArgs e)
    {
      CurrentTime = Math.Min(MaxTime, CurrentTime + 0.033);
      UpdateTime();
    }

    private void OnFwd2Button_Click(object sender, EventArgs e)
    {
      CurrentTime = Math.Min(MaxTime, CurrentTime + 1.0);
      UpdateTime();
    }

    private void OnComboKeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyValue == (int) Keys.Enter || e.KeyValue == (int) Keys.Return)
      {
        try
        {
          CurrentTime = Double.Parse(m_Time.Text);
        }
        catch (Exception)
        {
        }
        UpdateTime();
      }
    }

    private void OnTimeValueSelected(object sender, EventArgs e)
    {
      var item = (Mark) m_Time.SelectedItem;
      CurrentTime = item.Time;
      UpdateTime();
    }

    private void UpdateTime()
    {
      var ev = new TimeChangedEvent { Time = CurrentTime };
      if (null != TimeChanged)
      {
        TimeChanged(this, ev);
      }
      CurrentTime = ev.Time;
      BeginInvoke(new Action(() => m_Time.Text = String.Format("{0:0.000}", CurrentTime)));
    }
  }

  public sealed class TimeChangedEvent
  {
    public double Time { get; set; }
  }

}
