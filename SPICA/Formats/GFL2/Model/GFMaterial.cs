using SPICA.Formats.GFL2.Utils;

using System.IO;

namespace SPICA.Formats.GFL2.Model
{
    class GFMaterial
    {
        public uint LUT0HashId;
        public uint LUT1HashId;
        public uint LUT2HashId;

        public GFMaterial(BinaryReader Reader)
        {
            GFSection MaterialSection = new GFSection(Reader);

            GFHashName[] Names = new GFHashName[4];

            for (int i = 0; i < Names.Length; i++)
            {
                uint Hash = Reader.ReadUInt32();
                string Name = GFString.ReadLength(Reader, Reader.ReadByte());

                Names[i] = new GFHashName(Hash, Name);
            }

            LUT0HashId = Reader.ReadUInt32();
            LUT1HashId = Reader.ReadUInt32();
            LUT2HashId = Reader.ReadUInt32();
        }
    }
}
