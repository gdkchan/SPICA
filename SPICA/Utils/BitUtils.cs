using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
