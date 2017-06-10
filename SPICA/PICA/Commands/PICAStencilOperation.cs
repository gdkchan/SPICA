namespace SPICA.PICA.Commands
{
    public struct PICAStencilOperation
    {
        public PICAStencilOp FailOp;
        public PICAStencilOp ZFailOp;
        public PICAStencilOp ZPassOp;

        public PICAStencilOperation(uint Param)
        {
            FailOp  = (PICAStencilOp)((Param >> 0) & 7);
            ZFailOp = (PICAStencilOp)((Param >> 4) & 7);
            ZPassOp = (PICAStencilOp)((Param >> 8) & 7);
        }

        public uint ToUInt32()
        {
            uint Param = 0;

            Param |= ((uint)FailOp  & 7) << 0;
            Param |= ((uint)ZFailOp & 7) << 4;
            Param |= ((uint)ZPassOp & 7) << 8;

            return Param;
        }
    }
}
