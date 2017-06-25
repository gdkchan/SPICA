using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace SPICA.Formats.Generic.COLLADA
{
    public class DAEMesh
    {
        [XmlElement("source")] public List<DAESource> source = new List<DAESource>();

        public DAEVertices vertices = new DAEVertices();
        public DAETriangles triangles = new DAETriangles();
    }

    public class DAEVertices
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

    public class DAETriangles
    {
        [XmlAttribute] public string material;

        [XmlAttribute] public uint count;

        [XmlElement("input")] public List<DAEInputOffset> input = new List<DAEInputOffset>();

        public string p;

        public void AddInput(string semantic, string source, uint offset = 0, uint set = 0)
        {
            input.Add(new DAEInputOffset()
            {
                semantic = semantic,
                source   = source,
                offset   = offset,
                set      = set
            });
        }

        public void Set_p(ushort[] Indices)
        {
            StringBuilder SB = new StringBuilder();

            for (int i = 0; i < Indices.Length; i++)
            {
                if (i < Indices.Length - 1)
                    SB.Append($"{Indices[i].ToString(CultureInfo.InvariantCulture)} ");
                else
                    SB.Append(Indices[i].ToString(CultureInfo.InvariantCulture));
            }

            count = (uint)(Indices.Length / 3);

            p = SB.ToString();
        }
    }
}
