using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AltoControls
{
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
            box.Location = new Point(3, 3);
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

            PointF p1 = new PointF(w / 2 - 2, h * 1.5f);
            PointF p2 = new PointF(3 * w / 2 + 2, h * 1.5f);
            PointF p11 = new PointF(w, h - 2);
            PointF p22 = new PointF(w, 2 * h + 2);
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
            btnUp.Left = this.Width - btnUp.Width * 2 + 1;
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
}
