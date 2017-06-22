using SPICA.Formats.CtrH3D.Texture;
using SPICA.Serialization.Serializer;

using System;

namespace SPICA.Formats.CtrH3D
{
    static class H3DComparers
    {
        public static Comparison<RefValue> GetComparisonStr()
        {
            return new Comparison<RefValue>(CompareString);
        }

        public static Comparison<RefValue> GetComparisonRaw()
        {
            return new Comparison<RefValue>(CompareBuffer);
        }

        public static int CompareString(RefValue LHS, RefValue RHS)
        {
            return CompareString(LHS.Value.ToString(), RHS.Value.ToString());
        }

        public static int CompareBuffer(RefValue LHS, RefValue RHS)
        {
            if      (LHS.Parent == RHS.Parent)
                return  0;
            else if (LHS.Parent is H3DTexture)
                return -1;
            else
                return  1;
        }

        public static int CompareString(string LHS, string RHS)
        {
            int MinLength = Math.Min(LHS.Length, RHS.Length);

            for (int Index = 0; Index < MinLength; Index++)
            {
                byte L = (byte)LHS[Index];
                byte R = (byte)RHS[Index];

                if (L != R) return L < R ? -1 : 1;
            }

            if      (LHS.Length == RHS.Length)
                return  0;
            else if (LHS.Length <  RHS.Length)
                return -1;
            else
                return  1;
        }
    }
}
