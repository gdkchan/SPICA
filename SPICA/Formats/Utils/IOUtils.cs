using System;
using System.IO;

namespace SPICA.Formats.Utils
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

        public static ulong Swap64(ulong Value)
        {
            Value = ((Value & 0xffffffff00000000ul) >> 32) | ((Value & 0x00000000fffffffful) << 32);
            Value = ((Value & 0xffff0000ffff0000ul) >> 16) | ((Value & 0x0000ffff0000fffful) << 16);
            Value = ((Value & 0xff00ff00ff00ff00ul) >>  8) | ((Value & 0x00ff00ff00ff00fful) <<  8);

            return Value;
        }

        public static uint ReadUInt24(BinaryReader Reader)
        {
            return (uint)(
                Reader.ReadByte() << 0 |
                Reader.ReadByte() << 8 |
                Reader.ReadByte() << 16);
        }

        public static uint Pow2RoundUp(uint Value)
        {
            Value--;

            Value |= (Value >> 1);
            Value |= (Value >> 2);
            Value |= (Value >> 4);
            Value |= (Value >> 8);
            Value |= (Value >> 16);

            return ++Value;
        }

        public static uint Pow2RoundDown(uint Value)
        {
            return IsPow2(Value) ? Value : Pow2RoundUp(Value) >> 1;
        }

        public static bool IsPow2(uint Value)
        {
            return Value != 0 && (Value & (Value - 1)) == 0;
        }
    }
}
