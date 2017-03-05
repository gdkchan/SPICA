using OpenTK.Graphics.ES30;

using System;

namespace SPICA.Renderer.Shaders
{
    class Shader : IDisposable
    {
        public int Handle { get; private set; }

        private int VertexShaderHandle;
        private int FragmentShaderHandle;

        public Shader() { }

        public Shader(string VShCode, string FShCode)
        {
            SetVertexShaderCode(VShCode);
            SetFragmentShaderCode(FShCode);

            Link();
        }

        public void SetVertexShaderHandle(int Handle)
        {
            VertexShaderHandle = Handle;
        }

        public void SetFragmentShaderHandle(int Handle)
        {
            FragmentShaderHandle = Handle;
        }

        public void SetVertexShaderCode(string Code)
        {
            VertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);

            GL.ShaderSource(VertexShaderHandle, Code);
            GL.CompileShader(VertexShaderHandle);
        }

        public void SetFragmentShaderCode(string Code)
        {
            FragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(FragmentShaderHandle, Code);
            GL.CompileShader(FragmentShaderHandle);
        }

        public void Link()
        {
            if ((VertexShaderHandle | FragmentShaderHandle) != 0)
            {
                Handle = GL.CreateProgram();

                GL.AttachShader(Handle, VertexShaderHandle);
                GL.AttachShader(Handle, FragmentShaderHandle);
                GL.LinkProgram(Handle);

                int Status = 0;

                GL.GetShader(Handle, ShaderParameter.CompileStatus, out Status);

                if (Status != 0)
                {
                    throw new ShaderCompilationException(
                        "Error compiling Fragment Shader!"        + Environment.NewLine +
                        "Error log (Vertex/Fragment/Program):"    + Environment.NewLine +
                        GL.GetShaderInfoLog(VertexShaderHandle)   + Environment.NewLine +
                        GL.GetShaderInfoLog(FragmentShaderHandle) + Environment.NewLine +
                        GL.GetProgramInfoLog(Handle));
                }
            }
        }

        private bool Disposed;

        protected virtual void Dispose(bool Disposing)
        {
            if (!Disposed)
            {
                GL.DeleteProgram(Handle);

                GL.DeleteShader(VertexShaderHandle);
                GL.DeleteShader(FragmentShaderHandle);

                Disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
