namespace SPICA.PICA.Commands
{
    public struct PICABlendingFunction
    {
        public PICABlendingEquation RGBEquation;
        public PICABlendingEquation AlphaEquation;

        public PICABlendingFunc RGBSourceFunc;
        public PICABlendingFunc RGBDestFunc;

        public PICABlendingFunc AlphaSourceFunc;
        public PICABlendingFunc AlphaDestFunc;

        public PICABlendingFunction(uint Param)
        {
            RGBEquation = (PICABlendingEquation)((Param >> 0) & 7);
            AlphaEquation = (PICABlendingEquation)((Param >> 8) & 7);

            RGBSourceFunc = (PICABlendingFunc)((Param >> 16) & 0xf);
            RGBDestFunc = (PICABlendingFunc)((Param >> 20) & 0xf);

            AlphaSourceFunc = (PICABlendingFunc)((Param >> 24) & 0xf);
            AlphaDestFunc = (PICABlendingFunc)((Param >> 28) & 0xf);
        }
    }
}
