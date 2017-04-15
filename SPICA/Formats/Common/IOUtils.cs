using System;
using System.IO;

namespace SPICA.Formats.Common
{
    static class IOUtils
    {
        public static float ToSingle(uint Value)
        {
            byte[] Bytes = BitConverter.GetBytes(Value);

            if (!BitConverter.IsLittleEndian) Array.Reverse(Bytes);

            return BitConverter.ToSingle(Bytes, 0);
        }

        public static uint ToUInt32(float Value)
        {
            byte[] Bytes = BitConverter.GetBytes(Value);

            if (!BitConverter.IsLittleEndian) Array.Reverse(Bytes);

            return BitConverter.ToUInt32(Bytes, 0);
        }

        public static uint ReadUInt24(BinaryReader Reader)
        {
            return (uint)(
                Reader.ReadByte() << 0 |
                Reader.ReadByte() << 8 |
                Reader.ReadByte() << 16);
        }

        public static void WriteUInt24(BinaryWriter Writer, uint Value)
        {
            Writer.Write((byte)(Value >>  0));
            Writer.Write((byte)(Value >>  8));
            Writer.Write((byte)(Value >> 16));
        }
    }
}
