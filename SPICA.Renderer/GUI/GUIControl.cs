using OpenTK;
using OpenTK.Graphics.ES30;

using System;
using System.Drawing;

namespace SPICA.Renderer.GUI
{
    public abstract class GUIControl : IDisposable
    {
        private int VBOHandle;
        private int VAOHandle;

        private int TextureId;

        private int X;
        private int Y;

        private GUIDockMode DockMode;

        public GUIControl(int X, int Y, GUIDockMode DockMode)
        {
            this.X = X;
            this.Y = Y;

            this.DockMode = DockMode;
        }

        internal abstract void Focus();

        internal abstract void KeyDown();

        internal abstract void Render();

        internal abstract void Resize();

        protected void RenderQuad()
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, TextureId);

            GL.BindVertexArray(VAOHandle);
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
            GL.BindVertexArray(0);
        }

        protected void CreateQuad(Vector2 Size)
        {
            if ((VBOHandle | VAOHandle) != 0)
            {
                GL.DeleteBuffer(VBOHandle);
                GL.DeleteVertexArray(VAOHandle);
            }

            int[] Viewport = new int[4];

            GL.GetInteger(GetPName.Viewport, Viewport);

            float ScrnWidth = Viewport[2];
            float ScrnHeight = Viewport[3];

            float SzX = Size.X / ScrnWidth;
            float SzY = Size.Y / ScrnHeight;

            float PosX = X / ScrnWidth;
            float PosY = Y / ScrnHeight;

            float InvX = (1 - PosX) - SzX;
            float InvY = (1 - PosY) - SzY;

            float SX = 0.5f - SzX * 0.5f;
            float SY = 0.5f - SzY * 0.5f;

            if ((DockMode & GUIDockMode.Top)    != 0) SY = InvY;
            if ((DockMode & GUIDockMode.Left)   != 0) SX = PosX;
            if ((DockMode & GUIDockMode.Right)  != 0) SX = InvX;
            if ((DockMode & GUIDockMode.Bottom) != 0) SY = PosY;

            float EX = SX + SzX;
            float EY = SY + SzY;

            Vector4[] Buffer = new Vector4[]
            {
                new Vector4(SX, SY, 0, 1),
                new Vector4(SX, EY, 0, 0),
                new Vector4(EX, SY, 1, 1),
                new Vector4(EX, EY, 1, 0)
            };

            VBOHandle = GL.GenBuffer();
            VAOHandle = GL.GenVertexArray();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(4 * 16), Buffer, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindVertexArray(VAOHandle);

            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOHandle);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 16, 0);

            GL.BindVertexArray(0);
        }

        protected void CreateTexture(IntPtr ImgPtr, int Width, int Height)
        {
            if (TextureId != 0) GL.DeleteTexture(TextureId);

            TextureId = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, TextureId);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexImage2D(TextureTarget2d.Texture2D,
                0,
                TextureComponentCount.Rgba,
                Width,
                Height,
                0,
                PixelFormat.Rgba,
                PixelType.UnsignedByte,
                ImgPtr);
        }

        private bool Disposed;

        protected virtual void Dispose(bool Disposing)
        {
            if (!Disposed)
            {
                GL.DeleteBuffer(VBOHandle);
                GL.DeleteVertexArray(VAOHandle);

                GL.DeleteTexture(TextureId);

                Disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
