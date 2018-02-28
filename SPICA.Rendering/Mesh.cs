using OpenTK;
using OpenTK.Graphics.OpenGL;

using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.Formats.CtrH3D.Model.Mesh;
using SPICA.PICA.Commands;
using SPICA.PICA.Converters;
using SPICA.Rendering.Shaders;
using SPICA.Rendering.SPICA_GL;

using System;
using System.IO;

namespace SPICA.Rendering
{
    public class Mesh : IDisposable
    {
        private int VBOHandle;
        private int VAOHandle;
        
        private  Model       Parent;
        internal H3DMesh     BaseMesh;
        internal H3DMaterial Material;
        private  Vector4     Scales0;
        private  Vector4     Scales1;
        private  Vector4     PosOffs;

        internal string Texture0Name;
        internal string Texture1Name;
        internal string Texture2Name;

        public Mesh(Model Parent, H3DMesh BaseMesh)
        {
            this.Parent   = Parent;
            this.BaseMesh = BaseMesh;

            Material = Parent.BaseModel.Materials[BaseMesh.MaterialIndex];

            PosOffs = BaseMesh.PositionOffset.ToVector4();

            int VtxCount = 1, FAOffset = 0;

            if (BaseMesh.VertexStride > 0)
            {
                VtxCount = BaseMesh.RawBuffer.Length / BaseMesh.VertexStride;
                FAOffset = BaseMesh.RawBuffer.Length;
            }

            byte[] Buffer;

            using (MemoryStream VertexStream = new MemoryStream())
            {
                BinaryWriter Writer = new BinaryWriter(VertexStream);

                Writer.Write(BaseMesh.RawBuffer);

                VertexStream.Seek(0, SeekOrigin.End);

                foreach (PICAFixedAttribute Attrib in BaseMesh.FixedAttributes)
                {
                    /*
                     * OpenGL doesn't support constant attributes, so we need to write
                     * them as a tight array (waste of space).
                     */
                    for (int i = 0; i < VtxCount; i++)
                    {
                        Writer.Write(Attrib.Value.X);
                        Writer.Write(Attrib.Value.Y);
                        Writer.Write(Attrib.Value.Z);
                        Writer.Write(Attrib.Value.W);
                    }
                }

                Buffer = VertexStream.ToArray();
            }

            IntPtr Length = new IntPtr(Buffer.Length);

            VBOHandle = GL.GenBuffer();
            VAOHandle = GL.GenVertexArray();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, Length, Buffer, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindVertexArray(VAOHandle);

            int Offset = 0;
            int Stride = BaseMesh.VertexStride;

            for (int AttribIndex = 0; AttribIndex < 16; AttribIndex++)
            {
                GL.DisableVertexAttribArray(AttribIndex);
            }

            foreach (PICAAttribute Attrib in BaseMesh.Attributes)
            {
                int Size = Attrib.Elements;

                VertexAttribPointerType Type = VertexAttribPointerType.Byte;

                switch (Attrib.Format)
                {
                    case PICAAttributeFormat.Byte:  Type = VertexAttribPointerType.Byte;              break;
                    case PICAAttributeFormat.Ubyte: Type = VertexAttribPointerType.UnsignedByte;      break;
                    case PICAAttributeFormat.Short: Type = VertexAttribPointerType.Short; Size <<= 1; break;
                    case PICAAttributeFormat.Float: Type = VertexAttribPointerType.Float; Size <<= 2; break;
                }

                int AttribIndex = (int)Attrib.Name;

                GL.EnableVertexAttribArray(AttribIndex);

                GL.BindBuffer(BufferTarget.ArrayBuffer, VBOHandle);

                //Short and Float types needs to be aligned into 2 bytes boundaries.
                if (Attrib.Format != PICAAttributeFormat.Byte &&
                    Attrib.Format != PICAAttributeFormat.Ubyte)
                {
                    Offset += Offset & 1;
                }

                GL.VertexAttribPointer(AttribIndex, Attrib.Elements, Type, false, Stride, Offset);

                SetScale(Attrib.Name, Attrib.Scale);

                Offset += Size;
            }

            foreach (PICAFixedAttribute Attrib in BaseMesh.FixedAttributes)
            {
                int AttribIndex = (int)Attrib.Name;

                GL.EnableVertexAttribArray(AttribIndex);

                GL.BindBuffer(BufferTarget.ArrayBuffer, VBOHandle);

                GL.VertexAttribPointer(AttribIndex, 4, VertexAttribPointerType.Float, false, 0, FAOffset);

                /*
                 * Pokémon Sun/Moon seems to have fixed attributes for all unused attributes,
                 * with the vector set to zero (?). On the H3D shader this would normally
                 * make the Tangent scale one, and this signals the shader to use the Tangent
                 * to calculate the normal quaternion, and that would ruin the lighting
                 * 'cause the tangent is actually zero, so we force it to use normals by
                 * setting the Scale to zero here. Using the original Sun/Moon shaders
                 * should avoid this issue entirely.
                 */
                SetScale(Attrib.Name, Attrib.Name != PICAAttributeName.Tangent ? 1 : 0);

                FAOffset += 0x10 * VtxCount;
            }

            GL.BindVertexArray(0);
        }

        private void SetScale(PICAAttributeName Name, float Scale)
        {
            switch (Name)
            {
                case PICAAttributeName.Position:   Scales0.X = Scale; break;
                case PICAAttributeName.Normal:     Scales0.Y = Scale; break;
                case PICAAttributeName.Tangent:    Scales0.Z = Scale; break;
                case PICAAttributeName.Color:      Scales0.W = Scale; break;
                case PICAAttributeName.TexCoord0:  Scales1.X = Scale; break;
                case PICAAttributeName.TexCoord1:  Scales1.Y = Scale; break;
                case PICAAttributeName.TexCoord2:  Scales1.Z = Scale; break;
                case PICAAttributeName.BoneWeight: Scales1.W = Scale; break;
            }
        }

