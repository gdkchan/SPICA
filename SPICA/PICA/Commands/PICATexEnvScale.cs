namespace SPICA.PICA.Commands
{
    public struct PICATexEnvScale
    {
        public PICATextureCombinerScale Color;
        public PICATextureCombinerScale Alpha;

        public PICATexEnvScale(uint Param)
        {
            Color = (PICATextureCombinerScale)((Param >> 0) & 3);
            Alpha = (PICATextureCombinerScale)((Param >> 16) & 3);
        }

        public uint ToUInt32()
        {
            uint Param = 0;

            Param |= ((uint)Color & 3) << 0;
            Param |= ((uint)Alpha & 3) << 16;

            return Param;
        }
    }
}
