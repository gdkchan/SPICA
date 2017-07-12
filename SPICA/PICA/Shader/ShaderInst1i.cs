namespace SPICA.PICA.Shader
{
    public struct ShaderInst1i
    {
        public uint Dest;
        public uint Idx2;
        public uint Src1;
        public uint Src2;
        public uint Desc;

        public ShaderInst1i(uint Inst)
        {
            Dest = (Inst >> 21) & 0x1f;
            Idx2 = (Inst >> 19) & 0x3;
            Src1 = (Inst >> 14) & 0x1f;
            Src2 = (Inst >>  7) & 0x7f;
            Desc = (Inst >>  0) & 0x7f;
        }
    }
}
