namespace SPICA.PICA.Commands
{
    public struct PICALUTInputSel
    {
        public PICALUTInput Dist0Input;
        public PICALUTInput Dist1Input;
        public PICALUTInput SpecularInput;
        public PICALUTInput FresnelInput;
        public PICALUTInput ReflecRInput;
        public PICALUTInput ReflecGInput;
        public PICALUTInput ReflecBInput;

        public PICALUTInputSel(uint Param)
        {
            Dist0Input = (PICALUTInput)((Param >> 0) & 7);
            Dist1Input = (PICALUTInput)((Param >> 4) & 7);
            SpecularInput = (PICALUTInput)((Param >> 8) & 7);
            FresnelInput = (PICALUTInput)((Param >> 12) & 7);
            ReflecRInput = (PICALUTInput)((Param >> 16) & 7);
            ReflecGInput = (PICALUTInput)((Param >> 20) & 7);
            ReflecBInput = (PICALUTInput)((Param >> 24) & 7);
        }

        public uint ToUInt32()
        {
            uint Param = 0;

            Param |= ((uint)Dist0Input & 7) << 0;
            Param |= ((uint)Dist1Input & 7) << 4;
            Param |= ((uint)SpecularInput & 7) << 8;
            Param |= ((uint)FresnelInput & 7) << 12;
            Param |= ((uint)ReflecRInput & 7) << 16;
            Param |= ((uint)ReflecGInput & 7) << 20;
            Param |= ((uint)ReflecBInput & 7) << 24;

            return Param;
        }
    }
}
