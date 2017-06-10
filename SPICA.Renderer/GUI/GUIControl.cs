using OpenTK;
using OpenTK.Graphics.OpenGL;

using System;

namespace SPICA.Renderer.GUI
{
    public abstract class GUIControl : IDisposable
    {
        private int VBOHandle;
        private int VAOHandle;

        private int TextureId;

        protected Vector2 Position;

        protected GUIDockMode DockMode;

        public GUIControl(int X, int Y, GUIDockMode DockMode) : this(new Vector2(X, Y), DockMode) { }

        public GUIControl(Vector2 Position, GUIDockMode DockMode)
        {
            this.Position = Position;
            this.DockMode = DockMode;
        }

        internal void Render()
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, TextureId);

            GL.BindVertexArray(VAOHandle);
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
            GL.BindVertexArray(0);
        }

        internal abstract void Resize();

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
