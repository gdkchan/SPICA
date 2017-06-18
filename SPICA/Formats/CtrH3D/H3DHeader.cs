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

        //Only newer versions have this section.
        [IfVersion(CmpOp.Gequal, 0x21)]
        public uint RawExtAddress;

        public uint RelocationAddress;

        public int ContentsLength;
        public int StringsLength;
        public int CommandsLength;
        public int RawDataLength;

        //Only newer versions have this section.
        [IfVersion(CmpOp.Gequal, 0x21)]
        public int RawExtLength;

        public int RelocationLength;

        public int UnInitDataLength;
        public int UnInitCommandsLength;

        [IfVersion(CmpOp.Gequal, 0x20), Padding(2)]
        public H3DFlags Flags;

        [IfVersion(CmpOp.Gequal, 0x20)]
        public ushort AddressCount;
    }
}
