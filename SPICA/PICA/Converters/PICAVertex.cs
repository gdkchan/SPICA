using SPICA.Math3D;

using System;
using System.Linq;

namespace SPICA.PICA.Converters
{
    public class PICAVertex : ICloneable
    {
        public Vector3D Position;

        public Vector3D Normal;

        public Vector3D Tangent;

        public RGBAFloat Color;

        public Vector2D TextureCoord0;
        public Vector2D TextureCoord1;
        public Vector2D TextureCoord2;

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

            Output.Position = Position;
            Output.Normal = Normal;
            Output.Tangent = Tangent;
            Output.Color = Color;
            Output.TextureCoord0 = TextureCoord0;
            Output.TextureCoord1 = TextureCoord1;
            Output.TextureCoord2 = TextureCoord2;

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
            Hash ^= TextureCoord0.GetHashCode();
            Hash ^= TextureCoord1.GetHashCode();
            Hash ^= TextureCoord2.GetHashCode();
            
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

            Equals &= LHS.Position == RHS.Position;
            Equals &= LHS.Normal == RHS.Normal;
            Equals &= LHS.Tangent == RHS.Tangent;
            Equals &= LHS.Color == RHS.Color;
            Equals &= LHS.TextureCoord0 == RHS.TextureCoord0;
            Equals &= LHS.TextureCoord1 == RHS.TextureCoord1;
            Equals &= LHS.TextureCoord2 == RHS.TextureCoord2;

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
