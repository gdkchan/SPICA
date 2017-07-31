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

        public static int MaskBits(int Value, int Start, int Count)
        {
            int Mask = (1 << Count) - 1;

            Mask  <<= Start;
            Value &=  Mask;

            return Value;
        }

        public static int MaskOutBits(int Value, int Start, int Count)
        {
            int Mask = (1 << Count) - 1;

            Mask  <<= Start;
            Value &=  ~Mask;

            return Value;
        }

        public static dynamic GetBits(int Value, int Start, int Count)
        {
            int Mask = (1 << Count) - 1;

            Value >>= Start;
            Value &=  Mask;

            return Value;
        }

        public static int SetBits(int Value, int Bits, int Start, int Count)
        {
            int Mask = (1 << Count) - 1;

            Value &= ~(Mask << Start);
            Value |=   Bits << Start;

            return Value;
        }

        public static bool GetBit(int Value, int Start)
        {
            return ((Value >> Start) & 1) != 0;
        }

        public static int SetBit(int Value, bool Bit, int Start)
        {
            return Bit
                ? Value |  (1 << Start)
                : Value & ~(1 << Start);
        }
    }
}
