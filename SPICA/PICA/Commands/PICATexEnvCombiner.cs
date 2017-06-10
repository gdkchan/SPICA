namespace SPICA.PICA.Commands
{
    public struct PICATexEnvCombiner
    {
        public PICATextureCombinerMode Color;
        public PICATextureCombinerMode Alpha;

        public PICATexEnvCombiner(uint Param)
        {
            Color = (PICATextureCombinerMode)((Param >>  0) & 0xf);
            Alpha = (PICATextureCombinerMode)((Param >> 16) & 0xf);
        }

        public uint ToUInt32()
        {
            uint Param = 0;

            Param |= ((uint)Color & 0xf) <<  0;
            Param |= ((uint)Alpha & 0xf) << 16;

            return Param;
        }
    }
}
