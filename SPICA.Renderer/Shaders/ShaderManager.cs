using OpenTK.Graphics.ES30;

using System;
using System.Diagnostics;

namespace SPICA.Renderer.Shaders
{
    public class ShaderManager : IDisposable
    {
        public int Handle { get; private set; }

        private int VertexShaderHandle;
        private int FragmentShaderHandle;

        public ShaderManager(string VShCode, string FShCode)
        {
            Handle = GL.CreateProgram();

            VertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            FragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(VertexShaderHandle, VShCode);
            GL.ShaderSource(FragmentShaderHandle, FShCode);

            GL.CompileShader(VertexShaderHandle);
            GL.CompileShader(FragmentShaderHandle);

            GL.AttachShader(Handle, VertexShaderHandle);
            GL.AttachShader(Handle, FragmentShaderHandle);

            GL.LinkProgram(Handle);

            Debug.WriteLine("[RenderEngine] Shader compilation result (Vertex/Fragment/Shader):");

            Debug.WriteLine(GL.GetShaderInfoLog(VertexShaderHandle));
            Debug.WriteLine(GL.GetShaderInfoLog(FragmentShaderHandle));
            Debug.WriteLine(GL.GetProgramInfoLog(Handle));
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
