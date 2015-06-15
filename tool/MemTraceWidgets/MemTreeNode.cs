using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemTraceTool
{
  public sealed class MemTreeNode
  {
    private Dictionary<string, MemTreeNode> m_Children = new Dictionary<string,MemTreeNode>();
    private MemTreeNode[] m_ChildArray = null;

    public MemTreeNode[] Children { get { return m_ChildArray; } }
    public bool HasChildren { get { return m_ChildArray != null; } }

    public MemTreeNode Parent { get; private set; }
    public string Key { get; private set; }
    public string FileName { get; set; }
    public int LineNumber { get; set; }
    public string Icon { get; set; }
    public long SizeBytes { get; set; }
    public ulong Count { get; set; }

    public MemTreeNode(MemTreeNode parent, string key)
    {
      Parent = parent;
      Key = key;
    }

    public MemTreeNode GetChild(string key)
    {
      MemTreeNode child;
      if (m_Children.TryGetValue(key, out child))
        return child;

      child = new MemTreeNode(this, key);
      m_Children.Add(key, child);
      return child;
    }

    public void Freeze()
    {
      if (m_Children.Count > 0)
      {
        m_ChildArray = m_Children.Values.ToArray();
        foreach (var child in m_ChildArray)
        {
          child.Freeze();
        }
      }
      else
      {
        m_ChildArray = null;
      }
      m_Children = null;
    }
  }

}
