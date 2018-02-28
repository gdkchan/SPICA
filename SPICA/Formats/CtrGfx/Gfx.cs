using SPICA.Formats.CtrGfx.Animation;
using SPICA.Formats.CtrGfx.Camera;
using SPICA.Formats.CtrGfx.Emitter;
using SPICA.Formats.CtrGfx.Fog;
using SPICA.Formats.CtrGfx.Light;
using SPICA.Formats.CtrGfx.LUT;
using SPICA.Formats.CtrGfx.Model;
using SPICA.Formats.CtrGfx.Model.Material;
using SPICA.Formats.CtrGfx.Model.Mesh;
using SPICA.Formats.CtrGfx.Scene;
using SPICA.Formats.CtrGfx.Shader;
using SPICA.Formats.CtrGfx.Texture;
using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Animation;
using SPICA.Formats.CtrH3D.LUT;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.Formats.CtrH3D.Model.Mesh;
using SPICA.Formats.CtrH3D.Texture;
using SPICA.PICA.Commands;
using SPICA.PICA.Converters;
using SPICA.Serialization;
using SPICA.Serialization.Serializer;

using System;
using System.IO;
using System.Numerics;

namespace SPICA.Formats.CtrGfx
{
    enum GfxSectionId
    {
        Contents,
        Strings,
        Image
    }

    public class Gfx
    {
        public readonly GfxDict<GfxModel>     Models;
        public readonly GfxDict<GfxTexture>   Textures;
        public readonly GfxDict<GfxLUT>       LUTs;
        public readonly GfxDict<GfxMaterial>  Materials;
        public readonly GfxDict<GfxShader>    Shaders;
        public readonly GfxDict<GfxCamera>    Cameras;
        public readonly GfxDict<GfxLight>     Lights;
        public readonly GfxDict<GfxFog>       Fogs;
        public readonly GfxDict<GfxScene>     Scenes;
        public readonly GfxDict<GfxAnimation> SkeletalAnimations;
        public readonly GfxDict<GfxAnimation> MaterialAnimations;
        public readonly GfxDict<GfxAnimation> VisibilityAnimations;
        public readonly GfxDict<GfxAnimation> CameraAnimations;
        public readonly GfxDict<GfxAnimation> LightAnimations;
        public readonly GfxDict<GfxAnimation> FogAnimations;
        public readonly GfxDict<GfxEmitter>   Emitters;

        public Gfx()
        {
            Models               = new GfxDict<GfxModel>();
            Textures             = new GfxDict<GfxTexture>();
            LUTs                 = new GfxDict<GfxLUT>();
            Materials            = new GfxDict<GfxMaterial>();
            Shaders              = new GfxDict<GfxShader>();
            Cameras              = new GfxDict<GfxCamera>();
            Lights               = new GfxDict<GfxLight>();
            Fogs                 = new GfxDict<GfxFog>();
            Scenes               = new GfxDict<GfxScene>();
            SkeletalAnimations   = new GfxDict<GfxAnimation>();
            MaterialAnimations   = new GfxDict<GfxAnimation>();
            VisibilityAnimations = new GfxDict<GfxAnimation>();
            CameraAnimations     = new GfxDict<GfxAnimation>();
            LightAnimations      = new GfxDict<GfxAnimation>();
            FogAnimations        = new GfxDict<GfxAnimation>();
            Emitters             = new GfxDict<GfxEmitter>();
        }

        public static Gfx Open(string FileName)
        {
            using (FileStream Input = new FileStream(FileName, FileMode.Open))
            {
                BinaryDeserializer Deserializer = new BinaryDeserializer(Input, GetSerializationOptions());

                GfxHeader Header = Deserializer.Deserialize<GfxHeader>();
                Gfx       Scene  = Deserializer.Deserialize<Gfx>();

                return Scene;
            }
        }

