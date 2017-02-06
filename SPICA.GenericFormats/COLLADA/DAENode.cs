using SPICA.Math3D;

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SPICA.GenericFormats.COLLADA
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

        [XmlElement("node", IsNullable = false)] public List<DAENode> Nodes;

        [XmlElement(IsNullable = false)] public DAENodeInstance instance_geometry;
        [XmlElement(IsNullable = false)] public DAENodeInstance instance_controller;

        public void SetBoneEuler(Vector3D T, Vector3D R, Vector3D S)
        {
            Rotation = new DAEVector4[3];

            Translation = new DAEVector3 { sid = "translate" };
            Rotation[0] = new DAEVector4 { sid = "rotateZ" };
            Rotation[1] = new DAEVector4 { sid = "rotateY" };
            Rotation[2] = new DAEVector4 { sid = "rotateX" };
            Scale       = new DAEVector3 { sid = "scale" };

            Translation.Set(T);
            Rotation[0].Set(new Vector4D(0, 0, 1, ToAngle(R.Z)));
            Rotation[1].Set(new Vector4D(0, 1, 0, ToAngle(R.Y)));
            Rotation[2].Set(new Vector4D(1, 0, 0, ToAngle(R.X)));
            Scale.Set(S);
        }

        private float ToAngle(float Radians)
        {
            return (float)((Radians / Math.PI) * 180);
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
