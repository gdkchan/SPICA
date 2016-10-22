namespace SPICA.PICA.Commands
{
    public struct PICALUTInputScaleSel
    {
        public PICALUTInputScale Dist0Scale;
        public PICALUTInputScale Dist1Scale;
        public PICALUTInputScale SpecularScale;
        public PICALUTInputScale FresnelScale;
        public PICALUTInputScale ReflecRScale;
        public PICALUTInputScale ReflecGScale;
        public PICALUTInputScale ReflecBScale;

        public PICALUTInputScaleSel(uint Param)
        {
            Dist0Scale = (PICALUTInputScale)((Param >> 0) & 7);
            Dist1Scale = (PICALUTInputScale)((Param >> 4) & 7);
            SpecularScale = (PICALUTInputScale)((Param >> 8) & 7);
            FresnelScale = (PICALUTInputScale)((Param >> 12) & 7);
            ReflecRScale = (PICALUTInputScale)((Param >> 16) & 7);
            ReflecGScale = (PICALUTInputScale)((Param >> 20) & 7);
            ReflecBScale = (PICALUTInputScale)((Param >> 24) & 7);
        }

        public uint ToUInt32()
        {
            uint Param = 0;

            Param |= ((uint)Dist0Scale & 7) << 0;
            Param |= ((uint)Dist1Scale & 7) << 4;
            Param |= ((uint)SpecularScale & 7) << 8;
            Param |= ((uint)FresnelScale & 7) << 12;
            Param |= ((uint)ReflecRScale & 7) << 16;
            Param |= ((uint)ReflecGScale & 7) << 20;
            Param |= ((uint)ReflecBScale & 7) << 24;

            return Param;
        }
    }
}
