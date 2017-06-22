using SPICA.Formats.Common;

namespace SPICA.Formats.CtrGfx
{
    struct GfxSectionHeader
    {
        public uint MagicNumber;
        public int  Length;

        public GfxSectionHeader(string Magic)
        {
            MagicNumber = IOUtils.ToUInt32(Magic);
            Length      = 0;
        }
    }
}
