using System;

namespace SPICA.Math3D
{
    struct Vector4D
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public Vector4D(float X, float Y, float Z, float W)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.W = W;
        }

        public float this[int Index]
        {
            get
            {
                switch (Index)
                {
                    case 0: return X;
                    case 1: return Y;
                    case 2: return Z;
                    case 3: return W;

                    default: throw new IndexOutOfRangeException("Expected 0-3 (X-W) range!");
                }
            }
            set
            {
                switch (Index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    case 2: Z = value; break;
                    case 3: W = value; break;

                    default: throw new IndexOutOfRangeException("Expected 0-3 (X-W) range!");
                }
            }
        }

        public override string ToString()
        {
            return string.Format("X: {0} Y: {1} Z: {2} W: {3}", X, Y, Z, W);
        }
    }
}
