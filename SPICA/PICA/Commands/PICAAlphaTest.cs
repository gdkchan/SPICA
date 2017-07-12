namespace SPICA.PICA.Commands
{
    public struct PICAAlphaTest
    {
        public bool Enabled;

        public PICATestFunc Function;

        public byte Reference;

        public PICAAlphaTest(uint Param)
        {
            Enabled = (Param & 1) != 0;

            Function = (PICATestFunc)((Param >> 4) & 7);

            Reference = (byte)(Param >> 8);
        }

        public uint ToUInt32()
        {
            uint Param = 0;

            Param |= Enabled ? 1u : 0u;
            
            Param |= ((uint)Function & 7) << 4;

            Param |= (uint)Reference << 8;

            return Param;
        }
    }
}
