using OpenTK;
using OpenTK.Graphics.OpenGL;

using SPICA.Rendering;

using System;

namespace SPICA.WinForms.GUI.Viewport
{
    class AxisLines : IDisposable
    {
        private int VBOHandle;
        private int VAOHandle;

        public bool Visible;

        public Matrix4 Transform;

        private Renderer Renderer;
        private Shader   Shader;

        public AxisLines(Renderer Renderer, Shader Shader)
        {
            this.Renderer = Renderer;
            this.Shader   = Shader;

            Vector4[] Buffer = new Vector4[]
            {
                new Vector4(0), new Vector4(1, 0, 0, 1), new Vector4(20,  0,  0, 1), new Vector4(1, 0, 0, 1),
                new Vector4(0), new Vector4(0, 1, 0, 1), new Vector4( 0, 20,  0, 1), new Vector4(0, 1, 0, 1),
                new Vector4(0), new Vector4(0, 0, 1, 1), new Vector4( 0,  0, 20, 1), new Vector4(0, 0, 1, 1)
            };

            VBOHandle = GL.GenBuffer();
            VAOHandle = GL.GenVertexArray();

            Transform = Matrix4.Identity;

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(Buffer.Length * 16), Buffer, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindVertexArray(VAOHandle);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOHandle);

            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 32, 0);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 32, 16);

            GL.BindVertexArray(0);
        }

        public void Render()
        {
            if (Visible)
            {
                GL.UseProgram(Shader.Handle);

                int ProjMtxLocation = GL.GetUniformLocation(Shader.Handle, "ProjMatrix");
                int ViewMtxLocation = GL.GetUniformLocation(Shader.Handle, "ViewMatrix");
                int WrldMtxLocation = GL.GetUniformLocation(Shader.Handle, "ModelMatrix");

                GL.UniformMatrix4(ProjMtxLocation, false, ref Renderer.Camera.ProjectionMatrix);
                GL.UniformMatrix4(ViewMtxLocation, false, ref Renderer.Camera.ViewMatrix);
                GL.UniformMatrix4(WrldMtxLocation, false, ref Transform);

                GL.LineWidth(2);

                GL.Disable(EnableCap.CullFace);
                GL.Disable(EnableCap.StencilTest);
                GL.Disable(EnableCap.DepthTest);
                GL.Disable(EnableCap.Blend);

                GL.DepthFunc(DepthFunction.Always);

                GL.BindVertexArray(VAOHandle);

                GL.DrawArrays(PrimitiveType.Lines, 0, 6);

                GL.BindVertexArray(0);
            }
        }

        private bool Disposed;

        protected virtual void Dispose(bool Disposing)
        {
            if (!Disposed)
            {
                GL.DeleteBuffer(VBOHandle);
                GL.DeleteVertexArray(VAOHandle);

                Disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
