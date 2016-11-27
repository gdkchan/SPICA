using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

namespace SPICA.WinForms.GUI
{
    class SUIAnimSeekBar : Control
    {
        private float Max;
        private float Cursor;

        private Color _CursorColor = Color.Orange;

        [Category("Appearance"), Description("Color of the animation cursor.")]
        public Color CursorColor
        {
            get
            {
                return _CursorColor;
            }
            set
            {
                _CursorColor = value;

                Invalidate();
            }
        }

        [Category("Behavior"), Description("Total number of frames that the animation have.")]
        public float Maximum
        {
            get
            {
                return Max;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException(MaxTooLowEx);
                }

                Max = value;

                Cursor = Math.Min(Cursor, Max);

                Invalidate();
            }
        }

        [Category("Behavior"), Description("Current animation frame.")]
        public float Value
        {
            get
            {
                return Cursor;
            }
            set
            {
                if (value < 0 || value > Maximum)
                {
                    throw new ArgumentOutOfRangeException(string.Format(ValueOutOfRangeEx, Max));
                }

                Cursor = value;

                Invalidate();
            }
        }

        public event EventHandler Seek;

        private const int MarginX = 4;
        private const int RulerMinDist = 64;

        private const string MaxTooLowEx = "Invalid maximum value! Expected a value >= 0!";
        private const string ValueOutOfRangeEx = "Invalid value! Expected >= 0 and <= {0}!";

        public SUIAnimSeekBar()
        {
            SetStyle(
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.SupportsTransparentBackColor, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            e.Graphics.Clear(Parent.BackColor);

            Brush TextBrush = new SolidBrush(ForeColor);
            Rectangle Rect = new Rectangle(MarginX, 3, Width - MarginX * 2, Height - 6);

            e.Graphics.FillRectangle(new SolidBrush(BackColor), Rect);

            int HalfH = Rect.Height >> 1;

            //"Ruler"
            if (Max > 0)
            {
                int Index = 1;

                float PartStep = Rect.Width / Max;
                float FrameStep = Math.Max(RulerMinDist / PartStep, 1);

                if (FrameStep > 1) PartStep = RulerMinDist;

                int HalfStep = (int)PartStep >> 1;

                for (float Frame = FrameStep; Frame <= Max; Frame += FrameStep, Index++)
                {
                    int RulerX = (int)(Index * PartStep) + Rect.X - 1;

                    //Short line
                    e.Graphics.DrawLine(
                        new Pen(ForeColor),
                        new Point(RulerX - HalfStep, Rect.Y + 1),
                        new Point(RulerX - HalfStep, Rect.Y + 1 + HalfH >> 1));

                    //Longer line
                    e.Graphics.DrawLine(
                        new Pen(ForeColor), 
                        new Point(RulerX, Rect.Y + 1), 
                        new Point(RulerX, Rect.Y + 1 + HalfH));

                    //Frame number
                    string Text = Frame.ToString("0.0", CultureInfo.InvariantCulture);
                    int TextWidth = (int)e.Graphics.MeasureString(Text, Font).Width;

                    Point TextPt = new Point(RulerX - TextWidth, Rect.Y);

                    e.Graphics.DrawString(Text, Font, TextBrush, TextPt);
                }
            }

            //Draw inset box shade effect
            int L = Rect.X + 1;
            int T = Rect.Y + 1;

            int R = L + Rect.Width - 2;
            int B = T + Rect.Height - 2;

            Pen DarkShade = new Pen(Color.FromArgb(0x3f, Color.Black), 2);
            Pen BrighShade = new Pen(Color.FromArgb(0x3f, Color.White), 2);

            e.Graphics.DrawLine(DarkShade, new Point(L, T), new Point(R, T));
            e.Graphics.DrawLine(DarkShade, new Point(L, T), new Point(L, B));
            e.Graphics.DrawLine(BrighShade, new Point(R, B), new Point(R, T));
            e.Graphics.DrawLine(BrighShade, new Point(R, B), new Point(L, B));

            e.Graphics.DrawRectangle(new Pen(Parent.BackColor), Rect);

            //Cursor line
            int CurX = (int)(Max > 0 ? ((Value / Max) * Rect.Width) : 0) + Rect.X - 1;

            e.Graphics.DrawLine(
                new Pen(CursorColor),
                new Point(CurX, Rect.Y + 1),
                new Point(CurX, Rect.Y + 1 + Rect.Height - 2));

            //Draw a triangle on the bottom side of the cursor
            Point[] Points = new Point[3];

            Points[0] = new Point(CurX - 4, Rect.Y + 1 + Rect.Height - 2); //Left
            Points[1] = new Point(CurX + 4, Rect.Y + 1 + Rect.Height - 2); //Right
            Points[2] = new Point(CurX,     Rect.Y + 1 + HalfH); //Middle

            e.Graphics.FillPolygon(new SolidBrush(CursorColor), Points);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) MoveTo(e.X);

            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) MoveTo(e.X);

            base.OnMouseMove(e);
        }

        private void MoveTo(int X)
        {
            Cursor = Math.Max(Math.Min(((float)(X - MarginX) / (Width - MarginX * 2)) * Max, Max), 0);

            Seek?.Invoke(this, EventArgs.Empty);

            Invalidate();
        }
    }
}
