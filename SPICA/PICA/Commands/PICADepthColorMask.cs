namespace SPICA.PICA.Commands
{
    struct PICADepthColorMask
    {
        public bool Enabled;
        public PICATestFunc DepthFunc;
        public bool RedWrite;
        public bool GreenWrite;
        public bool BlueWrite;
        public bool AlphaWrite;
        public bool DepthWrite;

        public PICADepthColorMask(uint Param)
        {
            Enabled = (Param & 1) != 0;
            DepthFunc = (PICATestFunc)((Param >> 4) & 7);
            RedWrite = (Param & 0x100) != 0;
            GreenWrite = (Param & 0x200) != 0;
            BlueWrite = (Param & 0x400) != 0;
            AlphaWrite = (Param & 0x800) != 0;
            DepthWrite = (Param & 0x1000) != 0;
        }
    }
}
