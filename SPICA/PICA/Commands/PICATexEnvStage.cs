namespace SPICA.PICA.Commands
{
    class PICATexEnvStage
    {
        public PICATexEnvSource Source;
        public PICATexEnvOperand Operand;
        public PICATexEnvCombiner Combiner;
        public PICATexEnvColor Color;
        public PICATexEnvScale Scale;

        public PICATexEnvStage()
        {
            Source = new PICATexEnvSource();
            Operand = new PICATexEnvOperand();
            Combiner = new PICATexEnvCombiner();
            Color = new PICATexEnvColor();
            Scale = new PICATexEnvScale();
        }
    }
}
