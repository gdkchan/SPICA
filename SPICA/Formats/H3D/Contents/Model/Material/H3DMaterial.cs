using SPICA.Formats.H3D.Contents.Model.Texture;

using SPICA.Serialization.BinaryAttributes;

namespace SPICA.Formats.H3D.Contents.Model.Material
{
    class H3DMaterial
    {
        [PointerOf("MaterialParams")]
        private uint MaterialParamsAddress;

        private uint Texture0ConfigAddress;
        private uint Texture1ConfigAddress;
        private uint Texture2ConfigAddress;

        [PointerOf("TextureCommands")]
        private uint TextureCommandsAddress;

        [CountOf("TextureCommands")]
        private uint TextureCommandsCount;

        [PointerOf("TextureMappers")]
        private uint TextureMapperAddress;

        [PointerOf("TextureNames")]
        private uint[] TextureNamesAddress;

        [PointerOf("MaterialName")]
        private uint MaterialNameAddress;

        [TargetSection("CommandsSection")]
        public uint[] TextureCommands;

        [TargetSection("StringsSection"), FixedCount(3)]
        public string[] TextureNames;

        [TargetSection("StringsSection")]
        public string MaterialName;

        [TargetSection("DescriptorsSection", 2)]
        public H3DMaterialParams MaterialParams;

        [TargetSection("DescriptorsSection", 2), FixedCount(3)]
        public H3DTextureMapper[] TextureMappers;

        [TargetSection("DescriptorsSection", 2), Section("MetaDataSection")]
        private byte[] MetaData;
    }
}
