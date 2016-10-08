namespace SPICA.PICA.Commands
{
    class PICATexEnvColor
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public static PICATexEnvColor FromParameter(uint Param)
        {
            PICATexEnvColor Output = new PICATexEnvColor();

            Output.R = (byte)(Param >> 0);
            Output.G = (byte)(Param >> 8);
            Output.B = (byte)(Param >> 16);
            Output.A = (byte)(Param >> 24);

            return Output;
        }
    }
}
