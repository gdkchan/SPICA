namespace SPICA.Utils
{
    static class BitUtils
    {
        public static ulong Swap64(ulong Value)
        {
            Value = (Value >> 32) | (Value << 32);

            Value = ((Value & 0xffff0000ffff0000ul) >> 16) | ((Value & 0x0000ffff0000fffful) << 16);

            Value = ((Value & 0xff00ff00ff00ff00ul) >> 8) | ((Value & 0x00ff00ff00ff00fful) << 8);

            return Value;
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

        public static ushort SetBits(ushort Value, uint Bits, int Start, int Count)
        {
            Value &= (ushort)(~(((1u << Count) - 1) << Start));
            Value |= (ushort)(Bits << Start);

            return Value;
        }

        public static byte SetBits(byte Value, uint Bits, int Start, int Count)
        {
            Value &= (byte)(~(((1u << Count) - 1) << Start));
            Value |= (byte)(Bits << Start);

            return Value;
        }

        public static uint SetBit(uint Value, bool Bit, int Start)
        {
            if (Bit)
                return Value | (1u << Start);
            else
                return Value & ~(1u << Start);
        }

        public static ushort SetBit(ushort Value, bool Bit, int Start)
        {
            if (Bit)
                return (ushort)(Value | (1u << Start));
            else
                return (ushort)(Value & ~(1u << Start));
        }

        public static byte SetBit(byte Value, bool Bit, int Start)
        {
            if (Bit)
                return (byte)(Value | (1u << Start));
            else
                return (byte)(Value & ~(1u << Start));
        }
    }
}
