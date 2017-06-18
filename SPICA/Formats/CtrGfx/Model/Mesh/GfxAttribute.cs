namespace SPICA.Formats.CtrGfx.Model.Mesh
{
    public class GfxAttribute : GfxVertexBuffer
    {
        private uint BufferObj;
        private uint LocationFlag;

        private uint StreamLength;
        private uint StreamOffset;

        private uint LocationPtr;
        private uint MemoryArea;

        public GfxGLDataType Format;

        public int Elements;

        public float Scale;

        public int Offset;
    }
}
