using SPICA.Formats.H3D.Contents.Model;
using SPICA.Formats.H3D.Contents.Model.Material;
using SPICA.Serialization;
using SPICA.Serialization.BinaryAttributes;
using System.Collections.Generic;
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

        [SectionPointerOf("DescriptorsSection")]
        internal uint DescriptorsAddress;

        [SectionPointerOf("StringsSection")]
        internal uint StringsAddress;

        [SectionPointerOf("CommandsSection")]
        internal uint CommandsAddress;

        [SectionPointerOf("RawDataSection")]
        internal uint RawDataAddress;

        [SectionPointerOf("RawExtSection")]
        internal uint RawExtAddress;

        [PointerOf("RelocationTable")]
        internal uint RelocationAddress;

        [SectionLengthOf("DescriptorsSection")]
        internal uint DescriptorsLength;

        [SectionLengthOf("StringsSection")]
        internal uint StringsLength;

        [SectionLengthOf("CommandsSection")]
        internal uint CommandsLength;

        [SectionLengthOf("RawDataSection")]
        internal uint RawDataLength;

        [SectionLengthOf("RawExtSection")]
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

        [TargetSection("DescriptorsSection"), CustomSerialization]
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
            if (FName == "RelocationTable") new H3DRelocator(this, Deserializer.BaseStream).ToAbsolute();
        }

        public object Serialize(BinarySerializer Serializer, string FName)
        {
            switch (FName)
            {
                case "RelocationTable": return ((H3DRelocator)Serializer.Relocator).GetPointerTable();
                case "Contents": return (Contents.Materials.ParentRef = Contents);
            }

            return null;
        }
    }
}
