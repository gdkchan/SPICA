using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrGfx.Model.Material
{
    public class GfxTextureMapper
    {
        private uint Unk;

        private uint DynamicAllocPtr;

        public GfxTextureReference Texture;

        public readonly GfxTextureSampler Sampler;

        [Inline, FixedLength(14)] private uint[] Commands;
    }
}
