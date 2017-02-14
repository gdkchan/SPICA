using System.Xml.Serialization;

namespace SPICA.Formats.Generic.COLLADA
{
    public class DAEInput
    {
        [XmlAttribute] public string semantic;
        [XmlAttribute] public string source;
    }
}
