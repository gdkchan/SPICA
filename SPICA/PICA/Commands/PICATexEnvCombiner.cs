namespace SPICA.PICA.Commands
{
    struct PICATexEnvCombiner
    {
        public PICATextureCombinerMode RGBCombiner;
        public PICATextureCombinerMode AlphaCombiner;

        public PICATexEnvCombiner(uint Param)
        {
            RGBCombiner = (PICATextureCombinerMode)((Param >> 0) & 0xf);
            AlphaCombiner = (PICATextureCombinerMode)((Param >> 16) & 0xf);
        }
    }
}
