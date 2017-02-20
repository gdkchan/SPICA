using SPICA.PICA.Converters;

using System.Collections.Generic;

namespace SPICA.Formats.Generic.WavefrontOBJ
{
    class OBJMesh
    {
        public List<PICAVertex> Vertices;
        public List<ushort> Indices;

        public OBJMesh()
        {
            Vertices = new List<PICAVertex>();
            Indices = new List<ushort>();
        }
    }
}
