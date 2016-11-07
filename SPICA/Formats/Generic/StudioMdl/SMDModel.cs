using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.Formats.CtrH3D.Model.Mesh;
using SPICA.Math3D;
using SPICA.PICA.Commands;
using SPICA.PICA.Converters;

using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

namespace SPICA.Formats.Generic.StudioMdl
{
    class SMDModel
    {
        private ConversionParams ConvParams;

        private List<SMDNode> Nodes;
        private List<SMDBone> Skeleton;
        private List<SMDMesh> Meshes;

        private enum SMDSection
        {
            None,
            Nodes,
            Skeleton,
            Triangles
        }

        public SMDModel(string FileName, ConversionParams ConvParams)
        {
            using (FileStream FS = new FileStream(FileName, FileMode.Open)) SMDModelImpl(FS, ConvParams);
        }

        public SMDModel(Stream Stream, ConversionParams ConvParams)
        {
            SMDModelImpl(Stream, ConvParams);
        }

        private void SMDModelImpl(Stream Stream, ConversionParams ConvParams)
        {
            this.ConvParams = ConvParams;

            TextReader Reader = new StreamReader(Stream);

            Nodes = new List<SMDNode>();
            Skeleton = new List<SMDBone>();
            Meshes = new List<SMDMesh>();

            SMDMesh CurrMesh = new SMDMesh();

            SMDSection CurrSection = SMDSection.None;

            int SkeletalFrame = 0;
            int VerticesLine = 0;

            string Line;
            while ((Line = Reader.ReadLine()) != null)
            {
                string[] Params = Line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                if (Params.Length > 0)
                {
                    switch (Params[0])
                    {
                        case "version": break;
                        case "nodes": CurrSection = SMDSection.Nodes; break;
                        case "skeleton": CurrSection = SMDSection.Skeleton; break;
                        case "time": SkeletalFrame = int.Parse(Params[1]); break;
                        case "triangles": CurrSection = SMDSection.Triangles; break;
                        case "end": CurrSection = SMDSection.None; break;

                        default:
                            switch (CurrSection)
                            {
                                case SMDSection.Nodes:
                                    int NameStart = Line.IndexOf('"') + 1;
                                    int NameLength = Line.LastIndexOf('"') - NameStart;

                                    Nodes.Add(new SMDNode
                                    {
                                        Index = int.Parse(Params[0]),
                                        Name = Line.Substring(NameStart, NameLength),
                                        ParentIndex = int.Parse(Params[2])
                                    });
                                    break;

                                case SMDSection.Skeleton:
                                    Skeleton.Add(new SMDBone
                                    {
                                        NodeIndex = int.Parse(Params[0]),
                                        Translation = new Vector3D
                                        {
                                            X = ParseFloat(Params[1]),
                                            Y = ParseFloat(Params[2]),
                                            Z = ParseFloat(Params[3])
                                        },
                                        Rotation = new Vector3D
                                        {
                                            X = ParseFloat(Params[4]),
                                            Y = ParseFloat(Params[5]),
                                            Z = ParseFloat(Params[6])
                                        }
                                    });
                                    break;

                                case SMDSection.Triangles:
                                    if ((VerticesLine++ & 3) == 0)
                                    {
                                        if (CurrMesh.MaterialName != Line)
                                        {
                                            Meshes.Add(CurrMesh = new SMDMesh { MaterialName = Line });
                                        }
                                    }
                                    else
                                    {
                                        PICAVertex Vertex = new PICAVertex();

                                        Vertex.Position.X = ParseFloat(Params[1]);
                                        Vertex.Position.Y = ParseFloat(Params[2]);
                                        Vertex.Position.Z = ParseFloat(Params[3]);

                                        Vertex.Normal.X = ParseFloat(Params[4]);
                                        Vertex.Normal.Y = ParseFloat(Params[5]);
                                        Vertex.Normal.Z = ParseFloat(Params[6]);

                                        Vertex.TexCoord0.X = ParseFloat(Params[7]);
                                        Vertex.TexCoord0.Y = ParseFloat(Params[8]);

                                        if (Params.Length > 9)
                                        {
                                            int NodesCount = int.Parse(Params[9]);

                                            for (int Node = 0; Node < NodesCount; Node++)
                                            {
                                                Vertex.Indices[Node] = int.Parse(Params[10 + Node * 2]);
                                                Vertex.Weights[Node] = ParseFloat(Params[11 + Node * 2]);
                                            }
                                        }

                                        CurrMesh.Vertices.Add(Vertex);
                                    }
                                    break;
                            }
                            break;
                    }
                }
            }
        }

