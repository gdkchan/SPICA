using SPICA.Math3D;

using System.Numerics;

namespace SPICA.Formats.CtrGfx.Model.Material
{
    public struct GfxMaterialColor
    {
        private Vector4 EmissionF;
        private Vector4 AmbientF;
        private Vector4 DiffuseF;
        private Vector4 Specular0F;
        private Vector4 Specular1F;
        private Vector4 Constant0F;
        private Vector4 Constant1F;
        private Vector4 Constant2F;
        private Vector4 Constant3F;
        private Vector4 Constant4F;
        private Vector4 Constant5F;

        public RGBA Emission;
        public RGBA Ambient;
        public RGBA Diffuse;
        public RGBA Specular0;
        public RGBA Specular1;
        public RGBA Constant0;
        public RGBA Constant1;
        public RGBA Constant2;
        public RGBA Constant3;
        public RGBA Constant4;
        public RGBA Constant5;

        private uint CommandCache;

        public float Scale
        {
            get
            {
                return AmbientF.W;
            }
            set
            {
                AmbientF.W = value;
            }
        }
    }
}
