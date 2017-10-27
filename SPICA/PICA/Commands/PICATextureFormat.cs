using SPICA.Formats.GFL2.Texture;
using System;

namespace SPICA.PICA.Commands
{
    public enum PICATextureFormat : uint
    {
        RGBA8,
        RGB8,
        RGBA5551,
        RGB565,
        RGBA4,
        LA8,
        HiLo8,
        L8,
        A8,
        LA4,
        L4,
        A4,
        ETC1,
        ETC1A4
    }

	public static class H3DTextureFormatExtensions {
		public static GFTextureFormat ToGFTextureFormat(this PICATextureFormat Format) {
			switch (Format) {
				case PICATextureFormat.RGB565: return GFTextureFormat.RGB565;
				case PICATextureFormat.RGB8: return GFTextureFormat.RGB8;
				case PICATextureFormat.RGBA8: return GFTextureFormat.RGBA8;
				case PICATextureFormat.RGBA4: return GFTextureFormat.RGBA4;
				case PICATextureFormat.RGBA5551: return GFTextureFormat.RGBA5551;
				case PICATextureFormat.LA8: return GFTextureFormat.LA8;
				case PICATextureFormat.HiLo8: return GFTextureFormat.HiLo8;
				case PICATextureFormat.L8: return GFTextureFormat.L8;
				case PICATextureFormat.A8: return GFTextureFormat.A8;
				case PICATextureFormat.LA4: return GFTextureFormat.LA4;
				case PICATextureFormat.L4: return GFTextureFormat.L4;
				case PICATextureFormat.A4: return GFTextureFormat.A4;
				case PICATextureFormat.ETC1: return GFTextureFormat.ETC1;
				case PICATextureFormat.ETC1A4: return GFTextureFormat.ETC1A4;

				default: throw new ArgumentException("Invalid format!");
			}
		}
	}
}
