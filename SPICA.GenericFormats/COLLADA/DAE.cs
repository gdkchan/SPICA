using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Animation;
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

namespace SPICA.GenericFormats.COLLADA
{
    [XmlRoot("COLLADA", Namespace = "http://www.collada.org/2005/11/COLLADASchema")]
    public class DAE
    {
        [XmlAttribute] public string version = "1.4.1";

        public DAEAsset asset = new DAEAsset();

        [XmlArrayItem("animation")]    public List<DAEAnimation>   library_animations    = new List<DAEAnimation>();
        [XmlArrayItem("image")]        public List<DAEImage>       library_images        = new List<DAEImage>();
        [XmlArrayItem("material")]     public List<DAEMaterial>    library_materials     = new List<DAEMaterial>();
        [XmlArrayItem("effect")]       public List<DAEEffect>      library_effects       = new List<DAEEffect>();
        [XmlArrayItem("geometry")]     public List<DAEGeometry>    library_geometries    = new List<DAEGeometry>();
        [XmlArrayItem("controller")]   public List<DAEController>  library_controllers   = new List<DAEController>();
        [XmlArrayItem("visual_scene")] public List<DAEVisualScene> library_visual_scenes = new List<DAEVisualScene>();

        public DAEScene scene = new DAEScene();

        public DAE() { }

