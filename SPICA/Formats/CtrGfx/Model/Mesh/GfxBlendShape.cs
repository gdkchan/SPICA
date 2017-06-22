using SPICA.Serialization.Attributes;

using System.Collections.Generic;

namespace SPICA.Formats.CtrGfx.Model.Mesh
{
    public class GfxBlendShapeTarget
    {
        //TODO
    }

    [TypeChoice(0x00000000u, typeof(GfxBlendShape))]
    public class GfxBlendShape
    {
        public List<GfxBlendShapeTarget> Targets;

        public List<GfxVertexBufferType> Types;
    }
}
