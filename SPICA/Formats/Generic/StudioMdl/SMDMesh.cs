using SPICA.PICA.Converters;

using System.Collections.Generic;

namespace SPICA.Formats.Generic.StudioMdl
{
    class SMDMesh
    {
        public string MaterialName;

        public List<PICAVertex> Vertices;

        public SMDMesh()
        {
            Vertices = new List<PICAVertex>();
        }
    }
}
