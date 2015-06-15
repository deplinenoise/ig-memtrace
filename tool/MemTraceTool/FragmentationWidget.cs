using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace MemTraceTool
{
  public interface IMemoryInfo
  {
    ulong MinAddress { get; }
    ulong MaxAddress { get; }

    void GetOccupancyMask(uint[] out_bits, ulong addr_lo, ulong addr_hi);

    void GetAllocationInfo(ulong address);
  }

  public partial class FragmentationWidget : UserControl, IDisposable
  {
    public IMemoryInfo MemoryInfo { get; set; }

    public int BytesPerPixelLog2 { get; set; } 

    private Brush m_BackgroundBrush;
    private uint[] m_Buffer = new uint[1024];

    public FragmentationWidget()
    {
      m_BackgroundBrush = new SolidBrush(Color.DarkGray);
      InitializeComponent();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      var g = e.Graphics;
      Draw(g);
    }

    private Point AddressToPoint(ulong address)
    {
      var mem = MemoryInfo;
      if (null == mem)
        return new Point(0, 0);

      var sz = this.ClientSize;
      ulong w = (ulong) sz.Width;
      int bbp = BytesPerPixelLog2;

      ulong offset = (address - mem.MinAddress);
      int yoff = (int) ((offset >> bbp) / w);
      int xoff = (int) ((offset >> bbp) % w);

      return new Point(xoff, yoff);
    }

    private ulong PointToAddress(Point p)
    {
      var mem = MemoryInfo;
      var sz = this.ClientSize;

      if (mem == null || p.X < 0 || p.X >= sz.Width || p.Y <0 || p.Y >= sz.Height)
        return 0;

      int bbp = BytesPerPixelLog2;
      ulong x = (ulong) p.X;
      ulong y = (ulong) p.Y;

      ulong off = (x + y * (ulong)sz.Width) << bbp;

      return mem.MinAddress + off;
    }

    private void Draw(Graphics g)
    {
      var mem = MemoryInfo;
      var rect = g.ClipBounds;
      g.FillRectangle(m_BackgroundBrush, rect);

      if (null == mem)
        return;

      ulong start_addr = PointToAddress(new Point((int)rect.Left, (int)rect.Top));
      ulong end_addr = PointToAddress(new Point((int)rect.Right, (int)rect.Bottom));

      // Make sure the buffer is large enough.
      int bytes = (int)(end_addr - start_addr);
      int count = (bytes + 31) / 32;
      if (count > m_Buffer.Length)
        m_Buffer = new uint[count + 1024];

      mem.GetOccupancyMask(m_Buffer, start_addr, end_addr);

      for (ulong addr = start_addr; addr < end_addr; ++addr)
      {

      }
    }
  }
}
