using SPICA.PICA.Commands;

using System;

namespace SPICA.Formats.CtrGfx.Model.Mesh
{
    public enum GfxGLDataType : uint
    {
        GL_BYTE           = 0x1400,
        GL_UNSIGNED_BYTE  = 0x1401,
        GL_SHORT          = 0x1402,
        GL_UNSIGNED_SHORT = 0x1403,
        GL_FLOAT          = 0x1406,
        GL_FIXED          = 0x140C
    }

    static class GfxGLDataTypeExtensions
    {
        public static PICAAttributeFormat ToPICAAttributeFormat(this GfxGLDataType Format)
        {
            switch (Format)
            {
                case GfxGLDataType.GL_BYTE:          return PICAAttributeFormat.Byte;
                case GfxGLDataType.GL_UNSIGNED_BYTE: return PICAAttributeFormat.Ubyte;
                case GfxGLDataType.GL_SHORT:         return PICAAttributeFormat.Short;
                case GfxGLDataType.GL_FLOAT:         return PICAAttributeFormat.Float;

                default: throw new ArgumentException($"Invalid format {Format}!");
            }
        }
    }
}
