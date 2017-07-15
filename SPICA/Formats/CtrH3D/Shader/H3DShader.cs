using SPICA.Formats.Common;

namespace SPICA.Formats.CtrH3D.Shader
{
    public class H3DShader : INamed
    {
        public byte[] Program;

        public uint[] ShaderAllCommands;
        public uint[] ShaderCommands;
        public uint[] ShaderSetupCommands;

        public short VtxShaderIndex;
        public short GeoShaderIndex;

        public readonly H3DShaderBinding Binding;

        private string _Name;

        public string Name
        {
            get => _Name;
            set => _Name = value ?? throw Exceptions.GetNullException("Name");
        }

        public H3DShader()
        {
            Binding = new H3DShaderBinding();
        }
    }
}
