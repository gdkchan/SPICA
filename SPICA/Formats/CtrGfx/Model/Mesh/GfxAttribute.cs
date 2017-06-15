using SPICA.Formats.Common;
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

        private uint FormatFlags;

        public PICAAttributeFormat Format
        {
            get
            {
                return (PICAAttributeFormat)BitUtils.GetBits(FormatFlags, 0, 4);
            }
            set
            {
                FormatFlags = BitUtils.SetBits(FormatFlags, (uint)value, 0, 4);
            }
        }

        public int Elements;

        public float Scale;

        public int Offset;
    }
}
