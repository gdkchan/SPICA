using SPICA.Formats.Common;

using System;
using System.Xml.Serialization;

namespace SPICA.PICA.Commands
{
    public struct PICAVectorFloat24
    {
        [XmlAttribute] public float X;
        [XmlAttribute] public float Y;
        [XmlAttribute] public float Z;
        [XmlAttribute] public float W;

        private uint _Word0;
        private uint _Word1;
        private uint _Word2;

        internal uint Word0
        {
            get { CalculateWords(); return _Word0; }
            set { _Word0 = value; }
        }

        internal uint Word1
        {
            get { return _Word1; }
            set { _Word1 = value;  }
        }

        internal uint Word2
        {
            get { return _Word2; }
            set { _Word2 = value; CalculateFloats(); }
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

                    default: throw new ArgumentOutOfRangeException("Expected 0-3 (X-W) range!");
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

                    default: throw new ArgumentOutOfRangeException("Expected 0-3 (X-W) range!");
                }
            }
        }

        public PICAVectorFloat24(float Value) : this(Value, Value, Value, Value) { }

        public PICAVectorFloat24(float X, float Y, float Z, float W)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.W = W;

            _Word0 = _Word1 = _Word2 = 0;

            CalculateWords();
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

        private void CalculateFloats()
        {
            X = GetFloat24(_Word2 & 0xffffff);
            Y = GetFloat24((_Word2 >> 24) | ((_Word1 & 0xffff) << 8));
            Z = GetFloat24((_Word1 >> 16) | ((_Word0 & 0xff) << 16));
            W = GetFloat24(_Word0 >> 8);
        }

        private void CalculateWords()
        {
            uint WX = GetWord24(X);
            uint WY = GetWord24(Y);
            uint WZ = GetWord24(Z);
            uint WW = GetWord24(W);

            _Word0 = (WW << 8) | (WZ >> 16);
            _Word1 = (WZ << 16) | (WY >> 8);
            _Word2 = (WY << 24) | WX;
        }

        private float GetFloat24(uint Value)
        {
            uint Float;

            if ((Value & 0x7fffff) != 0)
            {
                uint Mantissa = Value & 0xffff;
                uint Exponent = ((Value >> 16) & 0x7f) + 64;
                uint SignBit = (Value >> 23) & 1;

                Float = Mantissa << 7;
                Float |= Exponent << 23;
                Float |= SignBit << 31;
            }
            else
            {
                Float = (Value & 0x800000) << 8;
            }

            return IOUtils.ToSingle(Float);
        }

        private uint GetWord24(float Value)
        {
            uint Word = IOUtils.ToUInt32(Value);

            if ((Word & 0x7fffffff) != 0)
            {
                uint Mantissa = Word & 0x7fffff;
                uint Exponent = ((Word >> 23) & 0xff) - 64;
                uint SignBit = Word >> 31;

                Word = Mantissa >> 7;
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
