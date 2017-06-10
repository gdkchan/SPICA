namespace SPICA.PICA.Commands
{
    public class PICATexEnvOperand
    {
        public PICATextureCombinerColorOp[] Color;
        public PICATextureCombinerAlphaOp[] Alpha;

        public PICATexEnvOperand()
        {
            Color = new PICATextureCombinerColorOp[3];
            Alpha = new PICATextureCombinerAlphaOp[3];
        }

        public PICATexEnvOperand(uint Param)
        {
            Color = new PICATextureCombinerColorOp[3];

            Color[0] = (PICATextureCombinerColorOp)((Param >> 0) & 0xf);
            Color[1] = (PICATextureCombinerColorOp)((Param >> 4) & 0xf);
            Color[2] = (PICATextureCombinerColorOp)((Param >> 8) & 0xf);

            Alpha = new PICATextureCombinerAlphaOp[3];

            Alpha[0] = (PICATextureCombinerAlphaOp)((Param >> 12) & 7);
            Alpha[1] = (PICATextureCombinerAlphaOp)((Param >> 16) & 7);
            Alpha[2] = (PICATextureCombinerAlphaOp)((Param >> 20) & 7);
        }

        public uint ToUInt32()
        {
            uint Param = 0;

            Param |= ((uint)Color[0] & 0xf) << 0;
            Param |= ((uint)Color[1] & 0xf) << 4;
            Param |= ((uint)Color[2] & 0xf) << 8;

            Param |= ((uint)Alpha[0] & 0xf) << 12;
            Param |= ((uint)Alpha[1] & 0xf) << 16;
            Param |= ((uint)Alpha[2] & 0xf) << 20;

            return Param;
        }
    }
}
