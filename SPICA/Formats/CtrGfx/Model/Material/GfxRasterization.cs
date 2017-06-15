using SPICA.PICA.Commands;
using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrGfx.Model.Material
{
    public struct GfxRasterization
    {
        private uint PolygonOffsetEnabled;

        public bool IsPolygonOffsetEnabled
        {
            get
            {
                return PolygonOffsetEnabled != 0;
            }
            set
            {
                PolygonOffsetEnabled = value ? 1u : 0u;
            }
        }

        public GfxFaceCulling FaceCulling;

        public float PolygonOffsetUnit;

        [Inline, FixedLength(2)] private uint[] FaceCullingCommand;
    }
}
