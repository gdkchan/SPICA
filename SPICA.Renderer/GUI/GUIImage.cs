using OpenTK;
using OpenTK.Graphics.OpenGL;

using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace SPICA.Renderer.GUI
{
    class GUIImage : GUIControl
    {
        private int VBOHandle;
        private int VAOHandle;

        private int TextureId;

        private Bitmap _Image;

        public Bitmap Image
        {
            get { return _Image; }
            set { _Image = value; UploadTexture(); }
        }

        public GUIImage(int X, int Y, GUIDockMode DockMode, Bitmap Image) : base(X, Y, DockMode)
        {
            this.Image = Image;

            Resize();
        }

        internal override void Resize()
        {
            if ((VBOHandle | VAOHandle) != 0)
            {
                GL.DeleteBuffer(VBOHandle);
                GL.DeleteVertexArray(VAOHandle);
            }

            Vector2 Size = new Vector2(_Image.Width, _Image.Height);

            Tuple<int, int> Handles = RenderUtils.UploadQuad(Position, Size, DockMode);

            VBOHandle = Handles.Item1;
            VAOHandle = Handles.Item2;
        }

        private void UploadTexture()
        {
            if (TextureId != 0) GL.DeleteTexture(TextureId);

            Rectangle Rect = new Rectangle(0, 0, _Image.Width, _Image.Height);

            BitmapData ImgData = _Image.LockBits(Rect, ImageLockMode.ReadOnly, _Image.PixelFormat);

            TextureId = RenderUtils.Upload2DTexture(ImgData.Scan0, _Image.Width, _Image.Height);

            _Image.UnlockBits(ImgData);
        }
    }
}
