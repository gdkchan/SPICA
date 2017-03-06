namespace SPICA.Formats.CtrH3D.Animation
{
    public struct H3DFloatKeyFrame
    {
        public float Frame;
        public float Value;
        public float InSlope;
        public float OutSlope;

        public H3DFloatKeyFrame(float Frame, float Value, float InSlope, float OutSlope)
        {
            this.Frame    = Frame;
            this.Value    = Value;
            this.InSlope  = InSlope;
            this.OutSlope = OutSlope;
        }

        public H3DFloatKeyFrame(float Frame, float Value)
        {
            this.Frame = Frame;
            this.Value = Value;

            InSlope = OutSlope = 0;
        }
    }
}
