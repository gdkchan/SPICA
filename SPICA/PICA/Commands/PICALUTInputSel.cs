namespace SPICA.PICA.Commands
{
    class PICALUTInputSel
    {
        public PICALUTInput Dist0Input;
        public PICALUTInput Dist1Input;
        public PICALUTInput SpecularInput;
        public PICALUTInput FresnelInput;
        public PICALUTInput ReflecRInput;
        public PICALUTInput ReflecGInput;
        public PICALUTInput ReflecBInput;

        public static PICALUTInputSel FromParameter(uint Param)
        {
            PICALUTInputSel Output = new PICALUTInputSel();

            Output.Dist0Input = (PICALUTInput)((Param >> 0) & 7);
            Output.Dist1Input = (PICALUTInput)((Param >> 4) & 7);
            Output.SpecularInput = (PICALUTInput)((Param >> 8) & 7);
            Output.FresnelInput = (PICALUTInput)((Param >> 12) & 7);
            Output.ReflecRInput = (PICALUTInput)((Param >> 16) & 7);
            Output.ReflecGInput = (PICALUTInput)((Param >> 20) & 7);
            Output.ReflecBInput = (PICALUTInput)((Param >> 24) & 7);

            return Output;
        }
    }
}
