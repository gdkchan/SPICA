namespace SPICA.PICA.Commands
{
    public struct PICATexEnvCombiner
    {
        public PICATextureCombinerMode RGBCombiner;
        public PICATextureCombinerMode AlphaCombiner;

        public PICATexEnvCombiner(uint Param)
        {
            RGBCombiner = (PICATextureCombinerMode)((Param >> 0) & 0xf);
            AlphaCombiner = (PICATextureCombinerMode)((Param >> 16) & 0xf);
        }

        public uint ToUInt32()
        {
            uint Param = 0;

            Param |= ((uint)RGBCombiner & 0xf) << 0;
            Param |= ((uint)AlphaCombiner & 0xf) << 16;

            return Param;
        }
    }
}
