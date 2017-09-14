using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrGfx.LUT
{
    [TypeChoice(0x04000000u, typeof(GfxLUT))]
    public class GfxLUT : GfxObject
    {
        public readonly GfxDict<GfxLUTSampler> Samplers;

        public GfxLUT()
        {
            Samplers = new GfxDict<GfxLUTSampler>();
        }
    }
}
