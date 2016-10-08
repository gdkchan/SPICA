namespace SPICA.PICA.Commands
{
    class PICATexEnvSource
    {
        public PICATextureCombinerSource[] RGBSource;
        public PICATextureCombinerSource[] AlphaSource;

        public PICATexEnvSource()
        {
            RGBSource = new PICATextureCombinerSource[3];
            AlphaSource = new PICATextureCombinerSource[3];
        }

        public static PICATexEnvSource FromParameter(uint Param)
        {
            PICATexEnvSource Output = new PICATexEnvSource();

            Output.RGBSource[0] = (PICATextureCombinerSource)((Param >> 0) & 0xf);
            Output.RGBSource[1] = (PICATextureCombinerSource)((Param >> 4) & 0xf);
            Output.RGBSource[2] = (PICATextureCombinerSource)((Param >> 8) & 0xf);

            Output.AlphaSource[0] = (PICATextureCombinerSource)((Param >> 16) & 0xf);
            Output.AlphaSource[1] = (PICATextureCombinerSource)((Param >> 20) & 0xf);
            Output.AlphaSource[2] = (PICATextureCombinerSource)((Param >> 24) & 0xf);

            return Output;
        }
    }
}
