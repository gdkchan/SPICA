using System.Xml.Serialization;

namespace SPICA.GenericFormats.COLLADA
{
    public class DAEInput
    {
        [XmlAttribute] public string semantic;
        [XmlAttribute] public string source;
    }
}
