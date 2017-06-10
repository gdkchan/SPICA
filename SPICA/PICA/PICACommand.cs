namespace SPICA.PICA
{
    struct PICACommand
    {
        public PICARegister Register;
        public uint[]       Parameters;
        public uint         Mask;
    }
}
