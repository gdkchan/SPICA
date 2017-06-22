using SPICA.Formats.Common;

namespace SPICA.Formats.CtrGfx
{
    struct GfxHeader
    {
        public uint   MagicNumber;
        public ushort ByteOrderMark;
        public ushort HeaderLength;
        public uint   Revision;
        public int    FileLength;
        public int    UsedSectionsCount;

        public GfxSectionHeader Data;

        public GfxHeader(int Length, int SectionsCount)
        {
            MagicNumber       = IOUtils.ToUInt32("CGFX");
            ByteOrderMark     = 0xfeff;
            HeaderLength      = 0x14;
            Revision          = 0x05000000u;
            FileLength        = Length;
            UsedSectionsCount = SectionsCount;

            Data = new GfxSectionHeader("DATA");
        }
    }
}
