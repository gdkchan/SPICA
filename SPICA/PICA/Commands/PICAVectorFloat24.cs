using SPICA.Math3D;
using SPICA.Utils;

namespace SPICA.PICA.Commands
{
    struct PICAVectorFloat24
    {
        public uint Word0;
        public uint Word1;
        public uint Word2;

        public Vector4D Vector
        {
            get
            {
                return new Vector4D
                {
                    X = GetFloat24(Word2 & 0xffffff),
                    Y = GetFloat24((Word2 >> 24) | ((Word1 & 0xffff) << 8)),
                    Z = GetFloat24((Word1 >> 16) | ((Word0 & 0xff) << 16)),
                    W = GetFloat24(Word0 >> 8)
                };
            }
            //TODO: set
        }

        private float GetFloat24(uint Value)
        {
            //FIXME: This doesn't work
            uint Float;

            uint Mantissa = Value & 0xffff;
            uint Exponent = ((Value >> 16) & 0x7f) + 64;
            uint SignBit = (Value >> 23) & 1;

            Float = Mantissa << 7;
            Float |= Exponent << 23;
            Float |= SignBit << 31;

            return IOUtils.ToFloat(Float);
        }

        public override string ToString()
        {
            return Vector.ToString();
        }
    }
}
