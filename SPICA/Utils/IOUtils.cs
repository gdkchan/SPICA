using System;

namespace SPICA.Utils
{
    class IOUtils
    {
        public static float ToFloat(uint Value)
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(Value), 0);
        }

        public static uint ToUInt(float Value)
        {
            return BitConverter.ToUInt32(BitConverter.GetBytes(Value), 0);
        }
    }
}
