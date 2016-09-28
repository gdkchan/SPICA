namespace SPICA.PICA
{
    struct PICACommand
    {
        public PICARegister Register;
        public uint[] Parameters;
        public int ParametersIndex;
        public uint Mask;
    }
}
