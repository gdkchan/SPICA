using System.IO;
using System.Text;

namespace SPICA.WinForms.Formats
{
    static class GFPackage
    {
        public struct Header
        {
            public string Magic;
            public Entry[] Entries;
        }

        public struct Entry
        {
            public uint Address;
            public int Length;
        }

        public static Header GetPackageHeader(Stream Input)
        {
            BinaryReader Reader = new BinaryReader(Input);

            Header Output = new Header();

            Output.Magic = Encoding.ASCII.GetString(Reader.ReadBytes(2));

            ushort Entries = Reader.ReadUInt16();

            Output.Entries = new Entry[Entries];

            long Position = Input.Position;

            for (int Index = 0; Index < Entries; Index++)
            {
                Input.Seek(Position + Index * 4, SeekOrigin.Begin);

                uint StartAddress = Reader.ReadUInt32();
                uint EndAddress = Reader.ReadUInt32();

                int Length = (int)(EndAddress - StartAddress);

                Output.Entries[Index] = new Entry
                {
                    Address = (uint)(Position - 4) + StartAddress,
                    Length = Length
                };
            }

            return Output;
        }

        public static bool IsValidPackage(Stream Input)
        {
            long Position = Input.Position;

            BinaryReader Reader = new BinaryReader(Input);

            bool Result = IsValidPackage(Reader);

            Input.Seek(Position, SeekOrigin.Begin);

            return Result;
        }

        private static bool IsValidPackage(BinaryReader Reader)
        {
            if (Reader.BaseStream.Length < 0x80) return false;

            byte Magic0 = Reader.ReadByte();
            byte Magic1 = Reader.ReadByte();

            if (Magic0 < 'A' || Magic0 > 'Z' ||
                Magic1 < 'A' || Magic1 > 'Z')
                return false;

            return true;
        }
    }
}
