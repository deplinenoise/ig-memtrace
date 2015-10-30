using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MemTraceTool;

namespace MemTrace.Widgets
{
  public partial class HeapTreeList : UserControl
  {
    public HeapTreeList()
    {
      InitializeComponent();

      // Hook up treelist view stuff.
      m_TreeList.CanExpandGetter = (object o) =>
      {
        return ((MemTreeNode)o).HasChildren;
      };
      m_TreeList.ChildrenGetter = (object o) =>
      {
        return ((MemTreeNode)o).Children;
      };
      m_KeyColumn.ImageGetter = (object o) =>
      {
        return ((MemTreeNode)o).Icon;
      };
      m_LineNumberColumn.AspectGetter = (object o) =>
      {
        int value = ((MemTreeNode)o).LineNumber;
        if (value == 0)
          return null;
        return value;
      };
      m_SizeColumn.AspectToStringConverter = (object o) =>
      {
        long size = (long)o;
        long abs = Math.Abs(size);
        if ((abs >> 30) > 0)
        {
          return String.Format("{0:n2} GB", size / (1024.0 * 1024.0 * 1024.0));
        }
        if ((abs >> 20) > 0)
        {
          return String.Format("{0:n2} MB", size / (1024.0 * 1024.0));
        }
        if ((abs >> 10) > 0)
        {
          return String.Format("{0:n2} KB", size / (1024.0));
        }
        return String.Format("{0:n0} B", size);
      };
    }
          
    public event EventHandler CellDblClick
    {
        add
        {
            m_TreeList.DoubleClick += value;
        }
        remove
        {
            m_TreeList.DoubleClick -= value;
        }
    }

    public MemTreeNode SelectedNode
    {
      get { return (MemTreeNode)m_TreeList.SelectedObject; }
    }

    public void SetRoot(MemTreeNode root)
    {
      m_TreeList.Roots = new object[] { root };
      m_TreeList.RebuildAll(true);
      m_TreeList.Expand(root);
    }
  }
}
