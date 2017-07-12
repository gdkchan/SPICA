namespace SPICA.PICA.Shader
{
    public class ShaderUniform
    {
        public bool IsConstant;
        public bool IsArray;

        public int ArrayIndex;
        public int ArrayLength = 1;

        public string Name;
    }
}
