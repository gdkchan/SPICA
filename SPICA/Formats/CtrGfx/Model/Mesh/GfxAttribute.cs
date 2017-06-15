using SPICA.PICA.Commands;

namespace SPICA.Formats.CtrGfx.Model.Mesh
{
    public class GfxAttribute
    {
        public uint Flags;

        public PICAAttributeName AttrName;

        public uint StreamFlags;

        public uint BufferObject;
        public uint LocationFlags;

        public uint StreamLength;
        public uint StreamOffset;

        public uint LocationAddress;
        public uint MemoryArea;

        public GfxAttributeFormat Format;

        public int Elements;

        public float Scale;

        public int Offset;
    }
}
