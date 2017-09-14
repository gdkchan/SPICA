using System.Numerics;

namespace SPICA.Formats.CtrGfx.Light
{
    public class GfxVertexLight : GfxLight
    {
        public GfxLightType Type;

        public Vector4 AmbientColor;
        public Vector4 DiffuseColor;

        public Vector3 Direction;

        public float AttenuationConstant;
        public float AttenuationLinear;
        public float AttenuationQuadratic;
        public float AttenuationEnabled;

        public float SpotExponent;
        public float SpotCutOffAngle;

        public GfxVertexLightFlags Flags;
    }
}
