namespace SPICA.Formats.GFL.Motion
{
    public struct GF1MotKeyFrame
    {
        public int   Frame;
        public float Value;
        public float Slope;

        public GF1MotKeyFrame(int Frame, float Value, float Slope = 0)
        {
            this.Frame = Frame;
            this.Value = Value;
            this.Slope = Slope;
        }
    }
}
