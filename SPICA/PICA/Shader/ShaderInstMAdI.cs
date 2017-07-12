namespace SPICA.PICA.Shader
{
    public struct ShaderInstMAdI
    {
        public uint Dest;
        public uint Idx3;
        public uint Src1;
        public uint Src2;
        public uint Src3;
        public uint Desc;

        public ShaderInstMAdI(uint Inst)
        {
            Dest = (Inst >> 24) & 0x1f;
            Idx3 = (Inst >> 22) & 0x3;
            Src1 = (Inst >> 17) & 0x1f;
            Src2 = (Inst >> 12) & 0x1f;
            Src3 = (Inst >>  5) & 0x7f;
            Desc = (Inst >>  0) & 0x1f;
        }
    }
}
