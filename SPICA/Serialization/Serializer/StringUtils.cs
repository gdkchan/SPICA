using System;

namespace SPICA.Serialization.Serializer
{
    class StringUtils
    {
        public static int CompareString(RefValue LHS, RefValue RHS)
        {
            return CompareString((string)LHS.Value, (string)RHS.Value);
        }

        public static int CompareString(string LHS, string RHS)
        {
            for (int Index = 0; Index < Math.Min(LHS.Length, RHS.Length); Index++)
            {
                byte L = (byte)LHS[Index];
                byte R = (byte)RHS[Index];

                if (L != R) return L < R ? -1 : 1;
            }

            if (LHS.Length == RHS.Length)
            {
                return 0;
            }
            else if (LHS.Length < RHS.Length)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }
}
