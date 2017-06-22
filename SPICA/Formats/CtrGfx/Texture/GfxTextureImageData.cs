using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrGfx.Texture
{
    public class GfxTextureImageData
    {
        public int Height;
        public int Width;

        [Section((uint)GfxSectionId.Image)] public byte[] RawBuffer;

        private uint DynamicAlloc;

        public int BitsPerPixel;

        private uint LocationPtr;
        private uint MemoryArea;
    }
}
