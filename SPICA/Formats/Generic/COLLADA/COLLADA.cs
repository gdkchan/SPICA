using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.Formats.CtrH3D.Model.Material.Texture;
using SPICA.Formats.CtrH3D.Model.Mesh;
using SPICA.Formats.CtrH3D.Texture;
using SPICA.Math3D;
using SPICA.PICA.Commands;
using SPICA.PICA.Converters;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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

            List<image> Imgs = new List<image>();
            List<material> Mats = new List<material>();
            List<effect> Effs = new List<effect>();
            List<geometry> Geos = new List<geometry>();
            List<controller> Ctrls = new List<controller>();
            List<visual_scene> VScns = new List<visual_scene>();

            foreach (H3DTexture Tex in BaseModel.Textures)
            {
                Imgs.Add(new image
                {
                    id = Tex.Name,
                    Item = "./" + Tex.Name + ".png"
                });
            }

            for (int MdlIndex = 0; MdlIndex < BaseModel.Models.Count; MdlIndex++)
            {
                H3DModel Mdl = BaseModel.Models[MdlIndex];

                List<node> Nodes = new List<node>();

                //Materials
                foreach (H3DMaterial Mtl in Mdl.Materials)
                {
                    string MtlName = MdlIndex.ToString("D2") + "_" + Mtl.Name;

                    effect Eff = new effect();

                    Eff.id = MtlName + "_eff_id";
                    Eff.name = Mtl.Name;

                    common_newparam_type ImgSurface = new common_newparam_type
                    {
                        sid = Mtl.Name + "_surface",
                        ItemElementName = ItemChoiceType.surface,
                        Item = new fx_surface_common
                        {
                            type = fx_surface_type_enum.Item2D,
                            init_from = new fx_surface_init_from_common[] 
                            {
                                new fx_surface_init_from_common { Value = Mtl.Texture0Name }
                            },
                            format = "PNG" 
                        }
                    };

                    common_newparam_type ImgSampler = new common_newparam_type
                    {
                        sid = Mtl.Name + "_sampler",
                        ItemElementName = ItemChoiceType.sampler2D,
                        Item = new fx_sampler2D_common
                        {
                            source = ImgSurface.sid,
                            wrap_s = GetWrap(Mtl.TextureMappers[0].WrapU),
                            wrap_t = GetWrap(Mtl.TextureMappers[0].WrapV),
                            minfilter = fx_sampler_filter_common.LINEAR,
                            magfilter = fx_sampler_filter_common.LINEAR,
                            mipfilter = fx_sampler_filter_common.LINEAR
                        }
                    };

                    Eff.Items = new object[]
                    {
                        new effectFx_profile_abstractProfile_COMMON
                        {
                            Items = new common_newparam_type[] { ImgSurface, ImgSampler },
                            technique = new effectFx_profile_abstractProfile_COMMONTechnique
                            {
                                Item = new effectFx_profile_abstractProfile_COMMONTechniquePhong
                                {
                                    emission = GetColor(Mtl.MaterialParams.EmissionColor),
                                    ambient = GetColor(Mtl.MaterialParams.AmbientColor),
                                    diffuse = new common_color_or_texture_type
                                    {
                                        Item = new common_color_or_texture_typeTexture
                                        {
                                            texture = ImgSampler.sid,
                                            texcoord = string.Empty
                                        }
                                    },
                                    specular = GetColor(Mtl.MaterialParams.Specular0Color)
                                }
                            }
                        }
                    };

                    Effs.Add(Eff);
                    Mats.Add(new material
                    {
                        id = MtlName + "_id",
                        name = Mtl.Name,
                        instance_effect = new instance_effect { url = "#" + Eff.id }
                    });
                }

                //Skeleton nodes
                string RootBoneId = string.Empty;

                if ((Mdl.Skeleton?.Count ?? 0) > 0)
                {
                    Queue<Tuple<H3DBone, node>> ChildBones = new Queue<Tuple<H3DBone, node>>();

                    node RootNode = new node();

                    ChildBones.Enqueue(Tuple.Create(Mdl.Skeleton[0], RootNode));

                    RootBoneId = "#" + Mdl.Skeleton[0].Name + "_bone_id";

                    while (ChildBones.Count > 0)
                    {
                        Tuple<H3DBone, node> Bone_Node = ChildBones.Dequeue();

                        H3DBone Bone = Bone_Node.Item1;

                        Bone_Node.Item2.id = Bone.Name + "_bone_id";
                        Bone_Node.Item2.name = Bone.Name;
                        Bone_Node.Item2.sid = Bone.Name;
                        Bone_Node.Item2.type = NodeType.JOINT;
                        Bone_Node.Item2.ItemsElementName = new ItemsChoiceType2[]
                        {
                            ItemsChoiceType2.matrix
                        };
                        Bone_Node.Item2.Items = new object[]
                        {
                            new matrix { Text = Bone.Transform.ToSerializableString() + " 0 0 0 1" }
                        };

                        List<node> ChildNodes = new List<node>();

                        foreach (H3DBone B in Mdl.Skeleton)
                        {
                            if (B.ParentIndex == -1) continue;

                            H3DBone ParentBone = Mdl.Skeleton[B.ParentIndex];

                            if (ParentBone == Bone)
                            {
                                node Node = new node();

                                ChildBones.Enqueue(Tuple.Create(B, Node));
                                ChildNodes.Add(Node);
                            }
                        }

                        Bone_Node.Item2.node1 = ChildNodes.ToArray();
                    }

                    Nodes.Add(RootNode);
                }

                //Mesh
                for (int MeshIndex = 0; MeshIndex < Mdl.Meshes.Count; MeshIndex++)
                {
                    H3DMesh Mesh = Mdl.Meshes[MeshIndex];

                    string MtlName = MdlIndex.ToString("D2") + "_" + Mdl.Materials[Mesh.MaterialIndex].Name;

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

                        //Geometry
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

                            string[] Values = new string[Vertices.Length];

                            for (int Index = 0; Index < Vertices.Length; Index++)
                            {
                                PICAVertex V = Vertices[Index];

                                switch (Attr.Name)
                                {
                                    case PICAAttributeName.Color:     Values[Index] = V.Color.ToSerializableString();     break;

                                    case PICAAttributeName.Position:  Values[Index] = V.Position.ToSerializableString();  break;
                                    case PICAAttributeName.Normal:    Values[Index] = V.Normal.ToSerializableString();    break;
                                    case PICAAttributeName.Tangent:   Values[Index] = V.Tangent.ToSerializableString();   break;

                                    case PICAAttributeName.TexCoord0: Values[Index] = V.TexCoord0.ToSerializableString(); break;
                                    case PICAAttributeName.TexCoord1: Values[Index] = V.TexCoord1.ToSerializableString(); break;
                                    case PICAAttributeName.TexCoord2: Values[Index] = V.TexCoord2.ToSerializableString(); break;
                                }
                            }

                            float_array Array = new float_array
                            {
                                id = SrcName + "_array_id",
                                count = (ulong)(Vertices.Length * Attr.Elements),
                                Text = string.Join(" ", Values)
                            };

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
                                    case PICAAttributeName.Color:    Semantic = "COLOR";    break;

                                    case PICAAttributeName.Position: Semantic = "POSITION"; break;
                                    case PICAAttributeName.Normal:   Semantic = "NORMAL";   break;
                                    case PICAAttributeName.Tangent:  Semantic = "TANGENT";  break;
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
                            material = "_" + MtlName,
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

                        //Controller
                        string CtrlName = MeshName + "_ctrl";
                        string CtrlId = CtrlName + "_id";

                        string[] BoneNames = new string[SM.BoneIndicesCount];
                        string[] BindPoses = new string[SM.BoneIndicesCount];

                        for (int Index = 0; Index < SM.BoneIndicesCount; Index++)
                        {
                            BoneNames[Index] = Mdl.Skeleton[SM.BoneIndices[Index]].Name;
                            BindPoses[Index] = Mdl.Skeleton[SM.BoneIndices[Index]].InverseTransform.ToSerializableString() + " 0 0 0 1";
                        }

                        string SrcNamesName = CtrlName + "_names";
                        string SrcPosesName = CtrlName + "_poses";
                        string SrcWeightsName = CtrlName + "_weights";

                        //Controller accessors
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

                        //Controller sources
                        source SrcNames = new source
                        {
                            id = SrcNamesName + "_id",
                            Item = new Name_array
                            {
                                id = SrcNamesName + "_array_id",
                                count = SM.BoneIndicesCount,
                                Text = string.Join(" ", BoneNames)
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
                                Text = string.Join(" ", BindPoses)
                            },
                            technique_common = new sourceTechnique_common { accessor = PosesAcc }
                        };

                        //Controller indices and weights
                        StringBuilder v = new StringBuilder();
                        StringBuilder vcount = new StringBuilder();

                        Dictionary<string, int> Weights = new Dictionary<string, int>();

                        bool HasFixedIndices = Mesh.FixedAttributes.Any(x => x.Name == PICAAttributeName.BoneIndex);
                        bool HasFixedWeights = Mesh.FixedAttributes.Any(x => x.Name == PICAAttributeName.BoneWeight);

                        PICAVectorFloat24 FixedIndices = default(PICAVectorFloat24);
                        PICAVectorFloat24 FixedWeights = default(PICAVectorFloat24);

                        if (HasFixedIndices || HasFixedWeights)
                        {
                            foreach (PICAFixedAttribute Attr in Mesh.FixedAttributes)
                            {
                                switch (Attr.Name)
                                {
                                    case PICAAttributeName.BoneIndex: FixedIndices = Attr.Value; break;
                                    case PICAAttributeName.BoneWeight: FixedWeights = Attr.Value; break;
                                }
                            }
                        }

                        if (SM.Skinning == H3DSubMeshSkinning.Smooth)
                        {
                            foreach (PICAVertex Vertex in Vertices)
                            {
                                int Count = 0;

                                for (int Index = 0; Index < 4; Index++)
                                {
                                    float BIndex = HasFixedIndices ? FixedIndices[Index] : Vertex.Indices[Index];
                                    float Weight = HasFixedWeights ? FixedWeights[Index] : Vertex.Weights[Index];

                                    if (Weight == 0) break;

                                    string WStr = Weight.ToString(CultureInfo.InvariantCulture);

                                    v.Append((int)BIndex + " ");

                                    if (Weights.ContainsKey(WStr))
                                    {
                                        v.Append(Weights[WStr] + " ");
                                    }
                                    else
                                    {
                                        v.Append(Weights.Count + " ");

                                        Weights.Add(WStr, Weights.Count);
                                    }

                                    Count++;
                                }

                                vcount.Append(Count + " ");
                            }
                        }
                        else
                        {
                            foreach (PICAVertex Vertex in Vertices)
                            {
                                if (HasFixedIndices)
                                    v.Append((int)FixedIndices[0] + " 1 ");
                                else
                                    v.Append(Vertex.Indices[0] + " 1 ");

                                vcount.Append("1 ");
                            }

                            Weights.Add("1", 0);
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

                        //Visual Scene
                        Nodes.Add(new node
                        {
                            id = MeshName + "_node_id",
                            name = ShortName,
                            type = NodeType.NODE,
                            instance_controller = new instance_controller[]
                            {
                                new instance_controller
                                {
                                    url = "#" + CtrlId,
                                    skeleton = new string[] { RootBoneId },
                                    bind_material = new bind_material
                                    {
                                        technique_common = new instance_material[]
                                        {
                                            new instance_material
                                            {
                                                symbol = "_" + MtlName,
                                                target = "#" + MtlName + "_id"
                                            }
                                        }
                                    }
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
                new library_images { image = Imgs.ToArray() },
                new library_materials { material = Mats.ToArray() },
                new library_effects { effect = Effs.ToArray() },
                new library_geometries { geometry = Geos.ToArray() },
                new library_controllers { controller = Ctrls.ToArray() },
                new library_visual_scenes { visual_scene = VScns.ToArray() }
            };
        }

        public COLLADA() { }

        private fx_sampler_wrap_common GetWrap(H3DTextureWrap Wrap)
        {
            switch (Wrap)
            {
                case H3DTextureWrap.ClampToEdge:   return fx_sampler_wrap_common.CLAMP;
                case H3DTextureWrap.ClampToBorder: return fx_sampler_wrap_common.BORDER;
                case H3DTextureWrap.Repeat:        return fx_sampler_wrap_common.WRAP;
                case H3DTextureWrap.Mirror:        return fx_sampler_wrap_common.MIRROR;

                default: throw new ArgumentException("Invalid Texture wrap!");
            }
        }

        private common_color_or_texture_type GetColor(RGBA Color)
        {
            return new common_color_or_texture_type
            {
                Item = new common_color_or_texture_typeColor
                {
                    Text = string.Format(CultureInfo.InvariantCulture, "{0} {1} {2} {3}",
                        Color.R / (float)byte.MaxValue,
                        Color.G / (float)byte.MaxValue,
                        Color.B / (float)byte.MaxValue,
                        Color.A / (float)byte.MaxValue)
                }
            };
        }

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
