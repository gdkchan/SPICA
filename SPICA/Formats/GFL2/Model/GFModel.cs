using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.Formats.CtrH3D.Model.Material.Texture;
using SPICA.Formats.CtrH3D.Model.Mesh;
using SPICA.Formats.GFL2.Model.Material;
using SPICA.Formats.GFL2.Model.Mesh;
using SPICA.Formats.GFL2.Utils;
using SPICA.Math3D;
using SPICA.PICA.Commands;

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SPICA.Formats.GFL2.Model
{
    public class GFModel
    {
        internal const string DefaultLUTName = "LookupTableSetContentCtrName";

        public string Name;

        public Vector4D BBoxMinVector;
        public Vector4D BBoxMaxVector;

        public Matrix4x4 Transform;

        public List<GFBone>     Skeleton;
        public List<GFLUT>      LUTs;
        public List<GFMaterial> Materials;
        public List<GFMesh>     Meshes;

        public GFModel()
        {
            Skeleton  = new List<GFBone>();
            LUTs      = new List<GFLUT>();
            Materials = new List<GFMaterial>();
            Meshes    = new List<GFMesh>();
        }

        public GFModel(BinaryReader Reader, string ModelName)
        {
            Name = ModelName;

            uint MagicNumber = Reader.ReadUInt32();
            uint ShaderCount = Reader.ReadUInt32();

            GFSection.SkipPadding(Reader);
            GFSection ModelSection = new GFSection(Reader);

            GFHashName[] ShaderNames   = ReadStringTable(Reader);
            GFHashName[] LUTNames      = ReadStringTable(Reader);
            GFHashName[] MaterialNames = ReadStringTable(Reader);
            GFHashName[] MeshNames     = ReadStringTable(Reader);

            BBoxMinVector = new Vector4D(Reader);
            BBoxMaxVector = new Vector4D(Reader);

            Transform = new Matrix4x4(Reader);

            //TODO: Investigate what is this (maybe Tile permissions?)
            uint UnknownDataLength = Reader.ReadUInt32();
            uint UnknownDataOffset = Reader.ReadUInt32();
            ulong Padding = Reader.ReadUInt64();

            Reader.BaseStream.Seek(UnknownDataOffset + UnknownDataLength, SeekOrigin.Current);

            int BonesCount = Reader.ReadInt32();

            Reader.BaseStream.Seek(0xc, SeekOrigin.Current);

            Skeleton = GFBone.ReadList(Reader, BonesCount);

            GFSection.SkipPadding(Reader);

            int LUTsCount = Reader.ReadInt32();
            int LUTLength = Reader.ReadInt32();

            GFSection.SkipPadding(Reader);

            LUTs      = GFLUT.ReadList(Reader, LUTLength, LUTsCount);
            Materials = GFMaterial.ReadList(Reader, MaterialNames);
            Meshes    = GFMesh.ReadList(Reader, MeshNames.Length);
        }

        private GFHashName[] ReadStringTable(BinaryReader Reader)
        {
            uint Count = Reader.ReadUInt32();

            GFHashName[] Values = new GFHashName[Count];

            for (int Index = 0; Index < Count; Index++)
            {
                Values[Index].Hash = Reader.ReadUInt32();
                Values[Index].Name = GFString.ReadLength(Reader, 0x40);
            }

            return Values;
        }

        public H3DModel ToH3DModel()
        {
            H3DModel Output = new H3DModel();

            Output.Name = Name;

            //Skeleton
            foreach (GFBone Bone in Skeleton)
            {
                Output.Skeleton.Add(new H3DBone
                {
                    ParentIndex = (short)Skeleton.FindIndex(x => x.Name == Bone.ParentName),

                    Name        = Bone.Name,
                    Scale       = Bone.Scale,
                    Rotation    = Bone.Rotation,
                    Translation = Bone.Translation
                });
            }

            foreach (H3DBone Bone in Output.Skeleton)
            {
                Bone.InverseTransform = Bone.CalculateTransform(Output.Skeleton);
                Bone.InverseTransform.Invert();
            }

            //Materials
            foreach (GFMaterial Material in Materials)
            {
                H3DMaterial Mat = new H3DMaterial();

                H3DMaterialParams Params = Mat.MaterialParams;

                Mat.Name = Material.Name;

                Params.FragmentFlags = H3DFragmentFlags.IsLUTReflectionEnabled;

                Params.RimPower = Material.RimPower;
                Params.RimScale = Material.RimScale;
                Params.PhongPower = Material.PhongPower;
                Params.PhongScale = Material.PhongScale;

                for (int Unit = 0; Unit < Material.TextureCoords.Length; Unit++)
                {
                    string TextureName = Material.TextureCoords[Unit].Name;

                    Mat.EnabledTextures[Unit] = TextureName != null;

                    switch (Unit)
                    {
                        case 0: Mat.Texture0Name = TextureName; break;
                        case 1: Mat.Texture1Name = TextureName; break;
                        case 2: Mat.Texture2Name = TextureName; break;
                    }

                    //Texture Coords
                    GFTextureMappingType MappingType = Material.TextureCoords[Unit].MappingType;

                    switch (MappingType)
                    {
                        case GFTextureMappingType.CameraCubeEnvMap: Params.TextureSources[Unit] = 3; break;
                        case GFTextureMappingType.CameraSphereEnvMap: Params.TextureSources[Unit] = 4; break;
                    }

                    Params.TextureCoords[Unit].MappingType = (H3DTextureMappingType)MappingType;

                    Params.TextureCoords[Unit].Scale       = Material.TextureCoords[Unit].Scale;
                    Params.TextureCoords[Unit].Rotation    = Material.TextureCoords[Unit].Rotation;
                    Params.TextureCoords[Unit].Translation = Material.TextureCoords[Unit].Translation;

                    //Texture Mapper
                    Mat.TextureMappers[Unit].WrapU = (H3DTextureWrap)Material.TextureCoords[Unit].WrapU;
                    Mat.TextureMappers[Unit].WrapV = (H3DTextureWrap)Material.TextureCoords[Unit].WrapV;

                    Mat.TextureMappers[Unit].MagFilter = (H3DTextureMagFilter)Material.TextureCoords[Unit].MagFilter;
                    Mat.TextureMappers[Unit].MinFilter = (H3DTextureMinFilter)Material.TextureCoords[Unit].MinFilter;

                    Mat.TextureMappers[Unit].MinLOD = (byte)Material.TextureCoords[Unit].MinLOD;
                }

                Params.EmissionColor  = Material.EmissionColor;
                Params.AmbientColor   = Material.AmbientColor;
                Params.DiffuseColor   = Material.DiffuseColor;
                Params.Specular0Color = Material.Specular0Color;
                Params.Specular1Color = Material.Specular1Color;
                Params.Constant0Color = Material.Constant0Color;
                Params.Constant1Color = Material.Constant1Color;
                Params.Constant2Color = Material.Constant2Color;
                Params.Constant3Color = Material.Constant3Color;
                Params.Constant4Color = Material.Constant4Color;
                Params.Constant5Color = Material.Constant5Color;
                Params.BlendColor     = Material.BlendColor;

                Params.LUTInputAbs      = Material.LUTInputAbs;
                Params.LUTInputSel      = Material.LUTInputSel;
                Params.LUTInputScaleSel = Material.LUTInputScaleSel;

                Params.ColorOperation   = Material.ColorOperation;
                Params.BlendFunction    = Material.BlendFunction;
                Params.LogicalOperation = Material.LogicalOperation;
                Params.AlphaTest        = Material.AlphaTest;
                Params.StencilTest      = Material.StencilTest;
                Params.StencilOperation = Material.StencilOperation;
                Params.DepthColorMask   = Material.DepthColorMask;
                Params.FaceCulling      = Material.FaceCulling;

                Params.ColorBufferRead  = Material.ColorBufferRead;
                Params.ColorBufferWrite = Material.ColorBufferWrite;

                Params.StencilBufferRead  = Material.StencilBufferRead;
                Params.StencilBufferWrite = Material.StencilBufferWrite;

                Params.DepthBufferRead  = Material.DepthBufferRead;
                Params.DepthBufferWrite = Material.DepthBufferWrite;

                if (Material.LUT0HashId != 0)
                {
                    Params.LUTReflecRTableName = DefaultLUTName;
                    Params.LUTReflecRSamplerName = LUTs.First(x => x.Hash == Material.LUT0HashId).Name;
                }

                if (Material.LUT1HashId != 0)
                {
                    Params.LUTReflecGTableName = DefaultLUTName;
                    Params.LUTReflecGSamplerName = LUTs.First(x => x.Hash == Material.LUT1HashId).Name;
                }

                if (Material.LUT2HashId != 0)
                {
                    Params.LUTReflecBTableName = DefaultLUTName;
                    Params.LUTReflecBSamplerName = LUTs.First(x => x.Hash == Material.LUT2HashId).Name;
                }

                Params.UniqueId = (uint)Params.GetHashCode();

                if (Material.BumpTexture != -1)
                {
                    Params.BumpTexture = (byte)Material.BumpTexture;
                    Params.BumpMode = H3DBumpMode.AsBump;
                }

                Output.Materials.Add(Mat);
            }

            //Meshes
            Output.MeshNodesTree = new PatriciaTree();

            foreach (GFMesh Mesh in Meshes)
            {
                //Note: GFModel have one Vertex Buffer for each SubMesh,
                //while on H3D all SubMeshes shares the same Vertex Buffer
                //For this reason we need to store SubMeshes as Meshes on H3D
                foreach (GFSubMesh SubMesh in Mesh.SubMeshes)
                {
                    int NodeIndex = Output.MeshNodesTree.Find(Mesh.Name);

                    if (NodeIndex == -1)
                    {
                        Output.MeshNodesTree.Add(Mesh.Name);
                        Output.MeshNodesVisibility.Add(true);

                        NodeIndex = Output.MeshNodesCount++;
                    }

                    H3DMesh M = new H3DMesh();

                    M.Skinning = H3DMeshSkinning.Smooth;

                    M.MaterialIndex = (ushort)Materials.FindIndex(x => x.SubMeshName.Name == SubMesh.Name);
                    M.NodeIndex     = (ushort)NodeIndex;

                    M.RawBuffer       = SubMesh.RawBuffer;
                    M.Attributes      = SubMesh.Attributes;
                    M.FixedAttributes = SubMesh.FixedAttributes;
                    M.VertexStride    = SubMesh.VertexStride;

                    ushort[] BoneIndices = new ushort[SubMesh.BoneIndicesCount];

                    for (int Index = 0; Index < BoneIndices.Length; Index++)
                    {
                        BoneIndices[Index] = SubMesh.BoneIndices[Index];
                    }

                    ushort BoolUniforms = 0x6e02; //Smooth Skin + UV012 + Tex12 

                    if (!M.FixedAttributes.Any(x => x.Name == PICAAttributeName.BoneWeight))
                    {
                        //This enables the W component of the bone weight
                        //For some reason fixed attributes have it set to 1 (which is unused)
                        BoolUniforms |= 0x100;
                    }

                    M.SubMeshes.Add(new H3DSubMesh
                    {
                        Skinning         = H3DSubMeshSkinning.Smooth,
                        BoneIndicesCount = SubMesh.BoneIndicesCount,
                        BoneIndices      = BoneIndices,
                        Indices          = SubMesh.Indices,
                        BoolUniforms     = BoolUniforms
                    });

                    Output.AddMesh(M);
                }
            }

            return Output;
        }

        public void Write(BinaryWriter Writer)
        {
            //TODO
        }
    }
}
