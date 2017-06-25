using SPICA.Formats.Common;
using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrGfx
{
    [Inline]
    class GfxHeader
    {
        public uint   MagicNumber;
        public ushort ByteOrderMark;
        public ushort HeaderLength;
        public uint   Revision;
        public int    FileLength;
        public int    SectionsCount;

        public GfxSectionHeader Data;

        public GfxHeader()
        {
            MagicNumber   = IOUtils.ToUInt32("CGFX");
            ByteOrderMark = 0xfeff;
            HeaderLength  = 0x14;
            Revision      = 0x05000000u;
            FileLength    = 0;
            SectionsCount = 0;

            Data = new GfxSectionHeader("DATA");
        }
    }
}
