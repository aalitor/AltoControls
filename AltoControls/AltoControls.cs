using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AltoControls
{
    #region RoundedRectangle
    public class RoundedRectangleF
    {

        Point location;
        float radius;
        GraphicsPath grPath;
        float x, y;
        float width, height;
       
        
        public RoundedRectangleF(float width, float height, float radius,float x = 0,float y = 0)
        {
           
            location = new Point(0, 0);
            this.radius = radius;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            grPath = new GraphicsPath();
            if (radius <= 0)
            {
                grPath.AddRectangle(new RectangleF(x, y, width, height));
                return;
            }
            RectangleF upperLeftRect = new RectangleF(x, y, 2 * radius, 2 * radius);
            RectangleF upperRightRect = new RectangleF(width - 2 * radius-1, x, 2 * radius, 2 * radius);
            RectangleF lowerLeftRect = new RectangleF(x, height - 2 * radius-1, 2 * radius, 2 * radius);
            RectangleF lowerRightRect = new RectangleF(width - 2 * radius-1, height - 2 * radius-1, 2 * radius, 2 * radius);

            grPath.AddArc(upperLeftRect, 180, 90);
            grPath.AddArc(upperRightRect, 270, 90);
            grPath.AddArc(lowerRightRect, 0, 90);
            grPath.AddArc(lowerLeftRect, 90, 90);
            grPath.CloseAllFigures();
            
        }
        public RoundedRectangleF()
        {
        }
        public GraphicsPath Path
        {
            get
            {
                return grPath;
            }
        }
        public RectangleF Rect
        {
            get
            {
                return new RectangleF(x, y, width, height);
            }
        }
        public float Radius
        {
            get
            {
                return radius;
            }
            set
            {
                radius = value;
            }
        }
        
    }
    #endregion

    #region CircularPB
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
    #endregion

    #region AltoButton
    [Description("Text")]
    public class AltoButton : Control, IButtonControl
    {
        #region Variables
        int radius;
        bool transparency;
        MouseState state;
        RoundedRectangleF roundedRect;
        Color inactive1, inactive2, active1, active2;
        private Color strokeColor;
        private bool stroke;

        public bool Stroke
        {
            get { return stroke; }
            set
            {
                stroke = value;
                Invalidate();
            }
        }

        public Color StrokeColor
        {
            get { return strokeColor; }
            set
            {
                strokeColor = value;
                Invalidate();
            }
        }
        #endregion
        #region AltoButton
        public AltoButton()
        {
            Width = 65;
            Height = 30;
            stroke = false;
            strokeColor = Color.Gray;
            inactive1 = Color.FromArgb(44, 188, 210);
            inactive2 = Color.FromArgb(33, 167, 188);
            active1 = Color.FromArgb(64, 168, 183);
            active2 = Color.FromArgb(36, 164, 183);

            radius = 10;
            roundedRect = new RoundedRectangleF(Width, Height, radius);

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.UserPaint, true);
            BackColor = Color.Transparent;
            ForeColor = Color.Black;
            Font = new System.Drawing.Font("Comic Sans MS", 10, FontStyle.Bold);
            state = MouseState.Leave;
            transparency = false;
        }
        #endregion
        #region Events
        protected override void OnPaint(PaintEventArgs e)
        {
            #region Transparency
            if (transparency)
                Transparencer.MakeTransparent(this, e.Graphics);
            #endregion

            #region Drawing
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            roundedRect = new RoundedRectangleF(Width, Height, radius);
            e.Graphics.FillRectangle(Brushes.Transparent, this.ClientRectangle);

            int R1 = (active1.R + inactive1.R) / 2;
            int G1 = (active1.G + inactive1.G) / 2;
            int B1 = (active1.B + inactive1.B) / 2;

            int R2 = (active2.R + inactive2.R) / 2;
            int G2 = (active2.G + inactive2.G) / 2;
            int B2 = (active2.B + inactive2.B) / 2;

            Rectangle rect = new Rectangle(0, 0, Width, Height);

            if (this.Enabled)
            {
                if (state == MouseState.Leave)
                    using (LinearGradientBrush inactiveGB = new LinearGradientBrush(rect, inactive1, inactive2, 90f))
                        e.Graphics.FillPath(inactiveGB, roundedRect.Path);
                else if (state == MouseState.Enter)
                    using (LinearGradientBrush activeGB = new LinearGradientBrush(rect, active1, active2, 90f))
                        e.Graphics.FillPath(activeGB, roundedRect.Path);
                else if (state == MouseState.Down)
                    using (LinearGradientBrush downGB = new LinearGradientBrush(rect, Color.FromArgb(R1, G1, B1), Color.FromArgb(R2, G2, B2), 90f))
                        e.Graphics.FillPath(downGB, roundedRect.Path);
                if (stroke)
                    using (Pen pen = new Pen(strokeColor, 1))
                    using (GraphicsPath path = new RoundedRectangleF(Width - (radius > 0 ? 0 : 1), Height - (radius > 0 ? 0 : 1), radius).Path)
                        e.Graphics.DrawPath(pen, path);
            }
            else
            {
                Color linear1 = Color.FromArgb(190, 190, 190);
                Color linear2 = Color.FromArgb(210, 210, 210);
                using (LinearGradientBrush inactiveGB = new LinearGradientBrush(rect, linear1, linear2, 90f))
                {
                    e.Graphics.FillPath(inactiveGB, roundedRect.Path);
                    e.Graphics.DrawPath(new Pen(inactiveGB), roundedRect.Path);
                }
            }


            #endregion

            #region Text Drawing
            using (StringFormat sf = new StringFormat()
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            })
            using (Brush brush = new SolidBrush(ForeColor))
                e.Graphics.DrawString(Text, Font, brush, this.ClientRectangle, sf);
            #endregion
            base.OnPaint(e);
        }
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            base.OnClick(e);
        }
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
        }
        protected override void OnEnabledChanged(EventArgs e)
        {
            Invalidate();
            base.OnEnabledChanged(e);
        }
        protected override void OnResize(EventArgs e)
        {
            Invalidate();
            base.OnResize(e);
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            state = MouseState.Enter;
            base.OnMouseEnter(e);
            Invalidate();
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            state = MouseState.Leave;
            base.OnMouseLeave(e);
            Invalidate();
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            Capture = false;
            state = MouseState.Down;
            base.OnMouseDown(e);
            Invalidate();
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (state != MouseState.Leave)
                state = MouseState.Enter;
            base.OnMouseUp(e);
            Invalidate();
        }
        #endregion
        #region Properties

        
        public int Radius
        {
            get
            {
                return radius;
            }
            set
            {
                radius = value;
                Invalidate();
            }
        }
        public Color Inactive1
        {
            get
            {
                return inactive1;
            }
            set
            {
                inactive1 = value;
                Invalidate();
            }
        }
        public Color Inactive2
        {
            get
            {
                return inactive2;
            }
            set
            {
                inactive2 = value;
                Invalidate();
            }
        }
        public Color Active1
        {
            get
            {
                return active1;
            }
            set
            {
                active1 = value;
                Invalidate();
            }
        }
        public Color Active2
        {
            get
            {
                return active2;
            }
            set
            {
                active2 = value;
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
            }
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
                Invalidate();
            }
        }
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
                Invalidate();
            }
        }

        public DialogResult DialogResult
        {
            get
            {
                return System.Windows.Forms.DialogResult.OK;
            }
            set
            {
            }
        }

        public void NotifyDefault(bool value)
        {
        }

        public void PerformClick()
        {
            OnClick(EventArgs.Empty);
        }
        #endregion
    }
    public enum MouseState
    {
        Enter,
        Leave,
        Down,
        Up,
    }
    #endregion

    #region ProcessingControl
    public class ProcessingControl : Control
    {
        #region Variables
        int n, circleIndex, radius, interval;
        bool spin;
        System.Windows.Forms.Timer timer;
        Color other, index;
        #endregion
        #region ProcessingControl
        public ProcessingControl()
        {
            Width = 85;
            Height = 85;
            n = 6;
            circleIndex = 0;
            interval = 100;
            radius = 10;
            spin = true;
            other = Color.LightGray;
            index = Color.Gray;
            timer = new System.Windows.Forms.Timer();
            timer.Interval = interval;
            timer.Tick += timer_Tick;
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            timer.Enabled = spin;
            BackColor = Color.Transparent;
        }
        #endregion
        #region Events


        protected override void OnPaint(PaintEventArgs e)
        {
            Transparencer.MakeTransparent(this, e.Graphics);
            #region Drawing
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            int length = Math.Min(Width, Height);
            PointF center = new PointF(length / 2, length / 2);
            int bigRadius = length / 2 - radius;
            float unitAngle = 360 / n;
            if (spin)
            {
                circleIndex++;
                circleIndex = circleIndex >= n ? circleIndex % n : circleIndex;
            }
            for (int i = 0; i < n; i++)
            {
                float c1X = center.X + (float)(bigRadius * Math.Cos(unitAngle * i * Math.PI / 180));
                float c1Y = center.Y + (float)(bigRadius * Math.Sin(unitAngle * i * Math.PI / 180));
                PointF loc1 = new PointF(c1X - radius, c1Y - radius);
                if (i == circleIndex)
                    using (SolidBrush brush = new SolidBrush(index))
                        e.Graphics.FillEllipse(brush, loc1.X, loc1.Y, 2 * radius, 2 * radius);
                else
                    using (SolidBrush brush = new SolidBrush(other))
                        e.Graphics.FillEllipse(brush, loc1.X, loc1.Y, 2 * radius, 2 * radius);
            }
            #endregion
        }
        void timer_Tick(object sender, EventArgs e)
        {
            Invalidate();
            if (!spin) timer.Stop();
        }
        #endregion
        #region Properties
        public int NCircle
        {
            get
            {
                return n;
            }
            set
            {
                n = value > 1 ? value : 2;
                Invalidate();
            }
        }
        public int Interval
        {
            get
            {
                return interval;
            }
            set
            {
                interval = value >= 1 ? value : 1;
                timer.Interval = interval;
            }
        }
        public bool Spin
        {
            get
            {
                return spin;
            }
            set
            {
                spin = value;
                timer.Enabled = spin;
            }
        }
        public int Radius
        {
            get
            {
                return radius;
            }
            set
            {
                radius = value >= 1 ? value : 1;
                Invalidate();

            }
        }
        public Color Others
        {
            get
            {
                return other;
            }
            set
            {
                other = value;
                if (!spin) Invalidate();
            }
        }
        public Color IndexColor
        {
            get
            {
                return index;
            }
            set
            {
                index = value;
                if (!spin) Invalidate();
            }
        }
        #endregion
    }
    public class SpinningCircles : Control
    {
        bool fullTransparency = true;
        float increment = 1f;
        float radius = 2.5f;
        int n = 8;
        int next = 0;
        System.Windows.Forms.Timer timer;
        public SpinningCircles()
        {
            Width = 90;
            Height = 100;
            timer = new System.Windows.Forms.Timer();
            timer.Tick += timer_Tick;
            timer.Enabled = false;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
        }
        void timer_Tick(object sender, EventArgs e)
        {
            Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (fullTransparency)
            {
                Transparencer.MakeTransparent(this, e.Graphics);
            }
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            int length = Math.Min(Width, Height);
            PointF center = new PointF(length / 2, length / 2);
            float bigRadius = length / 2 - radius - (n - 1) * increment;
            float unitAngle = 360 / n;
            next++;
            next = next >= n ? 0 : next;
            int a = 0;
            for (int i = next; i < next + n; i++)
            {
                int factor = i % n;
                float c1X = center.X + (float)(bigRadius * Math.Cos(unitAngle * factor * Math.PI / 180));
                float c1Y = center.Y + (float)(bigRadius * Math.Sin(unitAngle * factor * Math.PI / 180));
                float currRad = radius + a * increment;
                PointF c1 = new PointF(c1X - currRad, c1Y - currRad);
                e.Graphics.FillEllipse(Brushes.Black, c1.X, c1.Y, 2 * currRad, 2 * currRad);
                using (Pen pen = new Pen(Color.White, 2))
                    e.Graphics.DrawEllipse(pen, c1.X, c1.Y, 2 * currRad, 2 * currRad);
                a++;
            }
        }
        protected override void OnVisibleChanged(EventArgs e)
        {
            timer.Enabled = Visible;
            base.OnVisibleChanged(e);
        }
        public bool FullTransparent
        {
            get
            {
                return fullTransparency;
            }
            set
            {
                fullTransparency = value;
            }
        }
        public int N
        {
            get
            {
                return n;
            }
            set
            {
                n = value >= 2 ? value : 2;
                Invalidate();
            }
        }
        public float Increment
        {
            get
            {
                return increment;
            }
            set
            {
                increment = value >= 0 ? value : 0;
                Invalidate();
            }
        }
        public float Radius
        {
            get
            {
                return radius;
            }
            set
            {
                radius = value >= 1 ? value : 1;
                Invalidate();
            }
        }
    }
    public class Transparencer
    {
        public static void MakeTransparent(Control control, Graphics g)
        {
            var parent = control.Parent;
            if (parent == null) return;
            var bounds = control.Bounds;
            var siblings = parent.Controls;
            int index = siblings.IndexOf(control);
            Bitmap behind = null;
            for (int i = siblings.Count - 1; i > index; i--)
            {
                var c = siblings[i];
                if (!c.Bounds.IntersectsWith(bounds)) continue;
                if (behind == null)
                    behind = new Bitmap(control.Parent.ClientSize.Width, control.Parent.ClientSize.Height);
                c.DrawToBitmap(behind, c.Bounds);
            }
            if (behind == null) return;
            g.DrawImage(behind, control.ClientRectangle, bounds, GraphicsUnit.Pixel);
            behind.Dispose();
        }
    }
    #endregion

    #region AltoBorder

    public class Border : Control
    {
        Color titleColor;
        Color downColor;
        AltoButton exitButton;
        AltoButton minButton;
        private bool _dragging = false;
        private Point _start_point = new Point(0, 0);
        int titleStroke = 30;
        int frameStroke = 10;
        public Border()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
            Location = new Point(0, 0);
            downColor = Color.LightGreen;
            titleColor = Color.Green;
            exitButton = new AltoButton();
            minButton = new AltoButton();
            minButton.Radius = exitButton.Radius = 3;
            minButton.Width = 30;
            exitButton.Width = 40;
            minButton.Height = exitButton.Height = 20;
            exitButton.Active2 = exitButton.Active1 = Color.FromArgb(219, 66, 66);
            exitButton.Inactive2 = exitButton.Inactive1 = Color.FromArgb(199, 80, 80);
            minButton.Active1 = minButton.Active2 = Color.Gray;
            minButton.Inactive2 = minButton.Inactive1 = Color.DarkGray;
            exitButton.Click += exitButton_Click;
            exitButton.Paint += exitButton_Paint;
            minButton.Click += minButton_Click;
            minButton.Paint += minButton_Paint;
            minButton.Parent = exitButton.Parent = this;
        }

        void minButton_Paint(object sender, PaintEventArgs e)
        {
            using (Pen pen = new Pen(Color.White, 3))
                e.Graphics.DrawLine(pen, 8, minButton.Height / 2, minButton.Width - 8, minButton.Height / 2);
        }

        void minButton_Click(object sender, EventArgs e)
        {
            if (Parent is Form)
                (Parent as Form).WindowState = FormWindowState.Minimized;
        }

        void exitButton_Paint(object sender, PaintEventArgs e)
        {
            using (Pen pen = new Pen(Color.White, 2))
            {
                int x = exitButton.Width / 3;
                int y = exitButton.Height / 3;
                e.Graphics.DrawLine(pen, x, y, 2 * x, 2 * y);
                e.Graphics.DrawLine(pen, 2 * x, y, x, 2 * y);
            }
        }

        void exitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Transparencer.MakeTransparent(this, e.Graphics);

            #region Drawing

            Width = Parent.Width;
            Height = Parent.Height;

            Point p1 = new Point(frameStroke, titleStroke);
            Point p2 = new Point(Width - frameStroke, titleStroke);
            Point p3 = new Point(Width - frameStroke, Height - frameStroke);
            Point p4 = new Point(frameStroke, Height - frameStroke);

            Point f1 = new Point(0, 0);
            Point f2 = new Point(Width, 0);
            Point f3 = new Point(Width, Height);
            Point f4 = new Point(0, Height);

            Rectangle rect1 = new Rectangle(f1.X, f1.Y, frameStroke, Height);
            Rectangle rect2 = new Rectangle(f1.X, f1.Y, Width, titleStroke);
            Rectangle rect3 = new Rectangle(p2.X, f2.Y, frameStroke, Height);
            Rectangle rect4 = new Rectangle(f4.X, p4.Y, Width, frameStroke);

            using (GraphicsPath path = new GraphicsPath())
            using (LinearGradientBrush brush = new LinearGradientBrush(rect1, titleColor, downColor, 90f))
            {
                e.Graphics.FillRectangle(brush, rect1);
                e.Graphics.FillRectangle(brush, rect2);
                e.Graphics.FillRectangle(brush, rect3);
                e.Graphics.FillRectangle(brush, rect4);

                e.Graphics.FillPath(brush, path);
            }
            #endregion
            exitButton.Location = new Point(this.Width - frameStroke - exitButton.Width, 4);
            minButton.Location = new Point(exitButton.Location.X - 10 - minButton.Width, exitButton.Location.Y);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            RectangleF inner = new RectangleF(frameStroke, titleStroke, Width - 2 * frameStroke, Height - titleStroke - frameStroke);
            RectangleF miniRect = new RectangleF(e.Location.X, e.Location.Y, 0.1f, 0.1f);
            RectangleF rect = new RectangleF(frameStroke, frameStroke, Width - 2 * frameStroke, titleStroke - frameStroke);
            if (inner.IntersectsWith(miniRect)) return;
            if (rect.IntersectsWith(miniRect))
            {
                _dragging = true;  // _dragging is your variable flag
                _start_point = new Point(e.X, e.Y);
            }
            else
            {
                Cursor = Cursors.SizeNESW;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _dragging = false;
            Cursor = Cursors.Arrow;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_dragging)
            {
                Point p = PointToScreen(e.Location);
                Parent.Location = new Point(p.X - this._start_point.X, p.Y - this._start_point.Y);
            }
            exitButton.Invalidate();
            minButton.Invalidate();
        }


        #region Properties

        public Color TitleColor
        {
            get
            {
                return titleColor;
            }
            set
            {
                titleColor = value;
                Invalidate();
            }
        }
        public Color BarColor
        {
            get
            {
                return downColor;
            }
            set
            {
                downColor = value;
                Invalidate();
            }
        }
        #endregion

    }
    #endregion

    #region AltoProgressBar

    public class AltoPB : Control
    {
        int value;
        int maxValue;
        int minValue;
        int old, current;
        Color prColor;
        System.Windows.Forms.Timer timer;

        public AltoPB()
        {
            prColor = Color.LightBlue;
            maxValue = 100;
            minValue = 0;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            Width = 100;
            Height = 20;
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1;
            timer.Tick += timer_Tick;
            timer.Enabled = true;
        }
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
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            int length = Width * value / maxValue;
            e.Graphics.DrawRectangle(Pens.Silver, 0, 0, Width - 1, Height - 1);
            using (SolidBrush brush = new SolidBrush(prColor))
                e.Graphics.FillRectangle(brush, 1, 1, length - 2, Height - 2);

            Size textSize = TextRenderer.MeasureText(value + "", this.Font);
            int x = (Width - textSize.Width) / 2;
            int y = (Height - textSize.Height) / 2;
            using (SolidBrush textBrush = new SolidBrush(this.ForeColor))
                e.Graphics.DrawString("%" + (value * 100 / maxValue), this.Font, textBrush, x, y);
            base.OnPaint(e);
        }
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
                return prColor;
            }
            set
            {
                prColor = value;
                Invalidate();
            }
        }
    }

    #endregion

    #region AltoTextBox

    public class AltoTextBox : Control
    {
        int radius = 15;
        public TextBox box = new TextBox();
        GraphicsPath shape;
        GraphicsPath innerRect;
        Color br;
        public AltoTextBox()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);

            box.Parent = this;
            Controls.Add(box);

            box.BorderStyle = BorderStyle.None;
            box.TextAlign = HorizontalAlignment.Left;
            box.Font = Font;

            BackColor = Color.Transparent;
            ForeColor = Color.DimGray;
            br = Color.White;
            box.BackColor = br;
            Text = null;
            Font = new Font("Comic Sans MS", 11);
            Size = new Size(135, 33);
            DoubleBuffered = true;
            box.KeyDown += box_KeyDown;
            box.TextChanged += box_TextChanged;
            box.MouseDoubleClick += box_MouseDoubleClick;
        }

        void box_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;

            box.SelectAll();
        }

        void box_TextChanged(object sender, EventArgs e)
        {
            Text = box.Text;
        }
        public void SelectAll()
        {
            box.SelectAll();
        }
        void box_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                box.SelectionStart = 0;
                box.SelectionLength = Text.Length;
            }
        }
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            box.Text = Text;
        }
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            box.Font = Font;
            Invalidate();
        }
        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);
            box.ForeColor = ForeColor;
            Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            shape = new RoundedRectangleF(Width, Height, radius).Path;
            innerRect = new RoundedRectangleF(Width - 0.5f, Height - 0.5f, radius, 0.5f, 0.5f).Path;
            if (box.Height >= Height - 4)
                Height = box.Height + 4;
            box.Location = new Point(radius - 5, Height / 2 - box.Font.Height / 2);
            box.Width = Width - (int)(radius * 1.5);

            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            Bitmap bmp = new Bitmap(Width, Height);
            Graphics grp = Graphics.FromImage(bmp);
            e.Graphics.DrawPath(Pens.Gray, shape);
            using (SolidBrush brush = new SolidBrush(br))
            e.Graphics.FillPath(brush, innerRect);
            Transparencer.MakeTransparent(this, e.Graphics);

            base.OnPaint(e);
        }
        public Color Br
        {
            get
            {
                return br;
            }
            set
            {
                br = value;
                if (br != Color.Transparent)
                box.BackColor = br;
                Invalidate();
            }
        }
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = Color.Transparent;
            }
        }
    }

    #endregion
    
    #region AltoNMUpDown
    public class AltoNMUpDown : Control
    {
        AltoButton btnUp = new AltoButton();
        AltoButton btnDown = new AltoButton();
        TextBox box = new TextBox();
        private double value;
        private Color signColor;
        bool dec;
        System.Windows.Forms.Timer timer;
        #region Constructor
        
        public AltoNMUpDown()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                        ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor |
                        ControlStyles.UserPaint, true);
            Width = 60;
            Height = 20;
            signColor = Color.White;
            value = 0;
            box.Text = value.ToString();
            dec = false;
            btnDown.Active1 = btnDown.Active2 = btnUp.Active1 = btnUp.Active2 = Color.Gray;
            btnDown.Inactive1 = btnDown.Inactive2 = btnUp.Inactive1 = btnUp.Inactive2 = Color.LightSlateGray;
            btnDown.Radius = btnUp.Radius = 0;
            btnDown.Width = btnUp.Width = 20;
            btnDown.Parent = btnUp.Parent = this;
            btnDown.Paint += btnDown_Paint;
            btnUp.Stroke = btnDown.Stroke = true;
            btnUp.StrokeColor = btnDown.StrokeColor = Color.DarkGray;
            btnUp.Paint += btnUp_Paint;
            btnUp.Click += btnUp_Click;
            btnUp.MouseDown += btnUp_MouseDown;
            btnUp.MouseUp += btnUp_MouseUp;
            btnDown.MouseDown += btnDown_MouseDown;
            btnDown.MouseUp += btnDown_MouseUp;
            btnDown.Click += btnDown_Click;
            box.Parent = this;
            box.KeyDown += box_KeyDown;
            box.Location = new Point(3,3);
            btnDown.Top = 0;
            box.BorderStyle = BorderStyle.None;
            Font = new Font("Comic Sans MS", 12);
            box.KeyPress += box_KeyPress;
            this.Invalidate();
            timer = new System.Windows.Forms.Timer()
            {
                Interval = 400
            };
            timer.Tick += timer_Tick;
        }

        void box_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                System.Media.SystemSounds.Asterisk.Play();
            }
        }

        

        void timer_Tick(object sender, EventArgs e)
        {
            value = dec ? value - 1 : value + 1;
            box.Text = value.ToString();
            if (timer.Interval >= 50)
            timer.Interval /= 2;

        }

        void btnDown_MouseUp(object sender, MouseEventArgs e)
        {
            timer.Interval = 400;
            timer.Stop();
            dec = false;
        }

        void btnDown_MouseDown(object sender, MouseEventArgs e)
        {
            btnDown.Focus();
            dec = true;
            value = double.Parse(box.Text);
            timer.Start();
        }

        void btnUp_MouseUp(object sender, MouseEventArgs e)
        {
            timer.Interval = 400;
            timer.Stop();
            dec = false;
        }

        void btnUp_MouseDown(object sender, MouseEventArgs e)
        {
            btnUp.Focus();
            dec = false;
            value = double.Parse(box.Text);
            timer.Start();
        }
        #endregion
        #region Press
        
        void btnDown_Click(object sender, EventArgs e)
        {
            value--;
            box.Text = value.ToString();
        }

        void btnUp_Click(object sender, EventArgs e)
        {
            value++;
            box.Text = value.ToString();
        }

        void box_KeyPress(object sender, KeyPressEventArgs e)
        {
            
            if (box.SelectedText.Length >= box.Text.Length)
                box.Text = "";
            if (!char.IsControl(e.KeyChar) && (!char.IsDigit(e.KeyChar))
        && (e.KeyChar != '.') && (e.KeyChar != '-'))
                e.Handled = true;

            // only allow one decimal point
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                e.Handled = true;

            // only allow minus sign at the beginning
            if (e.KeyChar == '-' && (sender as TextBox).Text.Length > 0)
                e.Handled = true;
        }
        protected override void OnResize(EventArgs e)
        {
            
            base.OnResize(e);
        }
        #endregion
        #region Paint

        void btnUp_Paint(object sender, PaintEventArgs e)
        {
            if (Height > 0)
            {
                box.Font = new System.Drawing.Font("Arial", Height * 0.5f);
                box.Location = new Point(3, Height / 2 - box.Height / 2);
            }
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            float w = btnUp.Width / 2 - 1;
            float h = btnUp.Height / 3;

            PointF p1 = new PointF(w / 2-2, h * 1.5f);
            PointF p2 = new PointF(3 * w / 2+2, h * 1.5f);
            PointF p11 = new PointF(w, h-2);
            PointF p22 = new PointF(w, 2 * h+2);
            using (Pen pen = new Pen(signColor, 3))
            {
                e.Graphics.DrawLine(pen, p1, p2);
                e.Graphics.DrawLine(pen, p11, p22);
            }
        }
        void btnDown_Paint(object sender, PaintEventArgs e)
        {
            float w = btnUp.Width / 3;
            float h = btnUp.Height / 3;
            PointF p1 = new PointF(btnDown.Width / 2 - w / 2, h * 1.5f);
            PointF p2 = new PointF(p1.X + w, h * 1.5f);
            using (Pen pen = new Pen(signColor, 3))
            {
                e.Graphics.DrawLine(pen, p1, p2);
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            btnUp.Left = this.Width - btnUp.Width * 2+1;
            btnDown.Left = this.Width - btnDown.Width;
            btnDown.Top = 0;
            btnUp.Height = btnDown.Height = Height - 1;
            e.Graphics.FillRectangle(Brushes.White, 0, 0, Width, Height);
            e.Graphics.DrawRectangle(Pens.Gray, 0, 0, Width - 1, Height - 1);
            box.Width = this.Width - 2 * btnDown.Width - 4;
            using (Pen pen = new Pen(Color.Black, 2))
            base.OnPaint(e);
        }
        #endregion
        
        protected override void OnFontChanged(EventArgs e)
        {
            box.Font = Font;
            Height = Font.Height * 2;
            base.OnFontChanged(e);
        }
        public Color SignColor
        {
            get { return signColor; }
            set
            {
                signColor = value;
                Invalidate();
            }
        }

        public double Value
        {
            get { return value; }
            set
            {
                this.value = value;
                box.Text = value.ToString();
                Invalidate();
            }
        }
        
    }
    #endregion

    #region AltoSlidingLabel
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
    #endregion
        
}
