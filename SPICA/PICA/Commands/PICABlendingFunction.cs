namespace SPICA.PICA.Commands
{
    class PICABlendingFunction
    {
        public PICABlendingEquation RGBEquation;
        public PICABlendingEquation AlphaEquation;

        public PICABlendingFunc RGBSourceFunc;
        public PICABlendingFunc RGBDestFunc;

        public PICABlendingFunc AlphaSourceFunc;
        public PICABlendingFunc AlphaDestFunc;

        public static PICABlendingFunction FromParameter(uint Param)
        {
            PICABlendingFunction Output = new PICABlendingFunction();

            Output.RGBEquation = (PICABlendingEquation)((Param >> 0) & 7);
            Output.AlphaEquation = (PICABlendingEquation)((Param >> 8) & 7);

            Output.RGBSourceFunc = (PICABlendingFunc)((Param >> 16) & 0xf);
            Output.RGBDestFunc = (PICABlendingFunc)((Param >> 20) & 0xf);

            Output.AlphaSourceFunc = (PICABlendingFunc)((Param >> 24) & 0xf);
            Output.AlphaDestFunc = (PICABlendingFunc)((Param >> 28) & 0xf);

            return Output;
        }
    }
}
