using System;
using System.Globalization;
using System.IO;

namespace SPICA.Math3D
{
    public struct Vector4D
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public static Vector4D Empty { get { return new Vector4D(0, 0, 0, 0); } }

        public Vector4D(float X, float Y, float Z, float W)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.W = W;
        }

        public Vector4D(BinaryReader Reader)
        {
            X = Reader.ReadSingle();
            Y = Reader.ReadSingle();
            Z = Reader.ReadSingle();
            W = Reader.ReadSingle();
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

        public override bool Equals(object obj)
        {
            return obj is Vector4D && (Vector4D)obj == this;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ W.GetHashCode();
        }

        public static bool operator ==(Vector4D LHS, Vector4D RHS)
        {
            return LHS.X == RHS.X && LHS.Y == RHS.Y && LHS.Z == RHS.Z && LHS.W == RHS.W;
        }

        public static bool operator !=(Vector4D LHS, Vector4D RHS)
        {
            return !(LHS == RHS);
        }

        public static Vector4D operator +(Vector4D LHS, Vector4D RHS)
        {
            return new Vector4D(LHS.X + RHS.X, LHS.Y + RHS.Y, LHS.Z + RHS.Z, LHS.W + RHS.W);
        }

        public static Vector4D operator *(Vector4D LHS, Vector4D RHS)
        {
            return new Vector4D(LHS.X * RHS.X, LHS.Y * RHS.Y, LHS.Z * RHS.Z, LHS.W * RHS.W);
        }

        public static Vector4D operator -(Vector4D Vector)
        {
            return new Vector4D(-Vector.X, -Vector.Y, -Vector.Z, -Vector.W);
        }

        public static Vector4D operator -(Vector4D LHS, Vector4D RHS)
        {
            return LHS + (-RHS);
        }

        public static Vector4D operator *(Vector4D LHS, float RHS)
        {
            return new Vector4D(LHS.X * RHS, LHS.Y * RHS, LHS.Z * RHS, LHS.W * RHS);
        }

        public override string ToString()
        {
            return string.Format("X: {0} Y: {1} Z: {2} W: {3}", X, Y, Z, W);
        }

        public string ToSerializableString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} {1} {2} {3}", X, Y, Z, W);
        }

        public void Write(BinaryWriter Writer)
        {
            Writer.Write(X);
            Writer.Write(Y);
            Writer.Write(Z);
            Writer.Write(W);
        }
    }
}
