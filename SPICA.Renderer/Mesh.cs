using OpenTK;
using OpenTK.Graphics.ES30;

using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.Formats.CtrH3D.Model.Mesh;
using SPICA.Math3D;
using SPICA.PICA.Commands;

using System;
using System.Collections.Generic;

namespace SPICA.Renderer
{
    public class Mesh : IDisposable
    {
        private int VBOHandle;
        private int VAOHandle;

        private int ShaderHandle;

        private Model Parent;

        public H3DMesh BaseMesh;
        private List<H3DSubMesh> SubMeshes;
        private H3DMaterial Material;

        public Vector3 MeshCenter;

        public Mesh(Model Parent, H3DMesh Mesh, int ShaderHandle)
        {
            this.ShaderHandle = ShaderHandle;
            
            this.Parent = Parent;

            BaseMesh = Mesh;
            SubMeshes = BaseMesh.SubMeshes;
            Material = Parent.Materials[Mesh.MaterialIndex];

            MeshCenter = new Vector3(Mesh.MeshCenter.X, Mesh.MeshCenter.Y, Mesh.MeshCenter.Z);

            IntPtr Length = new IntPtr(Mesh.RawBuffer.Length);

            VBOHandle = GL.GenBuffer();
            VAOHandle = GL.GenVertexArray();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, Length, Mesh.RawBuffer, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindVertexArray(VAOHandle);

            int Offset = 0;

            for (int Index = 0; Index < Mesh.Attributes.Length; Index++)
            {
                GL.DisableVertexAttribArray(Index);
            }

            for (int Index = 0; Index < Mesh.Attributes.Length; Index++)
            {
                PICAAttribute Attrib = Mesh.Attributes[Index];

                int Size = Attrib.Elements;

                VertexAttribPointerType Type = default(VertexAttribPointerType);

                switch (Attrib.Format)
                {
                    case PICAAttributeFormat.Byte: Type = VertexAttribPointerType.Byte; break;
                    case PICAAttributeFormat.Ubyte: Type = VertexAttribPointerType.UnsignedByte; break;
                    case PICAAttributeFormat.Short: Type = VertexAttribPointerType.Short; Size <<= 1; break;
                    case PICAAttributeFormat.Float: Type = VertexAttribPointerType.Float; Size <<= 2; break;
                }

                int AttribIndex = (int)Attrib.Name;

                if (AttribIndex < 9)
                {
                    GL.EnableVertexAttribArray(AttribIndex);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, VBOHandle);
                    GL.VertexAttribPointer(AttribIndex, Attrib.Elements, Type, true, Mesh.VertexStride, Offset);
                }

                Offset += Size;
            }

            GL.BindVertexArray(0);
        }

