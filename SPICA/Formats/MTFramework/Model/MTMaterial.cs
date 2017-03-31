using SPICA.Formats.MTFramework.Shader;

namespace SPICA.Formats.MTFramework.Model
{
    struct MTMaterial
    {
        public uint NameHash;

        public MTAlphaBlendConfig AlphaBlend;
        public MTDepthStencilConfig DepthStencil;

        public string Texture0Name;
        public string Name;
    }
}
