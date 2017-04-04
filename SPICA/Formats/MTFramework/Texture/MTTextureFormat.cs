using SPICA.PICA.Commands;

using System;

namespace SPICA.Formats.MTFramework.Texture
{
    public enum MTTextureFormat
    {
        RGBA8  = 0x3,
        ETC1   = 0xb,
        ETC1A4 = 0xc,
        RGB8   = 0x11
    }

    public static class MTTextureFormatExtensions
    {
        public static PICATextureFormat ToPICATextureFormat(this MTTextureFormat Format)
        {
            switch (Format)
            {
                case MTTextureFormat.RGBA8:  return PICATextureFormat.RGBA8;
                case MTTextureFormat.ETC1:   return PICATextureFormat.ETC1;
                case MTTextureFormat.ETC1A4: return PICATextureFormat.ETC1A4;
                case MTTextureFormat.RGB8:   return PICATextureFormat.RGB8;

                default: throw new NotImplementedException($"Unimplemented format {Format}!");
            }
        }
    }
}
