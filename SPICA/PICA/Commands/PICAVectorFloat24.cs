using SPICA.Math3D;
using SPICA.Utils;

namespace SPICA.PICA.Commands
{
    public struct PICAVectorFloat24
    {
        public float X
        {
            get
            {
                return GetFloat24(Word2 & 0xffffff);
            }
        }

        public float Y
        {
            get
            {
                return GetFloat24((Word2 >> 24) | ((Word1 & 0xffff) << 8));
            }
        }

        public float Z
        {
            get
            {
                return GetFloat24((Word1 >> 16) | ((Word0 & 0xff) << 16));
            }
        }

        public float W
        {
            get
            {
                return GetFloat24(Word0 >> 8);
            }
        }

        internal uint Word0;
        internal uint Word1;
        internal uint Word2;

        private float GetFloat24(uint Value)
        {
            uint Float;

            if ((Value & 0x7fffff) != 0)
            {
                uint Mantissa = Value & 0xffff;
                uint Exponent = ((Value >> 16) & 0x7f) + 64;
                uint SignBit = (Value >> 23) & 1;

                Float = Mantissa << 7;
                Float |= Exponent << 23;
                Float |= SignBit << 31;
            }
            else
            {
                Float = (Value & 0x800000)  << 8;
            }

            return IOUtils.ToSingle(Float);
        }

        public override string ToString()
        {
            return string.Format("X: {0} Y: {1} Z: {2} W: {3}", X, Y, Z, W);
        }
    }
}
