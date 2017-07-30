using SPICA.Math3D;
using SPICA.PICA;
using SPICA.PICA.Commands;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrGfx.Model.Material
{
    [TypeChoice(0x80000000u, typeof(GfxTextureMapper))]
    public class GfxTextureMapper : ICustomSerialization
    {
        private uint DynamicAlloc;

        public   GfxTextureReference Texture;
        internal GfxTextureSampler   Sampler;

        [Inline, FixedLength(14)] private uint[] Commands;

        private int CommandsLength;

        [Ignore] public RGBA BorderColor;

        [Ignore] public PICATextureFilter MagFilter;
        [Ignore] public PICATextureFilter MinFilter;
        [Ignore] public PICATextureFilter MipFilter;

        [Ignore] public PICATextureWrap WrapU;
        [Ignore] public PICATextureWrap WrapV;

        [Ignore] public float LODBias;
        [Ignore] public byte  MinLOD;

        [Ignore] internal int MapperIndex;

        public GfxTextureMapper()
        {
            Texture = new GfxTextureReference();
            Sampler = new GfxTextureSamplerStd() { Parent = this };
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            PICACommandReader Reader = new PICACommandReader(Commands);

            int Index = 0;

            while (Reader.HasCommand && Index++ < 5)
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

                        WrapV = (PICATextureWrap)((Param >>  8) & 3);
                        WrapU = (PICATextureWrap)((Param >> 12) & 3);
                        break;

                    case PICARegister.GPUREG_TEXUNIT0_LOD:
                    case PICARegister.GPUREG_TEXUNIT1_LOD:
                    case PICARegister.GPUREG_TEXUNIT2_LOD:
                        LODBias = (((int)Param << 20) >> 20) / (float)0x100;
                        MinLOD  = (byte)((Param >> 24) & 0xf); 
                        break;
                }
            }
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            PICARegister[] Cmd0 = new PICARegister[]
            {
                PICARegister.GPUREG_TEXUNIT0_TYPE,
                PICARegister.GPUREG_TEXUNIT1_TYPE,
                PICARegister.GPUREG_TEXUNIT2_TYPE
            };

            PICARegister[] Cmd1 = new PICARegister[]
            {
                PICARegister.GPUREG_TEXUNIT0_BORDER_COLOR,
                PICARegister.GPUREG_TEXUNIT1_BORDER_COLOR,
                PICARegister.GPUREG_TEXUNIT2_BORDER_COLOR
            };

            PICACommandWriter Writer = new PICACommandWriter();

            uint Filter;

            Filter  = ((uint)MagFilter & 1) <<  1;
            Filter |= ((uint)MinFilter & 1) <<  2;
            Filter |= ((uint)MipFilter & 1) << 24;

            Filter |= ((uint)WrapV & 7) <<  8;
            Filter |= ((uint)WrapU & 7) << 12;

            uint LOD;

            LOD  = (uint)((int)(LODBias * 0x100) & 0x1fff);
            LOD |= ((uint)MinLOD & 0xf) << 24;

            Writer.SetCommand(Cmd0[MapperIndex], 0, 1);
            Writer.SetCommand(Cmd1[MapperIndex], true,
                BorderColor.ToUInt32(),
                0,
                Filter,
                LOD,
                0,
                0,
                0,
                0,
                0,
                0);

            Commands = Writer.GetBuffer();

            CommandsLength = Commands.Length * 4;

            //TODO: Don't assume it uses the Standard sampler, could be the Shadow sampler too
            GfxTextureSamplerStd Samp = (GfxTextureSamplerStd)Sampler;

            Samp.LODBias     = LODBias;
            Samp.BorderColor = BorderColor.ToVector4();
            Samp.MinFilter   = GetMinFilter();

            return false;
        }

        internal GfxTextureMinFilter GetMinFilter()
        {
            switch ((uint)MagFilter | ((uint)MipFilter << 1))
            {
                case 0: return GfxTextureMinFilter.NearestMipmapNearest;
                case 1: return GfxTextureMinFilter.LinearMipmapNearest;
                case 2: return GfxTextureMinFilter.NearestMipmapLinear;
                case 3: return GfxTextureMinFilter.LinearMipmapLinear;
            }

            return 0;
        }
    }
}
