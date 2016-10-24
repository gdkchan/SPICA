using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrH3D
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

        public int UnInitDataLength;
        public int UnInitCommandsLength;

        public H3DFlags Flags;
        private byte Padding;

        public ushort AddressCount;
    }
}
