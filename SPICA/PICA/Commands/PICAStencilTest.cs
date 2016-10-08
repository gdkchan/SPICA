namespace SPICA.PICA.Commands
{
    class PICAStencilTest
    {
        public bool Enabled;
        public PICATestFunc Function;
        public byte BufferMask;
        public sbyte Reference;
        public byte Mask;

        public static PICAStencilTest FromParameter(uint Param)
        {
            PICAStencilTest Output = new PICAStencilTest();

            Output.Enabled = (Param & 1) != 0;
            Output.Function = (PICATestFunc)((Param >> 4) & 7);
            Output.BufferMask = (byte)(Param >> 8);
            Output.Reference = (sbyte)(Param >> 16);
            Output.Mask = (byte)(Param >> 24);

            return Output;
        }
    }
}
