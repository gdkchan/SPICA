using OpenTK;
using OpenTK.Graphics.ES30;

using SPICA.Renderer;

using System;

namespace SPICA.WinForms.RenderExtensions
{
    class GridLines : TransformableObject, IDisposable
    {
        const int LinesCount = 102;

        private int VBOHandle;
        private int VAOHandle;

        public bool Visible;

        public GridLines()
        {
            Vector4[] Buffer = new Vector4[LinesCount * 4];

            int Index = 0;

            for (int i = -50; i <= 50; i += 2)
            {
                Vector4 Color;

                if ((i % 10) == 0)
                    Color = new Vector4(1f);
                else
                    Color = new Vector4(new Vector3(169 / 255f), 1f);

                Buffer[Index + 0] = new Vector4(i, 0, -50, 1);
                Buffer[Index + 2] = new Vector4(i, 0,  50, 1);

                Buffer[Index + 4] = new Vector4(-50, 0, i, 1);
                Buffer[Index + 6] = new Vector4( 50, 0, i, 1);

                Buffer[Index + 1] = Color;
                Buffer[Index + 3] = Color;

                Buffer[Index + 5] = Color;
                Buffer[Index + 7] = Color;

                Index += 8;
            }

            VBOHandle = GL.GenBuffer();
            VAOHandle = GL.GenVertexArray();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(Buffer.Length * 16), Buffer, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindVertexArray(VAOHandle);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(3);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOHandle);

            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 32, 0);
            GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, 32, 16);

            GL.BindVertexArray(0);
        }

        public void Render(int ShaderHandle)
        {
            if (Visible)
            {
                GL.UseProgram(ShaderHandle);

                RenderUtils.SetupShaderForPosCol(ShaderHandle);

                GL.UniformMatrix4(GL.GetUniformLocation(ShaderHandle, "ModelMatrix"), false, ref Transform);

                GL.LineWidth(1);

                GL.Disable(EnableCap.CullFace);
                GL.Disable(EnableCap.StencilTest);
                GL.Enable(EnableCap.DepthTest);
                GL.Disable(EnableCap.Blend);

                GL.DepthFunc(DepthFunction.Less);

                GL.BindVertexArray(VAOHandle);
                GL.DrawArrays(PrimitiveType.Lines, 0, LinesCount * 2);
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
