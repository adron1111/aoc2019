using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Concurrent;

namespace visgui
{
    public partial class MatrixVisualizer : Form
    {
        ConcurrentDictionary<Tuple<int, int>, string> data = new ConcurrentDictionary<Tuple<int, int>, string>();
        int minx, maxx, miny, maxy;

        public MatrixVisualizer()
        {
            InitializeComponent();
            VisualizerProgram.context.runningforms++;
        }

        public void SetMatrixData(int x, int y, string data)
        {
            this.data[Tuple.Create(x, y)] = data;
            bool changedsize = false;
            if (x < minx) {
                minx = x; 
                changedsize = true;
            }
            if (x > maxx) {
                maxx = x;
                changedsize = true;
            }
            if (y < miny) {
                miny = y;
                changedsize = true;
            }
            if (y > maxy) {
                maxy = y;
                changedsize = true;
            }
            if (changedsize) {
                if (InvokeRequired) {
                    BeginInvoke((Action)(() =>
                    {
                        Invalidate();
                    }));
                } else {
                    Invalidate();
                }
            } else {
                if (InvokeRequired) {
                    BeginInvoke((Action)(() =>
                    {
                        InvalidateCell(x, y);
                    }));
                } else {
                    InvalidateCell(x, y);
                }
            }

        }

        public void SetMatrixRange(int minx, int maxx, int miny, int maxy)
        {
            if (this.minx != minx || this.maxx != maxx || this.miny != miny || this.maxy != maxy) {
                this.minx = minx;
                this.maxx = maxx;
                this.miny = miny;
                this.maxy = maxy;
                if (InvokeRequired) {
                    BeginInvoke((Action)(() =>
                    {
                        Invalidate();
                    }));
                } else {
                    Invalidate();
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (--VisualizerProgram.context.runningforms == 0) {
                VisualizerProgram.context.ExitThread();
            }
        }

        const int toph = 16;
        const int sidew = 60;

        void InvalidateCell(int x, int y)
        {
            Rectangle rc = this.ClientRectangle;
            int rowh = (rc.Height - toph) / (maxx - minx + 1);
            int colw = (rc.Width - sidew) / (maxy - miny + 1);
            int wx = sidew + (x - minx) * colw;
            int wy = toph + (y - miny) * rowh;
            Invalidate(new Rectangle(wx, wy, colw, rowh));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);

            Rectangle rc = this.ClientRectangle;
            int rowh = (rc.Height - toph) / (maxy - miny + 1);
            int colw = (rc.Width - sidew) / (maxx - minx + 1);
            if (colw == 0 || rowh == 0)
                return;
            int mindx = (e.ClipRectangle.Left - sidew) / colw + minx;
            int maxdx = (e.ClipRectangle.Right - sidew) / colw + minx;
            int mindy = (e.ClipRectangle.Top - toph) / rowh + miny;
            int maxdy = (e.ClipRectangle.Bottom - toph) / rowh + miny;

            if (mindx < minx)
                mindx = minx;
            if (maxdx > maxx)
                maxdx = maxx;
            if (mindy < miny)
                mindy = miny;
            if (maxdy > maxy)
                maxdy = maxy;

            for (int x = mindx; x <= maxdx; x++) {
                int wx = sidew + (x - minx) * colw;
                TextRenderer.DrawText(e.Graphics, x.ToString(), this.Font, new Rectangle(wx, 0, colw, toph), Color.Black, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
            }
            for (int x = mindx; x <= maxdx; x++) {
                int wx = sidew + (x - minx) * colw;
                e.Graphics.DrawLine(Pens.Black, wx, rc.Top, wx, rc.Bottom);
            }
            for (int y = mindy; y <= maxdy; y++) {
                int wy = toph + (y - miny) * rowh;
                TextRenderer.DrawText(e.Graphics, y.ToString(), this.Font, new Rectangle(0, wy, sidew, rowh), Color.Black, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
            }
            for (int y = mindy; y <= maxdy; y++) {
                int wy = toph + (y - miny) * rowh;
                e.Graphics.DrawLine(Pens.Black, rc.Left, wy, rc.Right, wy);
            }


            for (int x = mindx; x <= maxdx; x++) {
                for (int y = mindy; y <= maxdy; y++) {
                    string value;
                    if (data.TryGetValue(Tuple.Create(x, y), out value)) {
                        int wx = sidew + (x - minx) * colw;
                        int wy = toph + (y - miny) * rowh;
                        TextRenderer.DrawText(e.Graphics, value, this.Font, new Rectangle(wx, wy, colw, rowh), Color.Black, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.EndEllipsis);
                    }
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }
    }
}
