namespace SPICA.PICA.Commands
{
    class PICALUTInputAbs
    {
        public bool Dist0Abs;
        public bool Dist1Abs;
        public bool SpecularAbs;
        public bool FresnelAbs;
        public bool ReflecRAbs;
        public bool ReflecGAbs;
        public bool ReflecBAbs;

        public static PICALUTInputAbs FromParameter(uint Param)
        {
            PICALUTInputAbs Output = new PICALUTInputAbs();

            Output.Dist0Abs = (Param & 0x2) != 0;
            Output.Dist1Abs = (Param & 0x20) != 0;
            Output.SpecularAbs = (Param & 0x200) != 0;
            Output.FresnelAbs = (Param & 0x2000) != 0;
            Output.ReflecRAbs = (Param & 0x20000) != 0;
            Output.ReflecGAbs = (Param & 0x200000) != 0;
            Output.ReflecBAbs = (Param & 0x2000000) != 0;

            return Output;
        }
    }
}
