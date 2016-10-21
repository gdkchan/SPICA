namespace SPICA.PICA.Commands
{
    public struct PICAColorOperation
    {
        public PICAFragmentOpMode FragmentOpMode;
        public PICABlendingMode BlendingMode;

        public PICAColorOperation(uint Param)
        {
            FragmentOpMode = (PICAFragmentOpMode)((Param >> 0) & 3);
            BlendingMode = (PICABlendingMode)((Param >> 8) & 1);
        }
    }
}
