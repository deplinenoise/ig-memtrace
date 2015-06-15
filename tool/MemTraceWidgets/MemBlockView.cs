using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace MemTrace.Widgets
{
  public partial class MemBlockView : UserControl
  {
    private List<FragAllocData> m_Allocs = null;

    private long m_LoAddress;
    private long m_HiAddress;
    private int[] m_Pixels = new int[256 * 256];
    private bool m_BitmapValid = false;
    private Bitmap m_Bitmap = new Bitmap(256, 256);

    public MemBlockView()
    {
      InitializeComponent();
    }

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      if (disposing && (m_Bitmap != null))
      {
        m_Bitmap.Dispose();
      }
      base.Dispose(disposing);
    }


    public void SetBlock(ulong start, ulong end, List<FragAllocData> allocs)
    {
      m_Allocs = allocs;
      m_LoAddress = (long) start;
      m_HiAddress = (long) end;

      m_BitmapValid = false;
      Invalidate();
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
      if (DesignMode)
      {
        base.OnPaintBackground(e);
      }
    }

    protected override void OnPaint(PaintEventArgs ev)
    {
      base.OnPaint(ev);

      var g = ev.Graphics;

      var w = Width;
      var h = Height;
      var lo = m_LoAddress;
      var hi = m_HiAddress;
      var rng = hi - lo;
      double bytes_per_row = (double) rng / (double)w;
      double bytes_per_pixel = (double)w / bytes_per_row;

      if (m_Allocs == null)
      {
        g.Clear(BackColor);
        return;
      }

      if (!m_BitmapValid)
      {
        UpdateBitmap();
      }

      g.DrawImage(m_Bitmap, 0, 0);

    }

    private void UpdateBitmap()
    {
      var pixels = m_Pixels;
      var bg = BackColor.ToArgb();

      for (int i = 0; i < pixels.Length; ++i)
      {
        pixels[i] = bg;
      }

      foreach (var a in m_Allocs)
      {
        long s = (long) a.Alloc.Address - m_LoAddress;
        long e = s + (long) a.Alloc.SizeBytes;

        s = Math.Max(s, 0);
        e = Math.Min(e, 256 * 256);

        for (; s < e; ++s)
        {
          pixels[s] = a.Color.ToArgb();
        }
      }

      var bm = m_Bitmap;
      var rect = new Rectangle(0, 0, 256, 256);

      BitmapData data = null;

      try
      {
        data = bm.LockBits(rect, ImageLockMode.ReadWrite, bm.PixelFormat);
        System.Runtime.InteropServices.Marshal.Copy(pixels, 0, data.Scan0, Math.Abs(data.Stride) * bm.Height / 4);
      }
      finally
      {
        if (data != null)
        {
          bm.UnlockBits(data);
        }
      }
    }
  }
}
