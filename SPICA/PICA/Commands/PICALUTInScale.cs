namespace SPICA.PICA.Commands
{
    public struct PICALUTInScale
    {
        public PICALUTScale Dist0;
        public PICALUTScale Dist1;
        public PICALUTScale Specular;
        public PICALUTScale Fresnel;
        public PICALUTScale ReflecR;
        public PICALUTScale ReflecG;
        public PICALUTScale ReflecB;

        public PICALUTInScale(uint Param)
        {
            Dist0    = (PICALUTScale)((Param >> 0)  & 7);
            Dist1    = (PICALUTScale)((Param >> 4)  & 7);
            Specular = (PICALUTScale)((Param >> 8)  & 7);
            Fresnel  = (PICALUTScale)((Param >> 12) & 7);
            ReflecR  = (PICALUTScale)((Param >> 16) & 7);
            ReflecG  = (PICALUTScale)((Param >> 20) & 7);
            ReflecB  = (PICALUTScale)((Param >> 24) & 7);
        }

        public uint ToUInt32()
        {
            uint Param = 0;

            Param |= ((uint)Dist0    & 7) << 0;
            Param |= ((uint)Dist1    & 7) << 4;
            Param |= ((uint)Specular & 7) << 8;
            Param |= ((uint)Fresnel  & 7) << 12;
            Param |= ((uint)ReflecR  & 7) << 16;
            Param |= ((uint)ReflecG  & 7) << 20;
            Param |= ((uint)ReflecB  & 7) << 24;

            return Param;
        }
    }
}
