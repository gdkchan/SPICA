using System.Xml.Serialization;

namespace SPICA.GenericFormats.COLLADA
{
    public class DAESource
    {
        [XmlAttribute] public string id;
        [XmlAttribute] public string name;

        [XmlElement(IsNullable = false)] public DAEArray Name_array;
        [XmlElement(IsNullable = false)] public DAEArray float_array;

        public DAESourceTechniqueCommon technique_common = new DAESourceTechniqueCommon();
    }

    public class DAESourceTechniqueCommon
    {
        public DAEAccessor accessor = new DAEAccessor();
    }
}
