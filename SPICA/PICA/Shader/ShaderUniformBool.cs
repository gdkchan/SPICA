namespace SPICA.PICA.Shader
{
    public class ShaderUniformBool : ShaderUniform
    {
        public bool Constant;

        public ShaderUniformBool() { }

        public ShaderUniformBool(bool Constant)
        {
            this.Constant = Constant;

            IsConstant = true;
        }
    }
}
