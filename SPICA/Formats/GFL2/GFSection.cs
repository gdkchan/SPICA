using SPICA.Formats.Common;

using System.IO;

namespace SPICA.Formats.GFL2
{
    class GFSection
    {
        public  string Magic;
        public  uint   Length;
        private uint   Padding;

        public GFSection()
        {
            Padding = 0xffffffff;
        }

<<<<<<< HEAD
        public GFSection(string Magic) : this()
=======
        public GFSection(string magic, uint length)
>>>>>>> ca59ba8d42bfa4bd096e4061f63acd01a77a125c
        {
            Magic = magic;
			Length = length;
        }

        public GFSection(BinaryReader Reader)
        {
            Magic   = Reader.ReadPaddedString(8);
            Length  = Reader.ReadUInt32();
            Padding = Reader.ReadUInt32();
        }

        public void Write(BinaryWriter Writer)
        {
            Writer.WritePaddedString(Magic, 8);
            Writer.Write(Length);
            Writer.Write(0xffffffffu);
        }

        public static void SkipPadding(Stream BaseStream)
        {
            if ((BaseStream.Position & 0xf) != 0)
            {
                BaseStream.Seek(0x10 - (BaseStream.Position & 0xf), SeekOrigin.Current);
            }
        }
    }
}
