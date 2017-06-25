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
            ByteOrderMark = GfxConstants.ByteOrderMark;
            HeaderLength  = GfxConstants.GfxHeaderLength;
            Revision      = GfxConstants.CGFXRevision;
            FileLength    = 0;
            SectionsCount = 0;

            Data = new GfxSectionHeader("DATA");
        }
    }
}
