namespace SPICA.PICA.Commands
{
    struct PICATexEnvScale
    {
        public PICATextureCombinerScale RGBScale;
        public PICATextureCombinerScale AlphaScale;

        public PICATexEnvScale(uint Param)
        {
            RGBScale = (PICATextureCombinerScale)((Param >> 0) & 3);
            AlphaScale = (PICATextureCombinerScale)((Param >> 16) & 3);
        }
    }
}
