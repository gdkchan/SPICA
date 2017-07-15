namespace SPICA.PICA.Shader
{
    public struct ShaderInst3
    {
        public uint RegId;
        public uint Dest;
        public uint Count;

        public ShaderInst3(uint Inst)
        {
            RegId = (Inst >> 22) & 0xf;
            Dest  = (Inst >> 10) & 0xfff;
            Count = (Inst >>  0) & 0xff;
        }
    }
}
