using System;
using System.Globalization;
using System.IO;

namespace SPICA.Math3D
{
    public struct Quaternion
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public Vector3D VectorPart { get { return new Vector3D(X, Y, Z); } }

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
            return
                FromAxisAngle(new Vector3D(1, 0, 0), Rotation.X) *
                FromAxisAngle(new Vector3D(0, 1, 0), Rotation.Y) *
                FromAxisAngle(new Vector3D(0, 0, 1), Rotation.Z);
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

        public static Quaternion operator +(Quaternion LHS, Quaternion RHS)
        {
            return new Quaternion(LHS.X + RHS.X, LHS.Y + RHS.Y, LHS.Z + RHS.Z, LHS.W + RHS.W);
        }

        public static Quaternion operator *(Quaternion RHS, Quaternion LHS)
        {
            Vector3D VL = LHS.VectorPart;
            Vector3D VR = RHS.VectorPart;

            Vector3D Cross = Vector3D.Cross(VL, VR);

            return new Quaternion
            {
                X = LHS.X * RHS.W + LHS.W * RHS.X + Cross.X,
                Y = LHS.Y * RHS.W + LHS.W * RHS.Y + Cross.Y,
                Z = LHS.Z * RHS.W + LHS.W * RHS.Z + Cross.Z,
                W = LHS.W * RHS.W - Vector3D.Dot(VL, VR)
            };
        }

        public static Quaternion operator /(Quaternion RHS, Quaternion LHS)
        {
            float Inverse = 1f / Dot(RHS, RHS);

            RHS = new Quaternion
            {
                X = -RHS.X * Inverse,
                Y = -RHS.Y * Inverse,
                Z = -RHS.Z * Inverse,
                W =  RHS.W * Inverse
            };

            return RHS * LHS;
        }

        public static Quaternion operator -(Quaternion Quat)
        {
            return new Quaternion(-Quat.X, -Quat.Y, -Quat.Z, -Quat.W);
        }

        public static Quaternion operator -(Quaternion LHS, Quaternion RHS)
        {
            return LHS + (-RHS);
        }

        public override string ToString()
        {
            return string.Format("X: {0} Y: {1} Z: {2} W: {3}", X, Y, Z, W);
        }

        public static float Dot(Quaternion LHS, Quaternion RHS)
        {
            float X = LHS.X * RHS.X;
            float Y = LHS.Y * RHS.Y;
            float Z = LHS.Z * RHS.Z;
            float W = LHS.W * RHS.W;

            return X + Y + Z + W;
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
