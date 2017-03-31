using SPICA.Formats.Common;
using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.Formats.CtrH3D.Model.Mesh;
using SPICA.Formats.MTFramework.Model;
using SPICA.Formats.MTFramework.Shader;

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SPICA.Formats.MTFramework.Model
{
    class MTModel
    {
        private List<MTMaterial> Materials;
        private List<MTMesh>     Meshes;

        public MTModel()
        {
            Materials = new List<MTMaterial>();
            Meshes    = new List<MTMesh>();
        }

        public MTModel(BinaryReader Reader, MTMaterials MRLData, MTShaderEffects Shader) : this()
        {
            string Magic = StringUtils.ReadPaddedString(Reader, 4);

            ushort Version        = Reader.ReadUInt16();
            ushort BonesCount     = Reader.ReadUInt16();
            ushort MeshesCount    = Reader.ReadUInt16();
            ushort MaterialsCount = Reader.ReadUInt16();

            uint TotalVerticesCount = Reader.ReadUInt32();
            uint TotalIndicesCount = Reader.ReadUInt32();
            uint TotalTrianglesCount = Reader.ReadUInt32();
            uint VerticesBufferLength = Reader.ReadUInt32();
            uint unk2 = Reader.ReadUInt32();
            uint unk3 = Reader.ReadUInt32();
            uint unk4 = Reader.ReadUInt32();

            uint SkeletonTransAddress = Reader.ReadUInt32();
            uint SkeletonBonesAddress = Reader.ReadUInt32();
            uint MaterialNamesAddress = Reader.ReadUInt32();
            uint MeshesAddress = Reader.ReadUInt32();
            uint VerticesBufferAddress = Reader.ReadUInt32();
            uint IndicesBufferAddress = Reader.ReadUInt32();

            string[] MaterialNames = new string[MaterialsCount];

            Reader.BaseStream.Seek(MaterialNamesAddress, SeekOrigin.Begin);

            for (int Index = 0; Index < MaterialsCount; Index++)
            {
                MaterialNames[Index] = Reader.ReadPaddedString(0x80);
            }

            for (int Index = 0; Index < MeshesCount; Index++)
            {
                Reader.BaseStream.Seek(MeshesAddress + Index * 0x30, SeekOrigin.Begin);

                MTMesh Mesh = new MTMesh(
                    Reader,
                    Shader,
                    VerticesBufferAddress,
                    IndicesBufferAddress);

                string MaterialName = MaterialNames[Mesh.MaterialIndex];

                uint MaterialHash = CRC32Hash.Hash(MaterialName);

                int MaterialIndex = Materials.FindIndex(x => x.NameHash == MaterialHash);

                if (MaterialIndex == -1)
                {
                    MTMaterial Mat = MRLData.Materials.First(x => x.NameHash == MaterialHash);

                    Mat.Name = MaterialName;

                    MaterialIndex = Materials.Count;

                    Materials.Add(Mat);
                }

                Mesh.MaterialIndex = (uint)MaterialIndex;

                Meshes.Add(Mesh);
            }
        }

        public H3D ToH3D()
        {
            H3D Output = new H3D();

            H3DModel Model = new H3DModel();

            Model.MeshNodesTree = new PatriciaTree();
            Model.Name = "Model";

            foreach (MTMaterial Mat in Materials)
            {
                H3DMaterial Mtl = H3DMaterial.Default;

                Mtl.Name = Mat.Name;

                Mtl.Texture0Name = Path.GetFileNameWithoutExtension(Mat.Texture0Name);

                Mtl.MaterialParams.ColorOperation.BlendMode = Mat.AlphaBlend.BlendMode;
                Mtl.MaterialParams.BlendFunction = Mat.AlphaBlend.BlendFunction;

                Mtl.MaterialParams.DepthColorMask.RedWrite   = Mat.AlphaBlend.RedWrite;
                Mtl.MaterialParams.DepthColorMask.GreenWrite = Mat.AlphaBlend.GreenWrite;
                Mtl.MaterialParams.DepthColorMask.BlueWrite  = Mat.AlphaBlend.BlueWrite;
                Mtl.MaterialParams.DepthColorMask.AlphaWrite = Mat.AlphaBlend.AlphaWrite;
                Mtl.MaterialParams.DepthColorMask.Enabled    = Mat.DepthStencil.DepthTest;
                Mtl.MaterialParams.DepthColorMask.DepthWrite = Mat.DepthStencil.DepthWrite;
                Mtl.MaterialParams.DepthColorMask.DepthFunc  = Mat.DepthStencil.DepthFunc;

                Model.Materials.Add(Mtl);
            }

            ushort Index = 0;

            foreach (MTMesh Mesh in Meshes)
            {
                if (Mesh.RenderType != -1) continue;

                H3DMesh M = new H3DMesh
                {
                    MaterialIndex = (ushort)Mesh.MaterialIndex,
                    NodeIndex     = Index,
                    RawBuffer     = Mesh.RawBuffer,
                    VertexStride  = Mesh.VertexStride,
                    Attributes    = Mesh.Attributes
                };

                M.SubMeshes.Add(new H3DSubMesh
                {
                    Indices = Mesh.Indices
                });

                Model.AddMesh(M, Mesh.RenderLayer);

                Model.MeshNodesTree.Add($"Mesh_{Index++}");
                Model.MeshNodesVisibility.Add(true);
            }

            Model.Materials.Add(H3DMaterial.Default);

            Output.Models.Add(Model);

            Output.CopyMaterials();

            return Output;
        }
    }
}
