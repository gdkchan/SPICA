namespace SPICA.PICA.Commands
{
    class PICAColorOperation
    {
        public PICAFragmentOpMode FragmentOpMode;
        public PICABlendingMode BlendingMode;

        public static PICAColorOperation FromParameter(uint Param)
        {
            PICAColorOperation Output = new PICAColorOperation();

            Output.FragmentOpMode = (PICAFragmentOpMode)((Param >> 0) & 3);
            Output.BlendingMode = (PICABlendingMode)((Param >> 8) & 1);

            return Output;
        }
    }
}
