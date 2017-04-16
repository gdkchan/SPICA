using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrH3D
{
    class H3DHeader
    {
        [Inline]
        public string Magic;

        [Version]
        public byte BackwardCompatibility;
        public byte ForwardCompatibility;
        public ushort ConverterVersion;

        public uint ContentsAddress;
        public uint StringsAddress;
        public uint CommandsAddress;
        public uint RawDataAddress;
        [IfVersionGE(0x21)]
        public uint RawExtAddress;
        public uint RelocationAddress;

        public int ContentsLength;
        public int StringsLength;
        public int CommandsLength;
        public int RawDataLength;
        [IfVersionGE(0x21)]
        public int RawExtLength;
        public int RelocationLength;

        public int UnInitDataLength;
        public int UnInitCommandsLength;

        [IfVersionGE(0x20)]
        public H3DFlags Flags;

        [IfVersionGE(0x20)]
        private byte Padding;

        [IfVersionGE(0x20)]
        public ushort AddressCount;
    }
}
