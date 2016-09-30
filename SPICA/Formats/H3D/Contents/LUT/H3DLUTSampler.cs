using SPICA.Serialization.BinaryAttributes;

namespace SPICA.Formats.H3D.Contents.LUT
{
    struct H3DLUTSampler
    {
        public byte Flags;
        private byte Padding0;
        private ushort Padding1;

        [PointerOf("SamplerCommands")]
        private uint SamplerCommandsAddress;

        [CountOf("SamplerCommands")]
        private uint SamplerCommandsCount;

        [PointerOf("SamplerName")]
        private uint SamplerNameAddress;

        [TargetSection("CommandsSection")]
        public uint[] SamplerCommands;

        [TargetSection("StringsSection")]
        public string SamplerName;
    }
}
