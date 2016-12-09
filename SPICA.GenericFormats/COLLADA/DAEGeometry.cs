using System.Xml.Serialization;

namespace SPICA.GenericFormats.COLLADA
{
    public class DAEGeometry
    {
        [XmlAttribute] public string id;
        [XmlAttribute] public string name;

        public DAEMesh mesh = new DAEMesh();
    }
}
