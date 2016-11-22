using SPICA.PICA.Commands;
using System;

namespace SPICA.Formats.GFL2.Texture
{
    public enum GFTextureFormat : ushort
    {
        RGB565 = 0x2,
        RGB8 = 0x3,
        RGBA8 = 0x4,
        RGBA4 = 0x16,
        RGBA5551 = 0x17,
        LA8 = 0x23,
        HiLo8 = 0x24,
        L8 = 0x25,
        A8 = 0x26,
        LA4 = 0x27,
        L4 = 0x28,
        A4 = 0x29,
        ETC1 = 0x2a,
        ETC1A4 = 0x2b
    }

    public static class GFTextureFormatExtensions
    {
        public static PICATextureFormat ToPICATextureFormat(this GFTextureFormat Format)
        {
            switch (Format)
            {
                case GFTextureFormat.RGB565:   return PICATextureFormat.RGB565;
                case GFTextureFormat.RGB8:     return PICATextureFormat.RGB8;
                case GFTextureFormat.RGBA8:    return PICATextureFormat.RGBA8;
                case GFTextureFormat.RGBA4:    return PICATextureFormat.RGBA4;
                case GFTextureFormat.RGBA5551: return PICATextureFormat.RGBA5551;
                case GFTextureFormat.LA8:      return PICATextureFormat.LA8;
                case GFTextureFormat.HiLo8:    return PICATextureFormat.HiLo8;
                case GFTextureFormat.L8:       return PICATextureFormat.L8;
                case GFTextureFormat.A8:       return PICATextureFormat.A8;
                case GFTextureFormat.LA4:      return PICATextureFormat.LA4;
                case GFTextureFormat.L4:       return PICATextureFormat.L4;
                case GFTextureFormat.A4:       return PICATextureFormat.A4;
                case GFTextureFormat.ETC1:     return PICATextureFormat.ETC1;
                case GFTextureFormat.ETC1A4:   return PICATextureFormat.ETC1A4;

                default: throw new ArgumentException("Invalid format!");
            }
        }
    }
}
