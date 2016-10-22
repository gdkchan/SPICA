using System;

namespace SPICA.Utils
{
    class IOUtils
    {
        //Waiting for the 32 bits floating-point Math stuff to improve this...
        public static float ToSingle(uint Value)
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(Value), 0);
        }

        public static uint ToUInt32(float Value)
        {
            return BitConverter.ToUInt32(BitConverter.GetBytes(Value), 0);
        }
    }
}
