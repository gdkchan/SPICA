using System;

namespace SPICA.Formats.CtrH3D.Light
{
    [Flags]
    public enum H3DLightType : byte
    {
        Hemisphere    = 1,
        Ambient       = 2,
        Vertex        = 4,
        Fragment      = 8,
        VertexDir     = Vertex | 1,
        VertexPoint   = Vertex | 2,
        VertexSpot    = Vertex | 3,
        FragmentDir   = Fragment | 1,
        FragmentPoint = Fragment | 2,
        FragmentSpot  = Fragment | 3
    }
}
