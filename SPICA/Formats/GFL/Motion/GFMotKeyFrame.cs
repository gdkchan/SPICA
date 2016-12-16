namespace SPICA.Formats.GFL.Motion
{
    struct GFMotKeyFrame
    {
        public int Frame;
        public float Value;
        public float Slope;

        public GFMotKeyFrame(int Frame, float Value, float Slope = 0)
        {
            this.Frame = Frame;
            this.Value = Value;
            this.Slope = Slope;
        }
    }
}
