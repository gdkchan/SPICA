using System.IO;

using SPICA.Formats.Common;
using SPICA.Formats.CtrH3D;

namespace SPICA.Formats.Packages
{
    public class GFPackage
    {
        public string Magic;

        public byte[][] Files;

        public GFPackage(Stream Input)
        {
            BinaryReader Reader = new BinaryReader(Input);

            Magic = Reader.ReadPaddedString(2);

            ushort Count = Reader.ReadUInt16();

            Files = new byte[Count][];

            for (int i = 0; i < Count; i++)
            {
                Input.Seek(4 + i * 4, SeekOrigin.Begin);

                uint StartAddress = Reader.ReadUInt32();
                uint EndAddress   = Reader.ReadUInt32();

                int Length = (int)(EndAddress - StartAddress);

                Input.Seek(StartAddress, SeekOrigin.Begin);

                Files[i] = Reader.ReadBytes(Length);
            }
        }

        public H3D CreateH3DFromContent()
        {
            H3D Output = new H3D();

            foreach (byte[] File in Files)
            {
                if (File.Length < 4) continue;

                if (File[0] == 'B' &&
                    File[1] == 'C' &&
                    File[2] == 'H' &&
                    File[3] == '\0')
                {
                    Output.Merge(H3D.Open(File));
                }
            }

            return Output;
        }
    }
}