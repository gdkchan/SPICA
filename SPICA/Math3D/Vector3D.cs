using System;

namespace SPICA.Math3D
{
    public struct Vector3D
    {
        public float X;
        public float Y;
        public float Z;

        public Vector3D(float X, float Y, float Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
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

                    default: throw new IndexOutOfRangeException("Expected 0-2 (X-Z) range!");
                }
            }
            set
            {
                switch (Index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    case 2: Z = value; break;

                    default: throw new IndexOutOfRangeException("Expected 0-2 (X-Z) range!");
                }
            }
        }

        public override string ToString()
        {
            return string.Format("X: {0} Y: {1} Z: {2}", X, Y, Z);
        }
    }
}
