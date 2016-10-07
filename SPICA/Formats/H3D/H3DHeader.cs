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

        public uint DescriptorsAddress;
        public uint StringsAddress;
        public uint CommandsAddress;
        public uint RawDataAddress;
        public uint RawExtAddress;
        public uint RelocationAddress;

        public uint DescriptorsLength;
        public uint StringsLength;
        public uint CommandsLength;
        public uint RawDataLength;
        public uint RawExtLength;
        public uint RelocationLength;

        public uint UnInitDataLength;
        public uint UnInitCommandsLength;

        public byte Flags;
        public byte Padding;

        public ushort AddressCount;
    }
}
