namespace SPICA.Math3D
{
    struct RGBAFloat
    {
        public float R;
        public float G;
        public float B;
        public float A;

        public RGBAFloat(float R, float G, float B, float A)
        {
            this.R = R;
            this.G = G;
            this.B = B;
            this.A = A;
        }

        public override string ToString()
        {
            return string.Format("R: {0} G: {1} B: {2} A: {3}", R, G, B, A);
        }
    }
}
