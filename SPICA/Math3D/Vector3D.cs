using System;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;

namespace SPICA.Math3D
{
    public struct Vector3D
    {
        [XmlAttribute] public float X;
        [XmlAttribute] public float Y;
        [XmlAttribute] public float Z;

        public static Vector3D Empty { get { return new Vector3D(0, 0, 0); } }

        public float Length { get { return (float)Math.Sqrt(Dot(this, this)); } }

        public bool IsZero { get { return X == 0 && Y == 0 && Z == 0; } }

        public bool IsOne { get { return X == 1 && Y == 1 && Z == 1; } }

        public Vector3D(float Value) : this(Value, Value, Value) { }

        public Vector3D(float X, float Y, float Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public Vector3D(BinaryReader Reader)
        {
            X = Reader.ReadSingle();
            Y = Reader.ReadSingle();
            Z = Reader.ReadSingle();
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

                    default: throw new ArgumentOutOfRangeException("Expected 0-2 (X-Z) range!");
                }
            }
            set
            {
                switch (Index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    case 2: Z = value; break;

                    default: throw new ArgumentOutOfRangeException("Expected 0-2 (X-Z) range!");
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

        public static Vector3D operator /(Vector3D LHS, Vector3D RHS)
        {
            return new Vector3D(LHS.X / RHS.X, LHS.Y / RHS.Y, LHS.Z / RHS.Z);
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

        public static Vector3D operator /(Vector3D LHS, float RHS)
        {
            return new Vector3D(LHS.X / RHS, LHS.Y / RHS, LHS.Z / RHS);
        }

        public override string ToString()
        {
            return $"X: {X} Y: {Y} Z: {Z}";
        }

        public Vector3D Normalized()
        {
            return Length == 0 ? this : this / Length;
        }

        public static Vector3D Cross(Vector3D LHS, Vector3D RHS)
        {
            float X = LHS.Y * RHS.Z - LHS.Z * RHS.Y;
            float Y = LHS.Z * RHS.X - LHS.X * RHS.Z;
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

        public string ToSerializableString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", X, Y, Z);
        }

        public void Write(BinaryWriter Writer)
        {
            Writer.Write(X);
            Writer.Write(Y);
            Writer.Write(Z);
        }
    }
}
