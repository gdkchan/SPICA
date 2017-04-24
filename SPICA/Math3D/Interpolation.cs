namespace SPICA.Math3D
{
    static class Interpolation
    {
        public static float Lerp(float LHS, float RHS, float Weight)
        {
            return LHS * (1 - Weight) + RHS * Weight;
        }

        public static float Herp(float LHS, float RHS, float LS, float RS, float Diff, float Weight)
        {
            float Result;

            Result = LHS + (LHS - RHS) * (2 * Weight - 3) * Weight * Weight;
            Result += (Diff * (Weight - 1)) * (LS * (Weight - 1) + RS * Weight);

            return Result;
        }
    }
}
