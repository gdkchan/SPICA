namespace SPICA.PICA.Shader
{
    public struct ShaderInst2
    {
        public bool RefX;
        public bool RefY;

        public uint CondOp;
        public uint Dest;
        public uint Count;

        public ShaderInst2(uint Inst)
        {
            RefX   = ((Inst >> 25) & 1) != 0;
            RefY   = ((Inst >> 24) & 1) != 0;

            CondOp = (Inst >> 22) & 0x3;
            Dest   = (Inst >> 10) & 0xfff;
            Count  = (Inst >>  0) & 0xff;
        }
    }
}
