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
using System.Numerics;

namespace SPICA.Formats.MTFramework.Model
{
    public class MTModel
    {
        public readonly List<MTMaterial> Materials;
        public readonly List<MTMesh>     Meshes;
        public readonly List<MTBone>     Skeleton;

        public Vector4 BoundingSphere;
        public Vector4 BoundingBoxMin;
        public Vector4 BoundingBoxMax;

        public byte[][] BoneIndicesGroups;

        public MTModel()
        {
            Materials = new List<MTMaterial>();
            Meshes    = new List<MTMesh>();
            Skeleton  = new List<MTBone>();
        }

        public MTModel(BinaryReader Reader, MTMaterials MRLData, MTShaderEffects Shader) : this()
        {
            string Magic = Reader.ReadPaddedString(4);

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

            BoundingSphere = Reader.ReadVector4();
            BoundingBoxMin = Reader.ReadVector4();
            BoundingBoxMax = Reader.ReadVector4();

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

                Vector3 Position = Reader.ReadVector3();

                Skeleton.Add(new MTBone()
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
                Skeleton[Index].LocalTransform = Reader.ReadMatrix4x4RH();
            }

            for (int Index = 0; Index < BonesCount; Index++)
            {
                Skeleton[Index].WorldTransform = Reader.ReadMatrix4x4RH();
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

            Model.MeshNodesTree = new H3DPatriciaTree();

            Model.Flags = BoneIndicesGroups.Length > 0 ? H3DModelFlags.HasSkeleton : 0;
            Model.Name = "Model";

            foreach (MTMaterial Mat in Materials)
            {
                H3DMaterial Mtl = H3DMaterial.GetSimpleMaterial(
                    Model.Name,
                    Mat.Name,
                    Path.GetFileNameWithoutExtension(Mat.Texture0Name));

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

                H3DMesh M = new H3DMesh(
                    Mesh.RawBuffer,
                    Mesh.VertexStride,
                    Mesh.Attributes,
                    null,
                    null)
                {
                    MaterialIndex = (ushort)Mesh.MaterialIndex,
                    NodeIndex     = Index,
                    Priority      = Mesh.RenderPriority
                };

                byte[] BoneIndices = BoneIndicesGroups[Mesh.BoneIndicesIndex];

                if ((Model.Flags & H3DModelFlags.HasSkeleton) != 0 && BoneIndices.Length > 0)
                {
                    M.Skinning  = H3DMeshSkinning.Smooth;

                    PICAVertex[] Vertices = M.GetVertices();

                    for (int v = 0; v < Vertices.Length; v++)
                    {
                        Vector4 Position = Vector4.Zero;

                        float WeightSum = 0;

                        for (int i = 0; i < 4; i++)
                        {
                            if (Vertices[v].Weights[i] == 0) break;

                            WeightSum += Vertices[v].Weights[i];

                            int bi = BoneIndicesGroups[Mesh.BoneIndicesIndex][Vertices[v].Indices[i]];

                            Vector4 Trans = Vector4.Zero;

                            for (int b = bi; b != -1; b = Skeleton[b].ParentIndex)
                            {
                                Trans += new Vector4(
                                    Skeleton[b].LocalTransform.M41,
                                    Skeleton[b].LocalTransform.M42,
                                    Skeleton[b].LocalTransform.M43, 0);
                            }

                            Matrix4x4 WT = Skeleton[bi].WorldTransform;

                            Vector3 P = new Vector3(
                                Vertices[v].Position.X,
                                Vertices[v].Position.Y,
                                Vertices[v].Position.Z);

                            Vector4 TP = Vector4.Transform(P, WT);

                            Position += (TP + Trans) * Vertices[v].Weights[i];
                        }

                        if (WeightSum < 1) Position += Vertices[v].Position * (1 - WeightSum);

                        Vertices[v].Position = Position;
                    }

                    /*
                     * Removes unused bone from bone indices list, also splits sub meshes on exceeding bones if
                     * current Mesh uses more than 20 (BCH only supports up to 20).
                     */
                    Queue<ushort> IndicesQueue = new Queue<ushort>(Mesh.Indices);

                    while (IndicesQueue.Count > 0)
                    {
                        int Count = IndicesQueue.Count / 3;

                        List<ushort> Indices = new List<ushort>();
                        List<int>    Bones   = new List<int>();

                        while (Count-- > 0)
                        {
                            ushort i0 = IndicesQueue.Dequeue();
                            ushort i1 = IndicesQueue.Dequeue();
                            ushort i2 = IndicesQueue.Dequeue();

                            List<int> TempBones = new List<int>(12);

                            for (int j = 0; j < 4; j++)
                            {
                                int b0 = Vertices[i0].Indices[j];
                                int b1 = Vertices[i1].Indices[j];
                                int b2 = Vertices[i2].Indices[j];

                                if (!(Bones.Contains(b0) || TempBones.Contains(b0))) TempBones.Add(b0);
                                if (!(Bones.Contains(b1) || TempBones.Contains(b1))) TempBones.Add(b1);
                                if (!(Bones.Contains(b2) || TempBones.Contains(b2))) TempBones.Add(b2);
                            }

                            if (Bones.Count + TempBones.Count > 20)
                            {
                                IndicesQueue.Enqueue(i0);
                                IndicesQueue.Enqueue(i1);
                                IndicesQueue.Enqueue(i2);
                            }
                            else
                            {
                                Indices.Add(i0);
                                Indices.Add(i1);
                                Indices.Add(i2);

                                Bones.AddRange(TempBones);
                            }
                        }

                        H3DSubMesh SM = new H3DSubMesh();

                        SM.Skinning = H3DSubMeshSkinning.Smooth;
                        SM.Indices  = Indices.ToArray();
                        SM.BoneIndicesCount = (ushort)Bones.Count;

                        for (int i = 0; i < Bones.Count; i++)
                        {
                            SM.BoneIndices[i] = BoneIndices[Bones[i]];
                        }

                        bool[] Visited = new bool[Vertices.Length];

                        foreach (ushort i in Indices)
                        {
                            if (!Visited[i])
                            {
                                Visited[i] = true;

                                Vertices[i].Indices[0] = Bones.IndexOf(Vertices[i].Indices[0]);
                                Vertices[i].Indices[1] = Bones.IndexOf(Vertices[i].Indices[1]);
                                Vertices[i].Indices[2] = Bones.IndexOf(Vertices[i].Indices[2]);
                                Vertices[i].Indices[3] = Bones.IndexOf(Vertices[i].Indices[3]); 
                            }
                        }

                        M.SubMeshes.Add(SM);
                    }

                    M.RawBuffer = VerticesConverter.GetBuffer(Vertices, M.Attributes);
                }
                else
                {
                    M.SubMeshes.Add(new H3DSubMesh() { Indices = Mesh.Indices });
                }

                Model.AddMesh(M);

                Model.MeshNodesTree.Add($"Mesh_{Index++}");

                Model.MeshNodesVisibility.Add(true);
            }

            int BoneIndex = 0;

            foreach (MTBone Bone in Skeleton)
            {
                Model.Skeleton.Add(new H3DBone()
                {
                    Name        = $"Bone_{BoneIndex++}",
                    ParentIndex = Bone.ParentIndex,
                    Translation = Bone.Position,
                    Scale       = Vector3.One
                });
            }

            foreach (H3DBone Bone in Model.Skeleton)
            {
                Bone.CalculateTransform(Model.Skeleton);
            }

            if (Model.Materials.Count == 0)
            {
                Model.Materials.Add(H3DMaterial.GetSimpleMaterial(Model.Name, "DummyMaterial", null));
            }

            Output.Models.Add(Model);

            Output.CopyMaterials();

            return Output;
        }
    }
}
