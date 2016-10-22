namespace SPICA.PICA.Commands
{
    public struct PICABlendFunction
    {
        public PICABlendEquation RGBEquation;
        public PICABlendEquation AlphaEquation;

        public PICABlendFunc RGBSourceFunc;
        public PICABlendFunc RGBDestFunc;

        public PICABlendFunc AlphaSourceFunc;
        public PICABlendFunc AlphaDestFunc;

        public PICABlendFunction(uint Param)
        {
            RGBEquation = (PICABlendEquation)((Param >> 0) & 7);
            AlphaEquation = (PICABlendEquation)((Param >> 8) & 7);

            RGBSourceFunc = (PICABlendFunc)((Param >> 16) & 0xf);
            RGBDestFunc = (PICABlendFunc)((Param >> 20) & 0xf);

            AlphaSourceFunc = (PICABlendFunc)((Param >> 24) & 0xf);
            AlphaDestFunc = (PICABlendFunc)((Param >> 28) & 0xf);
        }

        public uint ToUInt32()
        {
            uint Param = 0;

            Param |= ((uint)RGBEquation & 7) << 0;
            Param |= ((uint)AlphaEquation & 7) << 8;

            Param |= ((uint)RGBSourceFunc & 0xf) << 16;
            Param |= ((uint)RGBDestFunc & 0xf) << 20;

            Param |= ((uint)AlphaSourceFunc & 0xf) << 24;
            Param |= ((uint)AlphaDestFunc & 0xf) << 28;

            return Param;
        }
    }
}
