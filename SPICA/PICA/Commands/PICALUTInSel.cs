namespace SPICA.PICA.Commands
{
    public struct PICALUTInSel
    {
        public PICALUTInput Dist0;
        public PICALUTInput Dist1;
        public PICALUTInput Specular;
        public PICALUTInput Fresnel;
        public PICALUTInput ReflecR;
        public PICALUTInput ReflecG;
        public PICALUTInput ReflecB;

        public PICALUTInSel(uint Param)
        {
            Dist0    = (PICALUTInput)((Param >> 0)  & 7);
            Dist1    = (PICALUTInput)((Param >> 4)  & 7);
            Specular = (PICALUTInput)((Param >> 8)  & 7);
            Fresnel  = (PICALUTInput)((Param >> 12) & 7);
            ReflecR  = (PICALUTInput)((Param >> 16) & 7);
            ReflecG  = (PICALUTInput)((Param >> 20) & 7);
            ReflecB  = (PICALUTInput)((Param >> 24) & 7);
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
