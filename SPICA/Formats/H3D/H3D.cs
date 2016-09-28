using SPICA.Serialization;
using SPICA.Serialization.BinaryAttributes;

using System.IO;

namespace SPICA.Formats.H3D
{
    [Section("DescriptorsSection")]
    [Section("StringsSection")]
    [Section("CommandsSection")]
    [Section("RawDataSection")]
    [Section("RawExtSection")]
    class H3D
    {
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

        public H3DContents Contents;

        public static H3D Open(string FileName)
        {
            using (MemoryStream MS = new MemoryStream(File.ReadAllBytes(FileName)))
            {
                H3DRelocator Relocator = new H3DRelocator(MS);

                Relocator.ToAbsolute();

                File.WriteAllBytes("D:\\relocated.bch", MS.ToArray());

                MS.Seek(0, SeekOrigin.Begin);

                BinaryDeserializer Deserializer = new BinaryDeserializer(MS);

                return Deserializer.Deserialize<H3D>();
            }
        }
    }
}
