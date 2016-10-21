using System;

namespace SPICA.Math3D
{
    public struct Vector2D
    {
        public float X;
        public float Y;

        public Vector2D(float X, float Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public float this[int Index]
        {
            get
            {
                switch (Index)
                {
                    case 0: return X;
                    case 1: return Y;

                    default: throw new IndexOutOfRangeException("Expected 0-1 (X-Y) range!");
                }
            }
            set
            {
                switch (Index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;

                    default: throw new IndexOutOfRangeException("Expected 0-1 (X-Y) range!");
                }
            }
        }

        public override string ToString()
        {
            return string.Format("X: {0} Y: {1}", X, Y);
        }
    }
}
