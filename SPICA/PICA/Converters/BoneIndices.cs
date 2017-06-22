using System;

namespace SPICA.PICA.Converters
{
    public struct BoneIndices
    {
        public int b0;
        public int b1;
        public int b2;
        public int b3;

        public int this[int Index]
        {
            get
            {
                switch (Index)
                {
                    case 0: return b0;
                    case 1: return b1;
                    case 2: return b2;
                    case 3: return b3;

                    default: throw new ArgumentOutOfRangeException();
                }
            }
            set
            {
                switch (Index)
                {
                    case 0: b0 = value; break;
                    case 1: b1 = value; break;
                    case 2: b2 = value; break;
                    case 3: b3 = value; break;

                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
