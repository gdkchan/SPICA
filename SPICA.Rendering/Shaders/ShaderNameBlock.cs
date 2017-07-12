namespace SPICA.Rendering.Shaders
{
    public struct ShaderNameBlock
    {
        public readonly string[] Vec4s;
        public readonly string[] IVec4s;
        public readonly string[] Bools;
        public readonly string[] Inputs;
        public readonly string[] Outputs;

        public ShaderNameBlock(
            string[] Vec4UniformNames,
            string[] IVec4UniformNames,
            string[] BoolUniformNames,
            string[] InputNames,
            string[] OutputNames)
        {
            Vec4s   = Vec4UniformNames;
            IVec4s  = IVec4UniformNames;
            Bools   = BoolUniformNames;
            Inputs  = InputNames;
            Outputs = OutputNames;
        }
    }
}