        public static H3D Open(Stream Input)
        {
            BinaryDeserializer Deserializer = new BinaryDeserializer(Input, GetSerializationOptions());

            GfxHeader Header = Deserializer.Deserialize<GfxHeader>();
            Gfx       Scene  = Deserializer.Deserialize<Gfx>();

            return Scene.ToH3D();
        }

        public H3D ToH3D()
        {
            H3D Output = new H3D();

            foreach (GfxModel Model in Models)
            {
                H3DModel Mdl = new H3DModel();

                Mdl.Name = Model.Name;

                Mdl.WorldTransform = Model.WorldTransform;

                foreach (GfxMaterial Material in Model.Materials)
                {
                    H3DMaterial Mat = new H3DMaterial() { Name = Material.Name };

                    Mat.MaterialParams.ModelReference  = $"{Mat.Name}@{Model.Name}";
                    Mat.MaterialParams.ShaderReference = "0@DefaultShader";

                    Mat.MaterialParams.Flags = (H3DMaterialFlags)Material.Flags;

                    Mat.MaterialParams.TranslucencyKind = (H3DTranslucencyKind)Material.TranslucencyKind;
                    Mat.MaterialParams.TexCoordConfig   = (H3DTexCoordConfig)Material.TexCoordConfig;

                    Mat.MaterialParams.EmissionColor  = Material.Colors.Emission;
                    Mat.MaterialParams.AmbientColor   = Material.Colors.Ambient;
                    Mat.MaterialParams.DiffuseColor   = Material.Colors.Diffuse;
                    Mat.MaterialParams.Specular0Color = Material.Colors.Specular0;
                    Mat.MaterialParams.Specular1Color = Material.Colors.Specular1;
                    Mat.MaterialParams.Constant0Color = Material.Colors.Constant0;
                    Mat.MaterialParams.Constant1Color = Material.Colors.Constant1;
                    Mat.MaterialParams.Constant2Color = Material.Colors.Constant2;
                    Mat.MaterialParams.Constant3Color = Material.Colors.Constant3;
                    Mat.MaterialParams.Constant4Color = Material.Colors.Constant4;
                    Mat.MaterialParams.Constant5Color = Material.Colors.Constant5;
                    Mat.MaterialParams.ColorScale     = Material.Colors.Scale;

                    if (Material.Rasterization.IsPolygonOffsetEnabled)
                    {
                        Mat.MaterialParams.Flags |= H3DMaterialFlags.IsPolygonOffsetEnabled;
                    }

                    Mat.MaterialParams.FaceCulling       = Material.Rasterization.FaceCulling.ToPICAFaceCulling();
                    Mat.MaterialParams.PolygonOffsetUnit = Material.Rasterization.PolygonOffsetUnit;

                    Mat.MaterialParams.DepthColorMask = Material.FragmentOperation.Depth.ColorMask;

                    Mat.MaterialParams.DepthColorMask.RedWrite   = true;
                    Mat.MaterialParams.DepthColorMask.GreenWrite = true;
                    Mat.MaterialParams.DepthColorMask.BlueWrite  = true;
                    Mat.MaterialParams.DepthColorMask.AlphaWrite = true;
                    Mat.MaterialParams.DepthColorMask.DepthWrite = true;

                    Mat.MaterialParams.ColorBufferRead  = false;
                    Mat.MaterialParams.ColorBufferWrite = true;

                    Mat.MaterialParams.StencilBufferRead  = false;
                    Mat.MaterialParams.StencilBufferWrite = false;

                    Mat.MaterialParams.DepthBufferRead  = true;
                    Mat.MaterialParams.DepthBufferWrite = true;

                    Mat.MaterialParams.ColorOperation   = Material.FragmentOperation.Blend.ColorOperation;
                    Mat.MaterialParams.LogicalOperation = Material.FragmentOperation.Blend.LogicalOperation;
                    Mat.MaterialParams.BlendFunction    = Material.FragmentOperation.Blend.Function;
                    Mat.MaterialParams.BlendColor       = Material.FragmentOperation.Blend.Color;

                    Mat.MaterialParams.StencilOperation = Material.FragmentOperation.Stencil.Operation;
                    Mat.MaterialParams.StencilTest      = Material.FragmentOperation.Stencil.Test;

                    int TCIndex = 0;

                    foreach (GfxTextureCoord TexCoord in Material.TextureCoords)
                    {
                        H3DTextureCoord TC = new H3DTextureCoord();

                        TC.MappingType = (H3DTextureMappingType)TexCoord.MappingType;

                        TC.ReferenceCameraIndex = (sbyte)TexCoord.ReferenceCameraIndex;

                        TC.TransformType = (H3DTextureTransformType)TexCoord.TransformType;

                        TC.Scale       = TexCoord.Scale;
                        TC.Rotation    = TexCoord.Rotation;
                        TC.Translation = TexCoord.Translation;

                        switch (TexCoord.MappingType)
                        {
                            case GfxTextureMappingType.UvCoordinateMap:
                                Mat.MaterialParams.TextureSources[TCIndex] = TexCoord.SourceCoordIndex;
                                break;

                            case GfxTextureMappingType.CameraCubeEnvMap:
                                Mat.MaterialParams.TextureSources[TCIndex] = 3;
                                break;

                            case GfxTextureMappingType.CameraSphereEnvMap:
                                Mat.MaterialParams.TextureSources[TCIndex] = 4;
                                break;
                        }

                        Mat.MaterialParams.TextureCoords[TCIndex++] = TC;

                        if (TCIndex == Material.UsedTextureCoordsCount) break;
                    }

                    int TMIndex = 0;

                    foreach (GfxTextureMapper TexMapper in Material.TextureMappers)
                    {
                        if (TexMapper == null) break;

                        H3DTextureMapper TM = new H3DTextureMapper();

                        TM.WrapU = TexMapper.WrapU;
                        TM.WrapV = TexMapper.WrapV;

                        TM.MagFilter = (H3DTextureMagFilter)TexMapper.MinFilter;

                        switch ((uint)TexMapper.MagFilter | ((uint)TexMapper.MipFilter << 1))
                        {
                            case 0: TM.MinFilter = H3DTextureMinFilter.NearestMipmapNearest; break;
                            case 1: TM.MinFilter = H3DTextureMinFilter.LinearMipmapNearest;  break;
                            case 2: TM.MinFilter = H3DTextureMinFilter.NearestMipmapLinear;  break;
                            case 3: TM.MinFilter = H3DTextureMinFilter.LinearMipmapLinear;   break;
                        }

                        TM.LODBias = TexMapper.LODBias;
                        TM.MinLOD  = TexMapper.MinLOD;

                        TM.BorderColor = TexMapper.BorderColor;

                        Mat.TextureMappers[TMIndex++] = TM;
                    }

                    Mat.EnabledTextures[0] = Material.TextureMappers[0] != null;
                    Mat.EnabledTextures[1] = Material.TextureMappers[1] != null;
                    Mat.EnabledTextures[2] = Material.TextureMappers[2] != null;

                    Mat.Texture0Name = Material.TextureMappers[0]?.Texture.Path;
                    Mat.Texture1Name = Material.TextureMappers[1]?.Texture.Path;
                    Mat.Texture2Name = Material.TextureMappers[2]?.Texture.Path;

                    GfxFragmentFlags SrcFlags = Material.FragmentShader.Lighting.Flags;
                    H3DFragmentFlags DstFlags = 0;

                    if ((SrcFlags & GfxFragmentFlags.IsClampHighLightEnabled) != 0)
                        DstFlags |= H3DFragmentFlags.IsClampHighLightEnabled;

                    if ((SrcFlags & GfxFragmentFlags.IsLUTDist0Enabled) != 0)
                        DstFlags |= H3DFragmentFlags.IsLUTDist0Enabled;

                    if ((SrcFlags & GfxFragmentFlags.IsLUTDist1Enabled) != 0)
                        DstFlags |= H3DFragmentFlags.IsLUTDist1Enabled;

                    if ((SrcFlags & GfxFragmentFlags.IsLUTGeoFactor0Enabled) != 0)
                        DstFlags |= H3DFragmentFlags.IsLUTGeoFactor0Enabled;

                    if ((SrcFlags & GfxFragmentFlags.IsLUTGeoFactor1Enabled) != 0)
                        DstFlags |= H3DFragmentFlags.IsLUTGeoFactor1Enabled;

                    if ((SrcFlags & GfxFragmentFlags.IsLUTReflectionEnabled) != 0)
                        DstFlags |= H3DFragmentFlags.IsLUTReflectionEnabled;

                    if (Material.FragmentShader.Lighting.IsBumpRenormalize)
                        DstFlags |= H3DFragmentFlags.IsBumpRenormalizeEnabled;

                    Mat.MaterialParams.FragmentFlags = DstFlags;

                    Mat.MaterialParams.FresnelSelector = (H3DFresnelSelector)Material.FragmentShader.Lighting.FresnelSelector;

                    Mat.MaterialParams.BumpTexture = (byte)Material.FragmentShader.Lighting.BumpTexture;

                    Mat.MaterialParams.BumpMode = (H3DBumpMode)Material.FragmentShader.Lighting.BumpMode;

                    Mat.MaterialParams.LUTInputSelection.ReflecR = Material.FragmentShader.LUTs.ReflecR?.Input ?? 0;
                    Mat.MaterialParams.LUTInputSelection.ReflecG = Material.FragmentShader.LUTs.ReflecG?.Input ?? 0;
                    Mat.MaterialParams.LUTInputSelection.ReflecB = Material.FragmentShader.LUTs.ReflecB?.Input ?? 0;
                    Mat.MaterialParams.LUTInputSelection.Dist0   = Material.FragmentShader.LUTs.Dist0?.Input   ?? 0;
                    Mat.MaterialParams.LUTInputSelection.Dist1   = Material.FragmentShader.LUTs.Dist1?.Input   ?? 0;
                    Mat.MaterialParams.LUTInputSelection.Fresnel = Material.FragmentShader.LUTs.Fresnel?.Input ?? 0;

                    Mat.MaterialParams.LUTInputScale.ReflecR = Material.FragmentShader.LUTs.ReflecR?.Scale ?? 0;
                    Mat.MaterialParams.LUTInputScale.ReflecG = Material.FragmentShader.LUTs.ReflecG?.Scale ?? 0;
                    Mat.MaterialParams.LUTInputScale.ReflecB = Material.FragmentShader.LUTs.ReflecB?.Scale ?? 0;
                    Mat.MaterialParams.LUTInputScale.Dist0   = Material.FragmentShader.LUTs.Dist0?.Scale   ?? 0;
                    Mat.MaterialParams.LUTInputScale.Dist1   = Material.FragmentShader.LUTs.Dist1?.Scale   ?? 0;
                    Mat.MaterialParams.LUTInputScale.Fresnel = Material.FragmentShader.LUTs.Fresnel?.Scale ?? 0;

                    Mat.MaterialParams.LUTReflecRTableName = Material.FragmentShader.LUTs.ReflecR?.Sampler.TableName;
                    Mat.MaterialParams.LUTReflecGTableName = Material.FragmentShader.LUTs.ReflecG?.Sampler.TableName;
                    Mat.MaterialParams.LUTReflecBTableName = Material.FragmentShader.LUTs.ReflecB?.Sampler.TableName;
                    Mat.MaterialParams.LUTDist0TableName   = Material.FragmentShader.LUTs.Dist0?.Sampler.TableName;
                    Mat.MaterialParams.LUTDist1TableName   = Material.FragmentShader.LUTs.Dist1?.Sampler.TableName;
                    Mat.MaterialParams.LUTFresnelTableName = Material.FragmentShader.LUTs.Fresnel?.Sampler.TableName;

                    Mat.MaterialParams.LUTReflecRSamplerName = Material.FragmentShader.LUTs.ReflecR?.Sampler.SamplerName;
                    Mat.MaterialParams.LUTReflecGSamplerName = Material.FragmentShader.LUTs.ReflecG?.Sampler.SamplerName;
                    Mat.MaterialParams.LUTReflecBSamplerName = Material.FragmentShader.LUTs.ReflecB?.Sampler.SamplerName;
                    Mat.MaterialParams.LUTDist0SamplerName   = Material.FragmentShader.LUTs.Dist0?.Sampler.SamplerName;
                    Mat.MaterialParams.LUTDist1SamplerName   = Material.FragmentShader.LUTs.Dist1?.Sampler.SamplerName;
                    Mat.MaterialParams.LUTFresnelSamplerName = Material.FragmentShader.LUTs.Fresnel?.Sampler.SamplerName;

                    Mat.MaterialParams.TexEnvStages[0] = Material.FragmentShader.TextureEnvironments[0].Stage;
                    Mat.MaterialParams.TexEnvStages[1] = Material.FragmentShader.TextureEnvironments[1].Stage;
                    Mat.MaterialParams.TexEnvStages[2] = Material.FragmentShader.TextureEnvironments[2].Stage;
                    Mat.MaterialParams.TexEnvStages[3] = Material.FragmentShader.TextureEnvironments[3].Stage;
                    Mat.MaterialParams.TexEnvStages[4] = Material.FragmentShader.TextureEnvironments[4].Stage;
                    Mat.MaterialParams.TexEnvStages[5] = Material.FragmentShader.TextureEnvironments[5].Stage;

                    Mat.MaterialParams.AlphaTest = Material.FragmentShader.AlphaTest.Test;

                    Mat.MaterialParams.TexEnvBufferColor = Material.FragmentShader.TexEnvBufferColor;

                    Mdl.Materials.Add(Mat);
                }

                foreach (GfxMesh Mesh in Model.Meshes)
                {
                    GfxShape Shape = Model.Shapes[Mesh.ShapeIndex];

                    H3DMesh M = new H3DMesh();

                    PICAVertex[] Vertices = null;

                    foreach (GfxVertexBuffer VertexBuffer in Shape.VertexBuffers)
                    {
                        /*
                         * CGfx supports 3 types of vertex buffer:
                         * - Non-Interleaved: Each attribute is stored on it's on stream, like this:
                         * P0 P1 P2 P3 P4 P5 ... N0 N1 N2 N3 N4 N5
                         * - Interleaved: All attributes are stored on the same stream, like this:
                         * P0 N0 P1 N1 P2 N2 P3 N3 P4 N4 P5 N5 ...
                         * - Fixed: The attribute have only a single fixed value, so instead of a stream,
                         * it have a single vector.
                         */
                        if (VertexBuffer is GfxAttribute)
                        {
                            //Non-Interleaved buffer
                            GfxAttribute Attr = (GfxAttribute)VertexBuffer;

                            M.Attributes.Add(Attr.ToPICAAttribute());

                            int Length = Attr.Elements;

                            switch (Attr.Format)
                            {
                                case GfxGLDataType.GL_SHORT: Length <<= 1; break;
                                case GfxGLDataType.GL_FLOAT: Length <<= 2; break;
                            }

                            M.VertexStride += Length;

                            Vector4[] Vectors = Attr.GetVectors();

                            if (Vertices == null)
                            {
                                Vertices = new PICAVertex[Vectors.Length];
                            }

                            for (int i = 0; i < Vectors.Length; i++)
                            {
                                switch(Attr.AttrName)
                                {
                                    case PICAAttributeName.Position:  Vertices[i].Position  = Vectors[i]; break;
                                    case PICAAttributeName.Normal:    Vertices[i].Normal    = Vectors[i]; break;
                                    case PICAAttributeName.Tangent:   Vertices[i].Tangent   = Vectors[i]; break;
                                    case PICAAttributeName.TexCoord0: Vertices[i].TexCoord0 = Vectors[i]; break;
                                    case PICAAttributeName.TexCoord1: Vertices[i].TexCoord1 = Vectors[i]; break;
                                    case PICAAttributeName.TexCoord2: Vertices[i].TexCoord2 = Vectors[i]; break;
                                    case PICAAttributeName.Color:     Vertices[i].Color     = Vectors[i]; break;

                                    case PICAAttributeName.BoneIndex:
                                        Vertices[i].Indices[0] = (int)Vectors[i].X;
                                        Vertices[i].Indices[1] = (int)Vectors[i].Y;
                                        Vertices[i].Indices[2] = (int)Vectors[i].Z;
                                        Vertices[i].Indices[3] = (int)Vectors[i].W;
                                        break;

                                    case PICAAttributeName.BoneWeight:
                                        Vertices[i].Weights[0] =      Vectors[i].X;
                                        Vertices[i].Weights[1] =      Vectors[i].Y;
                                        Vertices[i].Weights[2] =      Vectors[i].Z;
                                        Vertices[i].Weights[3] =      Vectors[i].W;
                                        break;
                                }
                            }
                        }
                        else if (VertexBuffer is GfxVertexBufferFixed)
                        {
                            //Fixed vector
                            float[] Vector = ((GfxVertexBufferFixed)VertexBuffer).Vector;

                            M.FixedAttributes.Add(new PICAFixedAttribute()
                            {
                                Name  = VertexBuffer.AttrName,

                                Value = new PICAVectorFloat24(
                                    Vector.Length > 0 ? Vector[0] : 0,
                                    Vector.Length > 1 ? Vector[1] : 0,
                                    Vector.Length > 2 ? Vector[2] : 0,
                                    Vector.Length > 3 ? Vector[3] : 0)
                            });
                        }
                        else
                        {
                            //Interleaved buffer
                            GfxVertexBufferInterleaved VtxBuff = (GfxVertexBufferInterleaved)VertexBuffer;

                            foreach (GfxAttribute Attr in ((GfxVertexBufferInterleaved)VertexBuffer).Attributes)
                            {
                                M.Attributes.Add(Attr.ToPICAAttribute());
                            }

                            M.RawBuffer    = VtxBuff.RawBuffer;
                            M.VertexStride = VtxBuff.VertexStride;
                        }
                    }

                    if (Vertices != null)
                    {
                        M.RawBuffer = VerticesConverter.GetBuffer(Vertices, M.Attributes);
                    }

                    Vector4 PositionOffset = new Vector4(Shape.PositionOffset, 0);

                    int Layer = (int)Model.Materials[Mesh.MaterialIndex].TranslucencyKind;

                    M.MaterialIndex  = (ushort)Mesh.MaterialIndex;
                    M.NodeIndex      = (ushort)Mesh.MeshNodeIndex;
                    M.PositionOffset = PositionOffset;
                    M.MeshCenter     = Shape.BoundingBox.Center;
                    M.Layer          = Layer;
                    M.Priority       = Mesh.RenderPriority;

                    H3DBoundingBox OBB = new H3DBoundingBox()
                    {
                        Center      = Shape.BoundingBox.Center,
                        Orientation = Shape.BoundingBox.Orientation,
                        Size        = Shape.BoundingBox.Size
                    };

                    M.MetaData = new H3DMetaData();

                    M.MetaData.Add(new H3DMetaDataValue(OBB));

                    int SmoothCount = 0;

                    foreach (GfxSubMesh SubMesh in Shape.SubMeshes)
                    {
                        foreach (GfxFace Face in SubMesh.Faces)
                        {
                            foreach (GfxFaceDescriptor Desc in Face.FaceDescriptors)
                            {
                                H3DSubMesh SM = new H3DSubMesh();

                                SM.BoneIndicesCount = (ushort)SubMesh.BoneIndices.Count;

                                for (int i = 0; i < SubMesh.BoneIndices.Count; i++)
                                {
                                    SM.BoneIndices[i] = (ushort)SubMesh.BoneIndices[i];
                                }

                                switch (SubMesh.Skinning)
                                {
                                    case GfxSubMeshSkinning.None:   SM.Skinning = H3DSubMeshSkinning.None;   break;
                                    case GfxSubMeshSkinning.Rigid:  SM.Skinning = H3DSubMeshSkinning.Rigid;  break;
                                    case GfxSubMeshSkinning.Smooth: SM.Skinning = H3DSubMeshSkinning.Smooth; break;
                                }

                                SM.Indices = Desc.Indices;

                                SM.Indices = new ushort[Desc.Indices.Length];

                                Array.Copy(Desc.Indices, SM.Indices, SM.Indices.Length);

                                M.SubMeshes.Add(SM);
                            }
                        }

                        if (SubMesh.Skinning == GfxSubMeshSkinning.Smooth)
                        {
                            SmoothCount++;
                        }
                    }

                    if (SmoothCount == Shape.SubMeshes.Count)
                        M.Skinning = H3DMeshSkinning.Smooth;
                    else if (SmoothCount > 0)
                        M.Skinning = H3DMeshSkinning.Mixed;
                    else
                        M.Skinning = H3DMeshSkinning.Rigid;

                    GfxMaterial Mat = Model.Materials[Mesh.MaterialIndex];

                    M.UpdateBoolUniforms(Mdl.Materials[Mesh.MaterialIndex]);

                    Mdl.AddMesh(M);
                }

                //Workaround to fix blending problems until I can find a proper way.
                Mdl.MeshesLayer1.Reverse();

                Mdl.MeshNodesTree = new H3DPatriciaTree();

                foreach (GfxMeshNodeVisibility MeshNode in Model.MeshNodeVisibilities)
                {
                    Mdl.MeshNodesTree.Add(MeshNode.Name);
                    Mdl.MeshNodesVisibility.Add(MeshNode.IsVisible);
                }

                if (Model is GfxModelSkeletal)
                {
                    foreach (GfxBone Bone in ((GfxModelSkeletal)Model).Skeleton.Bones)
                    {
                        H3DBone B = new H3DBone()
                        {
                            Name             = Bone.Name,
                            ParentIndex      = (short)Bone.ParentIndex,
                            Translation      = Bone.Translation,
                            Rotation         = Bone.Rotation,
                            Scale            = Bone.Scale,
                            InverseTransform = Bone.InvWorldTransform
                        };

                        bool ScaleCompensate = (Bone.Flags & GfxBoneFlags.IsSegmentScaleCompensate) != 0;

                        if (ScaleCompensate) B.Flags |= H3DBoneFlags.IsSegmentScaleCompensate;

                        Mdl.Skeleton.Add(B);
                    }

                    Mdl.Flags |= H3DModelFlags.HasSkeleton;

                    Mdl.BoneScaling = (H3DBoneScaling)((GfxModelSkeletal)Model).Skeleton.ScalingRule;
                }

                Output.Models.Add(Mdl);
            }

            foreach (GfxTexture Texture in Textures)
            {
                H3DTexture Tex = new H3DTexture()
                {
                    Name       = Texture.Name,
                    Width      = Texture.Width,
                    Height     = Texture.Height,
                    Format     = Texture.HwFormat,
                    MipmapSize = (byte)Texture.MipmapSize
                };

                if (Texture is GfxTextureCube)
                {
                    Tex.RawBufferXPos = ((GfxTextureCube)Texture).ImageXPos.RawBuffer;
                    Tex.RawBufferXNeg = ((GfxTextureCube)Texture).ImageXNeg.RawBuffer;
                    Tex.RawBufferYPos = ((GfxTextureCube)Texture).ImageYPos.RawBuffer;
                    Tex.RawBufferYNeg = ((GfxTextureCube)Texture).ImageYNeg.RawBuffer;
                    Tex.RawBufferZPos = ((GfxTextureCube)Texture).ImageZPos.RawBuffer;
                    Tex.RawBufferZNeg = ((GfxTextureCube)Texture).ImageZNeg.RawBuffer;
                }
                else
                {
                    Tex.RawBuffer = ((GfxTextureImage)Texture).Image.RawBuffer;
                }

                Output.Textures.Add(Tex);
            }

            foreach (GfxLUT LUT in LUTs)
            {
                H3DLUT L = new H3DLUT() { Name = LUT.Name };

                foreach (GfxLUTSampler Sampler in LUT.Samplers)
                {
                    L.Samplers.Add(new H3DLUTSampler()
                    {
                        Flags = Sampler.IsAbsolute ? H3DLUTFlags.IsAbsolute : 0,
                        Name  = Sampler.Name,
                        Table = Sampler.Table
                    });
                }

                Output.LUTs.Add(L);
            }

            foreach (GfxCamera Camera in Cameras)
            {
                Output.Cameras.Add(Camera.ToH3DCamera());
            }

            foreach (GfxLight Light in Lights)
            {
                Output.Lights.Add(Light.ToH3DLight());
            }

            foreach (GfxAnimation SklAnim in SkeletalAnimations)
            {
                Output.SkeletalAnimations.Add(SklAnim.ToH3DAnimation());
            }

            foreach (GfxAnimation MatAnim in MaterialAnimations)
            {
                Output.MaterialAnimations.Add(new H3DMaterialAnim(MatAnim.ToH3DAnimation()));
            }

            foreach (GfxAnimation VisAnim in VisibilityAnimations)
            {
                Output.VisibilityAnimations.Add(VisAnim.ToH3DAnimation());
            }

            foreach (GfxAnimation CamAnim in CameraAnimations)
            {
                Output.CameraAnimations.Add(CamAnim.ToH3DAnimation());
            }

            Output.CopyMaterials();

            return Output;
        }

