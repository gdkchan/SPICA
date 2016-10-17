namespace SPICA.PICA.Commands
{
    struct PICATexEnvSource
    {
        public PICATextureCombinerSource[] RGBSource;
        public PICATextureCombinerSource[] AlphaSource;

        public PICATexEnvSource(uint Param)
        {
            RGBSource = new PICATextureCombinerSource[3];

            RGBSource[0] = (PICATextureCombinerSource)((Param >> 0) & 0xf);
            RGBSource[1] = (PICATextureCombinerSource)((Param >> 4) & 0xf);
            RGBSource[2] = (PICATextureCombinerSource)((Param >> 8) & 0xf);

            AlphaSource = new PICATextureCombinerSource[3];

            AlphaSource[0] = (PICATextureCombinerSource)((Param >> 16) & 0xf);
            AlphaSource[1] = (PICATextureCombinerSource)((Param >> 20) & 0xf);
            AlphaSource[2] = (PICATextureCombinerSource)((Param >> 24) & 0xf);
        }
    }
}
