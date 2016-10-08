namespace SPICA.PICA.Commands
{
    class PICATexEnvOperand
    {
        public PICATextureCombinerRGBOp[] RGBOp;
        public PICATextureCombinerAlphaOp[] AlphaOp;

        public PICATexEnvOperand()
        {
            RGBOp = new PICATextureCombinerRGBOp[3];
            AlphaOp = new PICATextureCombinerAlphaOp[3];
        }

        public static PICATexEnvOperand FromParameter(uint Param)
        {
            PICATexEnvOperand Output = new PICATexEnvOperand();

            Output.RGBOp[0] = (PICATextureCombinerRGBOp)((Param >> 0) & 0xf);
            Output.RGBOp[1] = (PICATextureCombinerRGBOp)((Param >> 4) & 0xf);
            Output.RGBOp[2] = (PICATextureCombinerRGBOp)((Param >> 8) & 0xf);

            Output.AlphaOp[0] = (PICATextureCombinerAlphaOp)((Param >> 12) & 7);
            Output.AlphaOp[1] = (PICATextureCombinerAlphaOp)((Param >> 16) & 7);
            Output.AlphaOp[2] = (PICATextureCombinerAlphaOp)((Param >> 20) & 7);

            return Output;
        }
    }
}
