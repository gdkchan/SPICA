using OpenTK.Graphics.OpenGL;

using SPICA.Formats.CtrH3D.Shader;
using SPICA.PICA.Shader;
using SPICA.Rendering.Shaders;

using System;

namespace SPICA.Rendering
{
    public class VertexShader : IDisposable
    {
        public string Name { get; private set; }

        public int VtxShaderHandle { get; private set; }
        public int GeoShaderHandle { get; private set; }

        public readonly ShaderNameBlock VtxNames;
        public readonly ShaderNameBlock GeoNames;

        public VertexShader(H3DShader Shdr)
        {
            Name = Shdr.Name;

            ShaderBinary SHBin = new ShaderBinary(Shdr.Program);

            bool HasGeometryShader = Shdr.GeoShaderIndex != -1;

            if (Shdr.VtxShaderIndex != -1)
            {
                VtxShaderHandle = GL.CreateShader(ShaderType.VertexShader);

                VertexShaderGenerator VtxShaderGen = new VertexShaderGenerator(SHBin);

                string Code = VtxShaderGen.GetVtxShader(Shdr.VtxShaderIndex, HasGeometryShader, out VtxNames);

                Shader.CompileAndCheck(VtxShaderHandle, Code);
            }

            if (HasGeometryShader)
            {
                GeoShaderHandle = GL.CreateShader(ShaderType.GeometryShader);

                GeometryShaderGenerator GeoShaderGen = new GeometryShaderGenerator(SHBin);

                string Code = GeoShaderGen.GetGeoShader(Shdr.GeoShaderIndex, VtxNames.Outputs, out GeoNames);

                Shader.CompileAndCheck(GeoShaderHandle, Code);
            }
        }

        public VertexShader(int VertexShaderHandle)
        {
            VtxShaderHandle = VertexShaderHandle;

            VtxNames = new ShaderNameBlock();
            GeoNames = new ShaderNameBlock();

            VtxNames.Vec4Uniforms[DefaultShaderIds.PosOffs]     = "PosOffs";
            VtxNames.Vec4Uniforms[DefaultShaderIds.IrScale + 0] = "IrScale[0]";
            VtxNames.Vec4Uniforms[DefaultShaderIds.IrScale + 1] = "IrScale[1]";
            VtxNames.Vec4Uniforms[DefaultShaderIds.TexcMap]     = "TexcMap";
            VtxNames.Vec4Uniforms[DefaultShaderIds.TexMtx2 + 0] = "TexMtx2[0]";
            VtxNames.Vec4Uniforms[DefaultShaderIds.TexMtx2 + 1] = "TexMtx2[1]";
            VtxNames.Vec4Uniforms[DefaultShaderIds.TexTran]     = "TexTran";
            VtxNames.Vec4Uniforms[DefaultShaderIds.MatAmbi]     = "MatAmbi";
            VtxNames.Vec4Uniforms[DefaultShaderIds.MatDiff]     = "MatDiff";
            VtxNames.Vec4Uniforms[DefaultShaderIds.HsLGCol]     = "HslGCol";
            VtxNames.Vec4Uniforms[DefaultShaderIds.HsLSCol]     = "HslSCol";
            VtxNames.Vec4Uniforms[DefaultShaderIds.HsLSDir]     = "HslSDir";
            VtxNames.Vec4Uniforms[DefaultShaderIds.ProjMtx + 0] = "ProjMtx[0]";
            VtxNames.Vec4Uniforms[DefaultShaderIds.ProjMtx + 1] = "ProjMtx[1]";
            VtxNames.Vec4Uniforms[DefaultShaderIds.ProjMtx + 2] = "ProjMtx[2]";
            VtxNames.Vec4Uniforms[DefaultShaderIds.ProjMtx + 3] = "ProjMtx[3]";

            for (int i = 0; i < 3; i++)
            {
                VtxNames.Vec4Uniforms[DefaultShaderIds.WrldMtx + i] = $"WrldMtx[{i}]";
                VtxNames.Vec4Uniforms[DefaultShaderIds.NormMtx + i] = $"NormMtx[{i}]";
                VtxNames.Vec4Uniforms[DefaultShaderIds.TexMtx0 + i] = $"TexMtx0[{i}]";
                VtxNames.Vec4Uniforms[DefaultShaderIds.TexMtx1 + i] = $"TexMtx1[{i}]";
                VtxNames.Vec4Uniforms[DefaultShaderIds.ViewMtx + i] = $"ViewMtx[{i}]";
            }

            for (int i = 0; i < 60; i++)
            {
                VtxNames.Vec4Uniforms[DefaultShaderIds.UnivReg + i] = $"UnivReg[{i}]";
            }
        }

        private bool Disposed;

        protected virtual void Dispose(bool Disposing)
        {
            if (!Disposed)
            {
                GL.DeleteShader(VtxShaderHandle);
                GL.DeleteShader(GeoShaderHandle);

                Disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
