using SPICA.PICA.Commands;
using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrGfx.Texture
{
    [TypeChoice(0x20000009u, typeof(GfxTextureCube))]
    [TypeChoice(0x20000011u, typeof(GfxTextureImage))]
    public class GfxTexture : GfxObject
    {
        public int Height;
        public int Width;

        public uint GLFormat;
        public uint GLType;

        public int MipmapSize;

        private uint TextureObj;
        private uint LocationFlag;

        public PICATextureFormat HwFormat;
    }
}
