using OpenTK;

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace SPICA.Renderer.GUI
{
    public class GUILabel : GUIControl
    {
        private Font TextFont;
        private Brush TextBrush;

        private string _Text;

        public string Text
        {
            get { return _Text; }
            set { _Text = value; UploadNewText(); }
        }

        public GUILabel(int X, int Y, GUIDockMode DockMode, string Text) : base(X, Y, DockMode)
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

        internal override void Resize()
        {
            GetTextSize();
        }

        private void UploadNewText()
        {
            int[] Viewport = new int[4];

            SizeF TextSize = GetTextSize();

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
            SizeF Size;

            using (Bitmap Img = new Bitmap(1, 1))
            {
                using (Graphics g = Graphics.FromImage(Img))
                {
                    Size = g.MeasureString(_Text, TextFont);
                }
            }

            CreateQuad(new Vector2(Size.Width, Size.Height));

            return Size;
        }
    }
}
