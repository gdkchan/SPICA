using OpenTK;
using OpenTK.Graphics.ES30;

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace SPICA.Renderer.GUI
{
    class GUILabel : GUIControl
    {
        private Font TextFont;
        private Brush TextBrush;

        private string _Text;

        public string Text
        {
            get { return _Text; }
            set { _Text = value; SetNewText(); }
        }

        public GUILabel(Vector2 Position, string Text) : base(Position)
        {
            TextFont = new Font(FontFamily.GenericMonospace, 14, FontStyle.Bold);
            TextBrush = Brushes.White;

            this.Text = Text;
        }

        internal override void Focus()
        {
            throw new NotImplementedException();
        }

        internal override void KeyDown()
        {
            throw new NotImplementedException();
        }

        internal override void Render()
        {
            if (Text != null) RenderQuad();
        }

        private void SetNewText()
        {
            int[] Viewport = new int[4];

            GL.GetInteger(GetPName.Viewport, Viewport);

            int ScrnWidth = Viewport[2];
            int ScrnHeight = Viewport[3];

            SizeF TextSize = GetTextSize();

            CreateQuad(new Vector2(TextSize.Width / ScrnWidth, TextSize.Height / ScrnHeight));

            using (Bitmap Img = new Bitmap((int)TextSize.Width, (int)TextSize.Height))
            {
                using (Graphics g = Graphics.FromImage(Img))
                {
                    g.TextRenderingHint = TextRenderingHint.AntiAlias;

                    g.DrawString(Text, TextFont, TextBrush, PointF.Empty);
                }

                Rectangle Rect = new Rectangle(0, 0, Img.Width, Img.Height);

                BitmapData ImgData = Img.LockBits(Rect, ImageLockMode.ReadOnly, Img.PixelFormat);

                CreateTexture(ImgData.Scan0, Img.Width, Img.Height);

                Img.UnlockBits(ImgData);
            }
        }

        private SizeF GetTextSize()
        {
            //TODO: Find a less hacky way to measure the text
            using (Bitmap Img = new Bitmap(1, 1))
            {
                using (Graphics g = Graphics.FromImage(Img))
                {
                    return g.MeasureString(_Text, TextFont);
                }
            }
        }
    }
}
