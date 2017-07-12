namespace SPICA.PICA.Shader
{
    public struct ShaderInst1c
    {
        public uint CmpX;
        public uint CmpY;
        public uint Idx1;
        public uint Src1;
        public uint Src2;
        public uint Desc;

        public ShaderInst1c(uint Inst)
        {
            CmpX = (Inst >> 24) & 0x7;
            CmpY = (Inst >> 21) & 0x7;
            Idx1 = (Inst >> 19) & 0x3;
            Src1 = (Inst >> 12) & 0x7f;
            Src2 = (Inst >>  7) & 0x1f;
            Desc = (Inst >>  0) & 0x7f;
        }
    }
}
