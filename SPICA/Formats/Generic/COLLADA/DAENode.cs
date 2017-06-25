using System.Collections.Generic;
using System.Numerics;
using System.Xml.Serialization;

namespace SPICA.Formats.Generic.COLLADA
{
    public class DAENode
    {
        [XmlAttribute] public string id;
        [XmlAttribute] public string name;
        [XmlAttribute] public string sid;
        [XmlAttribute] public DAENodeType type = DAENodeType.NODE;

        public DAEMatrix matrix;

        [XmlElement("translate")] public DAEVector3   Translation;
        [XmlElement("rotate")]    public DAEVector4[] Rotation;
        [XmlElement("scale")]     public DAEVector3   Scale;

        [XmlElement("node")] public List<DAENode> Nodes;

        public DAENodeInstance instance_geometry;
        public DAENodeInstance instance_controller;

        public void SetBoneEuler(Vector3 T, Vector3 R, Vector3 S)
        {
            Rotation = new DAEVector4[3];

            Translation = new DAEVector3() { sid = "translate" };
            Rotation[0] = new DAEVector4() { sid = "rotateZ" };
            Rotation[1] = new DAEVector4() { sid = "rotateY" };
            Rotation[2] = new DAEVector4() { sid = "rotateX" };
            Scale       = new DAEVector3() { sid = "scale" };

            Translation.Set(T);
            Rotation[0].Set(new Vector4(0, 0, 1, DAEUtils.RadToDeg(R.Z)));
            Rotation[1].Set(new Vector4(0, 1, 0, DAEUtils.RadToDeg(R.Y)));
            Rotation[2].Set(new Vector4(1, 0, 0, DAEUtils.RadToDeg(R.X)));
            Scale.Set(S);
        }
    }

    public enum DAENodeType
    {
        NODE,
        JOINT
    }

    public class DAENodeInstance
    {
        [XmlAttribute] public string url;

        public string skeleton;

        public DAEBindMaterialTechniqueCommon bind_material = new DAEBindMaterialTechniqueCommon();
    }

    public class DAEBindMaterialTechniqueCommon
    {
        public DAEBindMaterial technique_common = new DAEBindMaterial();
    }

    public class DAEBindMaterial
    {
        public DAEBindInstanceMaterial instance_material = new DAEBindInstanceMaterial();
    }

    public class DAEBindInstanceMaterial
    {
        [XmlAttribute] public string symbol;
        [XmlAttribute] public string target;
    }
}
