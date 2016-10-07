using SPICA.Serialization.Attributes;

namespace SPICA.Formats.H3D.Model.Material
{
    class H3DMaterial
    {
        public H3DMaterialParams MaterialParams;

        public uint Texture0ConfigAddress;
        public uint Texture1ConfigAddress;
        public uint Texture2ConfigAddress;
        public uint[] TextureCommands;

        [FixedLength(3)]
        public H3DTextureMapper[] TextureMappers;

        public string Texture0Name;
        public string Texture1Name;
        public string Texture2Name;
        public string Name;
    }
}
