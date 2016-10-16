namespace SPICA.Utils
{
    class BitUtils
    {
        public static uint GetBits(uint Value, int Start, int Count)
        {
            Value >>= Start;
            Value &= (uint)((1 << Count) - 1);

            return Value;
        }

        public static uint SetBits(uint Value, uint Bits, int Start, int Count)
        {
            Value &= (uint)(~(((1 << Count) - 1) << Start));
            Value |= Bits << Start;

            return Value;
        }

        public static ushort SetBits(ushort Value, uint Bits, int Start, int Count)
        {
            Value &= (ushort)(~(((1 << Count) - 1) << Start));
            Value |= (ushort)(Bits << Start);

            return Value;
        }

        public static byte SetBits(byte Value, uint Bits, int Start, int Count)
        {
            Value &= (byte)(~(((1 << Count) - 1) << Start));
            Value |= (byte)(Bits << Start);

            return Value;
        }
    }
}
