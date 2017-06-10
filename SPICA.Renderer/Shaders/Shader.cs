using OpenTK.Graphics.OpenGL;

using System;

namespace SPICA.Renderer.Shaders
{
    class Shader : IDisposable
    {
        public int Handle               { get; private set; }
        public int VertexShaderHandle   { get; private set; }
        public int FragmentShaderHandle { get; private set; }

        /*
         * Only delete the Vertex and Fragment Shaders if they were not compiled externally.
         * This is done to avoid deleting shaders that may be used elsewhere.
         */
        private bool KeepVertexShader;
        private bool KeepFragmentShader;

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

            KeepVertexShader = true;
        }

        public void SetFragmentShaderHandle(int Handle)
        {
            FragmentShaderHandle = Handle;

            KeepFragmentShader = true;
        }

        public void SetVertexShaderCode(string Code)
        {
            CompileAndCheck(VertexShaderHandle = GL.CreateShader(ShaderType.VertexShader), Code);

            KeepVertexShader = false;
        }

        public void SetFragmentShaderCode(string Code)
        {
            CompileAndCheck(FragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader), Code);

            KeepFragmentShader = false;
        }

        public static void CompileAndCheck(int Handle, string Code)
        {
            GL.ShaderSource(Handle, Code);
            GL.CompileShader(Handle);
            CheckCompilation(Handle);
        }

        public static void CheckCompilation(int Handle)
        {
            int Status = 0;

            GL.GetShader(Handle, ShaderParameter.CompileStatus, out Status);

            if (Status == 0)
            {
                throw new ShaderCompilationException(
                    "Error compiling Shader!" + Environment.NewLine +
                    GL.GetShaderInfoLog(Handle));
            }
        }

        public void Link()
        {
            if ((VertexShaderHandle | FragmentShaderHandle) != 0)
            {
                Handle = GL.CreateProgram();

                GL.AttachShader(Handle, VertexShaderHandle);
                GL.AttachShader(Handle, FragmentShaderHandle);
                GL.LinkProgram(Handle);
            }
        }

        private bool Disposed;

        protected virtual void Dispose(bool Disposing)
        {
            if (!Disposed)
            {
                GL.DeleteProgram(Handle);

                if (!KeepVertexShader)   GL.DeleteShader(VertexShaderHandle);
                if (!KeepFragmentShader) GL.DeleteShader(FragmentShaderHandle);

                Disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
