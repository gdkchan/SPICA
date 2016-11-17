using System;
using System.IO;

namespace SPICA.Math3D
{
    public struct Quaternion
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public static Quaternion Empty { get { return new Quaternion(0, 0, 0, 0); } }
        public static Quaternion Identity { get { return new Quaternion(0, 0, 0, 1); } }

        public Quaternion(float X, float Y, float Z, float W)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.W = W;
        }

        public Quaternion(BinaryReader Reader)
        {
            X = Reader.ReadSingle();
            Y = Reader.ReadSingle();
            Z = Reader.ReadSingle();
            W = Reader.ReadSingle();
        }

        public static Quaternion FromEuler(Vector3D Rotation)
        {
            float c1 = (float)Math.Cos(Rotation.X * 0.5f);
            float c2 = (float)Math.Cos(Rotation.Y * 0.5f);
            float c3 = (float)Math.Cos(Rotation.Z * 0.5f);
            float s1 = (float)Math.Sin(Rotation.X * 0.5f);
            float s2 = (float)Math.Sin(Rotation.Y * 0.5f);
            float s3 = (float)Math.Sin(Rotation.Z * 0.5f);

            return new Quaternion
            {
                X = s1 * c2 * c3 + c1 * s2 * s3,
                Y = c1 * s2 * c3 - s1 * c2 * s3,
                Z = c1 * c2 * s3 + s1 * s2 * c3,
                W = c1 * c2 * c3 - s1 * s2 * s3
            };
        }

        public static Quaternion FromAxisHalvedAngle(Vector3D Axis, float Angle)
        {
            return new Quaternion
            {
                X = (float)(Math.Sin(Angle) * Axis.X),
                Y = (float)(Math.Sin(Angle) * Axis.Y),
                Z = (float)(Math.Sin(Angle) * Axis.Z),
                W = (float)Math.Cos(Angle)
            };
        }

        public static Quaternion FromAxisAngle(Vector3D Axis, float Angle)
        {
            return FromAxisHalvedAngle(Axis, Angle * 0.5f);
        }

        public override bool Equals(object obj)
        {
            return obj is Quaternion && (Quaternion)obj == this;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ W.GetHashCode();
        }

        public static bool operator ==(Quaternion LHS, Quaternion RHS)
        {
            return LHS.X == RHS.X && LHS.Y == RHS.Y && LHS.Z == RHS.Z && LHS.W == RHS.W;
        }

        public static bool operator !=(Quaternion LHS, Quaternion RHS)
        {
            return !(LHS == RHS);
        }

        public override string ToString()
        {
            return string.Format("X: {0} Y: {1} Z: {2} W: {3}", X, Y, Z, W);
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
