using System.Xml.Serialization;

namespace SPICA.GenericFormats.COLLADA
{
    public class DAEController
    {
        [XmlAttribute] public string id;
        [XmlAttribute] public string name;

        public DAESkin skin = new DAESkin();
    }
}
