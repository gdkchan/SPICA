namespace SPICA.PICA.Commands
{
    struct PICAStencilOperation
    {
        public PICAStencilOp FailOp;
        public PICAStencilOp ZFailOp;
        public PICAStencilOp ZPassOp;

        public PICAStencilOperation(uint Param)
        {
            FailOp = (PICAStencilOp)((Param >> 0) & 7);
            ZFailOp = (PICAStencilOp)((Param >> 4) & 7);
            ZPassOp = (PICAStencilOp)((Param >> 8) & 7);
        }
    }
}
