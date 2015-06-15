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
  public partial class WarningWindow : Form
  {
    public void SetWarnings(ICollection<String> data)
    {
      m_TextBox.Clear();
      m_TextBox.Text = String.Join("\r\n", data);
      m_TextBox.Select(0, -1);
    }

    public WarningWindow()
    {
      InitializeComponent();
    }
  }
}
