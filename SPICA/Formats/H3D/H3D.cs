using SPICA.Serialization;
using SPICA.Serialization.BinaryAttributes;

using System.IO;

namespace SPICA.Formats.H3D
{
    [Section("DescriptorsSection")]
    [Section("StringsSection", 0x10)]
    [Section("CommandsSection", 0x10)]
    [Section("RawDataSection", 0x80)]
    [Section("RawExtSection", 0x80)]
    [Section("RelocationSection")]
    class H3D : ICustomDeserializer, ICustomSerializer
    {
        public string Magic;
        public byte BackwardCompatibility;
        public byte ForwardCompatibility;
        public ushort ConverterVersion;

        internal uint DescriptorsAddress;
        internal uint StringsAddress;
        internal uint CommandsAddress;
        internal uint RawDataAddress;
        internal uint RawExtAddress;

        [PointerOf("RelocationTable")]
        internal uint RelocationAddress;

        internal uint DescriptorsLength;
        internal uint StringsLength;
        internal uint CommandsLength;
        internal uint RawDataLength;
        internal uint RawExtLength;

        [CountOf("RelocationTable")]
        internal uint RelocationLength;

        private uint UnInitDataLength;
        private uint UnInitCommandsLength;

        public byte Flags;
        private byte Padding;

        private ushort AddressCount;

        [TargetSection("RelocationSection"), CustomSerialization]
        internal byte[] RelocationTable;

        public H3DContents Contents;

        public static H3D Open(string FileName)
        {
            using (MemoryStream MS = new MemoryStream(File.ReadAllBytes(FileName)))
            {
                return new BinaryDeserializer(MS).Deserialize<H3D>();
            }
        }

        public static void Save(H3D Data, string FileName)
        {
            using (FileStream FS = new FileStream(FileName, FileMode.Create))
            {
                new BinarySerializer(FS, new H3DRelocator(FS)).Serialize(Data);
            }
        }

        public void Deserialize(BinaryDeserializer Deserializer, string FName)
        {
            new H3DRelocator(this, Deserializer.BaseStream).ToAbsolute();
        }

        public object Serialize(BinarySerializer Serializer, string FName)
        {
            return ((H3DRelocator)Serializer.Relocator).GetPointerTable();
        }
    }
}
