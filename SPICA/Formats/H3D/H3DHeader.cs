using SPICA.Serialization.Attributes;

namespace SPICA.Formats.H3D
{
    class H3DHeader
    {
        [Inline]
        public string Magic;
        public byte BackwardCompatibility;
        public byte ForwardCompatibility;
        public ushort ConverterVersion;

        public uint ContentsAddress;
        public uint StringsAddress;
        public uint CommandsAddress;
        public uint RawDataAddress;
        public uint RawExtAddress;
        public uint RelocationAddress;

        public int ContentsLength;
        public int StringsLength;
        public int CommandsLength;
        public int RawDataLength;
        public int RawExtLength;
        public int RelocationLength;

        public uint UnInitDataLength;
        public uint UnInitCommandsLength;

        public byte Flags;
        public byte Padding;

        public ushort AddressCount;
    }
}
