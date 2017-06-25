using System.Collections.Generic;
using System.Xml.Serialization;

namespace SPICA.Formats.Generic.COLLADA
{
    public class DAEAccessor
    {
        [XmlAttribute] public string source;

        [XmlAttribute] public uint count;
        [XmlAttribute] public uint stride;

        [XmlElement("param")] public List<DAEAccessorParam> param = new List<DAEAccessorParam>();

        public void AddParam(string name, string type)
        {
            param.Add(new DAEAccessorParam()
            {
                name = name,
                type = type
            });
        }

        public void AddParams(string type, params string[] names)
        {
            foreach (string name in names)
            {
                AddParam(name, type);
            }
        }
    }

    public class DAEAccessorParam
    {
        [XmlAttribute] public string name;
        [XmlAttribute] public string type;
    }
}
