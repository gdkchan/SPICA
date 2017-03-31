using SPICA.PICA.Commands;

using System;

namespace SPICA.Formats.MTFramework.Texture
{
    public enum MTTextureFormat
    {
        RGBA8 = 3,
        ETC1 = 0xb,
        ETC1A4 = 0xc
    }

    public static class MTTextureFormatExtensions
    {
        public static PICATextureFormat ToPICATextureFormat(this MTTextureFormat Format)
        {
            switch (Format)
            {
                case MTTextureFormat.RGBA8: return PICATextureFormat.RGBA8;
                case MTTextureFormat.ETC1: return PICATextureFormat.ETC1;
                case MTTextureFormat.ETC1A4: return PICATextureFormat.ETC1A4;

                default: throw new NotImplementedException($"Unimplemented format {Format}!");
            }
        }
    }
}
