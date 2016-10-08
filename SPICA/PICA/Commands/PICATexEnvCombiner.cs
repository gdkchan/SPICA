namespace SPICA.PICA.Commands
{
    class PICATexEnvCombiner
    {
        public PICATextureCombinerMode RGBCombiner;
        public PICATextureCombinerMode AlphaCombiner;

        public static PICATexEnvCombiner FromParameter(uint Param)
        {
            PICATexEnvCombiner Output = new PICATexEnvCombiner();

            Output.RGBCombiner = (PICATextureCombinerMode)((Param >> 0) & 0xf);
            Output.AlphaCombiner = (PICATextureCombinerMode)((Param >> 16) & 0xf);

            return Output;
        }
    }
}
