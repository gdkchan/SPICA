using SPICA.Formats.Common;

namespace SPICA.Formats.CtrGfx
{
    struct GfxSectionHeader
    {
        public uint Magic;
        public int  Length;

        public GfxSectionHeader(string Magic, int Length = 0)
        {
            this.Magic  = IOUtils.ToUInt32(Magic);
            this.Length = Length;
        }
    }
}
