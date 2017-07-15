namespace SPICA.Rendering.Shaders
{
    public struct ShaderNameBlock
    {
        public readonly string[] Vec4Uniforms;
        public readonly string[] IVec4Uniforms;
        public readonly string[] BoolUniforms;
        public readonly string[] Inputs;
        public readonly string[] Outputs;

        public ShaderNameBlock(
            string[] Vec4UniformNames,
            string[] IVec4UniformNames,
            string[] BoolUniformNames,
            string[] InputNames,
            string[] OutputNames)
        {
            Vec4Uniforms  = Vec4UniformNames;
            IVec4Uniforms = IVec4UniformNames;
            BoolUniforms  = BoolUniformNames;
            Inputs        = InputNames;
            Outputs       = OutputNames;
        }
    }
}