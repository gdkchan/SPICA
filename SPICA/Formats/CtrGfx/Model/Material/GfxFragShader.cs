using SPICA.Math3D;
using SPICA.PICA;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

using System.Numerics;

namespace SPICA.Formats.CtrGfx.Model.Material
{
    public class GfxFragShader : ICustomSerialization
    {
        private Vector4 TexEnvBufferColorF;

        public GfxFragLight Lighting;

        public GfxFragLightLUTs LUTs;

        [Inline, FixedLength(6)] public readonly GfxTexEnv[] TextureEnvironments;

        public GfxAlphaTest AlphaTest;

        [Inline, FixedLength(6)] private uint[] Commands;

        [Ignore] public RGBA TexEnvBufferColor;

        public GfxFragShader()
        {
            LUTs = new GfxFragLightLUTs();

            TextureEnvironments = new GfxTexEnv[6];
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            PICACommandReader Reader = new PICACommandReader(Commands);

            while (Reader.HasCommand)
            {
                PICACommand Cmd = Reader.GetCommand();

                uint Param = Cmd.Parameters[0];

                switch (Cmd.Register)
                {
                    case PICARegister.GPUREG_TEXENV_UPDATE_BUFFER:
                        TextureEnvironments[1].Stage.UpdateColorBuffer = (Param & 0x100)  != 0;
                        TextureEnvironments[2].Stage.UpdateColorBuffer = (Param & 0x200)  != 0;
                        TextureEnvironments[3].Stage.UpdateColorBuffer = (Param & 0x400)  != 0;
                        TextureEnvironments[4].Stage.UpdateColorBuffer = (Param & 0x800)  != 0;

                        TextureEnvironments[1].Stage.UpdateAlphaBuffer = (Param & 0x1000) != 0;
                        TextureEnvironments[2].Stage.UpdateAlphaBuffer = (Param & 0x2000) != 0;
                        TextureEnvironments[3].Stage.UpdateAlphaBuffer = (Param & 0x4000) != 0;
                        TextureEnvironments[4].Stage.UpdateAlphaBuffer = (Param & 0x8000) != 0;
                        break;

                    case PICARegister.GPUREG_TEXENV_BUFFER_COLOR: TexEnvBufferColor = new RGBA(Param); break;
                }
            }
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            //TODO

            return false;
        }
    }
}
