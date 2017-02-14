using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SPICA.WinForms.GUI
{
    class SUITabControl : TabControl
    {
        [Category("Appearance"), Description("Background color of the control.")]
        public Color BackgroundColor { get; set; } = Color.Black;

        [Category("Appearance"), Description("Foreground color of the control.")]
        public Color ForegroundColor { get; set; } = Color.White;

        [Category("Appearance"), Description("Text color when a Tab is selected.")]
        public Color SelectedForeColor { get; set; } = Color.Orange;

        public SUITabControl()
        {
            SetStyle(
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Parent.BackColor);

            Rectangle PageRect = default(Rectangle);

            switch (Alignment)
            {
                case TabAlignment.Top:    PageRect = RemoveMargin(DisplayRectangle, 0, 4, 4, 4); break;
                case TabAlignment.Left:   PageRect = RemoveMargin(DisplayRectangle, 4, 0, 4, 4); break;
                case TabAlignment.Right:  PageRect = RemoveMargin(DisplayRectangle, 4, 4, 0, 4); break;
                case TabAlignment.Bottom: PageRect = RemoveMargin(DisplayRectangle, 4, 4, 4, 0); break;
            }

            e.Graphics.FillRectangle(new SolidBrush(BackgroundColor), PageRect);

            for (int Index = 0; Index < TabCount; Index++)
            {
                bool IsSelected = Index == SelectedIndex;
                bool VertAlign = (Alignment & TabAlignment.Left) != 0;

                Rectangle Rect = RemoveMargin(GetTabRect(Index), 2, 2, 2, 2);
                SizeF TextSize = e.Graphics.MeasureString(TabPages[Index].Text, Font);

                if (IsSelected) e.Graphics.FillRectangle(new SolidBrush(BackgroundColor), Rect);

                if (ImageList != null && (
                    ImageList.Images.ContainsKey(TabPages[Index].ImageKey) || 
                    ImageList.Images.Count > (uint)TabPages[Index].ImageIndex))
                {
                    Image Icon = ImageList.Images.ContainsKey(TabPages[Index].ImageKey)
                        ? ImageList.Images[TabPages[Index].ImageKey]
                        : ImageList.Images[TabPages[Index].ImageIndex];

                    int RW =  VertAlign ? Rect.Width  : Rect.Width  - (int)TextSize.Width;
                    int RH = !VertAlign ? Rect.Height : Rect.Height - (int)TextSize.Height;

                    Point IconPt = new Point(
                        Rect.X + (RW >> 1) - (Icon.Width >> 1),
                        Rect.Y + (RH >> 1) - (Icon.Height >> 1));

                    e.Graphics.DrawImageUnscaled(Icon, IconPt);
                }

                if (VertAlign)
                {
                    //Left or Right Tab aligment (text is on vertical position and needs rotation)
                    e.Graphics.TranslateTransform(Rect.X + (Rect.Width >> 1), Rect.Y + (Rect.Height >> 1));
                    e.Graphics.RotateTransform(Alignment == TabAlignment.Right ? 90 : 270);

                    Rect = new Rectangle(
                        -Rect.Height >> 1,
                        -Rect.Width  >> 1,
                         Rect.Height,
                         Rect.Width);
                }

                Brush TextBrush = new SolidBrush(IsSelected ? SelectedForeColor : ForegroundColor);

                StringFormat TextFmt = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                e.Graphics.DrawString(TabPages[Index].Text, Font, TextBrush, Rect, TextFmt);
                e.Graphics.ResetTransform();
            }
        }

        private Rectangle RemoveMargin(Rectangle Input, int Top, int Left, int Right, int Bottom)
        {
            return new Rectangle
            {
                Y = Input.Y - Top,
                X = Input.X - Left,
                Width = Input.Width + Left + Right,
                Height = Input.Height + Top + Bottom
            };
        }
    }
}
