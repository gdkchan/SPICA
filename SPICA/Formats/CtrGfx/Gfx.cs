using SPICA.Formats.CtrGfx.LUT;
using SPICA.Formats.CtrGfx.Model;
using SPICA.Formats.CtrGfx.Model.Material;
using SPICA.Formats.CtrGfx.Model.Mesh;
using SPICA.Formats.CtrGfx.Texture;
using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.LUT;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.Formats.CtrH3D.Model.Mesh;
using SPICA.Formats.CtrH3D.Texture;
using SPICA.PICA.Commands;
using SPICA.Serialization;

using System.IO;
using System.Numerics;

namespace SPICA.Formats.CtrGfx
{
    public class Gfx
    {
        internal uint MagicNumber;
        internal uint SectionLength;

        public GfxDict<GfxModel>   Models;
        public GfxDict<GfxTexture> Textures;
        public GfxDict<GfxLUT>     LUTs;

        public static H3D Open(Stream Input)
        {
            BinaryDeserializer Deserializer = new BinaryDeserializer(Input, GetSerializationOptions());

            GfxHeader Header = Deserializer.Deserialize<GfxHeader>();

            Gfx Scene = Deserializer.Deserialize<Gfx>();

            System.Diagnostics.Debug.WriteLine(Scene.Models[0].Name);

            return Scene.ToH3D();
        }

        private static SerializationOptions GetSerializationOptions()
        {
            return new SerializationOptions(LengthPos.BeforePointer, PointerType.SelfRelative);
        }

        public H3D ToH3D()
        {
            H3D Output = new H3D();

            foreach (GfxModel Model in Models)
            {
                H3DModel Mdl = new H3DModel();

                Mdl.Name = Model.Name;

                Mdl.WorldTransform = Model.WorldTransform;

                foreach (GfxMesh Mesh in Model.Meshes)
                {
                    GfxShape Shape = Model.Shapes[Mesh.ShapeIndex];

                    H3DMesh M = new H3DMesh();

                    foreach (GfxAttribute Attr in Shape.VertexBuffers[0].Attributes)
                    {
                        M.Attributes.Add(new PICAAttribute
                        {
                            Name     = Attr.AttrName,
                            Format   = Attr.Format,
                            Elements = Attr.Elements,
                            Scale    = Attr.Scale
                        });
                    }

                    M.MaterialIndex  = (ushort)Mesh.MaterialIndex;
                    M.NodeIndex      = (ushort)Mesh.MeshNodeIndex;
                    M.PositionOffset = new Vector4(Shape.PositionOffset, 0);
                    M.RawBuffer      = Shape.VertexBuffers[0].RawBuffer;
                    M.VertexStride   = Shape.VertexBuffers[0].VertexStride;

                    int SmoothCount = 0;

                    foreach (GfxSubMesh SubMesh in Shape.SubMeshes)
                    {
                        foreach (GfxFace Face in SubMesh.Faces)
                        {
                            H3DSubMesh SM = new H3DSubMesh();

                            for (int i = 0; i < SubMesh.BoneIndices.Count; i++)
                            {
                                SM.BoneIndices[i] = (ushort)SubMesh.BoneIndices[i];
                            }

                            SM.BoneIndicesCount = (ushort)SubMesh.BoneIndices.Count;
                            SM.Skinning         = (H3DSubMeshSkinning)SubMesh.Skinning;
                            SM.Indices          = Face.FaceDescriptors[0].Indices;

                            SM.Indices = new ushort[Face.FaceDescriptors[0].Indices.Length * 9];

                            for (int i = 0; i < Face.FaceDescriptors[0].Indices.Length; i++)
                            {
                                SM.Indices[i] = Face.FaceDescriptors[0].Indices[i];
                            }

                            M.SubMeshes.Add(SM);
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

                    M.UpdateBoolUniforms();

                    Mdl.AddMesh(M);
                }

                foreach (GfxMaterial Material in Model.Materials)
                {
                    H3DMaterial Mat = H3DMaterial.GetSimpleMaterial(Mdl.Name, Material.Name, null);

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

                        if      (TexCoord.MappingType == GfxTextureMappingType.UvCoordinateMap)
                            Mat.MaterialParams.TextureSources[TCIndex] = TexCoord.SourceCoordIndex;
                        else if (TexCoord.MappingType == GfxTextureMappingType.CameraCubeEnvMap)
                            Mat.MaterialParams.TextureSources[TCIndex] = 3;
                        else if (TexCoord.MappingType == GfxTextureMappingType.CameraSphereEnvMap)
                            Mat.MaterialParams.TextureSources[TCIndex] = 4;

                        Mat.MaterialParams.TextureCoords[TCIndex++] = TC;

                        if (TCIndex == Material.UsedTextureCoordsCount) break;
                    }

                    int TMIndex = 0;

                    foreach (GfxTextureMapper TexMapper in Material.TextureMappers)
                    {
                        if (TexMapper == null) break;

                        H3DTextureMapper TM = new H3DTextureMapper();

                        TM.MinLOD = (byte)TexMapper.Sampler.MinLOD;

                        Mat.TextureMappers[TMIndex++] = TM;
                    }

                    Mat.EnabledTextures[0] = Material.TextureMappers[0] != null;
                    Mat.EnabledTextures[1] = Material.TextureMappers[1] != null;
                    Mat.EnabledTextures[2] = Material.TextureMappers[2] != null;

                    Mat.Texture0Name = Material.TextureMappers[0]?.Texture.TextureName;
                    Mat.Texture1Name = Material.TextureMappers[1]?.Texture.TextureName;
                    Mat.Texture2Name = Material.TextureMappers[2]?.Texture.TextureName;

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

                Output.Models.Add(Mdl);
            }

            foreach (GfxTexture Texture in Textures)
            {
                H3DTexture Tex = new H3DTexture()
                {
                    Name          = Texture.Name,
                    Width         = Texture.Width,
                    Height        = Texture.Height,
                    Format        = Texture.HwFormat,
                    MipmapSize    = (byte)Texture.MipmapSize,
                    RawBufferXPos = Texture.ImageXPos?.RawBuffer,
                    RawBufferXNeg = Texture.ImageXNeg?.RawBuffer,
                    RawBufferYPos = Texture.ImageYPos?.RawBuffer,
                    RawBufferYNeg = Texture.ImageYNeg?.RawBuffer,
                    RawBufferZPos = Texture.ImageZPos?.RawBuffer,
                    RawBufferZNeg = Texture.ImageZNeg?.RawBuffer
                };

                Output.Textures.Add(Tex);
            }

            foreach (GfxLUT LUT in LUTs)
            {
                H3DLUT L = new H3DLUT { Name = LUT.Name };

                foreach (GfxLUTSampler Sampler in LUT.Samplers)
                {
                    L.Samplers.Add(new H3DLUTSampler
                    {
                        Flags = Sampler.IsAbsolute ? H3DLUTFlags.IsAbsolute : 0,
                        Name  = Sampler.Name,
                        Table = Sampler.Table
                    });
                }

                Output.LUTs.Add(L);
            }

            Output.CopyMaterials();

            return Output;
        }
    }
}
