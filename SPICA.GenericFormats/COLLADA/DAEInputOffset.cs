using System.Xml.Serialization;

namespace SPICA.GenericFormats.COLLADA
{
    public class DAEInputOffset
    {
        [XmlAttribute] public string semantic;
        [XmlAttribute] public string source;
        [XmlAttribute] public uint offset;
        [XmlAttribute] public uint set;

        public bool ShouldSerializeset() { return semantic == "TEXCOORD" || set != 0; }
    }
}
