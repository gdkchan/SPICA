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
