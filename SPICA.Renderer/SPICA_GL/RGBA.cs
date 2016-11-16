using OpenTK.Graphics;

using SPICA.Math3D;
using SPICA.PICA.Commands;

namespace SPICA.Renderer.SPICA_GL
{
    static class RGBAExtensions
    {
        public static Color4 ToColor4(this RGBA Color)
        {
            return new Color4(Color.R, Color.G, Color.B, Color.A);
        }

        public static Color4 ToColor4(this RGBAFloat Color)
        {
            return new Color4(Color.R, Color.G, Color.B, Color.A);
        }

        public static Color4 ToColor4(this PICATexEnvColor Color)
        {
            return new Color4(Color.R, Color.G, Color.B, Color.A);
        }
    }
}
