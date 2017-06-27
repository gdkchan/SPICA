namespace SPICA.PICA.Commands
{
    public struct PICADepthColorMask
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

            RedWrite   = (Param & 0x0100) != 0;
            GreenWrite = (Param & 0x0200) != 0;
            BlueWrite  = (Param & 0x0400) != 0;
            AlphaWrite = (Param & 0x0800) != 0;
            DepthWrite = (Param & 0x1000) != 0;
        }

        public uint ToUInt32()
        {
            uint Param = 0;

            Param |= (Enabled ? 1u : 0u) << 0;

            Param |= ((uint)DepthFunc & 7) << 4;

            Param |= (RedWrite   ? 1u : 0u) << 8;
            Param |= (GreenWrite ? 1u : 0u) << 9;
            Param |= (BlueWrite  ? 1u : 0u) << 10;
            Param |= (AlphaWrite ? 1u : 0u) << 11;
            Param |= (DepthWrite ? 1u : 0u) << 12;

            return Param;
        }
    }
}
