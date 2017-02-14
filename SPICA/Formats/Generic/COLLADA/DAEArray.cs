using System.Xml.Serialization;

namespace SPICA.Formats.Generic.COLLADA
{
    public class DAEArray
    {
        [XmlAttribute] public string id;

        [XmlAttribute] public uint count;

        [XmlText] public string data;
    }
}
