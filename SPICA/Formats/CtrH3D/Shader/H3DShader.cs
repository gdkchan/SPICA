using SPICA.Formats.Common;

namespace SPICA.Formats.CtrH3D.Shader
{
    public class H3DShader : INamed
    {
        public byte[] Program;

        //Those seems to be always null?
        private uint[] ShaderAllCommands;
        private uint[] ShaderProgramCommands;
        private uint[] ShaderSetupCommands;

        public short VtxShaderIndex;
        public short GeoShaderIndex;

        private uint BindingAddress; //SBZ?

        private string _Name;

        public string Name
        {
            get => _Name;
            set => _Name = value ?? throw Exceptions.GetNullException("Name");
        }

        private uint UserDefinedAddress; //SBZ
    }
}
