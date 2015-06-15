using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace MemTrace.Widgets
{
  public partial class FragmentationWidget : UserControl, IDisposable
  {
    public IFragmentationData FragmentationData { get; set; }

    const int DotSize = 9;
    const int DotBytes = 65536;
    const int Thres0 = 60000;
    const int Thres1 = 30000;
    const int Thres2 = 10000;

    public ulong MinAddress { get; set; }
    public ulong MaxAddress { get; set; }

    private ulong m_SelectedAddress = 0;

    public delegate void BlockSelectedHandler(object sender, ulong start, ulong end);

    public event BlockSelectedHandler OnBlockSelected;

    public FragmentationWidget()
    {
      InitializeComponent();
    }

    protected override void OnClick(EventArgs e)
    {
      var me = e as MouseEventArgs;
      if (me != null)
      {
        if (0 != (me.Button & MouseButtons.Left))
        {
          int x = me.X;
          int y = me.Y;

          long dx = x / DotSize;
          long dy = y / DotSize;
          int xdot_count = Width / DotSize;
          long doti = xdot_count * dy + dx;
          ulong offset = (ulong) (doti * DotBytes);
          ulong addr = MinAddress + offset;
          if (addr >= MinAddress && addr < MaxAddress)
          {
            m_SelectedAddress = addr;
            if (OnBlockSelected != null)
            {
              OnBlockSelected(this, addr, addr + DotBytes);
            }
          }
          else
          {
            m_SelectedAddress = 0;
          }

          Invalidate();
        }
      }
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
      // We don't want it unless we're designing.
      if (DesignMode)
      {
        base.OnPaintBackground(e);
      }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      var g = e.Graphics;
      Draw(g);
    }

    private Bitmap m_Bitmap;

    private int AddressToDotIndex(long addr, long min)
    {
      long offset = addr - min;
      offset /= DotBytes;
      return (int) offset;
    }

    private int GetBlockOverlapSize(long block_lo, long block_hi, long range_lo, long range_hi)
    {
      long a = Math.Max(block_lo, range_lo);
      long b = Math.Min(block_hi, range_hi);
      long diff = b - a;
      if (diff < 0)
        diff = 0;
      return (int)diff;
    }

    private Task UpdateBitmap()
    {
      var min = (long) MinAddress;
      var max = (long) MaxAddress;
      int total_dot_count = (int) ((max - min + DotBytes - 1) / DotBytes);
      int xdot_count = Width / DotSize;
      int row_count = (total_dot_count + xdot_count - 1) / xdot_count;

      if (0 == row_count)
        row_count++;

      var bw = Width;
      var bh = DotSize * row_count;

      // Query for allocations in the range.
      var allocs = new List<FragAllocData>();
      FragmentationData.GetAllocations(allocs, MinAddress, MaxAddress);

      return Task.Run(() =>
      {
        try
        {
          var bm = new Bitmap(bw, bh);

          using (var bg = Graphics.FromImage(bm))
          {
            bg.Clear(this.BackColor);

            // Create histogram for each dot.
            int hdots = bw / DotSize;
            int vdots = bh / DotSize;
            long span = max - min;

            uint[] hist = new uint[hdots * vdots];
            
            foreach (var a in allocs)
            {
              long lo = (long) a.Alloc.Address - min;
              long hi = (long) (a.Alloc.Address + a.Alloc.SizeBytes) - min;
              long dot0 = lo / DotBytes;
              long dot1 = hi / DotBytes;

              dot0 = Math.Min(Math.Max(0, dot0), hist.Length-1);
              dot1 = Math.Min(Math.Max(0, dot1), hist.Length-1);

              long bstr = dot0 * DotBytes;

              for (long dot = dot0; dot <= dot1; ++dot, bstr += DotBytes)
              {
                // Figure out how much it contributes to the dot in terms of bytes allocated.
                int overlap = GetBlockOverlapSize(bstr, bstr + DotBytes, lo, hi);
                hist[dot] += (uint) overlap;
              }
            }

            var dot_img = WidgetResources.dots;

            // Blit dots where histogram is non-zero
            for (int row = 0; row < row_count; ++row)
            {
              for (int dx = 0; dx < xdot_count; ++dx)
              {
                uint val = hist[row * xdot_count + dx];
                if (val == 0)
                  continue;

                // Select a dot size based on allocation count.
                int src_x;
                if (val > Thres0)
                  src_x = 0;
                else if (val > Thres1)
                  src_x = 8;
                else if (val > Thres2)
                  src_x = 16;
                else
                  src_x = 24;

                Rectangle src_rect = new Rectangle(src_x, 0, 8, 8);

                bg.DrawImage(dot_img, dx * DotSize, row * DotSize, src_rect, GraphicsUnit.Pixel);
              }
            }
          }

          BeginInvoke(new Action(() => { UpdateBitmap(bm); }));
        }
        catch (Exception ex)
        {
          Debug.WriteLine("Update bitmap threw: {0}", ex.Message);
        }
      });
    }

    private Task m_BitmapTask;

    private void UpdateBitmap(Bitmap bm)
    {
      if (m_Bitmap != null)
      {
        m_Bitmap.Dispose();
        m_Bitmap = null;
      }

      if (bm.Width == Width)
      {
        m_Bitmap = bm;
        this.Height = bm.Height;
      }
      m_BitmapTask = null;

      Invalidate();

      if (m_Bitmap == null)
      {
        m_BitmapTask = UpdateBitmap();
      }
    }

    public void OnRangeUpdated()
    {
      if (m_BitmapTask == null)
      {
        m_BitmapTask = UpdateBitmap();
      }

      m_Bitmap = null;
      Invalidate();

      if (m_SelectedAddress != 0)
      {
        OnBlockSelected(this, m_SelectedAddress, m_SelectedAddress + DotBytes);
      }
    }

    private void Draw(Graphics g)
    {
      var mem = FragmentationData;
      var rect = g.ClipBounds;
      rect.Intersect(this.ClientRectangle);

      if (null == mem)
        return;

      if (null == m_Bitmap)
      {
        g.Clear(this.BackColor);
      }
      else
      {
        var bitmap = m_Bitmap;
        g.DrawImageUnscaled(bitmap, new Point(0, 0));

        if (m_SelectedAddress != 0)
        {
          // Map to dot.
          int total_dot_count = (int)((MaxAddress - MinAddress + DotBytes - 1) / DotBytes);
          int xdot_count = Width / DotSize;
          int row_count = (total_dot_count + xdot_count - 1) / xdot_count;

          ulong off = m_SelectedAddress - MinAddress;
          int off_dots = (int) (off / DotBytes);

          int row = off_dots / xdot_count;
          int col = off_dots % xdot_count;

          using (var p = new Pen(Color.Red))
          {
            g.DrawRectangle(p, col * DotSize - 1, row * DotSize - 1, DotSize + 1, DotSize + 1);
          }
        }
      }
    }
  }

  public struct FragAllocData
  {
    public HeapAllocationInfo Alloc;
    public Color Color;
  }

  public interface IFragmentationData
  {
    void GetAllocations(List<FragAllocData> out_allocs, ulong addr_lo, ulong addr_hi);
  }

}
