namespace SPICA.PICA.Commands
{
    public class PICATexEnvSource
    {
        public PICATextureCombinerSource[] RGBSource;
        public PICATextureCombinerSource[] AlphaSource;

        public PICATexEnvSource()
        {
            RGBSource = new PICATextureCombinerSource[3];
            AlphaSource = new PICATextureCombinerSource[3];
        }

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

        public uint ToUInt32()
        {
            uint Param = 0;

            Param |= ((uint)RGBSource[0] & 0xf) << 0;
            Param |= ((uint)RGBSource[1] & 0xf) << 4;
            Param |= ((uint)RGBSource[2] & 0xf) << 8;

            Param |= ((uint)AlphaSource[0] & 0xf) << 16;
            Param |= ((uint)AlphaSource[1] & 0xf) << 20;
            Param |= ((uint)AlphaSource[2] & 0xf) << 24;

            return Param;
        }
    }
}
