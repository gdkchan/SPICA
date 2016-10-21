namespace SPICA.PICA.Commands
{
    public struct PICAStencilTest
    {
        public bool Enabled;
        public PICATestFunc Function;
        public byte BufferMask;
        public sbyte Reference;
        public byte Mask;

        public PICAStencilTest(uint Param)
        {
            Enabled = (Param & 1) != 0;
            Function = (PICATestFunc)((Param >> 4) & 7);
            BufferMask = (byte)(Param >> 8);
            Reference = (sbyte)(Param >> 16);
            Mask = (byte)(Param >> 24);
        }
    }
}
