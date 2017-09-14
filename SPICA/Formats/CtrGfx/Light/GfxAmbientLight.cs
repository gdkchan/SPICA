using SPICA.Math3D;

using System.Numerics;

namespace SPICA.Formats.CtrGfx.Light
{
    public class GfxAmbientLight : GfxLight
    {
        private Vector4 ColorF;

        public RGBA Color;

        private bool IsDirty;
    }
}
