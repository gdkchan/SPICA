using OpenTK;
using OpenTK.Graphics.OpenGL;

using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.Formats.CtrH3D.Model.Mesh;
using SPICA.PICA.Commands;
using SPICA.Renderer.SPICA_GL;

using System;

namespace SPICA.Renderer
{
    public class Mesh : IDisposable
    {
        private int  VBOHandle;
        private int  VAOHandle;
        public  int  ShaderHandle;
        public  bool Visible;

        private  Model       Parent;
        internal H3DMesh     BaseMesh;
        internal H3DMaterial Material;
        private  Vector4     Scales0;
        private  Vector4     Scales1;

        public Mesh(Model Parent, H3DMesh BaseMesh, int ShaderHandle, bool Visible = true)
        {
            this.Parent   = Parent;
            this.BaseMesh = BaseMesh;

            this.ShaderHandle = ShaderHandle;
            this.Visible      = Visible;

            Material = Parent.BaseModel.Materials[BaseMesh.MaterialIndex];

            IntPtr Length = new IntPtr(BaseMesh.RawBuffer.Length);

            VBOHandle = GL.GenBuffer();
            VAOHandle = GL.GenVertexArray();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, Length, BaseMesh.RawBuffer, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindVertexArray(VAOHandle);

            int Offset = 0;

            foreach (PICAAttribute Attrib in BaseMesh.Attributes)
            {
                GL.DisableVertexAttribArray((int)Attrib.Name);
            }

            foreach (PICAAttribute Attrib in BaseMesh.Attributes)
            {
                int Size = Attrib.Elements;

                VertexAttribPointerType Type = default(VertexAttribPointerType);

                switch (Attrib.Format)
                {
                    case PICAAttributeFormat.Byte:  Type = VertexAttribPointerType.Byte;              break;
                    case PICAAttributeFormat.Ubyte: Type = VertexAttribPointerType.UnsignedByte;      break;
                    case PICAAttributeFormat.Short: Type = VertexAttribPointerType.Short; Size <<= 1; break;
                    case PICAAttributeFormat.Float: Type = VertexAttribPointerType.Float; Size <<= 2; break;
                }

                int AttribIndex = (int)Attrib.Name;

                if (AttribIndex < 9)
                {
                    GL.EnableVertexAttribArray(AttribIndex);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, VBOHandle);
                    GL.VertexAttribPointer(AttribIndex, Attrib.Elements, Type, false, BaseMesh.VertexStride, Offset);

                    //Bone Index (7) doesn't have Scale so we need to ignore it here
                    if (AttribIndex == 8) AttribIndex--;

                    int i = AttribIndex >> 2;
                    int j = AttribIndex &  3;

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
            H3DMaterialParams Params = Material.MaterialParams;

            Params.BlendFunction.SetGL();
            Params.StencilTest.SetGL();
            Params.StencilOperation.SetGL();
            Params.DepthColorMask.SetGL();

            GL.BlendColor(Params.BlendColor.ToColor4());

            GL.CullFace(Params.FaceCulling.ToCullFaceMode());

            GL.PolygonOffset(0, Params.PolygonOffsetUnit);

            RenderUtils.SetState(EnableCap.Blend,       Params.ColorOperation.BlendMode == PICABlendMode.Blend);
            RenderUtils.SetState(EnableCap.StencilTest, Params.StencilTest.Enabled);
            RenderUtils.SetState(EnableCap.DepthTest,   Params.DepthColorMask.Enabled);
            RenderUtils.SetState(EnableCap.CullFace,    Params.FaceCulling != PICAFaceCulling.Never);

            Parent.Renderer.BindLUT(4, Params.LUTDist0TableName,   Params.LUTDist0SamplerName);
            Parent.Renderer.BindLUT(5, Params.LUTDist1TableName,   Params.LUTDist1SamplerName);
            Parent.Renderer.BindLUT(6, Params.LUTFresnelTableName, Params.LUTFresnelSamplerName);
            Parent.Renderer.BindLUT(7, Params.LUTReflecRTableName, Params.LUTReflecRSamplerName);

            Parent.Renderer.BindLUT(8,
                Params.LUTReflecGTableName   ?? Params.LUTReflecRTableName, 
                Params.LUTReflecGSamplerName ?? Params.LUTReflecRSamplerName);

            Parent.Renderer.BindLUT(9,
                Params.LUTReflecBTableName   ?? Params.LUTReflecRTableName, 
                Params.LUTReflecBSamplerName ?? Params.LUTReflecRSamplerName);

            //Setup texture transforms
            Matrix3[] MaterialTransform = Parent.MaterialTransform[BaseMesh.MaterialIndex];

            for (int Index = 0; Index < MaterialTransform.Length; Index++)
            {
                int TransformLocation = GL.GetUniformLocation(ShaderHandle, $"UVTransforms[{Index}]");

                GL.UniformMatrix3(TransformLocation, false, ref MaterialTransform[Index]);
            }

            //Setup texture units
            if (Material.Texture0Name != null)
            {
                //Only the texture unit 0 can have a Cube Map texture
                if (Params.TextureCoords[0].MappingType == H3DTextureMappingType.CameraCubeEnvMap)
                {
                    Parent.Renderer.BindTexture(3, Material.Texture0Name);

                    SetWrapAndFilter(TextureTarget.TextureCubeMap, 0);
                }
                else
                {
                    Parent.Renderer.BindTexture(0, Material.Texture0Name);

                    SetWrapAndFilter(TextureTarget.Texture2D, 0);
                }
            }

            if (Material.Texture1Name != null)
            {
                Parent.Renderer.BindTexture(1, Material.Texture1Name);

                SetWrapAndFilter(TextureTarget.Texture2D, 1);
            }

            if (Material.Texture2Name != null)
            {
                Parent.Renderer.BindTexture(2, Material.Texture2Name);

                SetWrapAndFilter(TextureTarget.Texture2D, 2);
            }

            //Setup Fixed attributes
            int FixedAttributes = 0;

            if (BaseMesh.FixedAttributes != null)
            {
                int FixedNormalLocation  = GL.GetUniformLocation(ShaderHandle, "FixedNorm");
                int FixedTangentLocation = GL.GetUniformLocation(ShaderHandle, "FixedTan");
                int FixedColorLocation   = GL.GetUniformLocation(ShaderHandle, "FixedCol");
                int FixedBoneLocation    = GL.GetUniformLocation(ShaderHandle, "FixedBone");
                int FixedWeightLocation  = GL.GetUniformLocation(ShaderHandle, "FixedWeight");

                foreach (PICAFixedAttribute Attrib in BaseMesh.FixedAttributes)
                {
                    FixedAttributes |= 1 << (int)Attrib.Name;

                    Vector4 Value = Attrib.Value.ToVector4();

                    switch (Attrib.Name)
                    {
                        case PICAAttributeName.Normal:     GL.Uniform4(FixedNormalLocation,  Value); break;
                        case PICAAttributeName.Tangent:    GL.Uniform4(FixedTangentLocation, Value); break;
                        case PICAAttributeName.Color:      GL.Uniform4(FixedColorLocation,   Value); break;
                        case PICAAttributeName.BoneIndex:  GL.Uniform4(FixedBoneLocation,    Value); break;
                        case PICAAttributeName.BoneWeight: GL.Uniform4(FixedWeightLocation,  Value); break;
                    }
                }
            }

            int FixedAttrLocation = GL.GetUniformLocation(ShaderHandle, "FixedAttr");
            int PosOffsetLocation = GL.GetUniformLocation(ShaderHandle, "PosOffset");
            int Scales0Location   = GL.GetUniformLocation(ShaderHandle, "Scales0");
            int Scales1Location   = GL.GetUniformLocation(ShaderHandle, "Scales1");

            GL.Uniform1(FixedAttrLocation, FixedAttributes);
            GL.Uniform4(PosOffsetLocation, BaseMesh.PositionOffset.ToVector4());
            GL.Uniform4(Scales0Location,   Scales0);
            GL.Uniform4(Scales1Location,   Scales1);

            //Render all SubMeshes
            GL.BindVertexArray(VAOHandle);

            foreach (H3DSubMesh SM in BaseMesh.SubMeshes)
            {
                bool SmoothSkin = SM.Skinning == H3DSubMeshSkinning.Smooth;

                Matrix4[] Transforms = new Matrix4[32];

                for (int Index = 0; Index < Transforms.Length; Index++)
                {
                    Matrix4 Transform;

                    if (Index < SM.BoneIndicesCount && SM.BoneIndices[Index] < Parent.SkeletonTransform.Length)
                    {
                        int BoneIndex = SM.BoneIndices[Index];

                        Transform = Parent.SkeletonTransform[BoneIndex];

                        if (SmoothSkin)
                        {
                            Transform = Parent.InverseTransform[BoneIndex] * Transform;
                        }

                        //Build billboard matrix if needed (used to make the bones follow the camera view)
                        H3DBone Bone = Parent.BaseModel.Skeleton[BoneIndex];

                        if (Bone.BillboardMode != H3DBillboardMode.Off)
                        {
                            Matrix4 BillMtx = Matrix4.Identity;

                            Matrix4 WrldMtx = Parent.Transform * Parent.Renderer.ViewMatrix;

                            Matrix4 BoneMtx =
                                Matrix4.CreateRotationX(Bone.Rotation.X) *
                                Matrix4.CreateRotationY(Bone.Rotation.Y) *
                                Matrix4.CreateRotationZ(Bone.Rotation.Z);

                            Vector3 X, Y, Z;

                            X = Y = Z = Vector3.Zero;

                            switch (Bone.BillboardMode)
                            {
                                case H3DBillboardMode.World:
                                    Y = Vector3.Normalize(WrldMtx.Row1.Xyz); Z = Vector3.UnitZ;
                                    break;
                                case H3DBillboardMode.WorldViewpoint:
                                    Y = Vector3.Normalize(WrldMtx.Row1.Xyz); Z = Vector3.Normalize(-WrldMtx.Row3.Xyz);
                                    break;
                                case H3DBillboardMode.Screen:
                                    Y = Vector3.Normalize(BoneMtx.Row1.Xyz); Z = Vector3.UnitZ;
                                    break;
                                case H3DBillboardMode.ScreenViewpoint:
                                    Y = Vector3.Normalize(BoneMtx.Row1.Xyz); Z = Vector3.Normalize(-WrldMtx.Row3.Xyz);
                                    break;
                                case H3DBillboardMode.YAxial:
                                    Y = Vector3.Normalize(WrldMtx.Row1.Xyz); Z = Vector3.UnitZ;
                                    break;
                                case H3DBillboardMode.YAxialViewpoint:
                                    Y = Vector3.Normalize(WrldMtx.Row1.Xyz); Z = Vector3.Normalize(-WrldMtx.Row3.Xyz);
                                    break;
                            }

                            X = Vector3.Normalize(Vector3.Cross(Y, Z));
                            Y = Vector3.Normalize(Vector3.Cross(Z, X));

                            BillMtx.Row0 = new Vector4(X.X, Y.X, Z.X, 0);
                            BillMtx.Row1 = new Vector4(X.Y, Y.Y, Z.Y, 0);
                            BillMtx.Row2 = new Vector4(X.Z, Y.Z, Z.Z, 0);
                            BillMtx.Row3 = (Transform * WrldMtx).Row3;

                            WrldMtx = WrldMtx.ClearScale();

                            WrldMtx.Invert();

                            Transform = Matrix4.CreateScale(Transform.ExtractScale()) * BillMtx * WrldMtx;
                        }
                    }
                    else
                    {
                        Transform = Matrix4.Identity;
                    }

                    int Location = GL.GetUniformLocation(ShaderHandle, $"Transforms[{Index}]");

                    GL.UniformMatrix4(Location, false, ref Transform);
                }

                int BoolUniformsLocation = GL.GetUniformLocation(ShaderHandle, "BoolUniforms");

                GL.Uniform1(BoolUniformsLocation, SM.BoolUniforms);

                GL.DrawElements(PrimitiveType.Triangles, SM.Indices.Length, DrawElementsType.UnsignedShort, SM.Indices);
            }

            GL.BindVertexArray(0);

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindTexture(TextureTarget.TextureCubeMap, 0);
        }

        private void SetWrapAndFilter(TextureTarget Target, int Unit)
        {
            int WrapS = (int)GetWrap(Material.TextureMappers[Unit].WrapU);
            int WrapT = (int)GetWrap(Material.TextureMappers[Unit].WrapV);

            int MinFilter = (int)GetMinFilter(Material.TextureMappers[Unit].MinFilter);
            int MagFilter = (int)GetMagFilter(Material.TextureMappers[Unit].MagFilter);

            GL.TexParameter(Target, TextureParameterName.TextureWrapS, WrapS);
            GL.TexParameter(Target, TextureParameterName.TextureWrapT, WrapT);

            GL.TexParameter(Target, TextureParameterName.TextureMinFilter, MinFilter);
            GL.TexParameter(Target, TextureParameterName.TextureMagFilter, MagFilter);
        }

        private static All GetWrap(H3DTextureWrap Wrap)
        {
            switch (Wrap)
            {
                case H3DTextureWrap.ClampToEdge:   return All.ClampToEdge;
                case H3DTextureWrap.ClampToBorder: return All.ClampToBorder;
                case H3DTextureWrap.Repeat:        return All.Repeat;
                case H3DTextureWrap.Mirror:        return All.MirroredRepeat;

                default: throw new ArgumentException("Invalid wrap mode!");
            }
        }

        //TODO: Change this to use the Mipmaps once Mipmaps are implemented on the loaders
        private static All GetMinFilter(H3DTextureMinFilter Filter)
        {
            switch (Filter)
            {
                case H3DTextureMinFilter.Nearest:              return All.Nearest;
                case H3DTextureMinFilter.NearestMipmapNearest: return All.Nearest;
                case H3DTextureMinFilter.NearestMipmapLinear:  return All.Nearest;
                case H3DTextureMinFilter.Linear:               return All.Linear;
                case H3DTextureMinFilter.LinearMipmapNearest:  return All.Linear;
                case H3DTextureMinFilter.LinearMipmapLinear:   return All.Linear;

                default: throw new ArgumentException("Invalid minification filter!");
            }
        }

        private static All GetMagFilter(H3DTextureMagFilter Filter)
        {
            switch (Filter)
            {
                case H3DTextureMagFilter.Linear:  return All.Linear;
                case H3DTextureMagFilter.Nearest: return All.Nearest;

                default: throw new ArgumentException("Invalid magnification filter!");
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
