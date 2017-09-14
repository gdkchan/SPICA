using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrGfx
{
    public struct GfxRevHeader
    {
        public uint MagicNumber;

        [Version]
        public uint Revision;
    }
}
