namespace SPICA.PICA.Commands
{
    class PICADepthColorMask
    {
        public bool Enabled;
        public PICATestFunc DepthFunc;
        public bool RedWrite;
        public bool GreenWrite;
        public bool BlueWrite;
        public bool AlphaWrite;
        public bool DepthWrite;

        public static PICADepthColorMask FromParameter(uint Param)
        {
            PICADepthColorMask Output = new PICADepthColorMask();

            Output.Enabled = (Param & 1) != 0;
            Output.DepthFunc = (PICATestFunc)((Param >> 4) & 7);
            Output.RedWrite = (Param & 0x100) != 0;
            Output.GreenWrite = (Param & 0x200) != 0;
            Output.BlueWrite = (Param & 0x400) != 0;
            Output.AlphaWrite = (Param & 0x800) != 0;
            Output.DepthWrite = (Param & 0x1000) != 0;

            return Output;
        }
    }
}