        public void Render()
        {
            //Upload Material data to Fragment Shader
            H3DMaterialParams Params = Material.MaterialParams;

            //Coordinate sources
            int TexUnit0SourceLocation = GL.GetUniformLocation(ShaderHandle, "TexUnit0Source");
            int TexUnit1SourceLocation = GL.GetUniformLocation(ShaderHandle, "TexUnit1Source");
            int TexUnit2SourceLocation = GL.GetUniformLocation(ShaderHandle, "TexUnit2Source");

            GL.Uniform1(TexUnit0SourceLocation, (int)Params.TextureSources[0]);
            GL.Uniform1(TexUnit1SourceLocation, (int)Params.TextureSources[1]);
            GL.Uniform1(TexUnit2SourceLocation, (int)Params.TextureSources[2]);

            //Material colors
            int MEmissionLocation = GL.GetUniformLocation(ShaderHandle, "MEmission");
            int MDiffuseLocation = GL.GetUniformLocation(ShaderHandle, "MDiffuse");
            int MAmbientLocation = GL.GetUniformLocation(ShaderHandle, "MAmbient");
            int MSpecularLocation = GL.GetUniformLocation(ShaderHandle, "MSpecular");

            GL.Uniform4(MEmissionLocation, GetColorVector(Params.EmissionColor));
            GL.Uniform4(MDiffuseLocation, GetColorVector(Params.DiffuseColor));
            GL.Uniform4(MAmbientLocation, GetColorVector(Params.AmbientColor));
            GL.Uniform4(MSpecularLocation, GetColorVector(Params.Specular0Color));

            //Shader math parameters
            int FragFlagsLocation = GL.GetUniformLocation(ShaderHandle, "FragFlags");
            int FresnelSelLocation = GL.GetUniformLocation(ShaderHandle, "FresnelSel");
            int BumpIndexLocation = GL.GetUniformLocation(ShaderHandle, "BumpIndex");
            int BumpModeLocation = GL.GetUniformLocation(ShaderHandle, "BumpMode");

            GL.Uniform1(FragFlagsLocation, (int)Params.FragmentFlags);
            GL.Uniform1(FresnelSelLocation, (int)Params.FresnelSelector);
            GL.Uniform1(BumpIndexLocation, Params.BumpTexture);
            GL.Uniform1(BumpModeLocation, (int)Params.BumpMode);

            //Texture Environment stages
            for (int Index = 0; Index < 6; Index++)
            {
                PICATexEnvStage Stage = Params.TexEnvStages[Index];

                int ColorCombLocation = GL.GetUniformLocation(ShaderHandle, $"Combiners[{Index}].ColorCombine");
                int AlphaCombLocation = GL.GetUniformLocation(ShaderHandle, $"Combiners[{Index}].AlphaCombine");

                int ColorScaleLocation = GL.GetUniformLocation(ShaderHandle, $"Combiners[{Index}].ColorScale");
                int AlphaScaleLocation = GL.GetUniformLocation(ShaderHandle, $"Combiners[{Index}].AlphaScale");

                GL.Uniform1(ColorCombLocation, (int)Stage.Combiner.RGBCombiner);
                GL.Uniform1(AlphaCombLocation, (int)Stage.Combiner.AlphaCombiner);

                GL.Uniform1(ColorScaleLocation, (float)(1 << (int)Stage.Scale.RGBScale));
                GL.Uniform1(AlphaScaleLocation, (float)(1 << (int)Stage.Scale.AlphaScale));

                for (int Param = 0; Param < 3; Param++)
                {
                    int ColorSrcLocation = GL.GetUniformLocation(ShaderHandle, $"Combiners[{Index}].Args[{Param}].ColorSrc");
                    int AlphaSrcLocation = GL.GetUniformLocation(ShaderHandle, $"Combiners[{Index}].Args[{Param}].AlphaSrc");
                    int ColorOpLocation = GL.GetUniformLocation(ShaderHandle, $"Combiners[{Index}].Args[{Param}].ColorOp");
                    int AlphaOpLocation = GL.GetUniformLocation(ShaderHandle, $"Combiners[{Index}].Args[{Param}].AlphaOp");

                    GL.Uniform1(ColorSrcLocation, (int)Stage.Source.RGBSource[Param]);
                    GL.Uniform1(AlphaSrcLocation, (int)Stage.Source.AlphaSource[Param]);
                    GL.Uniform1(ColorOpLocation, (int)Stage.Operand.RGBOp[Param]);
                    GL.Uniform1(AlphaOpLocation, (int)Stage.Operand.AlphaOp[Param]);
                }

                Vector4 Color = new Vector4(
                    (float)Stage.Color.R / byte.MaxValue,
                    (float)Stage.Color.G / byte.MaxValue,
                    (float)Stage.Color.B / byte.MaxValue,
                    (float)Stage.Color.A / byte.MaxValue);

                int ColorLocation = GL.GetUniformLocation(ShaderHandle, $"Combiners[{Index}].Color");

                GL.Uniform4(ColorLocation, GetColorVector(Stage.Color.ToRGBA()));
            }

            //Setup LUTs
            for (int Index = 0; Index < 6; Index++)
            {
                int LUTIsAbsLocation = GL.GetUniformLocation(ShaderHandle, $"LUTs[{Index}].IsAbs");
                int LUTInputLocation = GL.GetUniformLocation(ShaderHandle, $"LUTs[{Index}].Input");
                int LUTScaleLocation = GL.GetUniformLocation(ShaderHandle, $"LUTs[{Index}].Scale");

                switch (Index)
                {
                    case 0:
                        GL.Uniform1(LUTIsAbsLocation, Params.LUTInputAbs.Dist0Abs ? 1 : 0);
                        GL.Uniform1(LUTInputLocation, (int)Params.LUTInputSel.Dist0Input);
                        GL.Uniform1(LUTScaleLocation, Params.LUTInputScaleSel.Dist0Scale.ToFloat());
                        break;
                    case 1:
                        GL.Uniform1(LUTIsAbsLocation, Params.LUTInputAbs.Dist1Abs ? 1 : 0);
                        GL.Uniform1(LUTInputLocation, (int)Params.LUTInputSel.Dist1Input);
                        GL.Uniform1(LUTScaleLocation, Params.LUTInputScaleSel.Dist1Scale.ToFloat());
                        break;
                    case 2:
                        GL.Uniform1(LUTIsAbsLocation, Params.LUTInputAbs.FresnelAbs ? 1 : 0);
                        GL.Uniform1(LUTInputLocation, (int)Params.LUTInputSel.FresnelInput);
                        GL.Uniform1(LUTScaleLocation, Params.LUTInputScaleSel.FresnelScale.ToFloat());
                        break;
                    case 3:
                        GL.Uniform1(LUTIsAbsLocation, Params.LUTInputAbs.ReflecRAbs ? 1 : 0);
                        GL.Uniform1(LUTInputLocation, (int)Params.LUTInputSel.ReflecRInput);
                        GL.Uniform1(LUTScaleLocation, Params.LUTInputScaleSel.ReflecRScale.ToFloat());
                        break;
                    case 4:
                        GL.Uniform1(LUTIsAbsLocation, Params.LUTInputAbs.ReflecGAbs ? 1 : 0);
                        GL.Uniform1(LUTInputLocation, (int)Params.LUTInputSel.ReflecGInput);
                        GL.Uniform1(LUTScaleLocation, Params.LUTInputScaleSel.ReflecGScale.ToFloat());
                        break;
                    case 5:
                        GL.Uniform1(LUTIsAbsLocation, Params.LUTInputAbs.ReflecBAbs ? 1 : 0);
                        GL.Uniform1(LUTInputLocation, (int)Params.LUTInputSel.ReflecBInput);
                        GL.Uniform1(LUTScaleLocation, Params.LUTInputScaleSel.ReflecBScale.ToFloat());
                        break;
                }
            }

            BindLUT(1, Params.LUTDist0TableName, Params.LUTDist0SamplerName);
            BindLUT(2, Params.LUTDist1TableName, Params.LUTDist1SamplerName);
            BindLUT(3, Params.LUTFresnelTableName, Params.LUTFresnelSamplerName);
            BindLUT(4, Params.LUTReflecRTableName, Params.LUTReflecRSamplerName);
            BindLUT(5, Params.LUTReflecGTableName, Params.LUTReflecGSamplerName);
            BindLUT(6, Params.LUTReflecBTableName, Params.LUTReflecBSamplerName);

            //Setup texture units
            if (Material.EnabledTextures[0])
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, Parent.GetTextureId(Material.Texture0Name));
            }

            if (Material.EnabledTextures[1])
            {
                GL.ActiveTexture(TextureUnit.Texture1);
                GL.BindTexture(TextureTarget.Texture2D, Parent.GetTextureId(Material.Texture1Name));
            }

            if (Material.EnabledTextures[2])
            {
                GL.ActiveTexture(TextureUnit.Texture2);
                GL.BindTexture(TextureTarget.Texture2D, Parent.GetTextureId(Material.Texture2Name));
            }

            //Render all SubMeshes
            GL.BindVertexArray(VAOHandle);

            foreach (H3DSubMesh SubMesh in SubMeshes)
            {
                GL.DrawElements(PrimitiveType.Triangles, SubMesh.Indices.Length, DrawElementsType.UnsignedShort, SubMesh.Indices);
            }

            GL.BindVertexArray(0);
        }

        private Vector4 GetColorVector(RGBA Color)
        {
            return new Vector4(
                (float)Color.R / byte.MaxValue,
                (float)Color.G / byte.MaxValue,
                (float)Color.B / byte.MaxValue,
                (float)Color.A / byte.MaxValue);
        }

        private void BindLUT(int Index, string TableName, string SamplerName)
        {
            if (!(TableName == null || SamplerName == null))
            {
                int UBOHandle = Parent.GetLUTHandle(TableName + "/" + SamplerName);

                GL.BindBufferBase(BufferRangeTarget.UniformBuffer, Index, UBOHandle);
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
