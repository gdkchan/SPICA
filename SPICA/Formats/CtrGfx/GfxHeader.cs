using SPICA.Formats.Common;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrGfx
{
    [Inline]
    class GfxHeader
    {
        public uint   MagicNumber;
        public ushort ByteOrderMark;
        public ushort HeaderLength;

        [Version]
        public uint Revision;
        public int  FileLength;
        public int  SectionsCount;

        //TODO: Version 1.0.0.0 is unsupported.
        [IfVersion(CmpOp.Gequal, 0x02000000)]
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
