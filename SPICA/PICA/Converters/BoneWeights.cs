using System;

namespace SPICA.PICA.Converters
{
    public struct BoneWeights
    {
        public float w0;
        public float w1;
        public float w2;
        public float w3;

        public float this[int Index]
        {
            get
            {
                switch (Index)
                {
                    case 0: return w0;
                    case 1: return w1;
                    case 2: return w2;
                    case 3: return w3;

                    default: throw new ArgumentOutOfRangeException();
                }
            }
            set
            {
                switch (Index)
                {
                    case 0: w0 = value; break;
                    case 1: w1 = value; break;
                    case 2: w2 = value; break;
                    case 3: w3 = value; break;

                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
