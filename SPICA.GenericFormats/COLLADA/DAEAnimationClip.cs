using System.Collections.Generic;
using System.Xml.Serialization;

namespace SPICA.GenericFormats.COLLADA
{
    public class DAEAnimationClip
    {
        [XmlAttribute] public string id;
        [XmlAttribute] public string name;

        [XmlAttribute] public float start;
        [XmlAttribute] public float end;

        [XmlElement("instance_animation")] public List<DAEInstanceAnimation> instance_animation = new List<DAEInstanceAnimation>();
    }

    public class DAEInstanceAnimation
    {
        [XmlAttribute] public string url;

        public DAEInstanceAnimation() { }

        public DAEInstanceAnimation(string url)
        {
            this.url = url;
        }
    }
}
