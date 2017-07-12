namespace SPICA.PICA.Shader
{
    public struct ShaderInst1
    {
        public uint Dest;
        public uint Idx1;
        public uint Src1;
        public uint Src2;
        public uint Desc;

        public ShaderInst1(uint Inst)
        {
            Dest = (Inst >> 21) & 0x1f;
            Idx1 = (Inst >> 19) & 0x3;
            Src1 = (Inst >> 12) & 0x7f;
            Src2 = (Inst >>  7) & 0x1f;
            Desc = (Inst >>  0) & 0x7f;
        }
    }
}
