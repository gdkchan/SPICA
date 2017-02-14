using System.Xml.Serialization;

namespace SPICA.Formats.Generic.COLLADA
{
    public class DAEImage
    {
        [XmlAttribute] public string id;
        [XmlAttribute] public string name;

        public string init_from;
    }
}
