using OpenTK;
using OpenTK.Graphics.ES30;

using System;
using System.Drawing;

namespace SPICA.Renderer.GUI
{
    abstract class GUIControl : IDisposable
    {
        private int VBOHandle;
        private int VAOHandle;

        private int TextureId;

        public Vector2 Position;

        public GUIControl(Vector2 Position)
        {
            this.Position = Position;
        }

        internal abstract void Focus();

        internal abstract void KeyDown();

        internal abstract void Render();

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

            float SX = Position.X;
            float SY = (1 - Position.Y) - Size.Y;
            float EX = SX + Size.X;
            float EY = SY + Size.Y;

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
