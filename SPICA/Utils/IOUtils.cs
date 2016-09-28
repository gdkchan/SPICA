using System;
using System.IO;
using System.Text;

namespace SPICA.Utils
{
    class IOUtils
    {
        public static float ToFloat(uint Value)
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(Value), 0);
        }

        public static string GetASCIIString(BinaryReader Reader, uint Address)
        {
            using (MemoryStream MS = new MemoryStream())
            {
                long Position = Reader.BaseStream.Position;
                Reader.BaseStream.Seek(Address, SeekOrigin.Begin);

                byte Chr;
                while ((Chr = Reader.ReadByte()) != 0)
                {
                    MS.WriteByte(Chr);
                }

                Reader.BaseStream.Seek(Position, SeekOrigin.Begin);

                return Encoding.ASCII.GetString(MS.ToArray());
            }
        }

        public static string GetUnicodeString(BinaryReader Reader, uint Address)
        {
            using (MemoryStream MS = new MemoryStream())
            {
                long Position = Reader.BaseStream.Position;
                Reader.BaseStream.Seek(Address, SeekOrigin.Begin);

                ushort Chr;
                while ((Chr = Reader.ReadUInt16()) != 0)
                {
                    MS.WriteByte((byte)Chr);
                    MS.WriteByte((byte)(Chr >> 8));
                }

                Reader.BaseStream.Seek(Position, SeekOrigin.Begin);

                return Encoding.Unicode.GetString(MS.ToArray());
            }
        }
    }
}
