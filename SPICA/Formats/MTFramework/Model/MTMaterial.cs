using SPICA.Formats.MTFramework.Shader;

namespace SPICA.Formats.MTFramework.Model
{
    class MTMaterial
    {
        public uint NameHash;

        public MTAlphaBlend   AlphaBlend;
        public MTDepthStencil DepthStencil;

        public string Texture0Name;
        public string Name;
    }
}
