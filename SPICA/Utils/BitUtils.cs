using System;
using System.Collections.Generic;
using System.Text;

namespace SPICA.Utils
{
    class BitUtils
    {
        public static byte GetBits(byte Value, int Start, int Count)
        {
            Value >>= Start;
            Value &= (byte)((1 << Count) - 1);

            return Value;
        }

        public static byte SetBits(byte Value, byte Bits, int Start, int Count)
        {
            Value &= (byte)(~(((1 << Count) - 1) << Start));
            Value |= (byte)(Bits << Start);

            return Value;
        }
    }
}
