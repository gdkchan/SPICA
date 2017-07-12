using SPICA.Formats.Common;

using System.Numerics;

namespace SPICA.PICA.Commands
{
    public struct PICAVectorFloat24
    {
        public float X
        {
            get => GetFloat24(Word2 & 0xffffff);
            set => CalculateWords(value, Y, Z, W);
        }

        public float Y
        {
            get => GetFloat24((Word2 >> 24) | ((Word1 & 0xffff) << 8));
            set => CalculateWords(X, value, Z, W);
        }

        public float Z
        {
            get => GetFloat24((Word1 >> 16) | ((Word0 & 0xff) << 16));
            set => CalculateWords(X, Y, value, W);
        }

        public float W
        {
            get => GetFloat24(Word0 >> 8);
            set => CalculateWords(X, Y, Z, value);
        }

        internal uint Word0;
        internal uint Word1;
        internal uint Word2;

        public PICAVectorFloat24(float Value) : this(Value, Value, Value, Value) { }

        public PICAVectorFloat24(float X, float Y, float Z, float W)
        {
            Word0 = Word1 = Word2 = 0;

            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.W = W;
        }

        public override bool Equals(object obj)
        {
            return obj is PICAVectorFloat24 && (PICAVectorFloat24)obj == this;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ W.GetHashCode();
        }

        public static bool operator ==(PICAVectorFloat24 LHS, PICAVectorFloat24 RHS)
        {
            return LHS.X == RHS.X && LHS.Y == RHS.Y && LHS.Z == RHS.Z && LHS.W == RHS.W;
        }

        public static bool operator !=(PICAVectorFloat24 LHS, PICAVectorFloat24 RHS)
        {
            return !(LHS == RHS);
        }

        public static implicit operator Vector4(PICAVectorFloat24 Value)
        {
            return new Vector4(Value.X, Value.Y, Value.Z, Value.W);
        }

        public static PICAVectorFloat24 operator +(PICAVectorFloat24 LHS, PICAVectorFloat24 RHS)
        {
            return new PICAVectorFloat24(LHS.X + RHS.X, LHS.Y + RHS.Y, LHS.Z + RHS.Z, LHS.W + RHS.W);
        }

        public static PICAVectorFloat24 operator *(PICAVectorFloat24 LHS, PICAVectorFloat24 RHS)
        {
            return new PICAVectorFloat24(LHS.X * RHS.X, LHS.Y * RHS.Y, LHS.Z * RHS.Z, LHS.W * RHS.W);
        }

        public static PICAVectorFloat24 operator /(PICAVectorFloat24 LHS, PICAVectorFloat24 RHS)
        {
            return new PICAVectorFloat24(LHS.X / RHS.X, LHS.Y / RHS.Y, LHS.Z / RHS.Z, LHS.W / RHS.W);
        }

        public static PICAVectorFloat24 operator -(PICAVectorFloat24 Vector)
        {
            return new PICAVectorFloat24(-Vector.X, -Vector.Y, -Vector.Z, -Vector.W);
        }

        public static PICAVectorFloat24 operator -(PICAVectorFloat24 LHS, PICAVectorFloat24 RHS)
        {
            return LHS + (-RHS);
        }

        public static PICAVectorFloat24 operator *(PICAVectorFloat24 LHS, float RHS)
        {
            return new PICAVectorFloat24(LHS.X * RHS, LHS.Y * RHS, LHS.Z * RHS, LHS.W * RHS);
        }

        public static PICAVectorFloat24 operator /(PICAVectorFloat24 LHS, float RHS)
        {
            return new PICAVectorFloat24(LHS.X / RHS, LHS.Y / RHS, LHS.Z / RHS, LHS.W / RHS);
        }

        public override string ToString()
        {
            return $"X: {X} Y: {Y} Z: {Z} W: {W}";
        }

        private void CalculateWords(float X, float Y, float Z, float W)
        {
            uint WX = GetWord24(X);
            uint WY = GetWord24(Y);
            uint WZ = GetWord24(Z);
            uint WW = GetWord24(W);

            Word0 = (WW <<  8) | (WZ >> 16);
            Word1 = (WZ << 16) | (WY >>  8);
            Word2 = (WY << 24) | (WX >>  0);
        }

        internal static float GetFloat24(uint Value)
        {
            uint Float;

            if ((Value & 0x7fffff) != 0)
            {
                uint Mantissa = Value & 0xffff;
                uint Exponent = ((Value >> 16) & 0x7f) + 64;
                uint SignBit = (Value >> 23) & 1;

                Float  = Mantissa << 7;
                Float |= Exponent << 23;
                Float |= SignBit << 31;
            }
            else
            {
                Float = (Value & 0x800000) << 8;
            }

            return IOUtils.ToSingle(Float);
        }

        internal static uint GetWord24(float Value)
        {
            uint Word = IOUtils.ToUInt32(Value);

            if ((Word & 0x7fffffff) != 0)
            {
                uint Mantissa = Word & 0x7fffff;
                uint Exponent = ((Word >> 23) & 0xff) - 64;
                uint SignBit = Word >> 31;

                Word  = Mantissa >> 7;
                Word |= (Exponent & 0x7f) << 16;
                Word |= SignBit << 23;
            }
            else
            {
                Word >>= 8;
            }

            return Word;
        }
    }
}
