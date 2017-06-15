namespace SPICA.Formats.CtrGfx.Texture
{
    public class GfxTextureImage
    {
        public int Height;
        public int Width;

        public byte[] RawBuffer;

        private uint DynamicAllocPtr;

        public int BitsPerPixel;

        private uint LocationPtr;
        private uint MemoryArea;
    }
}