        public static void Save(string FileName, Gfx Scene)
        {
            using (FileStream FS = new FileStream(FileName, FileMode.Create))
            {
                GfxHeader Header = new GfxHeader();

                BinarySerializer Serializer = new BinarySerializer(FS, GetSerializationOptions());

                Section Contents = Serializer.Sections[(uint)H3DSectionId.Contents];

                Contents.Header = Header;

                Section Strings = new Section();
                Section Image   = new Section();

                Image.Header = new GfxSectionHeader("IMAG");

                Serializer.AddSection((uint)GfxSectionId.Strings, Strings, typeof(string));
                Serializer.AddSection((uint)GfxSectionId.Strings, Strings, typeof(GfxStringUtf8));
                Serializer.AddSection((uint)GfxSectionId.Strings, Strings, typeof(GfxStringUtf16LE));
                Serializer.AddSection((uint)GfxSectionId.Strings, Strings, typeof(GfxStringUtf16BE));
                Serializer.AddSection((uint)GfxSectionId.Image,   Image);

                Serializer.Serialize(Scene);

                Header.FileLength = (int)FS.Length;

                Header.SectionsCount = Image.Values.Count > 0 ? 2 : 1;

                Header.Data.Length = Contents.Length + Strings.Length + 8;

                FS.Seek(0, SeekOrigin.Begin);

                Serializer.WriteValue(Header);

                FS.Seek(Image.Position - 4, SeekOrigin.Begin);

                Serializer.Writer.Write(Image.LengthWithHeader);
            }
        }

        private static SerializationOptions GetSerializationOptions()
        {
            return new SerializationOptions(LengthPos.BeforePtr, PointerType.SelfRelative);
        }
    }
}
