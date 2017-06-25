using System;
using System.IO;
using System.Text;

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

        public static uint ToUInt32(string Magic)
        {
            byte[] Bytes = Encoding.ASCII.GetBytes(Magic);

            if (!BitConverter.IsLittleEndian) Array.Reverse(Bytes);

            return BitConverter.ToUInt32(Bytes, 0);
        }

        public static byte[] ByteSwap16(byte[] Input)
        {
            byte[] Output = new byte[Input.Length];

            for (int i = 0; i < Input.Length; i += 2)
            {
                Output[i + 0] = Input[i + 1];
                Output[i + 1] = Input[i + 0];
            }

            return Output;
        }

        public static uint ReadUInt24(this BinaryReader Reader)
        {
            return (uint)(
                Reader.ReadByte() <<  0 |
                Reader.ReadByte() <<  8 |
                Reader.ReadByte() << 16);
        }

        public static void WriteUInt24(this BinaryWriter Writer, uint Value)
        {
            Writer.Write((byte)(Value >>  0));
            Writer.Write((byte)(Value >>  8));
            Writer.Write((byte)(Value >> 16));
        }

        public static void Align(this BinaryReader Reader, int BlockSize)
        {
            long Remainder = Reader.BaseStream.Position % BlockSize;

            if (Remainder != 0)
            {
                Reader.BaseStream.Seek(BlockSize - Remainder, SeekOrigin.Current);
            }
        }

        public static void Align(this BinaryWriter Writer, int BlockSize, byte FillByte)
        {
            while ((Writer.BaseStream.Position % BlockSize) != 0)
            {
                Writer.Write(FillByte);
            }
        }
    }
}
