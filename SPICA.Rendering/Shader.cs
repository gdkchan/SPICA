using OpenTK.Graphics.OpenGL;

using SPICA.Formats.CtrH3D.Shader;
using SPICA.PICA.Shader;
using SPICA.Rendering.Shaders;

using System;

namespace SPICA.Rendering
{
    public class Shader : IDisposable
    {
        public string Name { get; private set; }

        public int VertexShaderHandle   { get; private set; }
        public int GeometryShaderHandle { get; private set; }

        public readonly ShaderNameBlock VertexShaderUniforms;
        public readonly ShaderNameBlock GeometryShaderUniforms;

        public Shader(H3DShader Shader)
        {
            Name = Shader.Name;

            bool HasGeometryShader = Shader.GeometryShaderIndex != -1;

            if (Shader.VertexShaderIndex != -1)
            {
                VertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);

                ShaderBinary SHBin = new ShaderBinary(Shader.Program);

                VertexShaderGenerator VtxShaderGen = new VertexShaderGenerator(SHBin);

                string Code = VtxShaderGen.GetVtxShader(Shader.VertexShaderIndex, HasGeometryShader,
                    out VertexShaderUniforms);

                ShaderManager.CompileAndCheck(VertexShaderHandle, Code);
            }

            if (HasGeometryShader)
            {
                GeometryShaderHandle = GL.CreateShader(ShaderType.GeometryShader);

                ShaderBinary SHBin = new ShaderBinary(Shader.Program);

                GeometryShaderGenerator GeoShaderGen = new GeometryShaderGenerator(SHBin);

                string Code = GeoShaderGen.GetGeoShader(Shader.GeometryShaderIndex, VertexShaderUniforms.Outputs,
                    out GeometryShaderUniforms);

                ShaderManager.CompileAndCheck(GeometryShaderHandle, Code);
            }
        }

        private bool Disposed;

        protected virtual void Dispose(bool Disposing)
        {
            if (!Disposed)
            {
                GL.DeleteShader(VertexShaderHandle);
                GL.DeleteShader(GeometryShaderHandle);

                Disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
