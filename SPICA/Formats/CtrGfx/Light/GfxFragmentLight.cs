using SPICA.Math3D;

using System.Numerics;

namespace SPICA.Formats.CtrGfx.Light
{
    class GfxFragmentLight : GfxLight
    {
        public GfxLightType Type;

        private Vector4 AmbientColorF;
        private Vector4 DiffuseColorF;
        private Vector4 Specular0ColorF;
        private Vector4 Specular1ColorF;

        public RGBA AmbientColor;
        public RGBA DiffuseColor;
        public RGBA Specular0Color;
        public RGBA Specular1Color;

        public Vector3 Direction;

        public GfxLUTReference DistanceSampler;
        public GfxFragLightLUT AngleSampler;

        public float AttenuationStart;
        public float AttenuationEnd;

        private uint InvAttScaleF20;
        private uint AttBiasF20;

        private bool IsDirty;

        public GfxFragmentLightFlags Flags;
    }
}
