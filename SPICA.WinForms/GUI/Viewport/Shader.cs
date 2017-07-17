using OpenTK.Graphics.OpenGL;

using SPICA.WinForms.Properties;

using System;

namespace SPICA.WinForms.GUI.Viewport
{
    class Shader : IDisposable
    {
        public int Handle { get; private set; }

        private int FragShaderHandle;
        private int VtxShaderHandle;

        public Shader()
        {
            FragShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            VtxShaderHandle  = GL.CreateShader(ShaderType.VertexShader);

            GL.ShaderSource(FragShaderHandle, Resources.FragmentShader);
            GL.ShaderSource(VtxShaderHandle,  Resources.VertexShader);

            GL.CompileShader(FragShaderHandle);
            GL.CompileShader(VtxShaderHandle);

            Link();
        }

        private void Link()
        {
            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, FragShaderHandle);
            GL.AttachShader(Handle, VtxShaderHandle);

            GL.LinkProgram(Handle);
        }

        private bool Disposed;

        protected virtual void Dispose(bool Disposing)
        {
            if (!Disposed)
            {
                GL.DetachShader(Handle, FragShaderHandle);
                GL.DetachShader(Handle, VtxShaderHandle);

                GL.DeleteShader(FragShaderHandle);
                GL.DeleteShader(VtxShaderHandle);
                GL.DeleteProgram(Handle);

                Disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
