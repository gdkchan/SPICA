using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Animation;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.CtrH3D.Model.Material;
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
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;

namespace SPICA.Formats.Generic.COLLADA
{
    internal sealed partial class DAEExporter
    {
        private readonly DAE Output;
        private readonly H3D Scene;
        private readonly H3DModel Model;
        private readonly int AnimationIndex;
        private readonly DAEExportOptions Options;
        private readonly string Prefix;
        private readonly Dictionary<string, string> ImageIds = new Dictionary<string, string>();
        private Matrix4x4[] BindWorld;
        private Matrix4x4[] InverseBind;
        private DAENode[] BoneNodes;
        private DAENode ModelNode;
        public DAEExporter(DAE Output, H3D Scene, int ModelIndex, int AnimationIndex, DAEExportOptions Options)
        {
            this.Output = Output;
            this.Scene = Scene ?? throw new ArgumentNullException(nameof(Scene));
            this.Options = Options ?? throw new ArgumentNullException(nameof(Options));
            Options.Validate();
            if (ModelIndex == -1 && AnimationIndex >= 0 && Scene.Models.Count > 0)
            {
                ModelIndex = 0;
            }

            if (ModelIndex < 0 || ModelIndex >= Scene.Models.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(ModelIndex), "Choose the model to export, including for animation exports.");
            }

            if (AnimationIndex < -1 || AnimationIndex >= Scene.SkeletalAnimations.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(AnimationIndex));
            }

