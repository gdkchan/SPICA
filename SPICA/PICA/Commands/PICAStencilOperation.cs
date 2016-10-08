namespace SPICA.PICA.Commands
{
    class PICAStencilOperation
    {
        public PICAStencilOp FailOp;
        public PICAStencilOp ZFailOp;
        public PICAStencilOp ZPassOp;

        public static PICAStencilOperation FromParameter(uint Param)
        {
            PICAStencilOperation Output = new PICAStencilOperation();

            Output.FailOp = (PICAStencilOp)((Param >> 0) & 7);
            Output.ZFailOp = (PICAStencilOp)((Param >> 4) & 7);
            Output.ZPassOp = (PICAStencilOp)((Param >> 8) & 7);

            return Output;
        }
    }
}
