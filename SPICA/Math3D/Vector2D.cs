using System;
using System.Globalization;
using System.IO;

namespace SPICA.Math3D
{
    public struct Vector2D
    {
        public float X;
        public float Y;

        public static Vector2D Empty { get { return new Vector2D(0, 0); } }

        public Vector2D(float Value) : this(Value, Value) { }

        public Vector2D(float X, float Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public Vector2D(BinaryReader Reader)
        {
            X = Reader.ReadSingle();
            Y = Reader.ReadSingle();
        }

        public float this[int Index]
        {
            get
            {
                switch (Index)
                {
                    case 0: return X;
                    case 1: return Y;

                    default: throw new ArgumentOutOfRangeException("Expected 0-1 (X-Y) range!");
                }
            }
            set
            {
                switch (Index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;

                    default: throw new ArgumentOutOfRangeException("Expected 0-1 (X-Y) range!");
                }
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Vector2D && (Vector2D)obj == this;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public static bool operator ==(Vector2D LHS, Vector2D RHS)
        {
            return LHS.X == RHS.X && LHS.Y == RHS.Y;
        }

        public static bool operator !=(Vector2D LHS, Vector2D RHS)
        {
            return !(LHS == RHS);
        }

        public static Vector2D operator +(Vector2D LHS, Vector2D RHS)
        {
            return new Vector2D(LHS.X + RHS.X, LHS.Y + RHS.Y);
        }

        public static Vector2D operator *(Vector2D LHS, Vector2D RHS)
        {
            return new Vector2D(LHS.X * RHS.X, LHS.Y * RHS.Y);
        }

        public static Vector2D operator /(Vector2D LHS, Vector2D RHS)
        {
            return new Vector2D(LHS.X / RHS.X, LHS.Y / RHS.Y);
        }

        public static Vector2D operator -(Vector2D Vector)
        {
            return new Vector2D(-Vector.X, -Vector.Y);
        }

        public static Vector2D operator -(Vector2D LHS, Vector2D RHS)
        {
            return LHS + (-RHS);
        }

        public static Vector2D operator *(Vector2D LHS, float RHS)
        {
            return new Vector2D(LHS.X * RHS, LHS.Y * RHS);
        }

        public static Vector2D operator /(Vector2D LHS, float RHS)
        {
            return new Vector2D(LHS.X / RHS, LHS.Y / RHS);
        }

        public override string ToString()
        {
            return string.Format("X: {0} Y: {1}", X, Y);
        }

        public string ToSerializableString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} {1}", X, Y);
        }

        public void Write(BinaryWriter Writer)
        {
            Writer.Write(X);
            Writer.Write(Y);
        }
    }
}
