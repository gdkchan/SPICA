using System;

namespace SPICA.Math3D
{
    struct RGBAFloat
    {
        public float R;
        public float G;
        public float B;
        public float A;

        public RGBAFloat(float R, float G, float B, float A)
        {
            this.R = R;
            this.G = G;
            this.B = B;
            this.A = A;
        }

        public float this[int Index]
        {
            get
            {
                switch (Index)
                {
                    case 0: return R;
                    case 1: return G;
                    case 2: return B;
                    case 3: return A;

                    default: throw new IndexOutOfRangeException("Expected 0-3 (R-A) range!");
                }
            }
            set
            {
                switch (Index)
                {
                    case 0: R = value; break;
                    case 1: G = value; break;
                    case 2: B = value; break;
                    case 3: A = value; break;

                    default: throw new IndexOutOfRangeException("Expected 0-3 (R-A) range!");
                }
            }
        }

        public override string ToString()
        {
            return string.Format("R: {0} G: {1} B: {2} A: {3}", R, G, B, A);
        }
    }
}
