namespace SPICA.PICA.Commands
{
    class PICATexEnvScale
    {
        public PICATextureCombinerScale RGBScale;
        public PICATextureCombinerScale AlphaScale;

        public static PICATexEnvScale FromParameter(uint Param)
        {
            PICATexEnvScale Output = new PICATexEnvScale();

            Output.RGBScale = (PICATextureCombinerScale)((Param >> 0) & 3);
            Output.AlphaScale = (PICATextureCombinerScale)((Param >> 16) & 3);

            return Output;
        }
    }
}
