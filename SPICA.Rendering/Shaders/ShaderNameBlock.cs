namespace SPICA.Rendering.Shaders
{
    public class ShaderNameBlock
    {
        public readonly string[] Vec4Uniforms;
        public readonly string[] IVec4Uniforms;
        public readonly string[] BoolUniforms;
        public readonly string[] Inputs;
        public readonly string[] Outputs;

        public ShaderNameBlock()
        {
            Vec4Uniforms  = new string[96];
            IVec4Uniforms = new string[4];
            BoolUniforms  = new string[16];
            Inputs        = new string[16];
            Outputs       = new string[16];
        }

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