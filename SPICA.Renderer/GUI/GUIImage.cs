using OpenTK;

using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace SPICA.Renderer.GUI
{
    class GUIImage : GUIControl
    {
        private Bitmap Img;

        public Bitmap Image
        {
            get { return Img; }
            set { Img = value; CreateTexture(Img); }
        }

        public GUIImage(int X, int Y, GUIDockMode DockMode, Bitmap Img) : base(X, Y, DockMode)
        {
            Image = Img;

            Resize();
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
            RenderQuad();
        }

        internal override void Resize()
        {
            CreateQuad(new Vector2(Img.Width, Img.Height));
        }

        private void CreateTexture(Bitmap Img)
        {
            Rectangle Rect = new Rectangle(0, 0, Img.Width, Img.Height);

            BitmapData ImgData = Img.LockBits(Rect, ImageLockMode.ReadOnly, Img.PixelFormat);

            CreateTexture(ImgData.Scan0, Img.Width, Img.Height);

            Img.UnlockBits(ImgData);
        }
    }
}
