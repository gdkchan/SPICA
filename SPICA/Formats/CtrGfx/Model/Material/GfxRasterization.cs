using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrGfx.Model.Material
{
    public struct GfxRasterization
    {
        public bool IsPolygonOffsetEnabled;

        public GfxFaceCulling FaceCulling;

        public float PolygonOffsetUnit;

        [Inline, FixedLength(2)] private uint[] FaceCullingCommand;
    }
}
