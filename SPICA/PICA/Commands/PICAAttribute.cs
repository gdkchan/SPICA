using System.Xml.Serialization;

namespace SPICA.PICA.Commands
{
    public struct PICAAttribute
    {
        [XmlAttribute] public PICAAttributeName Name;
        [XmlAttribute] public PICAAttributeFormat Format;
        [XmlAttribute] public int Elements;
        [XmlAttribute] public float Scale;
    }
}
