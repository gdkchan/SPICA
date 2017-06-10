namespace SPICA.PICA.Commands
{
    public class PICATexEnvSource
    {
        public PICATextureCombinerSource[] Color;
        public PICATextureCombinerSource[] Alpha;

        public PICATexEnvSource()
        {
            Color = new PICATextureCombinerSource[3];
            Alpha = new PICATextureCombinerSource[3];
        }

        public PICATexEnvSource(uint Param)
        {
            Color = new PICATextureCombinerSource[3];

            Color[0] = (PICATextureCombinerSource)((Param >> 0) & 0xf);
            Color[1] = (PICATextureCombinerSource)((Param >> 4) & 0xf);
            Color[2] = (PICATextureCombinerSource)((Param >> 8) & 0xf);

            Alpha = new PICATextureCombinerSource[3];

            Alpha[0] = (PICATextureCombinerSource)((Param >> 16) & 0xf);
            Alpha[1] = (PICATextureCombinerSource)((Param >> 20) & 0xf);
            Alpha[2] = (PICATextureCombinerSource)((Param >> 24) & 0xf);
        }

        public uint ToUInt32()
        {
            uint Param = 0;

            Param |= ((uint)Color[0] & 0xf) << 0;
            Param |= ((uint)Color[1] & 0xf) << 4;
            Param |= ((uint)Color[2] & 0xf) << 8;

            Param |= ((uint)Alpha[0] & 0xf) << 16;
            Param |= ((uint)Alpha[1] & 0xf) << 20;
            Param |= ((uint)Alpha[2] & 0xf) << 24;

            return Param;
        }
    }
}
