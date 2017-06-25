using System.Collections.Generic;
using System.Xml.Serialization;

namespace SPICA.Formats.Generic.COLLADA
{
    public class DAESkin
    {
        [XmlAttribute] public string source;

        public DAEMatrix bind_shape_matrix = DAEMatrix.Identity;

        [XmlElement("source")] public List<DAESource> src = new List<DAESource>();

        public DAEJoints  joints         = new DAEJoints();
        public DAEWeights vertex_weights = new DAEWeights();
    }

    public class DAEJoints
    {
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

    public class DAEWeights
    {
        [XmlAttribute] public uint count;

        [XmlElement("input")] public List<DAEInputOffset> input = new List<DAEInputOffset>();

        public string vcount;
        public string v;

        public void AddInput(string semantic, string source, uint offset = 0)
        {
            input.Add(new DAEInputOffset()
            {
                semantic = semantic,
                source   = source,
                offset   = offset
            });
        }
    }
}
