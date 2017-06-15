using SPICA.PICA.Commands;

namespace SPICA.Formats.CtrGfx.Model.Mesh
{
    public enum GfxAttributeFormat : uint
    {
        GL_BYTE           = 0x1400,
        GL_UNSIGNED_BYTE  = 0x1401,
        GL_SHORT          = 0x1402,
        GL_UNSIGNED_SHORT = 0x1403,
        GL_FLOAT          = 0x1406,
        GL_FIXED          = 0x140C
    }

    static class GfxAttributeFormatExtensions
    {
        public static PICAAttributeFormat ToPICAAttributeFormat(this GfxAttributeFormat Format)
        {
            switch (Format)
            {
                case GfxAttributeFormat.GL_BYTE:          return PICAAttributeFormat.Byte;
                case GfxAttributeFormat.GL_UNSIGNED_BYTE: return PICAAttributeFormat.Ubyte;
                case GfxAttributeFormat.GL_SHORT:         return PICAAttributeFormat.Short;
                case GfxAttributeFormat.GL_FLOAT:         return PICAAttributeFormat.Float;
            }

            return 0;
        }
    }
}
