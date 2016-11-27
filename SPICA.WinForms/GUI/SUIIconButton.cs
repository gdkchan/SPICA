using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SPICA.WinForms.GUI
{
    class SUIIconButton : Control
    {
        private bool Hover;

        private Bitmap _Icon;

        [Category("Appearance"), Description("Icon of the button.")]
        public Bitmap Icon
        {
            get
            {
                return _Icon;
            }
            set
            {
                _Icon = value;

                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(BackColor);

            if (Hover) e.Graphics.FillRectangle(
                new SolidBrush(Color.FromArgb(0x3f, Color.Black)),
                new Rectangle(0, 1, Width, Height - 2));

            if (_Icon != null)
            {
                e.Graphics.DrawImage(_Icon, new Rectangle(
                    (Width / 2) - (_Icon.Width / 2),
                    (Height / 2) - (_Icon.Height / 2),
                    _Icon.Width,
                    _Icon.Height));
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            Hover = true;

            Invalidate();

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            Hover = false;

            Invalidate();

            base.OnMouseLeave(e);
        }
    }
}
