namespace SPICA.PICA.Shader
{
    public struct ShaderInstMAd
    {
        public uint Dest;
        public uint Idx2;
        public uint Src1;
        public uint Src2;
        public uint Src3;
        public uint Desc;

        public ShaderInstMAd(uint Inst)
        {
            Dest = (Inst >> 24) & 0x1f;
            Idx2 = (Inst >> 22) & 0x3;
            Src1 = (Inst >> 17) & 0x1f;
            Src2 = (Inst >> 10) & 0x7f;
            Src3 = (Inst >>  5) & 0x1f;
            Desc = (Inst >>  0) & 0x1f;
        }
    }
}
