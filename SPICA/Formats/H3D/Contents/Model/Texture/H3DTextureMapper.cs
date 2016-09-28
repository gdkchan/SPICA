using SPICA.Math;

namespace SPICA.Formats.H3D.Contents.Model.Texture
{
    struct H3DTextureMapper
    {
        public byte SamplerType;
        public H3DTextureWrap WrapU;
        public H3DTextureWrap WrapV;
        public H3DTextureMagFilter MagFilter;
        public H3DTextureMinFilter MinFilter;
        public byte MinLOD;
        private ushort Padding;
        public float LODBias;
        public RGBA BorderColor;
    }
}
