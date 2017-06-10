namespace SPICA.PICA.Commands
{
    public struct PICALUTInAbs
    {
        public bool Dist0;
        public bool Dist1;
        public bool Specular;
        public bool Fresnel;
        public bool ReflecR;
        public bool ReflecG;
        public bool ReflecB;

        public PICALUTInAbs(uint Param)
        {
            Dist0    = (Param & 0x00000002) == 0;
            Dist1    = (Param & 0x00000020) == 0;
            Specular = (Param & 0x00000200) == 0;
            Fresnel  = (Param & 0x00002000) == 0;
            ReflecR  = (Param & 0x00020000) == 0;
            ReflecG  = (Param & 0x00200000) == 0;
            ReflecB  = (Param & 0x02000000) == 0;
        }

        public uint ToUInt32()
        {
            uint Param = 0;

            Param |= (Dist0    ? 0u : 1u) << 1;
            Param |= (Dist1    ? 0u : 1u) << 5;
            Param |= (Specular ? 0u : 1u) << 9;
            Param |= (Fresnel  ? 0u : 1u) << 13;
            Param |= (ReflecR  ? 0u : 1u) << 17;
            Param |= (ReflecG  ? 0u : 1u) << 21;
            Param |= (ReflecB  ? 0u : 1u) << 25;

            return Param;
        }
    }
}