        public DAE(H3D SceneData, int MdlIndex, int AnimIndex = -1)
        {
            if (MdlIndex != -1)
            {
                H3DModel Mdl = SceneData.Models[MdlIndex];

                DAEVisualScene VN = new DAEVisualScene();

                VN.name = Mdl.Name + "_" + MdlIndex.ToString("D2");
                VN.id = VN.name + "_id";

                //Materials
                foreach (H3DMaterial Mtl in Mdl.Materials)
                {
                    string MtlName = MdlIndex.ToString("D2") + "_" + Mtl.Name;

                    DAEEffect Effect = new DAEEffect();

                    Effect.name = Mtl.Name + "_eff";
                    Effect.id = Effect.name + "_id";

                    DAEEffectParam ImgSurface = new DAEEffectParam();
                    DAEEffectParam ImgSampler = new DAEEffectParam();

                    ImgSurface.surface = new DAEEffectParamSurfaceElement();
                    ImgSampler.sampler2D = new DAEEffectParamSampler2DElement();

                    ImgSurface.sid = Mtl.Name + "_surf";
                    ImgSurface.surface.type = "2D";
                    ImgSurface.surface.init_from = Mtl.Texture0Name;
                    ImgSurface.surface.format = "PNG";

                    ImgSampler.sid = Mtl.Name + "_samp";
                    ImgSampler.sampler2D.source = ImgSurface.sid;
                    ImgSampler.sampler2D.wrap_s = GetWrap(Mtl.TextureMappers[0].WrapU);
                    ImgSampler.sampler2D.wrap_t = GetWrap(Mtl.TextureMappers[0].WrapV);
                    ImgSampler.sampler2D.minfilter = DAEFilter.LINEAR;
                    ImgSampler.sampler2D.magfilter = DAEFilter.LINEAR;
                    ImgSampler.sampler2D.mipfilter = DAEFilter.LINEAR;

                    Effect.profile_COMMON.newparam.Add(ImgSurface);
                    Effect.profile_COMMON.newparam.Add(ImgSampler);

                    Effect.profile_COMMON.technique.sid = Mtl.Name + "_tech";
                    Effect.profile_COMMON.technique.phong.diffuse.texture.texture = ImgSampler.sid;

                    library_effects.Add(Effect);

                    DAEMaterial Material = new DAEMaterial();

                    Material.name = Mtl.Name + "_mat";
                    Material.id = Material.name + "_id";

                    Material.instance_effect.url = "#" + Effect.id;

                    library_materials.Add(Material);
                }

                //Skeleton nodes
                string RootBoneId = string.Empty;

                if ((Mdl.Skeleton?.Count ?? 0) > 0)
                {
                    Queue<Tuple<H3DBone, DAENode>> ChildBones = new Queue<Tuple<H3DBone, DAENode>>();

                    DAENode RootNode = new DAENode();

                    ChildBones.Enqueue(Tuple.Create(Mdl.Skeleton[0], RootNode));

                    RootBoneId = "#" + Mdl.Skeleton[0].Name + "_bone_id";

                    while (ChildBones.Count > 0)
                    {
                        Tuple<H3DBone, DAENode> Bone_Node = ChildBones.Dequeue();

                        H3DBone Bone = Bone_Node.Item1;

                        Bone_Node.Item2.id = Bone.Name + "_bone_id";
                        Bone_Node.Item2.name = Bone.Name;
                        Bone_Node.Item2.sid = Bone.Name;
                        Bone_Node.Item2.type = DAENodeType.JOINT;
                        Bone_Node.Item2.SetBoneEuler(Bone.Translation, Bone.Rotation, Bone.Scale);

                        foreach (H3DBone B in Mdl.Skeleton)
                        {
                            if (B.ParentIndex == -1) continue;

                            H3DBone ParentBone = Mdl.Skeleton[B.ParentIndex];

                            if (ParentBone == Bone)
                            {
                                DAENode Node = new DAENode();

                                ChildBones.Enqueue(Tuple.Create(B, Node));

                                if (Bone_Node.Item2.Nodes == null) Bone_Node.Item2.Nodes = new List<DAENode>();

                                Bone_Node.Item2.Nodes.Add(Node);
                            }
                        }
                    }

                    VN.node.Add(RootNode);
                }

                //Mesh
                for (int MeshIndex = 0; MeshIndex < Mdl.Meshes.Count; MeshIndex++)
                {
                    H3DMesh Mesh = Mdl.Meshes[MeshIndex];

                    string MtlName = string.Format("Mdl_{0:D2}_Mtl_{1}", MdlIndex, Mdl.Materials[Mesh.MaterialIndex].Name);
                    string MtlTgt = library_materials[Mesh.MaterialIndex].id;

                    for (int SMIndex = 0; SMIndex < Mesh.SubMeshes.Count; SMIndex++)
                    {
                        H3DSubMesh SM = Mesh.SubMeshes[SMIndex];

                        string ShortName = Mdl.MeshNodesTree?.Find(Mesh.NodeIndex);
                        string MeshName = string.Format("{0}_{1:D2}_{2:D2}_{3:D2}",
                            ShortName,
                            MdlIndex,
                            MeshIndex,
                            SMIndex);

                        DAEGeometry Geometry = new DAEGeometry();

                        Geometry.name = MeshName;
                        Geometry.id = Geometry.name + "_geo_id";

                        PICAVertex[] Vertices = Mesh.ToVertices(true);

                        //Geometry
                        string VertsId = MeshName + "_vtx_id";

                        Geometry.mesh.vertices.id = VertsId;
                        Geometry.mesh.triangles.material = MtlName;
                        Geometry.mesh.triangles.AddInput("VERTEX", "#" + VertsId);
                        Geometry.mesh.triangles.Set_p(SM.Indices);

                        foreach (PICAAttribute Attr in Mesh.Attributes)
                        {
                            if (Attr.Name >= PICAAttributeName.BoneIndex) continue;

                            string[] Values = new string[Vertices.Length];

                            for (int Index = 0; Index < Vertices.Length; Index++)
                            {
                                PICAVertex V = Vertices[Index];

                                switch (Attr.Name)
                                {
                                    case PICAAttributeName.Position:  Values[Index] = V.Position.ToSerializableString();  break;
                                    case PICAAttributeName.Normal:    Values[Index] = V.Normal.ToSerializableString();    break;
                                    case PICAAttributeName.Tangent:   Values[Index] = V.Tangent.ToSerializableString();   break;

                                    case PICAAttributeName.Color:     Values[Index] = V.Color.ToSerializableString();     break;

                                    case PICAAttributeName.TexCoord0: Values[Index] = V.TexCoord0.ToSerializableString(); break;
                                    case PICAAttributeName.TexCoord1: Values[Index] = V.TexCoord1.ToSerializableString(); break;
                                    case PICAAttributeName.TexCoord2: Values[Index] = V.TexCoord2.ToSerializableString(); break;
                                }
                            }

                            DAESource Source = new DAESource();

                            Source.name = MeshName + "_" + Attr.Name;
                            Source.id = Source.name + "_id";

                            Source.float_array = new DAEArray
                            {
                                id = Source.name + "_array_id",
                                count = (uint)(Vertices.Length * Attr.Elements),
                                data = string.Join(" ", Values)
                            };

                            DAEAccessor Accessor = new DAEAccessor
                            {
                                source = "#" + Source.float_array.id,
                                count = (uint)Vertices.Length,
                                stride = (uint)Attr.Elements
                            };

                            switch (Attr.Elements)
                            {
                                case 2: Accessor.AddParams("float", "S", "T");           break;
                                case 3: Accessor.AddParams("float", "X", "Y", "Z");      break;
                                case 4: Accessor.AddParams("float", "R", "G", "B", "A"); break;
                            }

                            Source.technique_common.accessor = Accessor;

                            Geometry.mesh.source.Add(Source);

                            if (Attr.Name < PICAAttributeName.TexCoord0)
                            {
                                string Semantic = string.Empty;

                                switch (Attr.Name)
                                {
                                    case PICAAttributeName.Position: Semantic = "POSITION"; break;
                                    case PICAAttributeName.Normal:   Semantic = "NORMAL";   break;
                                    case PICAAttributeName.Tangent:  Semantic = "TANGENT";  break;
                                    case PICAAttributeName.Color:    Semantic = "COLOR";    break;
                                }

                                Geometry.mesh.vertices.AddInput(Semantic, "#" + Source.id);
                            }
                            else
                            {
                                Geometry.mesh.triangles.AddInput("TEXCOORD", "#" + Source.id, 0, (uint)Attr.Name - 4);
                            }
                        } //Attributes Loop

                        library_geometries.Add(Geometry);

                        //Controller
                        DAEController Controller = new DAEController();

                        Controller.name = MeshName + "_ctrl";
                        Controller.id = Controller.name + "_id";

                        Controller.skin.source = "#" + Geometry.id;
                        Controller.skin.vertex_weights.count = (uint)Vertices.Length;

                        string[] BoneNames = new string[SM.BoneIndicesCount];
                        string[] BindPoses = new string[SM.BoneIndicesCount];

                        for (int Index = 0; Index < SM.BoneIndicesCount; Index++)
                        {
                            BoneNames[Index] = Mdl.Skeleton[SM.BoneIndices[Index]].Name;
                            BindPoses[Index] = Mdl.Skeleton[SM.BoneIndices[Index]].InverseTransform.ToSerializableString() + " 0 0 0 1";
                        }

                        StringBuilder v = new StringBuilder();
                        StringBuilder vcount = new StringBuilder();

                        Dictionary<string, int> Weights = new Dictionary<string, int>();

                        if (SM.Skinning == H3DSubMeshSkinning.Smooth)
                        {
                            foreach (PICAVertex Vertex in Vertices)
                            {
                                int Count = 0;

                                for (int Index = 0; Index < 4; Index++)
                                {
                                    float BIndex = Vertex.Indices[Index];
                                    float Weight = Vertex.Weights[Index];

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
                                v.Append(Vertex.Indices[0] + " 1 ");

                                vcount.Append("1 ");
                            }

                            Weights.Add("1", 0);
                        }

                        Controller.skin.src.Add(GetSource(Controller.name + "_names", 1, BoneNames, "JOINT", "Name"));
                        Controller.skin.src.Add(GetSource(Controller.name + "_poses", 16, BindPoses, "TRANSFORM", "float4x4"));
                        Controller.skin.src.Add(GetSource(Controller.name + "_weights", 1, Weights.Keys.ToArray(), "WEIGHT", "float"));

                        Controller.skin.joints.AddInput("JOINT", "#" + Controller.skin.src[0].id);
                        Controller.skin.joints.AddInput("INV_BIND_MATRIX", "#" + Controller.skin.src[1].id);

                        Controller.skin.vertex_weights.AddInput("JOINT", "#" + Controller.skin.src[0].id, 0);
                        Controller.skin.vertex_weights.AddInput("WEIGHT", "#" + Controller.skin.src[2].id, 1);

                        Controller.skin.vertex_weights.vcount = vcount
                            .ToString()
                            .TrimEnd();

                        Controller.skin.vertex_weights.v = v
                            .ToString()
                            .TrimEnd();

                        library_controllers.Add(Controller);

                        //Mesh node
                        DAENode Node = new DAENode();

                        Node.name = MeshName + "_node";
                        Node.id = Node.name + "_id";

                        Node.instance_controller = new DAENodeInstance();

                        Node.instance_controller.url = "#" + Controller.id;
                        Node.instance_controller.bind_material.technique_common.instance_material.symbol = MtlName;
                        Node.instance_controller.bind_material.technique_common.instance_material.target = "#" + MtlTgt;

                        if ((Mdl.Skeleton?.Count ?? 0) > 0)
                        {
                            Node.instance_controller.skeleton = "#" + VN.node[0].id;
                        }

                        VN.node.Add(Node);
                    } //SubMesh Loop
                } //Mesh Loop

                library_visual_scenes.Add(VN);

                if (library_visual_scenes.Count > 0)
                {
                    scene.instance_visual_scene.url = "#" + library_visual_scenes[0].id;
                }

                foreach (H3DTexture Tex in SceneData.Textures)
                {
                    library_images.Add(new DAEImage
                    {
                        id = Tex.Name,
                        init_from = $"./{Tex.Name}.png"
                    });
                }
            }

            if (AnimIndex != -1)
            {
                string[] AnimElemNames = { "translate", "rotateX", "rotateY", "rotateZ", "scale" };

                H3DAnimation SklAnim = SceneData.SkeletalAnimations[AnimIndex];

                PatriciaList<H3DBone> Skeleton = SceneData.Models[0].Skeleton;

                int FramesCount = (int)SklAnim.FramesCount + 1;

                foreach (H3DAnimationElement Elem in SklAnim.Elements)
                {
                    if (Elem.PrimitiveType != H3DAnimPrimitiveType.Transform &&
                        Elem.PrimitiveType != H3DAnimPrimitiveType.QuatTransform) continue;

                    H3DBone SklBone = Skeleton.FirstOrDefault(x => x.Name == Elem.Name);
                    H3DBone Parent = null;

                    if (SklBone != null && SklBone.ParentIndex != -1)
                    {
                        Parent = Skeleton[SklBone.ParentIndex];
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        string[] AnimTimes = new string[FramesCount];
                        string[] AnimPoses = new string[FramesCount];
                        string[] AnimLerps = new string[FramesCount];

                        bool IsRotation = i > 0 && i < 4; //1, 2, 3

                        if (Elem.PrimitiveType == H3DAnimPrimitiveType.Transform)
                        {
                            if (SklBone == null) break;

                            if (IsRotation)
                            {
                                if (i == 1 && !((H3DAnimTransform)Elem.Content).RotationX.HasData) i++;
                                if (i == 2 && !((H3DAnimTransform)Elem.Content).RotationY.HasData) i++;
                                if (i == 3 && !((H3DAnimTransform)Elem.Content).RotationZ.HasData) i++;

                                IsRotation = i < 4;
                            }
                        }

                        for (int Frame = 0; Frame < FramesCount; Frame++)
                        {
                            string StrTrans = string.Empty;

                            H3DAnimationElement PElem = SklAnim.Elements.FirstOrDefault(x => x.Name == Parent?.Name);

                            Vector3D InvScale = new Vector3D(1);

                            switch (Elem.PrimitiveType)
                            {
                                case H3DAnimPrimitiveType.Transform:
                                    H3DAnimTransform Transform = (H3DAnimTransform)Elem.Content;

                                    //Compensate parent bone scale (basically, don't inherit scales)
                                    if (Parent != null)
                                    {
                                        if (PElem != null)
                                        {
                                            H3DAnimTransform PTrans = (H3DAnimTransform)PElem.Content;

                                            InvScale /= new Vector3D(
                                                PTrans.ScaleX.GetFrameValue(Frame),
                                                PTrans.ScaleY.GetFrameValue(Frame),
                                                PTrans.ScaleZ.GetFrameValue(Frame));
                                        }
                                        else
                                        {
                                            InvScale /= Parent.Scale;
                                        }
                                    }

                                    switch (i)
                                    {
                                        //Translation
                                        case 0:
                                            StrTrans = new Vector3D(
                                                Transform.TranslationX.HasData //X
                                                ? Transform.TranslationX.GetFrameValue(Frame) : SklBone.Translation.X,
                                                Transform.TranslationY.HasData //Y
                                                ? Transform.TranslationY.GetFrameValue(Frame) : SklBone.Translation.Y,
                                                Transform.TranslationZ.HasData //Z
                                                ? Transform.TranslationZ.GetFrameValue(Frame) : SklBone.Translation.Z)
                                                .ToSerializableString();
                                            break;

                                        //Scale
                                        case 4:
                                            StrTrans = (InvScale * new Vector3D(
                                                Transform.ScaleX.HasData //X
                                                ? Transform.ScaleX.GetFrameValue(Frame) : SklBone.Scale.X,
                                                Transform.ScaleY.HasData //Y
                                                ? Transform.ScaleY.GetFrameValue(Frame) : SklBone.Scale.Y,
                                                Transform.ScaleZ.HasData //Z
                                                ? Transform.ScaleZ.GetFrameValue(Frame) : SklBone.Scale.Z))
                                                .ToSerializableString();
                                            break;

                                        //Rotation
                                        case 1:
                                            StrTrans = ToAngle(Transform.RotationX.GetFrameValue(Frame))
                                                .ToString(CultureInfo.InvariantCulture);
                                            break;

                                        case 2:
                                            StrTrans = ToAngle(Transform.RotationY.GetFrameValue(Frame))
                                                .ToString(CultureInfo.InvariantCulture);
                                            break;

                                        case 3:
                                            StrTrans = ToAngle(Transform.RotationZ.GetFrameValue(Frame))
                                                .ToString(CultureInfo.InvariantCulture);
                                            break;
                                    }
                                    break;

                                case H3DAnimPrimitiveType.QuatTransform:
                                    H3DAnimQuatTransform QuatTransform = (H3DAnimQuatTransform)Elem.Content;

                                    Vector3D Rotation = Vector3D.Empty;

                                    //TODO: ToEuler is expensive, ideally call it only once per bone
                                    if (IsRotation) Rotation = QuatTransform.GetRotationValue(Frame).ToEuler();

                                    //Compensate parent bone scale (basically, don't inherit scales)
                                    if (Parent != null)
                                    {
                                        if (PElem != null)
                                            InvScale /= ((H3DAnimQuatTransform)PElem.Content).GetScaleValue(Frame);
                                        else
                                            InvScale /= Parent.Scale;
                                    }

                                    switch (i)
                                    {
                                        case 0: StrTrans = QuatTransform.GetTranslationValue(Frame).ToSerializableString(); break;
                                        case 1: StrTrans = ToAngle(Rotation.X).ToString(CultureInfo.InvariantCulture); break;
                                        case 2: StrTrans = ToAngle(Rotation.Y).ToString(CultureInfo.InvariantCulture); break;
                                        case 3: StrTrans = ToAngle(Rotation.Z).ToString(CultureInfo.InvariantCulture); break;
                                        case 4: StrTrans = (InvScale * QuatTransform.GetScaleValue(Frame)).ToSerializableString(); break;
                                    }
                                    break;
                            }

                            //This is the Time in seconds, so we divide by the target FPS
                            AnimTimes[Frame] = (Frame / 30f).ToString(CultureInfo.InvariantCulture);
                            AnimPoses[Frame] = StrTrans;
                            AnimLerps[Frame] = "LINEAR";
                        }

                        DAEAnimation Anim = new DAEAnimation();

                        Anim.name =
                            SklAnim.Name + "_" +
                            Elem.Name    + "_" +
                            AnimElemNames[i];

                        Anim.id = Anim.name + "_id";

                        Anim.src.Add(GetSource(Anim.name + "_frame", 1, AnimTimes, "TIME", "float"));
                        Anim.src.Add(GetSource(Anim.name + "_interp", 1, AnimLerps, "INTERPOLATION", "Name"));

                        Anim.src.Add(IsRotation
                            ? GetSource(Anim.name + "_pose", 1, AnimPoses, "ANGLE", "float")
                            : GetSource(Anim.name + "_pose", 3, AnimPoses,
                            "X", "float",
                            "Y", "float",
                            "Z", "float"));

                        Anim.sampler.AddInput("INPUT", "#" + Anim.src[0].id);
                        Anim.sampler.AddInput("INTERPOLATION", "#" + Anim.src[1].id);
                        Anim.sampler.AddInput("OUTPUT", "#" + Anim.src[2].id);

                        Anim.sampler.id = Anim.name + "_samp_id";
                        Anim.channel.source = "#" + Anim.sampler.id;
                        Anim.channel.target = Elem.Name + "_bone_id/" + AnimElemNames[i];

                        if (IsRotation) Anim.channel.target += ".ANGLE";

                        library_animations.Add(Anim);
                    }
                }
            }
        }

        private DAESource GetSource(string Name, int Stride, string[] Elems, params string[] Accs)
        {
            DAEArray Array = new DAEArray
            {
                id    = Name + "_array_id",
                count = (uint)(Elems.Length * Stride),
                data  = string.Join(" ", Elems)
            };

            DAEAccessor Accessor = new DAEAccessor
            {
                source = "#" + Array.id,
                count  = (uint)Elems.Length,
                stride = (uint)Stride
            };

            DAESource Source = new DAESource();

            Source.name = Name;
            Source.id   = Name + "_id";

            for (int Index = 0; Index < Accs.Length; Index += 2)
            {
                Accessor.AddParam(Accs[Index + 0], Accs[Index + 1]);
            }

            Source.technique_common.accessor = Accessor;

            if (Accs[1] == "Name")
                Source.Name_array = Array;
            else
                Source.float_array = Array;

            return Source;
        }

        private DAEWrap GetWrap(H3DTextureWrap Wrap)
        {
            switch (Wrap)
            {
                case H3DTextureWrap.ClampToEdge:   return DAEWrap.CLAMP;
                case H3DTextureWrap.ClampToBorder: return DAEWrap.BORDER;
                case H3DTextureWrap.Repeat:        return DAEWrap.WRAP;
                case H3DTextureWrap.Mirror:        return DAEWrap.MIRROR;

                default: throw new ArgumentException("Invalid Texture wrap!");
            }
        }

        private float ToAngle(float Radians)
        {
            return (float)((Radians / Math.PI) * 180);
        }

        public void Save(string FileName)
        {
            using (FileStream FS = new FileStream(FileName, FileMode.Create))
            {
                XmlSerializer Serializer = new XmlSerializer(typeof(DAE));

                Serializer.Serialize(FS, this);
            }
        }
    }
}
