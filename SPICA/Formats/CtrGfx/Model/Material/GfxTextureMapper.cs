using SPICA.Math3D;
using SPICA.PICA;
using SPICA.PICA.Commands;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrGfx.Model.Material
{
    public class GfxTextureMapper : ICustomSerialization
    {
        private uint Unk;

        private uint DynamicAllocPtr;

        public GfxTextureReference Texture;

        public readonly GfxTextureSampler Sampler;

        [Inline, FixedLength(14)] private uint[] Commands;

        [Ignore] public RGBA BorderColor;

        [Ignore] public PICATextureFilter MagFilter;
        [Ignore] public PICATextureFilter MinFilter;
        [Ignore] public PICATextureFilter MipFilter;

        [Ignore] public PICATextureWrap WrapU;
        [Ignore] public PICATextureWrap WrapV;

        [Ignore] public float LODBias;

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            PICACommandReader Reader = new PICACommandReader(Commands);

            while (Reader.HasCommand)
            {
                PICACommand Cmd = Reader.GetCommand();

                uint Param = Cmd.Parameters[0];

                switch (Cmd.Register)
                {
                    case PICARegister.GPUREG_TEXUNIT0_BORDER_COLOR:
                    case PICARegister.GPUREG_TEXUNIT1_BORDER_COLOR:
                    case PICARegister.GPUREG_TEXUNIT2_BORDER_COLOR:
                        BorderColor = new RGBA(Param);
                        break;

                    case PICARegister.GPUREG_TEXUNIT0_PARAM:
                    case PICARegister.GPUREG_TEXUNIT1_PARAM:
                    case PICARegister.GPUREG_TEXUNIT2_PARAM:
                        MagFilter = (PICATextureFilter)((Param >>  1) & 1);
                        MinFilter = (PICATextureFilter)((Param >>  2) & 1);
                        MipFilter = (PICATextureFilter)((Param >> 24) & 1);

                        WrapV = (PICATextureWrap)((Param >>  8) & 7);
                        WrapU = (PICATextureWrap)((Param >> 12) & 7);
                        break;

                    case PICARegister.GPUREG_TEXUNIT0_LOD:
                    case PICARegister.GPUREG_TEXUNIT1_LOD:
                    case PICARegister.GPUREG_TEXUNIT2_LOD:
                        LODBias = (((int)Param << 20) >> 20) / (float)0xff;
                        break;
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
