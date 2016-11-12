using OpenTK;
using OpenTK.Graphics.ES30;

using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.Formats.CtrH3D.Model.Material.Texture;
using SPICA.Formats.CtrH3D.Model.Mesh;
using SPICA.PICA.Commands;
using SPICA.Renderer.SPICA_GL;

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
        private Vector4 PosOffset;
        private Vector4 Scales0;
        private Vector4 Scales1;

        public Mesh(Model Parent, H3DMesh Mesh, int ShaderHandle)
        {
            this.ShaderHandle = ShaderHandle;
            
            this.Parent = Parent;

            BaseMesh = Mesh;
            SubMeshes = BaseMesh.SubMeshes;
            Material = Parent.Materials[Mesh.MaterialIndex];

            MeshCenter = GLConverter.ToVector3(Mesh.MeshCenter);
            PosOffset = GLConverter.ToVector4(Mesh.PositionOffset);

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
                    GL.VertexAttribPointer(AttribIndex, Attrib.Elements, Type, false, Mesh.VertexStride, Offset);

                    //Bone Index (7) doesn't have Scale so we need to ignore it here
                    if (AttribIndex == 8) AttribIndex--;

                    int i = AttribIndex >> 2;
                    int j = AttribIndex & 3;

                    if (i == 0)
                        Scales0[j] = Attrib.Scale;
                    else
                        Scales1[j] = Attrib.Scale;
                }

                Offset += Size;
            }

            GL.BindVertexArray(0);
        }

        public void Render()
        {
            //Upload Material data to Fragment Shader
            H3DMaterialParams Params = Material.MaterialParams;

            //Blending and Logical Operation
            BlendEquationMode ColorEquation = Params.BlendFunction.RGBEquation.ToBlendEquation();
            BlendEquationMode AlphaEquation = Params.BlendFunction.AlphaEquation.ToBlendEquation();

            BlendingFactorSrc ColorSrcFunc = Params.BlendFunction.RGBSourceFunc.ToBlendingFactorSrc();
            BlendingFactorDest ColorDstFunc = Params.BlendFunction.RGBDestFunc.ToBlendingFactorDest();
            BlendingFactorSrc AlphaSrcFunc = Params.BlendFunction.AlphaSourceFunc.ToBlendingFactorSrc();
            BlendingFactorDest AlphaDstFunc = Params.BlendFunction.AlphaDestFunc.ToBlendingFactorDest();

            GL.BlendEquationSeparate(ColorEquation, AlphaEquation);
            GL.BlendFuncSeparate(ColorSrcFunc, ColorDstFunc, AlphaSrcFunc, AlphaDstFunc);
            GL.BlendColor(GLConverter.ToColor(Params.BlendColor));

            Utils.SetState(EnableCap.Blend, Params.ColorOperation.BlendMode == PICABlendMode.Blend);

            //Alpha, Stencil and Depth testing
            StencilFunction StencilFunc = Params.StencilTest.Function.ToStencilFunction();
            DepthFunction DepthFunc = Params.DepthColorMask.DepthFunc.ToDepthFunction();

            StencilOp Fail = Params.StencilOperation.FailOp.ToStencilOp();
            StencilOp ZFail = Params.StencilOperation.ZFailOp.ToStencilOp();
            StencilOp ZPass = Params.StencilOperation.ZPassOp.ToStencilOp();

            GL.StencilFunc(StencilFunc, Params.StencilTest.Reference, Params.StencilTest.Mask);
            GL.DepthFunc(DepthFunc);
            GL.DepthMask(Params.DepthColorMask.DepthWrite);
            GL.StencilMask(Params.StencilTest.BufferMask);
            GL.StencilOp(Fail, ZFail, ZPass);

            Utils.SetState(EnableCap.AlphaTest, Params.AlphaTest.Enabled);
            Utils.SetState(EnableCap.StencilTest, Params.StencilTest.Enabled);
            Utils.SetState(EnableCap.DepthTest, Params.DepthColorMask.Enabled);

            GL.Uniform1(GL.GetUniformLocation(ShaderHandle, "AlphaTestEnb"), Params.AlphaTest.Enabled ? 1 : 0);
            GL.Uniform1(GL.GetUniformLocation(ShaderHandle, "AlphaTestFunc"), (int)Params.AlphaTest.Function);
            GL.Uniform1(GL.GetUniformLocation(ShaderHandle, "AlphaTestRef"), Params.AlphaTest.Reference);

            GL.CullFace(Params.FaceCulling.ToCullFaceMode());

            GL.PolygonOffset(0, Params.PolygonOffsetUnit);

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

            GL.Uniform4(MEmissionLocation, GLConverter.ToColor(Params.EmissionColor));
            GL.Uniform4(MDiffuseLocation, GLConverter.ToColor(Params.DiffuseColor));
            GL.Uniform4(MAmbientLocation, GLConverter.ToColor(Params.AmbientColor));
            GL.Uniform4(MSpecularLocation, GLConverter.ToColor(Params.Specular0Color));

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

                int UpColorBuffLocation = GL.GetUniformLocation(ShaderHandle, $"Combiners[{Index}].UpColorBuff");
                int UpAlphaBuffLocation = GL.GetUniformLocation(ShaderHandle, $"Combiners[{Index}].UpAlphaBuff");

                GL.Uniform1(ColorCombLocation, (int)Stage.Combiner.RGBCombiner);
                GL.Uniform1(AlphaCombLocation, (int)Stage.Combiner.AlphaCombiner);

                GL.Uniform1(ColorScaleLocation, (float)(1 << (int)Stage.Scale.RGBScale));
                GL.Uniform1(AlphaScaleLocation, (float)(1 << (int)Stage.Scale.AlphaScale));

                GL.Uniform1(UpColorBuffLocation, Stage.UpdateRGBBuffer ? 1 : 0);
                GL.Uniform1(UpAlphaBuffLocation, Stage.UpdateAlphaBuffer ? 1 : 0);

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

                int ColorLocation = GL.GetUniformLocation(ShaderHandle, $"Combiners[{Index}].Color");

                GL.Uniform4(ColorLocation, GLConverter.ToColor(Stage.Color.ToRGBA()));
            }

            int BuffColorLocation = GL.GetUniformLocation(ShaderHandle, "BuffColor");

            GL.Uniform4(BuffColorLocation, GLConverter.ToColor(Params.TexEnvBufferColor));

            //Setup LUTs
            for (int Index = 0; Index < 6; Index++)
            {
                int LUTIsAbsLocation = GL.GetUniformLocation(ShaderHandle, $"LUTs[{Index}].IsAbs");
                int LUTInputLocation = GL.GetUniformLocation(ShaderHandle, $"LUTs[{Index}].Input");
                int LUTScaleLocation = GL.GetUniformLocation(ShaderHandle, $"LUTs[{Index}].Scale");

                switch (Index)
                {
                    case 0:
                        GL.Uniform1(LUTInputLocation, (int)Params.LUTInputSel.Dist0Input);
                        GL.Uniform1(LUTScaleLocation, Params.LUTInputScaleSel.Dist0Scale.ToFloat());
                        break;
                    case 1:
                        GL.Uniform1(LUTInputLocation, (int)Params.LUTInputSel.Dist1Input);
                        GL.Uniform1(LUTScaleLocation, Params.LUTInputScaleSel.Dist1Scale.ToFloat());
                        break;
                    case 2:
                        GL.Uniform1(LUTInputLocation, (int)Params.LUTInputSel.FresnelInput);
                        GL.Uniform1(LUTScaleLocation, Params.LUTInputScaleSel.FresnelScale.ToFloat());
                        break;
                    case 3:
                        GL.Uniform1(LUTInputLocation, (int)Params.LUTInputSel.ReflecRInput);
                        GL.Uniform1(LUTScaleLocation, Params.LUTInputScaleSel.ReflecRScale.ToFloat());
                        break;
                    case 4:
                        GL.Uniform1(LUTInputLocation, (int)Params.LUTInputSel.ReflecGInput);
                        GL.Uniform1(LUTScaleLocation, Params.LUTInputScaleSel.ReflecGScale.ToFloat());
                        break;
                    case 5:
                        GL.Uniform1(LUTInputLocation, (int)Params.LUTInputSel.ReflecBInput);
                        GL.Uniform1(LUTScaleLocation, Params.LUTInputScaleSel.ReflecBScale.ToFloat());
                        break;
                }
            }

            BindLUT(0, Params.LUTDist0TableName, Params.LUTDist0SamplerName);
            BindLUT(1, Params.LUTDist1TableName, Params.LUTDist1SamplerName);
            BindLUT(2, Params.LUTFresnelTableName, Params.LUTFresnelSamplerName);
            BindLUT(3, Params.LUTReflecRTableName, Params.LUTReflecRSamplerName);

            //Use Reflectance R values if G/B are not present
            BindLUT(4,
                Params.LUTReflecGTableName   ?? Params.LUTReflecRTableName, 
                Params.LUTReflecGSamplerName ?? Params.LUTReflecRSamplerName);

            BindLUT(5,
                Params.LUTReflecBTableName   ?? Params.LUTReflecRTableName, 
                Params.LUTReflecBSamplerName ?? Params.LUTReflecRSamplerName);

            //Vertex related Uniforms
            GL.Uniform4(GL.GetUniformLocation(ShaderHandle, "PosOffset"), PosOffset);
            GL.Uniform4(GL.GetUniformLocation(ShaderHandle, "Scales0"), Scales0);
            GL.Uniform4(GL.GetUniformLocation(ShaderHandle, "Scales1"), Scales1);

            //Setup texture units
            if (Material.EnabledTextures[0])
            {
                int TextureId = Parent.GetTextureId(Material.Texture0Name);

                //Only the texture unit 0 can have a Cube Map texture
                if (Params.TextureCoords[0].MappingType == H3DTextureMappingType.CameraCubeEnvMap)
                {
                    GL.ActiveTexture(TextureUnit.Texture3);
                    GL.BindTexture(TextureTarget.TextureCubeMap, TextureId);
                }
                else
                {
                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.Texture2D, TextureId);
                }
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

            //Setup Fixed attributes
            int FixedColorLocation = GL.GetUniformLocation(ShaderHandle, "FixedColor");
            int FixedBoneLocation = GL.GetUniformLocation(ShaderHandle, "FixedBone");
            int FixedWeightLocation = GL.GetUniformLocation(ShaderHandle, "FixedWeight");

            GL.Uniform4(FixedColorLocation, new Vector4(-1));
            GL.Uniform4(FixedBoneLocation, new Vector4(0));
            GL.Uniform4(FixedWeightLocation, new Vector4(0));

            foreach (PICAFixedAttribute Attrib in BaseMesh.FixedAttributes)
            {
                Vector4 Value = GLConverter.ToVector4(Attrib.Value);

                switch (Attrib.Name)
                {
                    case PICAAttributeName.Color: GL.Uniform4(FixedColorLocation, Value); break;
                    case PICAAttributeName.BoneIndex: GL.Uniform4(FixedBoneLocation, Value); break;
                    case PICAAttributeName.BoneWeight: GL.Uniform4(FixedWeightLocation, Value); break;
                }
            }

            //Render all SubMeshes
            GL.BindVertexArray(VAOHandle);

            foreach (H3DSubMesh SubMesh in SubMeshes)
            {
                bool SmoothSkin = SubMesh.Skinning == H3DSubMeshSkinning.Smooth;

                Matrix4[] Transforms = new Matrix4[32];

                for (int Index = 0; Index < SubMesh.BoneIndicesCount; Index++)
                {
                    Matrix4 Transform = Parent.SkeletonTransform[SubMesh.BoneIndices[Index]];

                    if (SmoothSkin)
                    {
                        Transform = Parent.InverseTransform[SubMesh.BoneIndices[Index]] * Transform;
                    }

                    int Location = GL.GetUniformLocation(ShaderHandle, $"Transforms[{Index}]");

                    GL.UniformMatrix4(Location, false, ref Transform);
                }

                GL.Uniform1(GL.GetUniformLocation(ShaderHandle, "SmoothSkin"), SmoothSkin ? 1 : 0);

                GL.DrawElements(PrimitiveType.Triangles, SubMesh.Indices.Length, DrawElementsType.UnsignedShort, SubMesh.Indices);
            }

            GL.BindVertexArray(0);
        }

        private void BindLUT(int Index, string TableName, string SamplerName)
        {
            if (!(TableName == null || SamplerName == null))
            {
                string Name = TableName + "/" + SamplerName;

                int UBOHandle = Parent.GetLUTHandle(Name);
                int LUTIsAbsLocation = GL.GetUniformLocation(ShaderHandle, $"LUTs[{Index}].IsAbs");

                GL.BindBufferBase(BufferRangeTarget.UniformBuffer, Index, UBOHandle);
                GL.Uniform1(LUTIsAbsLocation, Parent.GetIsLUTAbs(Name) ? 1 : 0);
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
