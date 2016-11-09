namespace SPICA.PICA.Commands
{
    public struct PICALUTInputAbs
    {
        public bool Dist0Abs;
        public bool Dist1Abs;
        public bool SpecularAbs;
        public bool FresnelAbs;
        public bool ReflecRAbs;
        public bool ReflecGAbs;
        public bool ReflecBAbs;

        public PICALUTInputAbs(uint Param)
        {
            Dist0Abs = (Param & 0x2) == 0;
            Dist1Abs = (Param & 0x20) == 0;
            SpecularAbs = (Param & 0x200) == 0;
            FresnelAbs = (Param & 0x2000) == 0;
            ReflecRAbs = (Param & 0x20000) == 0;
            ReflecGAbs = (Param & 0x200000) == 0;
            ReflecBAbs = (Param & 0x2000000) == 0;
        }

        public uint ToUInt32()
        {
            uint Param = 0;

            Param |= (Dist0Abs ? 0u : 1u) << 1;
            Param |= (Dist1Abs ? 0u : 1u) << 5;
            Param |= (SpecularAbs ? 0u : 1u) << 9;
            Param |= (FresnelAbs ? 0u : 1u) << 13;
            Param |= (ReflecRAbs ? 0u : 1u) << 17;
            Param |= (ReflecGAbs ? 0u : 1u) << 21;
            Param |= (ReflecBAbs ? 0u : 1u) << 25;

            return Param;
        }
    }
}
