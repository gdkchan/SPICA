using System;

namespace SPICA.Utils
{
    class IOUtils
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
    }
}
