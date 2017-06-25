using System.Collections.Generic;
using System.Xml.Serialization;

namespace SPICA.Formats.Generic.COLLADA
{
    public class DAEAnimation
    {
        [XmlAttribute] public string id;
        [XmlAttribute] public string name;

        [XmlElement("source")] public List<DAESource> src = new List<DAESource>();

        public DAESamplers sampler = new DAESamplers();
        public DAEChannel  channel = new DAEChannel();
    }

    public class DAESamplers
    {
        [XmlAttribute] public string id;

        [XmlElement("input")] public List<DAEInput> input = new List<DAEInput>();

        public void AddInput(string semantic, string source)
        {
            input.Add(new DAEInput()
            {
                semantic = semantic,
                source   = source
            });
        }
    }

    public class DAEChannel
    {
        [XmlAttribute] public string source;
        [XmlAttribute] public string target;
    }
}
