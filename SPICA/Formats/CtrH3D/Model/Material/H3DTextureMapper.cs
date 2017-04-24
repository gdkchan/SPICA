using SPICA.Math3D;
using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrH3D.Model.Material
{
    public struct H3DTextureMapper
    {
        public byte SamplerType;

        public H3DTextureWrap WrapU;
        public H3DTextureWrap WrapV;

        public H3DTextureMagFilter MagFilter;
        public H3DTextureMinFilter MinFilter;

        [Padding(4)] public byte MinLOD;

        public float LODBias;

        public RGBA BorderColor;
    }
}
