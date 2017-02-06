using SPICA.PICA.Converters;

using System.Collections.Generic;

namespace SPICA.GenericFormats.StudioMdl
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
