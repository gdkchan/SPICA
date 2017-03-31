namespace SPICA.Formats.Common
{
    static class BitUtils
    {
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
