namespace SPICA.PICA.Commands
{
    public struct PICABlendFunction
    {
        public PICABlendEquation ColorEquation;
        public PICABlendEquation AlphaEquation;

        public PICABlendFunc ColorSrcFunc;
        public PICABlendFunc ColorDstFunc;

        public PICABlendFunc AlphaSrcFunc;
        public PICABlendFunc AlphaDstFunc;

        public PICABlendFunction(uint Param)
        {
            ColorEquation = (PICABlendEquation)((Param >> 0) & 7);
            AlphaEquation = (PICABlendEquation)((Param >> 8) & 7);

            ColorSrcFunc = (PICABlendFunc)((Param >> 16) & 0xf);
            ColorDstFunc = (PICABlendFunc)((Param >> 20) & 0xf);

            AlphaSrcFunc = (PICABlendFunc)((Param >> 24) & 0xf);
            AlphaDstFunc = (PICABlendFunc)((Param >> 28) & 0xf);
        }

        public uint ToUInt32()
        {
            uint Param = 0;

            Param |= ((uint)ColorEquation & 7) << 0;
            Param |= ((uint)AlphaEquation & 7) << 8;

            Param |= ((uint)ColorSrcFunc & 0xf) << 16;
            Param |= ((uint)ColorDstFunc & 0xf) << 20;

            Param |= ((uint)AlphaSrcFunc & 0xf) << 24;
            Param |= ((uint)AlphaDstFunc & 0xf) << 28;

            return Param;
        }
    }
}
