using System.Numerics;

namespace SPICA.Formats.CtrGfx.Light
{
    public class GfxHemisphereLight : GfxLight
    {
        public Vector4 GroundColor;
        public Vector4 SkyColor;

        public Vector3 Direction;

        public float LerpFactor;
    }
}
