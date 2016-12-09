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

        public DAEMatrix matrix = DAEMatrix.Identity;

        [XmlElement("node", IsNullable = false)] public List<DAENode> Nodes;

        [XmlElement(IsNullable = false)] public DAENodeInstance instance_geometry;
        [XmlElement(IsNullable = false)] public DAENodeInstance instance_controller;
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