        private float ParseFloat(string Value)
        {
            return float.Parse(Value, CultureInfo.InvariantCulture);
        }

        public H3D ToH3D()
        {
            H3D Output = new H3D();

            H3DModel Model = new H3DModel();

            Model.Name = "Model";

            if (Skeleton.Count > 0) Model.Flags = H3DModelFlags.HasSkeleton;

            Model.BoneScaling = H3DBoneScaling.Maya;
            Model.MeshNodesVisibility.Add(true);

            ushort MaterialIndex = 0;

            foreach (SMDMesh Mesh in Meshes)
            {
                Vector3D MinVector = new Vector3D();
                Vector3D MaxVector = new Vector3D();

                Dictionary<PICAVertex, int> Vertices = new Dictionary<PICAVertex, int>();

                List<H3DSubMesh> SubMeshes = new List<H3DSubMesh>();

                Queue<PICAVertex> VerticesQueue = new Queue<PICAVertex>();

                foreach (PICAVertex Vertex in Mesh.Vertices)
                {
                    VerticesQueue.Enqueue(Vertex.Clone());
                }

                while (VerticesQueue.Count > 2)
                {
                    List<ushort> Indices = new List<ushort>();
                    List<ushort> BoneIndices = new List<ushort>();

                    int TriCount = VerticesQueue.Count / 3;

                    while (TriCount-- > 0)
                    {
                        PICAVertex[] Triangle = new PICAVertex[3];

                        Triangle[0] = VerticesQueue.Dequeue();
                        Triangle[1] = VerticesQueue.Dequeue();
                        Triangle[2] = VerticesQueue.Dequeue();

                        List<ushort> TempIndices = new List<ushort>();

                        for (int Tri = 0; Tri < 3; Tri++)
                        {
                            PICAVertex Vertex = Triangle[Tri];

                            for (int i = 0; i < Vertex.Indices.Length; i++)
                            {
                                ushort Index = (ushort)Vertex.Indices[i];

                                if (!(BoneIndices.Contains(Index) || TempIndices.Contains(Index)))
                                {
                                    TempIndices.Add(Index);
                                }
                            }
                        }

                        if (BoneIndices.Count + TempIndices.Count > 20)
                        {
                            VerticesQueue.Enqueue(Triangle[0]);
                            VerticesQueue.Enqueue(Triangle[1]);
                            VerticesQueue.Enqueue(Triangle[2]);

                            continue;
                        }

                        for (int Tri = 0; Tri < 3; Tri++)
                        {
                            PICAVertex Vertex = Triangle[Tri];

                            for (int Index = 0; Index < Vertex.Indices.Length; Index++)
                            {
                                int BoneIndex = BoneIndices.IndexOf((ushort)Vertex.Indices[Index]);

                                if (BoneIndex == -1)
                                {
                                    BoneIndex = BoneIndices.Count;
                                    BoneIndices.Add((ushort)Vertex.Indices[Index]);
                                }

                                Vertex.Indices[Index] = BoneIndex;
                            }

                            if (Vertices.ContainsKey(Vertex))
                            {
                                Indices.Add((ushort)Vertices[Vertex]);
                            }
                            else
                            {
                                Indices.Add((ushort)Vertices.Count);

                                if (Vertex.Position.X < MinVector.X) MinVector.X = Vertex.Position.X;
                                if (Vertex.Position.Y < MinVector.Y) MinVector.Y = Vertex.Position.Y;
                                if (Vertex.Position.Z < MinVector.Z) MinVector.Z = Vertex.Position.Z;

                                if (Vertex.Position.X > MaxVector.X) MaxVector.X = Vertex.Position.X;
                                if (Vertex.Position.Y > MaxVector.Y) MaxVector.Y = Vertex.Position.Y;
                                if (Vertex.Position.Z > MaxVector.Z) MaxVector.Z = Vertex.Position.Z;

                                Vertices.Add(Vertex, Vertices.Count);
                            }
                        }
                    }

                    ushort[] BI = new ushort[20];

                    for (int Index = 0; Index < BoneIndices.Count; Index++) BI[Index] = BoneIndices[Index];

                    SubMeshes.Add(new H3DSubMesh
                    {
                        Skinning = H3DSubMeshSkinning.Smooth,
                        BoneIndicesCount = (ushort)BoneIndices.Count,
                        BoneIndices = BI,
                        Indices = Indices.ToArray()
                    });
                }

                //Mesh
                H3DMesh M = new H3DMesh(Vertices.Keys, GetAttributes(), SubMeshes);

                M.Skinning = H3DMeshSkinning.Smooth;
                M.MeshCenter = (MinVector + MaxVector) * 0.5f;
                M.MaterialIndex = MaterialIndex++;

                Model.AddMesh(M);

                //Material
                H3DMaterial Material = H3DMaterial.Default;

                string MatName = Path.GetFileNameWithoutExtension(Mesh.MaterialName);
                uint MatHash = (uint)MatName.GetHashCode();

                Material.Name = string.Format("{0}_Mat{1:D5}", MatName, MaterialIndex - 1);
                Material.Texture0Name = MatName;
                Material.MaterialParams.UniqueId = MatHash;

                Model.Materials.Add(Material);
            }

            //Build Skeleton
            foreach (SMDBone Bone in Skeleton)
            {
                H3DBone B = new H3DBone();

                SMDNode Node = Nodes[Bone.NodeIndex];

                B.Name = Node.Name;

                B.ParentIndex = (short)Node.ParentIndex;

                B.Translation = Bone.Translation;
                B.Rotation = Bone.Rotation;
                B.Scale = new Vector3D(1, 1, 1);

                Model.Skeleton.Add(B);
            }

            //Calculate Absolute Inverse Transforms for all bones
            foreach (H3DBone Bone in Model.Skeleton)
            {
                Bone.InverseTransform = Bone.CalculateTransform(Model.Skeleton);
                Bone.InverseTransform.Invert();
            }

            Output.Models.Add(Model);

            Output.CopyMaterials();

            Output.BackwardCompatibility = ConvParams.Compatibility;
            Output.ForwardCompatibility = ConvParams.Compatibility;

            Output.ConverterVersion = ConvParams.ConverterVersion;

            Output.Flags = H3DFlags.IsFromNewConverter;

            return Output;
        }

        private PICAAttribute[] GetAttributes()
        {
            PICAAttribute[] Attributes = new PICAAttribute[5];

            for (int i = 0; i < 3; i++)
            {
                PICAAttributeName Name = default(PICAAttributeName);

                switch (i)
                {
                    case 0: Name = PICAAttributeName.Position; break;
                    case 1: Name = PICAAttributeName.Normal; break;
                    case 2: Name = PICAAttributeName.TexCoord0; break;
                }

                Attributes[i] = new PICAAttribute
                {
                    Name = Name,
                    Format = PICAAttributeFormat.Float,
                    Elements = Name == PICAAttributeName.TexCoord0 ? 2 : 3,
                    Scale = 1
                };
            }

            Attributes[3] = new PICAAttribute
            {
                Name = PICAAttributeName.BoneIndex,
                Format = PICAAttributeFormat.Ubyte,
                Elements = 4,
                Scale = 1
            };

            Attributes[4] = new PICAAttribute
            {
                Name = PICAAttributeName.BoneWeight,
                Format = PICAAttributeFormat.Ubyte,
                Elements = 4,
                Scale = 0.01f
            };

            return Attributes;
        }
    }
}
