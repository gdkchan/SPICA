namespace SPICA.PICA.Commands
{
    public struct PICAFragmentAlphaTest
    {
        public bool Enabled;
        public PICATestFunc Function;
        public byte Reference;

        public PICAFragmentAlphaTest(uint Param)
        {
            Enabled = (Param & 1) != 0;
            Function = (PICATestFunc)((Param >> 4) & 7);
            Reference = (byte)(Param >> 8);
        }
    }
}
