namespace SPICA.Formats.GFL2.Motion
{
    struct GFMotKeyFrame
    {
        public byte Frame;
        public float Value;
        public float Slope;

        public GFMotKeyFrame(byte Frame, float Value, float Slope)
        {
            this.Frame = Frame;
            this.Value = Value;
            this.Slope = Slope;
        }
    }
}
