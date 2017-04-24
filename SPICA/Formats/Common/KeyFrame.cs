namespace SPICA.Formats.Common
{
    public struct KeyFrame
    {
        public float Frame    { get; set; }
        public float Value    { get; set; }
        public float InSlope  { get; set; }
        public float OutSlope { get; set; }

        public KeyFrame(float Frame, float Value, float InSlope, float OutSlope)
        {
            this.Frame    = Frame;
            this.Value    = Value;
            this.InSlope  = InSlope;
            this.OutSlope = OutSlope;
        }

        public KeyFrame(float Frame, float Value, float Slope)
        {
            this.Frame = Frame;
            this.Value = Value;
            InSlope    = Slope;
            OutSlope   = Slope;
        }

        public KeyFrame(float Frame, float Value)
        {
            this.Frame = Frame;
            this.Value = Value;
            InSlope    = 0;
            OutSlope   = 0;
        }
    }
}
