namespace SPICA.PICA.Commands
{
    public class PICATexEnvOperand
    {
        public PICATextureCombinerRGBOp[] RGBOp;
        public PICATextureCombinerAlphaOp[] AlphaOp;

        public PICATexEnvOperand()
        {
            RGBOp = new PICATextureCombinerRGBOp[3];
            AlphaOp = new PICATextureCombinerAlphaOp[3];
        }

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

        public uint ToUInt32()
        {
            uint Param = 0;

            Param |= ((uint)RGBOp[0] & 0xf) << 0;
            Param |= ((uint)RGBOp[1] & 0xf) << 4;
            Param |= ((uint)RGBOp[2] & 0xf) << 8;

            Param |= ((uint)AlphaOp[0] & 0xf) << 12;
            Param |= ((uint)AlphaOp[1] & 0xf) << 16;
            Param |= ((uint)AlphaOp[2] & 0xf) << 20;

            return Param;
        }
    }
}
