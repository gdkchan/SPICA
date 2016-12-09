using System.Xml.Serialization;

namespace SPICA.GenericFormats.COLLADA
{
    public class DAEArray
    {
        [XmlAttribute] public string id;

        [XmlAttribute] public uint count;

        [XmlText] public string data;
    }
}
