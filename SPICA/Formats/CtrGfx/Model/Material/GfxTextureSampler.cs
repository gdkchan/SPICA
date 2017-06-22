using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrGfx.Model.Material
{
    [TypeChoice(0x80000000u, typeof(GfxTextureSamplerStd))]
    public class GfxTextureSampler
    {
        public GfxTextureMapper Parent;

        public GfxTextureMinFilter MinFilter;
    }
}