        public void Render()
        {
            Shader Shader = Parent.Shaders[BaseMesh.MaterialIndex];

            H3DMaterialParams Params = Material.MaterialParams;

            Params.BlendFunction.SetGL();
            Params.StencilTest.SetGL();
            Params.StencilOperation.SetGL();
            Params.DepthColorMask.SetGL();

            GL.BlendColor(Params.BlendColor.ToColor4());

            GL.CullFace(Params.FaceCulling.ToCullFaceMode());

            GL.PolygonOffset(0, Params.PolygonOffsetUnit);

            SetState(EnableCap.Blend,       Params.ColorOperation.BlendMode == PICABlendMode.Blend);
            SetState(EnableCap.StencilTest, Params.StencilTest.Enabled);
            SetState(EnableCap.DepthTest,   Params.DepthColorMask.Enabled);
            SetState(EnableCap.CullFace,    Params.FaceCulling != PICAFaceCulling.Never);

            Parent.Renderer.TryBindLUT(4, Params.LUTDist0TableName,   Params.LUTDist0SamplerName);
            Parent.Renderer.TryBindLUT(5, Params.LUTDist1TableName,   Params.LUTDist1SamplerName);
            Parent.Renderer.TryBindLUT(6, Params.LUTFresnelTableName, Params.LUTFresnelSamplerName);
            Parent.Renderer.TryBindLUT(7, Params.LUTReflecRTableName, Params.LUTReflecRSamplerName);

            Parent.Renderer.TryBindLUT(8,
                Params.LUTReflecGTableName   ?? Params.LUTReflecRTableName, 
                Params.LUTReflecGSamplerName ?? Params.LUTReflecRSamplerName);

            Parent.Renderer.TryBindLUT(9,
                Params.LUTReflecBTableName   ?? Params.LUTReflecRTableName, 
                Params.LUTReflecBSamplerName ?? Params.LUTReflecRSamplerName);

            //Setup texture units
            if (Texture0Name != null)
            {
                //Only the texture unit 0 can have a Cube Map texture
                if (Params.TextureCoords[0].MappingType == H3DTextureMappingType.CameraCubeEnvMap)
                {
                    Parent.Renderer.TryBindTexture(3, Texture0Name);

                    SetWrapAndFilter(TextureTarget.TextureCubeMap, 0);
                }
                else
                {
                    Parent.Renderer.TryBindTexture(0, Texture0Name);

                    SetWrapAndFilter(TextureTarget.Texture2D, 0);
                }
            }

            if (Texture1Name != null)
            {
                Parent.Renderer.TryBindTexture(1, Texture1Name);

                SetWrapAndFilter(TextureTarget.Texture2D, 1);
            }

            if (Texture2Name != null)
            {
                Parent.Renderer.TryBindTexture(2, Texture2Name);

                SetWrapAndFilter(TextureTarget.Texture2D, 2);
            }

            Shader.SetVtxVector4(DefaultShaderIds.PosOffs,     PosOffs);
            Shader.SetVtxVector4(DefaultShaderIds.IrScale + 0, Scales0);
            Shader.SetVtxVector4(DefaultShaderIds.IrScale + 1, Scales1);

            //Render all SubMeshes
            GL.BindVertexArray(VAOHandle);

            foreach (H3DSubMesh SM in BaseMesh.SubMeshes)
            {
                bool SmoothSkin = SM.Skinning == H3DSubMeshSkinning.Smooth;

                Matrix4[] Transforms = new Matrix4[20];

                for (int Index = 0; Index < Transforms.Length; Index++)
                {
                    Matrix4 Transform = Matrix4.Identity;

                    if (Index < SM.BoneIndicesCount && SM.BoneIndices[Index] < Parent.SkeletonTransforms.Length)
                    {
                        int BoneIndex = SM.BoneIndices[Index];

                        Transform = Parent.SkeletonTransforms[BoneIndex];

                        if (SmoothSkin)
                        {
                            Transform = Parent.InverseTransforms[BoneIndex] * Transform;
                        }

                        //Build billboard matrix if needed (used to make the bones follow the camera view)
                        H3DBone Bone = Parent.BaseModel.Skeleton[BoneIndex];

                        if (Bone.BillboardMode != H3DBillboardMode.Off)
                        {
                            Matrix4 BillMtx = Matrix4.Identity;

                            Matrix4 WrldMtx = Parent.Transform * Parent.Renderer.Camera.ViewMatrix;

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

                    Shader.SetVtx3x4Array(DefaultShaderIds.UnivReg + Index * 3, Transform);
                }

                int BoolsLocation = GL.GetUniformLocation(Shader.Handle, ShaderGenerator.BoolsName);

                GL.Uniform1(BoolsLocation, SM.BoolUniforms);

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

        private static All GetWrap(PICATextureWrap Wrap)
        {
            switch (Wrap)
            {
                case PICATextureWrap.ClampToEdge:   return All.ClampToEdge;
                case PICATextureWrap.ClampToBorder: return All.ClampToBorder;
                case PICATextureWrap.Repeat:        return All.Repeat;
                case PICATextureWrap.Mirror:        return All.MirroredRepeat;

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

        private static void SetState(EnableCap Cap, bool Value)
        {
            if (Value)
                GL.Enable(Cap);
            else
                GL.Disable(Cap);
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
