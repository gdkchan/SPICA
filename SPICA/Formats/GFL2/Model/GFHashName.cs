using SPICA.Formats.Utils;

using System.IO;

namespace SPICA.Formats.GFL2.Model
{
    public struct GFHashName
    {
        public uint Hash;
        public string Name;

        public GFHashName(uint Hash, string Name)
        {
            this.Hash = Hash;
            this.Name = Name;
        }

        public GFHashName(BinaryReader Reader)
        {
            Hash = Reader.ReadUInt32();
            Name = Reader.ReadByteLengthString();
        }
    }
}
