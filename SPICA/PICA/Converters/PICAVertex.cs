using System;
using System.Linq;
using System.Numerics;

namespace SPICA.PICA.Converters
{
    public class PICAVertex : ICloneable
    {
        public Vector4 Position;
        public Vector4 Normal;
        public Vector4 Tangent;
        public Vector4 Color;
        public Vector4 TexCoord0;
        public Vector4 TexCoord1;
        public Vector4 TexCoord2;

        public int[] Indices;
        public float[] Weights;

        public PICAVertex()
        {
            Indices = new int[4];
            Weights = new float[4];
        }

        public PICAVertex Clone()
        {
            PICAVertex Output = new PICAVertex();

            Output.Position  = Position;
            Output.Normal    = Normal;
            Output.Tangent   = Tangent;
            Output.Color     = Color;
            Output.TexCoord0 = TexCoord0;
            Output.TexCoord1 = TexCoord1;
            Output.TexCoord2 = TexCoord2;

            for (int Index = 0; Index < 4; Index++)
            {
                Output.Indices[Index] = Indices[Index];
                Output.Weights[Index] = Weights[Index];
            }

            return Output;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public override bool Equals(object obj)
        {
            return obj is PICAVertex && (PICAVertex)obj == this;
        }

        public override int GetHashCode()
        {
            int Hash = 0;

            Hash ^= Position.GetHashCode();
            Hash ^= Tangent.GetHashCode();
            Hash ^= Color.GetHashCode();
            Hash ^= TexCoord0.GetHashCode();
            Hash ^= TexCoord1.GetHashCode();
            Hash ^= TexCoord2.GetHashCode();
            
            for (int Index = 0; Index < 4; Index++)
            {
                Hash ^= Indices[Index].GetHashCode();
                Hash ^= Weights[Index].GetHashCode();
            }

            return Hash;
        }

        public static bool operator ==(PICAVertex LHS, PICAVertex RHS)
        {
            bool Equals = true;

            Equals &= LHS.Position  == RHS.Position;
            Equals &= LHS.Normal    == RHS.Normal;
            Equals &= LHS.Tangent   == RHS.Tangent;
            Equals &= LHS.Color     == RHS.Color;
            Equals &= LHS.TexCoord0 == RHS.TexCoord0;
            Equals &= LHS.TexCoord1 == RHS.TexCoord1;
            Equals &= LHS.TexCoord2 == RHS.TexCoord2;

            Equals &= LHS.Indices.SequenceEqual(RHS.Indices);
            Equals &= LHS.Weights.SequenceEqual(RHS.Weights);

            return Equals;
        }

        public static bool operator !=(PICAVertex LHS, PICAVertex RHS)
        {
            return !(LHS == RHS);
        }
    }
}
