namespace SPICA.PICA.Commands
{
    public struct PICATexEnvScale
    {
        public PICATextureCombinerScale RGBScale;
        public PICATextureCombinerScale AlphaScale;

        public PICATexEnvScale(uint Param)
        {
            RGBScale = (PICATextureCombinerScale)((Param >> 0) & 3);
            AlphaScale = (PICATextureCombinerScale)((Param >> 16) & 3);
        }

        public uint ToUInt32()
        {
            uint Param = 0;

            Param |= ((uint)RGBScale & 3) << 0;
            Param |= ((uint)AlphaScale & 3) << 16;

            return Param;
        }
    }
}
