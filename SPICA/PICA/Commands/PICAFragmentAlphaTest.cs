namespace SPICA.PICA.Commands
{
    class PICAFragmentAlphaTest
    {
        public bool Enabled;
        public PICATestFunc Function;
        public byte Reference;

        public static PICAFragmentAlphaTest FromParameter(uint Param)
        {
            PICAFragmentAlphaTest Output = new PICAFragmentAlphaTest();

            Output.Enabled = (Param & 1) != 0;
            Output.Function = (PICATestFunc)((Param >> 4) & 7);
            Output.Reference = (byte)(Param >> 8);

            return Output;
        }
    }
}
