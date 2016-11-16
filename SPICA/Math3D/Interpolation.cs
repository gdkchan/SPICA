namespace SPICA.Math3D
{
    static class Interpolation
    {
        public static float Lerp(float L, float R, float W)
        {
            return L * (1 - W) + R * W;
        }

        public static float Herp(float L, float R, float LS, float RS, float Diff, float W)
        {
            float W1 = W - 1;

            float Result;

            Result = L + (L - R) * (2 * W - 3) * W * W;
            Result += (Diff * W1) * (LS * W1 + RS * W);

            return Result;
        }
    }
}
