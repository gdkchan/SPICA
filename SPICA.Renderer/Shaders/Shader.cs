using OpenTK.Graphics.ES30;

using System;
using System.Diagnostics;

namespace SPICA.Renderer.Shaders
{
    class Shader : IDisposable
    {
        public int ShaderHandle { get; private set; }

        private int VertexShaderHandle;
        private int FragmentShaderHandle;

        public Shader(string VShCode, string FShCode)
        {
            ShaderHandle = GL.CreateProgram();

            VertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            FragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(VertexShaderHandle, VShCode);
            GL.ShaderSource(FragmentShaderHandle, FShCode);

            GL.CompileShader(VertexShaderHandle);
            GL.CompileShader(FragmentShaderHandle);

            GL.AttachShader(ShaderHandle, VertexShaderHandle);
            GL.AttachShader(ShaderHandle, FragmentShaderHandle);

            GL.LinkProgram(ShaderHandle);

            Debug.WriteLine("[RenderEngine] Shader compilation result (Vertex/Fragment/Shader):");

            Debug.WriteLine(GL.GetShaderInfoLog(VertexShaderHandle));
            Debug.WriteLine(GL.GetShaderInfoLog(FragmentShaderHandle));
            Debug.WriteLine(GL.GetProgramInfoLog(ShaderHandle));
        }

        private bool Disposed;

        protected virtual void Dispose(bool Disposing)
        {
            if (!Disposed)
            {
                GL.DeleteProgram(ShaderHandle);

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
