using System;
using System.Drawing;
using System.Windows.Forms;

namespace AltoControls
{
    public class AltoSlidingLabel : Control
    {
        Timer timer = new Timer();
        private bool slide;
        int a;
        bool art = false;
        public bool Slide
        {
            get { return slide; }
            set
            {
                slide = value;
                timer.Enabled = slide;
                if (!slide)
                {
                    a = 0;
                    Invalidate();
                }
            }
        }

        public AltoSlidingLabel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                        ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor |
                        ControlStyles.UserPaint, true);
            AutoSize = false;
            Width = 30;
            Height = 15;
            a = 0;
            timer.Interval = 120;
            timer.Tick += timer_Tick;
            slide = false;
            timer.Enabled = false;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            Size tSize = TextRenderer.MeasureText(Text, Font);
            if (tSize.Width <= Width)
            {
                timer.Stop();
                a = 1;
                Invalidate();
                return;
            }
            int maxFar = tSize.Width >= Width ? tSize.Width - Width : 0;
            if (a >= 1)
                art = false;
            if (-a >= maxFar + Font.Height)
                art = true;
            a = art ? a + 1 : a - 1;
            Invalidate();
        }
        protected override void OnResize(EventArgs e)
        {
            timer.Enabled = true;
            base.OnResize(e);
        }
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            using (Brush brush = new SolidBrush(BackColor))
                e.Graphics.FillRectangle(brush, this.ClientRectangle);

            Size tSize = TextRenderer.MeasureText(Text, Font);
            int y = Height / 2 - tSize.Height / 2;

            using (Brush brush = new SolidBrush(ForeColor))
                e.Graphics.DrawString(Text, Font, brush, a, y);
            base.OnPaint(e);
        }
        protected override void OnBackColorChanged(EventArgs e)
        {
            Invalidate();
            base.OnBackColorChanged(e);
        }
        protected override void OnForeColorChanged(EventArgs e)
        {
            Invalidate();
            base.OnForeColorChanged(e);
        }
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
                timer.Start();
            }
        }
        protected override void Dispose(bool disposing)
        {
            timer.Stop();
            base.Dispose(disposing);
        }
    }


}
