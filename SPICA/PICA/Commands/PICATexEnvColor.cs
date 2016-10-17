namespace SPICA.PICA.Commands
{
    struct PICATexEnvColor
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public PICATexEnvColor(uint Param)
        {
            R = (byte)(Param >> 0);
            G = (byte)(Param >> 8);
            B = (byte)(Param >> 16);
            A = (byte)(Param >> 24);
        }
    }
}
