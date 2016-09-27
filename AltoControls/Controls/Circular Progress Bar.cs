using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AltoControls
{
    public class CircularPB : Control
    {
        #region Values
        int value;
        int maxValue;
        int minValue;
        int outerRadius;
        int innerRadius;
        int stroke;
        bool automaticFontCalculation;
        bool allowText;
        bool transparency;
        Color progressColor;
        SolidBrush brush;
        Timer timer = new Timer();
        #endregion
        #region CircularPB
        public CircularPB()
        {
            maxValue = 100;
            value = maxValue;
            minValue = 0;
            progressColor = Color.LightBlue;
            innerRadius = 30;
            outerRadius = 50;
            stroke = 10;
            MinimumSize = new System.Drawing.Size(60, 60);
            automaticFontCalculation = true;
            allowText = true;
            transparency = true;
            brush = new SolidBrush(progressColor);
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
            Location = new Point(100, 100);
            BackColor = Color.Transparent;
            timer.Tick += timer_Tick;
            timer.Interval = 1;
            timer.Enabled = true;
        }
        #endregion
        int old, current;
        void timer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < 3; i++)
            {
                int a = current - old;
                int k = maxValue / 100;
                int art = k < 1 ? 1 : k;
                old += Math.Abs(a) < 2 ? a : art * Math.Sign(a);
                this.value = old;
                Invalidate();
                if (current == old)
                    timer.Stop();
            }
        }
        #region Events
        protected override void OnPaint(PaintEventArgs e)
        {
            #region Transparency
            if (transparency && Parent != null)
            {
                Bitmap behind = new Bitmap(Parent.Width, Parent.Height);
                foreach (Control c in Parent.Controls)
                    if (c.Bounds.IntersectsWith(this.Bounds) & c != this)
                        c.DrawToBitmap(behind, c.Bounds);
                e.Graphics.DrawImage(behind, -Left, -Top);
                behind.Dispose();
            }
            #endregion
            #region DrawCircle
            float angle = (value * 360.0f / maxValue);

            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

            RectangleF outerRect = new RectangleF(0, 0, outerRadius * 2, outerRadius * 2);
            RectangleF innerRect = new RectangleF(outerRadius - innerRadius,
                                                  outerRadius - innerRadius, innerRadius * 2, innerRadius * 2);
            using (GraphicsPath progPath = new GraphicsPath())
            {
                progPath.AddArc(outerRect, angle - 90, -angle);
                if (allowText)
                {
                    progPath.AddArc(innerRect, -90, angle);
                }
                else
                {
                    progPath.AddLine(new Point(outerRadius, 0), new Point(outerRadius, outerRadius));
                }
                progPath.CloseFigure();
                e.Graphics.FillPath(brush, progPath);
            }


            #endregion
            #region DrawString
            if (!allowText) return;
            string text = "%" + (value * 100.0 / maxValue).ToString("0");
            Size textSize;
            float ratio = 1.0f;
            if (automaticFontCalculation)
            {
                string fullPercText = "%100";
                Size temp = TextRenderer.MeasureText(fullPercText, this.Font);
                float properWidth = innerRadius * 1.2f;
                ratio = properWidth / temp.Width;
            }
            Font font = new Font(Font.Name, Font.Height * ratio);
            textSize = TextRenderer.MeasureText(text, font);
            float x = (2 * outerRadius - textSize.Width) / 2f;
            float y = (2 * outerRadius - textSize.Height) / 2f;
            using (SolidBrush textBrush = new SolidBrush(ForeColor))
                e.Graphics.DrawString(text, font, textBrush, x + 1, y);
            #endregion
        }
        protected override void OnResize(EventArgs e)
        {
            if (allowText && outerRadius - stroke <= 15)
            {
                stroke--;
                return;
            }
            base.OnResize(e);
            Height = Width;
            outerRadius = Width / 2 - 1;
            innerRadius = outerRadius - stroke;

            Invalidate();
        }

        #endregion
        #region Properties
        public int Value
        {
            get
            {
                return current;
            }
            set
            {
                old = this.value;
                if (value > maxValue)
                    throw new Exception("The value is greater than the maximum value of progress");
                if (value < minValue)
                    throw new Exception("The value is lower than the maximum value of progress");
                current = value;
                timer.Start();
            }
        }
        public int MaxValue
        {
            get
            {
                return maxValue;
            }
            set
            {
                maxValue = value;
                Invalidate();
            }
        }
        public int MinValue
        {
            get
            {
                return minValue;
            }
            set
            {
                minValue = value;
                Invalidate();
            }
        }
        public Color ProgressColor
        {
            get
            {
                return progressColor;
            }
            set
            {
                progressColor = value;
                brush = new SolidBrush(progressColor);
                Invalidate();
            }
        }
        public bool AutomaticFontCalculation
        {
            get
            {
                return automaticFontCalculation;
            }
            set
            {
                automaticFontCalculation = value;
                Invalidate();
            }
        }
        public int Stroke
        {
            get
            {
                return stroke;
            }
            set
            {
                if (outerRadius - value >= 15)
                    stroke = value;
                innerRadius = outerRadius - stroke;
                Invalidate();
            }
        }
        public bool AllowText
        {
            get
            {
                return allowText;
            }
            set
            {
                allowText = value;
                Invalidate();
            }
        }
        public bool Transparency
        {
            get
            {
                return transparency;
            }
            set
            {
                transparency = value;
                Invalidate();
            }
        }
        #endregion
    }
}
