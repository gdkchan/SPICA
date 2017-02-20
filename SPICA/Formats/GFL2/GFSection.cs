using SPICA.Formats.Utils;

using System.IO;

namespace SPICA.Formats.GFL2
{
    class GFSection
    {
        public string Magic;
        public uint Length;
        private uint Padding;

        public GFSection() { }

        public GFSection(string Magic)
        {
            this.Magic = Magic;
        }

        public GFSection(BinaryReader Reader)
        {
            Magic = Reader.ReadPaddedString(8);
            Length = Reader.ReadUInt32();
            Padding = Reader.ReadUInt32();
        }

        public void Write(BinaryWriter Writer)
        {
            Writer.WritePaddedString(Magic, 8);
            Writer.Write(Length);
            Writer.Write(0xffffffffu);
        }

        public static void SkipPadding(BinaryReader Reader)
        {
            while ((Reader.BaseStream.Position & 0xf) != 0) Reader.ReadByte();
        }
    }
}