            Model = Scene.Models[ModelIndex];
            this.AnimationIndex = AnimationIndex;
            Prefix = "m" + ModelIndex.ToString(CultureInfo.InvariantCulture) + "_";
        }

        public void Export()
        {
            DAEVisualScene Visual = new DAEVisualScene
            {
                id = Prefix + "scene",
                name = XmlConvert.EncodeLocalName(Model.Name)
            };
            ModelNode = new DAENode
            {
                id = Prefix + "model",
                name = XmlConvert.EncodeLocalName(Model.Name),
                matrix = CreateMatrix(Model.WorldTransform),
                Nodes = new List<DAENode>()
            };
            if (Model.WorldTransform.Equals(default(Matrix3x4)))
            {
                ModelNode.matrix = DAEMatrix.Identity;
            }

            Visual.node.Add(ModelNode);
            Output.library_visual_scenes = new List<DAEVisualScene>
            {
                Visual
            };
            Output.scene.instance_visual_scene.url = "#" + Visual.id;
            ExportImages();
            ExportMaterials();
            ExportSkeleton();
            ExportMeshes();
            if (AnimationIndex >= 0 && Options.ExportSkinning)
            {
                ExportAnimation(Scene.SkeletalAnimations[AnimationIndex]);
            }

            if (AnimationIndex >= 0 && !Options.ExportSkinning)
            {
                Warn("Blender-compatible export writes static meshes. Enable ExportSkinning to include the skeleton and animation.");
            }

            if (Scene.MaterialAnimations.Count > 0 || Scene.VisibilityAnimations.Count > 0)
            {
                Warn("Material and visibility animations are not represented by this skeletal DAE export.");
            }

            Output.extra.Add(Extra("export_report", new
            {
                version = 1,
                model = Model.Name,
                Options.FramesPerSecond,
                Options.SamplesPerFrame,
                warnings = Output.ExportWarnings.ToArray()
            }));
            if (Output.library_images.Count == 0)
            {
                Output.library_images = null;
            }

            if (Output.library_materials.Count == 0)
            {
                Output.library_materials = null;
            }

            if (Output.library_effects.Count == 0)
            {
                Output.library_effects = null;
            }

            if (Output.library_geometries.Count == 0)
            {
                Output.library_geometries = null;
            }

            if (Output.library_controllers.Count == 0)
            {
                Output.library_controllers = null;
            }

            if (Output.library_animations?.Count == 0)
            {
                Output.library_animations = null;
            }
        }

        private void ExportImages()
        {
            Output.library_images = new List<DAEImage>();
            for (int I = 0; I < Scene.Textures.Count; I++)
            {
                H3DTexture Texture = Scene.Textures[I];
                string Id = Prefix + "image_" + I;
                ImageIds.Add(Texture.Name, Id);
                string File = Id + ".png";
                Output.library_images.Add(new DAEImage { id = Id, init_from = File });
                if (Options.ExportTextures)
                {
                    Output.ExportTextures.Add(Tuple.Create(Texture, File));
                }

                if (Texture.IsCubeTexture)
                {
                    Warn("Cubemap '" + Texture.Name + "' uses its +X face in standard COLLADA; the other faces are exported separately.");
                }

                if (Texture.IsCubeTexture && Options.ExportTextures)
                {
                    for (int Face = 1; Face < 6; Face++)
                    {
                        Output.ExportTextureFaces.Add(Tuple.Create(Texture, Id + "_face" + Face + ".png", Face));
                    }
                }
            }
        }

        private static DAEPhongDiffuse Color(RGBA Color)
        {
            return new DAEPhongDiffuse
            {
                color = new DAEVector4
                {
                    data = DAEUtils.VectorStr(new Vector4(Color.R / 255f, Color.G / 255f, Color.B / 255f, Color.A / 255f))
                }
            };
        }

        private void ExportMaterials()
        {
            Output.library_materials = new List<DAEMaterial>();
            Output.library_effects = new List<DAEEffect>();
            foreach (H3DMaterial Material in Model.Materials)
            {
                int Index = Output.library_materials.Count;
                DAEEffect Effect = new DAEEffect
                {
                    id = Prefix + "effect_" + Index,
                    name = XmlConvert.EncodeLocalName(Material.Name)
                };
                H3DMaterialParams Parameters = Material.MaterialParams;
                DAEPhong Phong = Effect.profile_COMMON.technique.phong;
                Effect.profile_COMMON.technique.sid = "common";
                Phong.emission = Color(Parameters.EmissionColor);
                Phong.ambient = Color(Parameters.AmbientColor);
                Phong.diffuse = Color(Parameters.DiffuseColor);
                Phong.specular = Color(Parameters.Specular0Color);
                string[] Names =
                {
                    Material.Texture0Name,
                    Material.Texture1Name,
                    Material.Texture2Name
                };
                int FirstTexture = -1;
                for (int Slot = 0; Slot < 3; Slot++)
                {
                    if (string.IsNullOrEmpty(Names[Slot]))
                    {
                        continue;
                    }

                    if (!ImageIds.TryGetValue(Names[Slot], out string Image))
                    {
                        Warn("Material '" + Material.Name + "' references missing texture '" + Names[Slot] + "'.");
                        continue;
                    }

                    string Surface = "surface_" + Slot, Sampler = "sampler_" + Slot;
                    H3DTextureMapper Mapper = Material.TextureMappers[Slot];
                    Effect.profile_COMMON.newparam.Add(new DAEEffectParam
                    {
                        sid = Surface,
                        surface = new DAEEffectParamSurfaceElement
                        {
                            type = "2D",
                            init_from = Image,
                            format = "PNG"
                        }
                    });

                    Effect.profile_COMMON.newparam.Add(new DAEEffectParam
                    {
                        sid = Sampler,
                        sampler2D = new DAEEffectParamSampler2DElement
                        {
                            source = Surface,
                            wrap_s = Mapper.WrapU.ToDAEWrap(),
                            wrap_t = Mapper.WrapV.ToDAEWrap(),
                            minfilter = Mapper.MinFilter.ToDAEFilter(),
                            magfilter = Mapper.MagFilter.ToDAEFilter(),
                            mipfilter = DAEFilter.LINEAR
                        }
                    });
                    if (Material.EnabledTextures[Slot] && FirstTexture < 0)
                    {
                        FirstTexture = Slot;
                    }
                }

                if (FirstTexture >= 0)
                {
                    Phong.diffuse = new DAEPhongDiffuse
                    {
                        texture = new DAEPhongDiffuseTexture
                        {
                            texture = "sampler_" + FirstTexture,
                            texcoord = "UV" + FirstTexture
                        }
                    };
                    if (Parameters.AlphaTest.Enabled || Parameters.DiffuseColor.A < 255)
                    {
                        Phong.transparent = new DAEPhongDiffuse
                        {
                            opaque = "A_ONE",
                            texture = new DAEPhongDiffuseTexture
                            {
                                texture = "sampler_" + FirstTexture,
                                texcoord = "UV" + FirstTexture
                            }
                        };
                    }
                }
                else if (Parameters.DiffuseColor.A < 255)
                {
                    Phong.transparent = Color(Parameters.DiffuseColor);
                    Phong.transparent.opaque = "A_ONE";
                }

                Effect.extra.Add(Extra("pica_material", new
                {
                    Material.Name,
                    textureNames = Names,
                    Material.EnabledTextures,
                    Material.TextureMappers,
                    Parameters.TextureCoords,
                    Parameters.TextureSources,
                    Parameters.TexCoordConfig,
                    Parameters.EmissionColor,
                    Parameters.AmbientColor,
                    Parameters.DiffuseColor,
                    Parameters.Specular0Color,
                    Parameters.Specular1Color,
                    Parameters.Constant0Color,
                    Parameters.Constant1Color,
                    Parameters.Constant2Color,
                    Parameters.Constant3Color,
                    Parameters.Constant4Color,
                    Parameters.Constant5Color,
                    Parameters.ColorScale,
                    Parameters.TexEnvStages,
                    Parameters.TexEnvBufferColor,
                    Parameters.ColorOperation,
                    Parameters.BlendFunction,
                    Parameters.BlendColor,
                    Parameters.AlphaTest,
                    Parameters.DepthColorMask,
                    Parameters.StencilTest,
                    Parameters.StencilOperation,
                    Parameters.FaceCulling,
                    Parameters.BumpMode,
                    Parameters.BumpTexture,
                    Parameters.ShaderReference,
                    Parameters.VtxShaderUniforms,
                    Parameters.GeoShaderUniforms,
                    Parameters.LUTDist0TableName,
                    Parameters.LUTDist1TableName,
                    Parameters.LUTFresnelTableName,
                    Parameters.LUTReflecRTableName,
                    Parameters.LUTReflecGTableName,
                    Parameters.LUTReflecBTableName
                }));
                Warn("Material '" + Material.Name + "' uses a standard shading fallback. Original PICA material state is stored in SPICA extras; generic importers do not reproduce the game shader.");
                Output.library_effects.Add(Effect);
                Output.library_materials.Add(new DAEMaterial
                {
                    id = Prefix + "material_" + Index,
                    name = XmlConvert.EncodeLocalName(Material.Name),
                    instance_effect = new DAEInstanceEffect
                    {
                        url = "#" + Effect.id
                    }
                });
            }
        }

        private void ExportSkeleton()
        {
            int Count = Model.Skeleton.Count;
            BindWorld = new Matrix4x4[Count];
            InverseBind = new Matrix4x4[Count];
            BoneNodes = new DAENode[Count];
            byte[] State = new byte[Count];
            Matrix4x4 Resolve(int I)
            {
                if (State[I] == 2)
                {
                    return BindWorld[I];
                }

                if (State[I] == 1)
                {
                    throw new InvalidDataException("Skeleton contains a parent cycle at '" + Model.Skeleton[I].Name + "'.");
                }

                State[I] = 1;
                H3DBone Bone = Model.Skeleton[I];
                int Parent = Bone.ParentIndex;
                if (Parent < -1 || Parent >= Count)
                {
                    throw new InvalidDataException("Invalid parent for bone '" + Bone.Name + "'.");
                }

                Matrix4x4 ParentWorld = Parent < 0 ? Matrix4x4.Identity : Resolve(Parent);
                InverseBind[I] = Affine(Bone.InverseTransform);
                if (!Matrix4x4.Invert(InverseBind[I], out BindWorld[I]))
                {
                    BindWorld[I] = Bone.Transform * ParentWorld;
                    if (!Matrix4x4.Invert(BindWorld[I], out InverseBind[I]))
                    {
                        throw new InvalidDataException("Singular bind transform for bone '" + Bone.Name + "'.");
                    }

                    Warn("Bone '" + Bone.Name + "' has no usable inverse bind matrix; its bind pose was reconstructed from local transforms.");
                }

                State[I] = 2;
                return BindWorld[I];
            }

            for (int I = 0; I < Count; I++)
            {
                Resolve(I);
            }

            if (!Options.ExportSkinning)
            {
                return;
            }

            for (int I = 0; I < Count; I++)
            {
                H3DBone Bone = Model.Skeleton[I];
                Matrix4x4 Local = BindWorld[I];
                if (Bone.ParentIndex >= 0)
                {
                    Local *= InverseBind[Bone.ParentIndex];
                }

                BoneNodes[I] = new DAENode
                {
                    id = BoneId(I),
                    sid = "joint_" + I,
                    name = XmlConvert.EncodeLocalName(Bone.Name),
                    type = DAENodeType.JOINT,
                    matrix = CreateMatrix(Local),
                    Nodes = new List<DAENode>()
                };
                BoneNodes[I].matrix.sid = "transform";
                if (Bone.BillboardMode != H3DBillboardMode.Off)
                {
                    Warn("Bone '" + Bone.Name + "' is camera-facing; COLLADA exports its bind/animated transform without a runtime billboard constraint.");
                }

                BoneNodes[I].extra.Add(Extra("bone", new
                {
                    originalName = Bone.Name,
                    Bone.Flags,
                    Bone.BillboardMode
                }));
            }

            for (int I = 0; I < Count; I++)
            {
                int Parent = Model.Skeleton[I].ParentIndex;
                if (Parent < 0)
                {
                    ModelNode.Nodes.Add(BoneNodes[I]);
                }
                else
                {
                    BoneNodes[Parent].Nodes.Add(BoneNodes[I]);
                }
            }
        }

        private void ExportMeshes()
        {
            Output.library_geometries = new List<DAEGeometry>();
            Output.library_controllers = new List<DAEController>();
            for (int M = 0; M < Model.Meshes.Count; M++)
            {
                H3DMesh Mesh = Model.Meshes[M];
                PICAVertex[] Decoded = Mesh.GetVertices();
                ApplyFixedAttributes(Mesh, Decoded);
                for (int S = 0; S < Mesh.SubMeshes.Count; S++)
                {
                    H3DSubMesh Sub = Mesh.SubMeshes[S];
                    ushort[] Triangles = Triangulate(Sub);
                    if (Triangles.Length == 0)
                    {
                        continue;
                    }

                    ushort[] Original = Triangles.Distinct().ToArray();
                    if (Original.Any(I => I >= Decoded.Length))
                    {
                        throw new InvalidDataException("Mesh contains an out-of-range vertex index.");
                    }

                    Dictionary<ushort, ushort> Remap = Original.Select((V, I) => (V, I)).ToDictionary(Pair => Pair.V, Pair => (ushort)Pair.I);
                    PICAVertex[] Vertices = Original.Select(I => Decoded[I]).ToArray();
                    bool HasBones = Sub.BoneIndicesCount > 0 && BoneNodes.Length > 0;
                    bool Skinned = HasBones && Options.ExportSkinning;
                    bool Smooth = Sub.Skinning == H3DSubMeshSkinning.Smooth;
                    if (HasBones && !Smooth)
                    {
                        for (int I = 0; I < Vertices.Length; I++)
                        {
                            int Bone = PaletteBone(Sub, Vertices[I].Indices[0]);
                            PICAVertex V = Vertices[I];
                            V.Position = Vector4.Transform(new Vector4(V.Position.X, V.Position.Y, V.Position.Z, 1), BindWorld[Bone]);
                            V.Normal = Direction(V.Normal, Matrix4x4.Transpose(InverseBind[Bone]));
                            V.Tangent = Direction(V.Tangent, BindWorld[Bone]);
                            Vertices[I] = V;
                        }
                    }

                    string Id = Prefix + "mesh_" + M + "_" + S;
                    DAEGeometry Geometry = new DAEGeometry
                    {
                        id = Id,
                        name = XmlConvert.EncodeLocalName(MeshName(Mesh, M, S))
                    };
                    Geometry.mesh.vertices.id = Id + "_vertices";
                    Geometry.mesh.triangles.AddInput("VERTEX", "#" + Geometry.mesh.vertices.id);
                    Geometry.mesh.triangles.Set_p(Triangles.Select(I => Remap[I]).ToArray());
                    HashSet<PICAAttributeName> Attributes = new HashSet<PICAAttributeName>(Mesh.Attributes.Select(A => A.Name));
                    Attributes.UnionWith(Mesh.FixedAttributes.Select(A => A.Name));
                    if (!Attributes.Contains(PICAAttributeName.Position))
                    {
                        throw new InvalidDataException("Mesh has no position attribute.");
                    }

                    AddAttribute(Geometry, "position", Vertices.Select(V => DAEUtils.Vector3Str(V.Position)), "POSITION", 3);
                    if (Attributes.Contains(PICAAttributeName.Normal))
                    {
                        AddAttribute(Geometry, "normal", Vertices.Select(V => DAEUtils.Vector3Str(V.Normal)), "NORMAL", 3);
                    }

                    if (Attributes.Contains(PICAAttributeName.Tangent))
                    {
                        AddAttribute(Geometry, "tangent", Vertices.Select(V => DAEUtils.Vector3Str(V.Tangent)), "TEXTANGENT", 3);
                    }

                    if (Attributes.Contains(PICAAttributeName.Color))
                    {
                        AddAttribute(Geometry, "color", Vertices.Select(V => DAEUtils.Vector4Str(V.Color)), "COLOR", 4);
                    }

                    for (int Uv = 0; Uv < 3; Uv++)
                    {
                        PICAAttributeName Attribute = PICAAttributeName.TexCoord0 + (uint)Uv;
                        if (Attributes.Contains(Attribute))
                        {
                            AddAttribute(Geometry, "uv" + Uv, Vertices.Select(V => DAEUtils.Vector2Str(UV(V, Uv))), "TEXCOORD", 2, (uint)Uv);
                        }
                    }

                    if (Mesh.MaterialIndex >= Output.library_materials.Count)
                    {
                        throw new InvalidDataException("Mesh '" + Geometry.name + "' references a missing material.");
                    }

                    string Symbol = "material_" + Mesh.MaterialIndex;
                    Geometry.mesh.triangles.material = Symbol;
                    DAENode Node = new DAENode
                    {
                        id = Id + "_node",
                        name = Geometry.name,
                        matrix = DAEMatrix.Identity
                    };
                    DAENodeInstance Instance = new DAENodeInstance();
                    Instance.bind_material.technique_common.instance_material.symbol = Symbol;
                    Instance.bind_material.technique_common.instance_material.target = "#" + Output.library_materials[Mesh.MaterialIndex].id;
                    BindTextureCoordinates(Mesh, Vertices, Geometry, Instance);
                    if (Skinned)
                    {
                        DAEController Controller = ExportController(Id, Sub, Vertices);
                        Output.library_controllers.Add(Controller);
                        Instance.url = "#" + Controller.id;
                        Instance.skeleton.AddRange(Enumerable.Range(0, BoneNodes.Length).Where(I => Model.Skeleton[I].ParentIndex < 0).Select(I => "#" + BoneId(I)));
                        Node.instance_controller = Instance;
                    }
                    else
                    {
                        Instance.url = "#" + Id;
                        Node.instance_geometry = Instance;
                    }

                    Node.extra.Add(Extra("mesh", new
                    {
                        Mesh.Type,
                        Mesh.Layer,
                        Mesh.Priority,
                        Mesh.NodeIndex,
                        Sub.Skinning,
                        visible = Mesh.NodeIndex >= Model.MeshNodesVisibility.Count || Model.MeshNodesVisibility[Mesh.NodeIndex]
                    }));
                    Output.library_geometries.Add(Geometry);
                    ModelNode.Nodes.Add(Node);
                }
            }
        }

        private void BindTextureCoordinates(H3DMesh Mesh, PICAVertex[] Vertices, DAEGeometry Geometry, DAENodeInstance Instance)
        {
            H3DMaterial Material = Model.Materials[Mesh.MaterialIndex];
            for (int Slot = 0; Slot < 3; Slot++)
            {
                string Name = Slot == 0 ? Material.Texture0Name : Slot == 1 ? Material.Texture1Name : Material.Texture2Name;
                if (string.IsNullOrEmpty(Name))
                {
                    continue;
                }

                int Coordinate = Slot;
                H3DTexCoordConfig Config = Material.MaterialParams.TexCoordConfig;
                if (Slot == 2 && (Config == H3DTexCoordConfig.Config0110 || Config == H3DTexCoordConfig.Config0111 || Config == H3DTexCoordConfig.Config0112))
                {
                    Coordinate = 1;
                }

                int Source = (int)Material.MaterialParams.TextureSources[Coordinate];
                H3DTextureCoord Coord = Material.MaterialParams.TextureCoords[Coordinate];
                if (Source < 0 || Source > 2 || Coord.MappingType != H3DTextureMappingType.UvCoordinateMap)
                {
                    Warn("Material '" + Material.Name + "' texture " + Slot + " uses generated coordinates; its mapping is preserved in SPICA extras.");
                    continue;
                }

                uint Set = (uint)(3 + Slot);
                Matrix4x4 Transform = Affine(Coord.GetTransform());
                AddAttribute(Geometry, "mapped_uv" + Slot, Vertices.Select(V => DAEUtils.Vector2Str(Vector4.Transform(new Vector4(UV(V, Source).X, UV(V, Source).Y, 0, 1), Transform))), "TEXCOORD", 2, Set);
                Instance.bind_material.technique_common.instance_material.bind_vertex_input.Add(new DAEBindVertexInput { semantic = "UV" + Slot, input_set = Set });
            }
        }

        private DAEController ExportController(string Id, H3DSubMesh Sub, PICAVertex[] Vertices)
        {
            DAEController Controller = new DAEController
            {
                id = Id + "_skin",
                name = Id + "_skin"
            };
            DAESkin Skin = Controller.skin;
            Skin.source = "#" + Id;
            List<string> Weights = new List<string>();
            List<int> Pairs = new List<int>();
            List<int> Counts = new List<int>();
            foreach (PICAVertex Vertex in Vertices)
            {
                int Count = 0;
                float Total = 0;
                for (int Influence = 0; Influence < (Sub.Skinning == H3DSubMeshSkinning.Smooth ? 4 : 1); Influence++)
                {
                    float Weight = Sub.Skinning == H3DSubMeshSkinning.Smooth ? Vertex.Weights[Influence] : 1;
                    if (!float.IsFinite(Weight) || Weight < 0)
                    {
                        throw new InvalidDataException("Invalid skin weight.");
                    }

                    if (Weight == 0)
                    {
                        continue;
                    }

                    Pairs.Add(PaletteBone(Sub, Vertex.Indices[Influence]));
                    Pairs.Add(Weights.Count);
                    Weights.Add(F(Weight));
                    Total += Weight;
                    Count++;
                }

                if (Count == 0)
                {
                    throw new InvalidDataException("Smooth-skinned vertex has no nonzero bone weights.");
                }

                if (Math.Abs(Total - 1) > 0.0001f)
                {
                    Warn("Non-normalized skin weights were preserved; an importing application may normalize them.");
                }

                Counts.Add(Count);
            }

            Skin.src.Add(new DAESource(Id + "_joints", 1, BoneNodes.Select(N => N.sid).ToArray(), "JOINT", "Name"));
            Skin.src.Add(new DAESource(Id + "_binds", 16, InverseBind.Select(MatrixText).ToArray(), "TRANSFORM", "float4x4"));
            Skin.src.Add(new DAESource(Id + "_weights", 1, Weights.ToArray(), "WEIGHT", "float"));
            Skin.joints.AddInput("JOINT", "#" + Skin.src[0].id);
            Skin.joints.AddInput("INV_BIND_MATRIX", "#" + Skin.src[1].id);
            Skin.vertex_weights.count = (uint)Vertices.Length;
            Skin.vertex_weights.AddInput("JOINT", "#" + Skin.src[0].id, 0);
            Skin.vertex_weights.AddInput("WEIGHT", "#" + Skin.src[2].id, 1);
            Skin.vertex_weights.vcount = string.Join(" ", Counts);
            Skin.vertex_weights.v = string.Join(" ", Pairs);
            return Controller;
        }

        private int PaletteBone(H3DSubMesh Sub, int Index)
        {
            if (Index < 0 || Index >= Sub.BoneIndicesCount || Sub.BoneIndices[Index] >= BoneNodes.Length)
            {
                throw new InvalidDataException("Submesh references an invalid bone palette entry.");
            }

            return Sub.BoneIndices[Index];
        }

        private static void AddAttribute(DAEGeometry Geometry, string Name, IEnumerable<string> Values, string Semantic, int Stride, uint Set = 0)
        {
            string[] Parameters = Stride == 2 ? new[]
            {
                "S",
                "float",
                "T",
                "float"
            }

            : Stride == 3 ? new[]
            {
                "X",
                "float",
                "Y",
                "float",
                "Z",
                "float"
            }

            : new[]
            {
                "R",
                "float",
                "G",
                "float",
                "B",
                "float",
                "A",
                "float"
            };
            DAESource Source = new DAESource(Geometry.id + "_" + Name, Stride, Values.ToArray(), Parameters);
            Geometry.mesh.source.Add(Source);
            if (Semantic == "POSITION")
            {
                Geometry.mesh.vertices.AddInput(Semantic, "#" + Source.id);
            }
            else
            {
                Geometry.mesh.triangles.AddInput(Semantic, "#" + Source.id, 0, Set);
            }
        }

        private static Vector4 UV(PICAVertex Vertex, int Set)
        {
            return Set == 0 ? Vertex.TexCoord0 : Set == 1 ? Vertex.TexCoord1 : Vertex.TexCoord2;
        }

        private static Vector4 Direction(Vector4 Value, Matrix4x4 Transform)
        {
            Vector3 Result = Vector3.TransformNormal(new Vector3(Value.X, Value.Y, Value.Z), Transform);
            if (Result.LengthSquared() > 0)
            {
                Result = Vector3.Normalize(Result);
            }

            return new Vector4(Result, 0);
        }

        private static void ApplyFixedAttributes(H3DMesh Mesh, PICAVertex[] Vertices)
        {
            for (int I = 0; I < Vertices.Length; I++)
            {
                PICAVertex V = Vertices[I];
                foreach (PICAFixedAttribute Attribute in Mesh.FixedAttributes)
                {
                    PICAVectorFloat24 A = Attribute.Value;
                    Vector4 Value = new Vector4(A.X, A.Y, A.Z, A.W);
                    switch (Attribute.Name)
                    {
                        case PICAAttributeName.Position:
                            V.Position = Value + Mesh.PositionOffset;
                            break;
                        case PICAAttributeName.Normal:
                            V.Normal = Value;
                            break;
                        case PICAAttributeName.Tangent:
                            V.Tangent = Value;
                            break;
                        case PICAAttributeName.Color:
                            V.Color = Value;
                            break;
                        case PICAAttributeName.TexCoord0:
                            V.TexCoord0 = Value;
                            break;
                        case PICAAttributeName.TexCoord1:
                            V.TexCoord1 = Value;
                            break;
                        case PICAAttributeName.TexCoord2:
                            V.TexCoord2 = Value;
                            break;
                        case PICAAttributeName.BoneIndex:
                            for (int J = 0; J < 4; J++)
                            {
                                V.Indices[J] = (int)Value[J];
                            }

                            break;
                        case PICAAttributeName.BoneWeight:
                            for (int J = 0; J < 4; J++)
                            {
                                V.Weights[J] = Value[J];
                            }

                            break;
                    }
                }

                Vertices[I] = V;
            }
        }

        private static ushort[] Triangulate(H3DSubMesh Sub)
        {
            if (Sub.Indices == null)
            {
                throw new InvalidDataException("Submesh has no index buffer.");
            }

            if (Sub.PrimitiveMode == PICAPrimitiveMode.Triangles)
            {
                if (Sub.Indices.Length % 3 != 0)
                {
                    throw new InvalidDataException("Triangle index count is not divisible by three.");
                }

                return Sub.Indices;
            }

            if (Sub.PrimitiveMode != PICAPrimitiveMode.TriangleStrip && Sub.PrimitiveMode != PICAPrimitiveMode.TriangleFan)
            {
                throw new NotSupportedException("Geometry shader primitives require evaluating the original shader before COLLADA export.");
            }

            List<ushort> Result = new List<ushort>();
            for (int I = 2; I < Sub.Indices.Length; I++)
            {
                ushort A = Sub.PrimitiveMode == PICAPrimitiveMode.TriangleFan ? Sub.Indices[0] : Sub.Indices[I - 2];
                ushort B = Sub.Indices[I - 1], C = Sub.Indices[I];
                if (Sub.PrimitiveMode == PICAPrimitiveMode.TriangleStrip && I % 2 != 0)
                {
                    (A, B) = (B, A);
                }

                if (A == B || B == C || A == C)
                {
                    continue;
                }

                Result.Add(A);
                Result.Add(B);
                Result.Add(C);
            }

            return Result.ToArray();
        }

        private string MeshName(H3DMesh Mesh, int M, int S)
        {
            string Name = Model.MeshNodesTree != null && Mesh.NodeIndex < Model.MeshNodesTree.Count ? Model.MeshNodesTree.Find(Mesh.NodeIndex) : "Mesh";
            return Name + "_" + M + "_" + S;
        }

        private string BoneId(int I)
        {
            return Prefix + "bone_" + I;
        }

        private void Warn(string Message)
        {
            if (!Output.ExportWarnings.Contains(Message))
            {
                Output.ExportWarnings.Add(Message);
            }
        }

        private static string F(float Value)
        {
            if (!float.IsFinite(Value))
            {
                throw new InvalidDataException("Non-finite value cannot be exported to COLLADA.");
            }

            return Value.ToString("R", CultureInfo.InvariantCulture);
        }

        private static Matrix4x4 Affine(Matrix3x4 M)
        {
            return new Matrix4x4(M.M11, M.M12, M.M13, 0, M.M21, M.M22, M.M23, 0, M.M31, M.M32, M.M33, 0, M.M41, M.M42, M.M43, 1);
        }

        private static DAEMatrix CreateMatrix(Matrix3x4 Matrix)
        {
            return CreateMatrix(Affine(Matrix));
        }

        private static DAEMatrix CreateMatrix(Matrix4x4 Matrix)
        {
            return new DAEMatrix
            {
                data = MatrixText(Matrix)
            };
        }

        private static string MatrixText(Matrix4x4 M)
        {
            return string.Join(" ", new[] { M.M11, M.M21, M.M31, M.M41, M.M12, M.M22, M.M32, M.M42, M.M13, M.M23, M.M33, M.M43, M.M14, M.M24, M.M34, M.M44 }.Select(F));
        }

        private static DAEExtra Extra(string Name, object Value)
        {
            XmlDocument Document = new XmlDocument();
            XmlElement Element = Document.CreateElement(Name, "urn:spica:collada:1");
            Element.SetAttribute("encoding", "json");
            Element.InnerText = JsonSerializer.Serialize(Value, new JsonSerializerOptions
            {
                IncludeFields = true,
                Converters = { new JsonStringEnumConverter() }
            });
            DAEExtra Extra = new DAEExtra();
            Extra.technique.data.Add(Element);
            return Extra;
        }

        private void ExportAnimation(H3DAnimation Animation)
        {
            ExportMatrixAnimation(Animation);
        }
    }
}


