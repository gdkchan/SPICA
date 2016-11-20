using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.CtrH3D.Model.Mesh;
using SPICA.Math3D;
using SPICA.PICA.Commands;
using SPICA.PICA.Converters;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace SPICA.Formats.Generic.COLLADA
{
    public partial class COLLADA
    {
        public COLLADA(H3D BaseModel)
        {
            asset = new asset
            {
                created = DateTime.Now,
                modified = DateTime.Now,
                contributor = new assetContributor[]
                {
                    new assetContributor
                    {
                        author = Environment.UserName,
                        authoring_tool = "SPICA"
                    }
                }
            };

            List<geometry> Geos = new List<geometry>();
            List<controller> Ctrls = new List<controller>();
            List<visual_scene> VScns = new List<visual_scene>();

            for (int MdlIndex = 0; MdlIndex < BaseModel.Models.Count; MdlIndex++)
            {
                H3DModel Mdl = BaseModel.Models[MdlIndex];

                List<node> Nodes = new List<node>();

                /*
                 * Mesh
                 */
                for (int MeshIndex = 0; MeshIndex < Mdl.Meshes.Count; MeshIndex++)
                {
                    H3DMesh Mesh = Mdl.Meshes[MeshIndex];

                    for (int SMIndex = 0; SMIndex < Mesh.SubMeshes.Count; SMIndex++)
                    {
                        H3DSubMesh SM = Mesh.SubMeshes[SMIndex];

                        string ShortName = Mdl.MeshNodesTree.Find(Mesh.NodeIndex);
                        string MeshName = string.Format("{0}_{1:D2}_{2:D2}_{3:D2}",
                            ShortName,
                            MdlIndex,
                            MeshIndex,
                            SMIndex);

                        PICAVertex[] Vertices = Mesh.ToVertices(true);

                        // *** Geometry ***
                        List<source> GeoSources = new List<source>();

                        List<InputLocal> VtxInputs = new List<InputLocal>();
                        List<InputLocalOffset> TriInputs = new List<InputLocalOffset>();

                        string VertsId = MeshName + "_verts";

                        TriInputs.Add(new InputLocalOffset
                        {
                            semantic = "VERTEX",
                            source = "#" + VertsId
                        });

                        foreach (PICAAttribute Attr in Mesh.Attributes)
                        {
                            if (Attr.Name >= PICAAttributeName.BoneIndex) continue;

                            string SrcName = MeshName + "_" + Attr.Name;
                            string SrcId = SrcName + "_id";

                            //Geometry
                            float_array Array = new float_array
                            {
                                id = SrcName + "_array_id",
                                count = (ulong)(Vertices.Length * Attr.Elements)
                            };

                            StringBuilder SB = new StringBuilder();

                            foreach (PICAVertex Vertex in Vertices)
                            {
                                switch (Attr.Name)
                                {
                                    case PICAAttributeName.Color: Append(SB, Vertex.Color); break;

                                    case PICAAttributeName.Position: Append(SB, Vertex.Position); break;
                                    case PICAAttributeName.Normal: Append(SB, Vertex.Normal); break;
                                    case PICAAttributeName.Tangent: Append(SB, Vertex.Tangent); break;

                                    case PICAAttributeName.TexCoord0: Append(SB, Vertex.TexCoord0); break;
                                    case PICAAttributeName.TexCoord1: Append(SB, Vertex.TexCoord1); break;
                                    case PICAAttributeName.TexCoord2: Append(SB, Vertex.TexCoord2); break;
                                }
                            }

                            Array.Text = SB.ToString().TrimEnd();

                            accessor Accessor = new accessor();

                            Accessor.source = "#" + Array.id;
                            Accessor.count = (ulong)Vertices.Length;
                            Accessor.stride = (ulong)Attr.Elements;

                            switch (Attr.Name)
                            {
                                case PICAAttributeName.Color: Accessor.param = GetParams("R", "G", "B", "A"); break;

                                case PICAAttributeName.Position:
                                case PICAAttributeName.Normal:
                                case PICAAttributeName.Tangent:
                                    Accessor.param = GetParams("X", "Y", "Z");
                                    break;

                                case PICAAttributeName.TexCoord0:
                                case PICAAttributeName.TexCoord1:
                                case PICAAttributeName.TexCoord2:
                                    Accessor.param = GetParams("S", "T");
                                    break;
                            }

                            if (Attr.Name < PICAAttributeName.TexCoord0)
                            {
                                string Semantic = string.Empty;

                                switch (Attr.Name)
                                {
                                    case PICAAttributeName.Color: Semantic = "COLOR"; break;

                                    case PICAAttributeName.Position: Semantic = "POSITION"; break;
                                    case PICAAttributeName.Normal: Semantic = "NORMAL"; break;
                                    case PICAAttributeName.Tangent: Semantic = "TANGENT"; break;
                                }

                                VtxInputs.Add(new InputLocal
                                {
                                    semantic = Semantic,
                                    source = "#" + SrcId
                                });
                            }
                            else
                            {
                                TriInputs.Add(new InputLocalOffset
                                {
                                    semantic = "TEXCOORD",
                                    source = "#" + SrcId,
                                    offset = 0,
                                    set = (ulong)Attr.Name - 4
                                });
                            }

                            GeoSources.Add(new source
                            {
                                id = SrcId,
                                name = SrcName,
                                Item = Array,
                                technique_common = new sourceTechnique_common { accessor = Accessor }
                            });
                        } //Attributes Loop

                        vertices Verts = new vertices
                        {
                            id = VertsId,
                            input = VtxInputs.ToArray()
                        };

                        triangles Tris = new triangles
                        {
                            count = (ulong)SM.Indices.Length,
                            input = TriInputs.ToArray(),
                            p = string.Join(" ", SM.Indices)
                        };

                        string GeoId = MeshName + "_geo_id";

                        Geos.Add(new geometry
                        {
                            id = GeoId,
                            name = MeshName,
                            Item = new mesh
                            {
                                source = GeoSources.ToArray(),
                                vertices = Verts,
                                Items = new triangles[] { Tris }
                            }
                        });

                        // *** Controller ***
                        string CtrlName = MeshName + "_ctrl";
                        string CtrlId = CtrlName + "_id";

                        StringBuilder BoneNames = new StringBuilder();
                        StringBuilder BindPoses = new StringBuilder();

                        for (int Index = 0; Index < SM.BoneIndicesCount; Index++)
                        {
                            BoneNames.Append(Mdl.Skeleton[SM.BoneIndices[Index]].Name + " ");
                            Append(BindPoses, Mdl.Skeleton[SM.BoneIndices[Index]].InverseTransform);
                        }

                        string SrcNamesName = CtrlName + "_names";
                        string SrcPosesName = CtrlName + "_poses";
                        string SrcWeightsName = CtrlName + "_weights";

                        //Accessors
                        accessor NamesAcc = new accessor
                        {
                            source = "#" + SrcNamesName + "_array_id",
                            count = SM.BoneIndicesCount,
                            stride = 1,
                            param = new param[]
                            {
                                new param { name = "JOINT", type = "Name" }
                            }
                        };

                        accessor PosesAcc = new accessor
                        {
                            source = "#" + SrcPosesName + "_array_id",
                            count = SM.BoneIndicesCount,
                            stride = 16,
                            param = new param[]
                            {
                                new param { name = "TRANSFORM", type = "float4x4" }
                            }
                        };

                        //Sources
                        source SrcNames = new source
                        {
                            id = SrcNamesName + "_id",
                            Item = new Name_array
                            {
                                id = SrcNamesName + "_array_id",
                                count = SM.BoneIndicesCount,
                                Text = BoneNames.ToString().TrimEnd()
                            },
                            technique_common = new sourceTechnique_common { accessor = NamesAcc }
                        };

                        source SrcPoses = new source
                        {
                            id = SrcPosesName + "_id",
                            Item = new float_array
                            {
                                id = SrcPosesName + "_array_id",
                                count = (ulong)(SM.BoneIndicesCount * 16),
                                Text = BindPoses.ToString().TrimEnd()
                            },
                            technique_common = new sourceTechnique_common { accessor = PosesAcc }
                        };

                        //Indices and weights
                        StringBuilder v = new StringBuilder();
                        StringBuilder vcount = new StringBuilder();

                        Dictionary<string, int> Weights = new Dictionary<string, int>();

                        foreach (PICAVertex Vertex in Vertices)
                        {
                            int Index;

                            for (Index = 0; Index < 4; Index++)
                            {
                                if (Vertex.Weights[Index] == 0) break;

                                string WStr = Vertex.Weights[Index].ToString(CultureInfo.InvariantCulture);

                                v.Append(Vertex.Indices[Index] + " ");

                                if (Weights.ContainsKey(WStr))
                                {
                                    v.Append(Weights[WStr] + " ");
                                }
                                else
                                {
                                    v.Append(Weights.Count + " ");

                                    Weights.Add(WStr, Weights.Count);
                                }
                            }

                            vcount.Append((Index + 1) + " ");
                        }

                        accessor WeightsAcc = new accessor
                        {
                            source = "#" + SrcWeightsName + "_array_id",
                            count = (ulong)Weights.Count,
                            stride = 1,
                            param = new param[]
                            {
                                new param { name = "WEIGHT", type = "float" }
                            }
                        };

                        source SrcWeights = new source
                        {
                            id = SrcWeightsName + "_id",
                            Item = new float_array
                            {
                                id = SrcWeightsName + "_array_id",
                                count = (ulong)Weights.Count,
                                Text = string.Join(" ", Weights.Keys)
                            },
                            technique_common = new sourceTechnique_common { accessor = WeightsAcc }
                        };

                        skin Skin = new skin();

                        Skin.source1 = "#" + GeoId;

                        Skin.source = new source[]
                        {
                            SrcNames,
                            SrcPoses,
                            SrcWeights
                        };

                        Skin.joints = new skinJoints
                        {
                            input = new InputLocal[]
                            {
                                new InputLocal { semantic = "JOINT", source = "#" + SrcNames.id },
                                new InputLocal { semantic = "INV_BIND_MATRIX", source = "#" + SrcPoses.id }
                            }
                        };

                        Skin.vertex_weights = new skinVertex_weights
                        {
                            count = (ulong)Vertices.Length,
                            input = new InputLocalOffset[]
                            {
                                new InputLocalOffset { semantic = "JOINT", source = "#" + SrcNames.id, offset = 0 },
                                new InputLocalOffset { semantic = "WEIGHT", source = "#" + SrcWeights.id, offset = 1 }
                            },
                            vcount = vcount.ToString().TrimEnd(),
                            v = v.ToString().TrimEnd()
                        };

                        Ctrls.Add(new controller
                        {
                            id = CtrlId,
                            Item = Skin
                        });

                        // *** Visual Scene ***
                        Nodes.Add(new node
                        {
                            id = MeshName + "_node_id",
                            name = ShortName,
                            type = NodeType.NODE,
                            instance_controller = new instance_controller[]
                            {
                                new instance_controller
                                {
                                    url = "#" + CtrlId
                                }
                            }
                        });
                    } //SubMesh Loop
                } //Mesh Loop

                string VSNId = string.Format("{0}_{1:D2}_vsn_id", Mdl.Name, MdlIndex);

                VScns.Add(new visual_scene
                {
                    id = VSNId,
                    name = Mdl.Name,
                    node = Nodes.ToArray()
                });

                if (MdlIndex == 0)
                {
                    scene = new COLLADAScene
                    {
                        instance_visual_scene = new InstanceWithExtra { url = "#" + VSNId }
                    };
                }
            } //Model Loop

            Items = new object[]
            {
                new library_geometries { geometry = Geos.ToArray() },
                new library_controllers { controller = Ctrls.ToArray() },
                new library_visual_scenes { visual_scene = VScns.ToArray() }
            };
        }

        public COLLADA() { }

        private param[] GetParams(params string[] Names)
        {
            param[] Output = new param[Names.Length];

            for (int Index = 0; Index < Names.Length; Index++)
            {
                Output[Index] = new param
                {
                    name = Names[Index],
                    type = "float"
                };
            }

            return Output;
        }

        private void Append(StringBuilder SB, Matrix3x4 Matrix)
        {
            SB.Append(Matrix.M11.ToString(CultureInfo.InvariantCulture) + " ");
            SB.Append(Matrix.M12.ToString(CultureInfo.InvariantCulture) + " ");
            SB.Append(Matrix.M13.ToString(CultureInfo.InvariantCulture) + " ");
            SB.Append(Matrix.M14.ToString(CultureInfo.InvariantCulture) + " ");

            SB.Append(Matrix.M21.ToString(CultureInfo.InvariantCulture) + " ");
            SB.Append(Matrix.M22.ToString(CultureInfo.InvariantCulture) + " ");
            SB.Append(Matrix.M23.ToString(CultureInfo.InvariantCulture) + " ");
            SB.Append(Matrix.M24.ToString(CultureInfo.InvariantCulture) + " ");

            SB.Append(Matrix.M31.ToString(CultureInfo.InvariantCulture) + " ");
            SB.Append(Matrix.M32.ToString(CultureInfo.InvariantCulture) + " ");
            SB.Append(Matrix.M33.ToString(CultureInfo.InvariantCulture) + " ");
            SB.Append(Matrix.M34.ToString(CultureInfo.InvariantCulture) + " ");

            SB.Append("0 0 0 1 ");
        }

        private void Append(StringBuilder SB, RGBAFloat Color)
        {
            SB.Append(Color.R.ToString(CultureInfo.InvariantCulture) + " ");
            SB.Append(Color.G.ToString(CultureInfo.InvariantCulture) + " ");
            SB.Append(Color.B.ToString(CultureInfo.InvariantCulture) + " ");
            SB.Append(Color.A.ToString(CultureInfo.InvariantCulture) + " ");
        }

        private void Append(StringBuilder SB, Vector3D Vector)
        {
            SB.Append(Vector.X.ToString(CultureInfo.InvariantCulture) + " ");
            SB.Append(Vector.Y.ToString(CultureInfo.InvariantCulture) + " ");
            SB.Append(Vector.Z.ToString(CultureInfo.InvariantCulture) + " ");
        }

        private void Append(StringBuilder SB, Vector2D Vector)
        {
            SB.Append(Vector.X.ToString(CultureInfo.InvariantCulture) + " ");
            SB.Append(Vector.Y.ToString(CultureInfo.InvariantCulture) + " ");
        }

        public void Save(string FileName)
        {
            using (FileStream FS = new FileStream(FileName, FileMode.Create))
            {
                XmlSerializer Serializer = new XmlSerializer(typeof(COLLADA));

                Serializer.Serialize(FS, this);
            }
        }
    }
}
