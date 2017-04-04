using SPICA.Formats.Common;
using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.Formats.CtrH3D.Model.Mesh;
using SPICA.Formats.MTFramework.Shader;
using SPICA.Math3D;
using SPICA.PICA.Converters;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SPICA.Formats.MTFramework.Model
{
    class MTModel
    {
        public readonly List<MTMaterial>    Materials;
        public readonly List<MTMesh>        Meshes;
        public readonly List<MTBone>        Skeleton;

        public Vector4D BoundingSphere;
        public Vector4D BoundingBoxMin;
        public Vector4D BoundingBoxMax;

        public byte[][] BoneIndicesGroups;

        public MTModel()
        {
            Materials = new List<MTMaterial>();
            Meshes    = new List<MTMesh>();
            Skeleton  = new List<MTBone>();
        }

        public MTModel(BinaryReader Reader, MTMaterials MRLData, MTShaderEffects Shader) : this()
        {
            string Magic = StringUtils.ReadPaddedString(Reader, 4);

            ushort Version               = Reader.ReadUInt16();
            ushort BonesCount            = Reader.ReadUInt16();
            ushort MeshesCount           = Reader.ReadUInt16();
            ushort MaterialsCount        = Reader.ReadUInt16();
            uint   TotalVerticesCount    = Reader.ReadUInt32();
            uint   TotalIndicesCount     = Reader.ReadUInt32();
            uint   TotalTrianglesCount   = Reader.ReadUInt32();
            uint   VerticesBufferLength  = Reader.ReadUInt32();
            uint   HeaderPadding1c       = Reader.ReadUInt32(); //?
            uint   MeshGroupsCount       = Reader.ReadUInt32();
            uint   BoneIndicesCount      = Reader.ReadUInt32();
            uint   SkeletonAddress       = Reader.ReadUInt32();
            uint   MeshGroupsAddr        = Reader.ReadUInt32();
            uint   MaterialNamesAddress  = Reader.ReadUInt32();
            uint   MeshesAddress         = Reader.ReadUInt32();
            uint   VerticesBufferAddress = Reader.ReadUInt32();
            uint   IndicesBufferAddress  = Reader.ReadUInt32();
            uint   ModelFileLength       = Reader.ReadUInt32();

            BoundingSphere = new Vector4D(Reader);
            BoundingBoxMin = new Vector4D(Reader);
            BoundingBoxMax = new Vector4D(Reader);

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
                    MTMaterial Mat = MRLData.Materials.FirstOrDefault(x => x.NameHash == MaterialHash);

                    if (Mat != null)
                    {
                        Mat.Name = MaterialName;

                        MaterialIndex = Materials.Count;

                        Materials.Add(Mat);
                    }
                    else
                    {
                        MaterialIndex = 0;
                    }
                }

                Mesh.MaterialIndex = (uint)MaterialIndex;

                Meshes.Add(Mesh);
            }

            for (int Index = 0; Index < BonesCount; Index++)
            {
                Reader.BaseStream.Seek(SkeletonAddress + Index * 0x18, SeekOrigin.Begin);

                sbyte BoneIndex = Reader.ReadSByte();
                sbyte Parent    = Reader.ReadSByte();
                sbyte Opposite  = Reader.ReadSByte();
                byte  Padding   = Reader.ReadByte();

                float ChildDistance  = Reader.ReadSingle();
                float ParentDistance = Reader.ReadSingle();

                Vector3D Position = new Vector3D(Reader);

                Skeleton.Add(new MTBone
                {
                    ParentIndex    = Parent,
                    OppositeIndex  = Opposite,
                    ChildDistance  = ChildDistance,
                    ParentDistance = ParentDistance,
                    Position       = Position
                });
            }

            for (int Index = 0; Index < BonesCount; Index++)
            {
                Skeleton[Index].LocalTransform = new Matrix4x4(Reader);
            }

            for (int Index = 0; Index < BonesCount; Index++)
            {
                Skeleton[Index].WorldTransform = new Matrix4x4(Reader);
            }

            Reader.BaseStream.Seek(0x100, SeekOrigin.Current);

            BoneIndicesGroups = new byte[BoneIndicesCount][];

            for (int i = 0; i < BoneIndicesCount; i++)
            {
                int Count = Reader.ReadInt32();

                BoneIndicesGroups[i] = new byte[Count];

                for (int j = 0; j < Count; j++)
                {
                    BoneIndicesGroups[i][j] = Reader.ReadByte();
                }

                Reader.BaseStream.Seek(0x18 - Count, SeekOrigin.Current);
            }
        }

        public H3D ToH3D()
        {
            H3D Output = new H3D();

            H3DModel Model = new H3DModel();

            Model.MeshNodesTree = new PatriciaTree();
            Model.Flags = BoneIndicesGroups.Length > 0 ? H3DModelFlags.HasSkeleton : 0;
            Model.Name = "Model";

            foreach (MTMaterial Mat in Materials)
            {
                H3DMaterial Mtl = H3DMaterial.Default;

                Mtl.Name = Mat.Name;

                Mtl.Texture0Name = Path.GetFileNameWithoutExtension(Mat.Texture0Name);

                Mtl.MaterialParams.ColorOperation.BlendMode  = Mat.AlphaBlend.BlendMode;
                Mtl.MaterialParams.BlendFunction             = Mat.AlphaBlend.BlendFunction;
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

                H3DSubMesh SM = new H3DSubMesh();

                if ((Model.Flags & H3DModelFlags.HasSkeleton) != 0)
                {
                    SM.Skinning = H3DSubMeshSkinning.Smooth;
                    M.Skinning  = H3DMeshSkinning.Smooth;

                    SM.BoneIndicesCount = (ushort)BoneIndicesGroups[Mesh.BoneIndicesIndex].Length;

                    SM.BoneIndices = new ushort[SM.BoneIndicesCount];

                    for (int i = 0; i < SM.BoneIndicesCount; i++)
                    {
                        SM.BoneIndices[i] = BoneIndicesGroups[Mesh.BoneIndicesIndex][i];
                    }

                    PICAVertex[] Vertices = M.ToVertices(true);

                    foreach (PICAVertex Vtx in Vertices)
                    {
                        Vector3D Position = new Vector3D(0);

                        float WeightSum = 0;

                        for (int i = 0; i < 4; i++)
                        {
                            if (Vtx.Weights[i] == 0) break;

                            WeightSum += Vtx.Weights[i];

                            int BI = SM.BoneIndices[Vtx.Indices[i]];

                            Vector3D Trans = new Vector3D(0);

                            for (int b = BI; b != -1; b = Skeleton[b].ParentIndex)
                            {
                                Trans += new Vector3D(
                                    Skeleton[b].LocalTransform.M41,
                                    Skeleton[b].LocalTransform.M42,
                                    Skeleton[b].LocalTransform.M43);
                            }

                            Matrix4x4 WT = Skeleton[BI].WorldTransform;

                            Vector3D P = Vtx.Position;

                            Vector3D TP = new Vector3D(0);

                            TP.X = P.X * WT.M11 + P.Y * WT.M21 + P.Z * WT.M31 + WT.M41;
                            TP.Y = P.X * WT.M12 + P.Y * WT.M22 + P.Z * WT.M32 + WT.M42;
                            TP.Z = P.X * WT.M13 + P.Y * WT.M23 + P.Z * WT.M33 + WT.M43;

                            Position += (TP + Trans) * Vtx.Weights[i];
                        }

                        if (WeightSum < 1) Position += Vtx.Position * (1 - WeightSum);

                        Vtx.Position = Position;
                    }

                    M.RawBuffer = VerticesConverter.GetBuffer(Vertices, M.Attributes);
                }

                SM.Indices = Mesh.Indices;

                M.SubMeshes.Add(SM);

                Model.AddMesh(M, 0, Mesh.RenderPriority);

                Model.MeshNodesTree.Add($"Mesh_{Index++}");

                Model.MeshNodesVisibility.Add(true);
            }

            Model.Skeleton = new PatriciaList<H3DBone>();

            int BoneIndex = 0;

            foreach (MTBone Bone in Skeleton)
            {
                Model.Skeleton.Add(new H3DBone
                {
                    Name             = $"Bone_{BoneIndex++}",
                    ParentIndex      = Bone.ParentIndex,
                    Translation      = Bone.Position,
                    Scale            = new Vector3D(1)
                });
            }

            foreach (var b in Model.Skeleton)
            {
                b.CalculateTransform(Model.Skeleton);
            }

            if (Model.Materials.Count == 0)
            {
                Model.Materials.Add(H3DMaterial.Default);
            }

            Output.Models.Add(Model);

            Output.CopyMaterials();

            return Output;
        }
    }
}
