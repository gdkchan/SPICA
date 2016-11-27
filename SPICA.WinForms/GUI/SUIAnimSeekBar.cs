using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
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
            e.Graphics.Clear(BackColor);

            Rectangle Rect = new Rectangle(MarginX, 1, Width - MarginX * 2, Height - 2);

            int CurX = (int)(Max > 0 ? ((Value / Max) * Rect.Width) : 0) + Rect.X - 1;

            //Cursor line
            e.Graphics.DrawLine(
                new Pen(CursorColor), 
                new Point(CurX, Rect.Y), 
                new Point(CurX, Rect.Y + Rect.Height - 1));

            //Draw a triangle on the bottom side of the cursor
            int HalfH = Rect.Height >> 1;

            Point[] Points = new Point[3];

            Points[0] = new Point(CurX - 4, Rect.Y + Rect.Height); //Left
            Points[1] = new Point(CurX + 4, Rect.Y + Rect.Height); //Right
            Points[2] = new Point(CurX, Rect.Y + HalfH); //Middle

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.FillPolygon(new SolidBrush(CursorColor), Points);

            //"Ruler"
            if (Max > 0)
            {
                double LastFrame = 0;

                float PartStep = Rect.Width / Max;

                float RulerStep = Math.Max(RulerMinDist / PartStep, 1);

                if (RulerStep > 1) PartStep *= RulerStep;

                int HalfStep = (int)PartStep >> 1;

                for (int Index = 1; LastFrame < Max; Index++)
                {
                    LastFrame = Math.Round(Index * RulerStep);

                    int RulerX = (int)(Index * PartStep) + Rect.X - 1;

                    //Middle line
                    e.Graphics.DrawLine(
                        new Pen(ForeColor),
                        new Point(RulerX - HalfStep, Rect.Y),
                        new Point(RulerX - HalfStep, Rect.Y + HalfH >> 1));

                    if (RulerX >= Rect.X + Rect.Width) break;

                    //Bigger line
                    e.Graphics.DrawLine(
                        new Pen(ForeColor), 
                        new Point(RulerX, Rect.Y), 
                        new Point(RulerX, Rect.Y + HalfH));

                    //Frame number
                    string Text = LastFrame.ToString();
                    int TextWidth = (int)e.Graphics.MeasureString(Text, Font).Width;

                    Brush TestBrush = new SolidBrush(ForeColor);
                    Point TextPt = new Point(RulerX - TextWidth, Rect.Y);

                    e.Graphics.DrawString(Text, Font, TestBrush, TextPt);
                }
            }

            //Draw inward box shade effect
            Pen DarkShade = new Pen(Color.FromArgb(0x3f, Color.Black));
            Pen BrighShade = new Pen(Color.FromArgb(0x3f, Color.White));

            e.Graphics.DrawLine(DarkShade, Point.Empty, new Point(Width - 1, 0));
            e.Graphics.DrawLine(DarkShade, Point.Empty, new Point(0, Height - 1));
            e.Graphics.DrawLine(BrighShade, new Point(Width - 1, Height - 1), new Point(Width - 1, 0));
            e.Graphics.DrawLine(BrighShade, new Point(Width - 1, Height - 1), new Point(0, Height - 1));
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
