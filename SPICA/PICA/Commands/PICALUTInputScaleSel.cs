namespace SPICA.PICA.Commands
{
    class PICALUTInputScaleSel
    {
        public PICALUTInputScale Dist0Scale;
        public PICALUTInputScale Dist1Scale;
        public PICALUTInputScale SpecularScale;
        public PICALUTInputScale FresnelScale;
        public PICALUTInputScale ReflecRScale;
        public PICALUTInputScale ReflecGScale;
        public PICALUTInputScale ReflecBScale;

        public static PICALUTInputScaleSel FromParameter(uint Param)
        {
            PICALUTInputScaleSel Output = new PICALUTInputScaleSel();

            Output.Dist0Scale = (PICALUTInputScale)((Param >> 0) & 7);
            Output.Dist1Scale = (PICALUTInputScale)((Param >> 4) & 7);
            Output.SpecularScale = (PICALUTInputScale)((Param >> 8) & 7);
            Output.FresnelScale = (PICALUTInputScale)((Param >> 12) & 7);
            Output.ReflecRScale = (PICALUTInputScale)((Param >> 16) & 7);
            Output.ReflecGScale = (PICALUTInputScale)((Param >> 20) & 7);
            Output.ReflecBScale = (PICALUTInputScale)((Param >> 24) & 7);

            return Output;
        }
    }
}
