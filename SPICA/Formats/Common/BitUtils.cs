namespace SPICA.Formats.Common
{
    static class BitUtils
    {
        public static ulong Swap64(ulong Value)
        {
            Value = ((Value & 0xffffffff00000000ul) >> 32) | ((Value & 0x00000000fffffffful) << 32);
            Value = ((Value & 0xffff0000ffff0000ul) >> 16) | ((Value & 0x0000ffff0000fffful) << 16);
            Value = ((Value & 0xff00ff00ff00ff00ul) >>  8) | ((Value & 0x00ff00ff00ff00fful) <<  8);

            return Value;
        }

        public static uint Pow2RoundUp(uint Value)
        {
            Value--;

            Value |= (Value >>  1);
            Value |= (Value >>  2);
            Value |= (Value >>  4);
            Value |= (Value >>  8);
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

        public static uint GetBits(uint Value, int Start, int Count)
        {
            Value >>= Start;
            Value &= (1u << Count) - 1;

            return Value;
        }

        public static uint SetBits(uint Value, uint Bits, int Start, int Count)
        {
            Value &= ~(((1u << Count) - 1) << Start);
            Value |= Bits << Start;

            return Value;
        }

        public static uint SetBit(uint Value, bool Bit, int Start)
        {
            if (Bit)
                return Value | (1u << Start);
            else
                return Value & ~(1u << Start);
        }

        public static ushort SetBits(ushort Value, uint Bits, int Start, int Count)
        {
            return (ushort)SetBits((uint)Value, Bits, Start, Count);
        }

        public static byte SetBits(byte Value, uint Bits, int Start, int Count)
        {
            return (byte)SetBits((uint)Value, Bits, Start, Count);
        }

        public static ushort SetBit(ushort Value, bool Bit, int Start)
        {
            return (ushort)SetBit((uint)Value, Bit, Start);
        }

        public static byte SetBit(byte Value, bool Bit, int Start)
        {
            return (byte)SetBit((uint)Value, Bit, Start);
        }
    }
}
