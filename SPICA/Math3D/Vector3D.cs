using System;

namespace SPICA.Math3D
{
    public struct Vector3D
    {
        public float X;
        public float Y;
        public float Z;

        public static Vector3D Empty { get { return new Vector3D(0, 0, 0); } }

        public float Length { get { return (float)Math.Sqrt(Dot(this, this)); } }

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

        public override bool Equals(object obj)
        {
            return obj is Vector3D && (Vector3D)obj == this;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        public static bool operator ==(Vector3D LHS, Vector3D RHS)
        {
            return LHS.X == RHS.X && LHS.Y == RHS.Y && LHS.Z == RHS.Z;
        }

        public static bool operator !=(Vector3D LHS, Vector3D RHS)
        {
            return !(LHS == RHS);
        }

        public static Vector3D operator+(Vector3D LHS, Vector3D RHS)
        {
            return new Vector3D(LHS.X + RHS.X, LHS.Y + RHS.Y, LHS.Z + RHS.Z);
        }

        public static Vector3D operator *(Vector3D LHS, Vector3D RHS)
        {
            return new Vector3D(LHS.X * RHS.X, LHS.Y * RHS.Y, LHS.Z * RHS.Z);
        }

        public static Vector3D operator -(Vector3D Vector)
        {
            return new Vector3D(-Vector.X, -Vector.Y, -Vector.Z);
        }

        public static Vector3D operator -(Vector3D LHS, Vector3D RHS)
        {
            return LHS + (-RHS);
        }

        public static Vector3D operator *(Vector3D LHS, float RHS)
        {
            return new Vector3D(LHS.X * RHS, LHS.Y * RHS, LHS.Z * RHS);
        }

        public static Vector3D Cross(Vector3D LHS, Vector3D RHS)
        {
            float X = LHS.Y * RHS.Z - LHS.Z * RHS.Y;
            float Y = LHS.X * RHS.Z - LHS.Z * RHS.X;
            float Z = LHS.X * RHS.Y - LHS.Y * RHS.X;

            return new Vector3D(X, Y, Z);
        }

        public static float Dot(Vector3D LHS, Vector3D RHS)
        {
            float X = LHS.X * RHS.X;
            float Y = LHS.Y * RHS.Y;
            float Z = LHS.Z * RHS.Z;

            return X + Y + Z;
        }

        public override string ToString()
        {
            return string.Format("X: {0} Y: {1} Z: {2}", X, Y, Z);
        }
    }
}
