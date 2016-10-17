namespace SPICA.PICA.Commands
{
    struct PICATexEnvOperand
    {
        public PICATextureCombinerRGBOp[] RGBOp;
        public PICATextureCombinerAlphaOp[] AlphaOp;

        public PICATexEnvOperand(uint Param)
        {
            RGBOp = new PICATextureCombinerRGBOp[3];
            
            RGBOp[0] = (PICATextureCombinerRGBOp)((Param >> 0) & 0xf);
            RGBOp[1] = (PICATextureCombinerRGBOp)((Param >> 4) & 0xf);
            RGBOp[2] = (PICATextureCombinerRGBOp)((Param >> 8) & 0xf);

            AlphaOp = new PICATextureCombinerAlphaOp[3];

            AlphaOp[0] = (PICATextureCombinerAlphaOp)((Param >> 12) & 7);
            AlphaOp[1] = (PICATextureCombinerAlphaOp)((Param >> 16) & 7);
            AlphaOp[2] = (PICATextureCombinerAlphaOp)((Param >> 20) & 7);
        }
    }
}
