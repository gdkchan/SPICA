using System.Collections.Generic;
using System.Xml.Serialization;

namespace SPICA.Formats.Generic.COLLADA
{
    public class DAEVisualScene
    {
        [XmlAttribute] public string id;
        [XmlAttribute] public string name;

        [XmlElement("node")] public List<DAENode> node = new List<DAENode>();
    }

    public class DAEScene
    {
        public DAEInstanceVisualScene instance_visual_scene = new DAEInstanceVisualScene();
    }

    public class DAEInstanceVisualScene
    {
        [XmlAttribute] public string url;
    }
}
