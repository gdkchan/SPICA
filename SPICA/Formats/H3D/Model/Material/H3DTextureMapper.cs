using SPICA.Math3D;

namespace SPICA.Formats.H3D.Model.Material
{
    class H3DTextureMapper
    {
        public byte SamplerType;

        public H3DTextureWrap WrapU;
        public H3DTextureWrap WrapV;

        public H3DTextureMagFilter MagFilter;
        public H3DTextureMinFilter MinFilter;

        public byte MinLOD;
        public ushort Padding;
        public float LODBias;

        public RGBA BorderColor;
    }
}
