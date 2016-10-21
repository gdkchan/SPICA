using SPICA.Formats.H3D.Model.Material.Texture;
using SPICA.Math3D;
using SPICA.PICA;
using SPICA.PICA.Commands;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;
using SPICA.Utils;

using System;

namespace SPICA.Formats.H3D.Model.Material
{
    public class H3DMaterialParams : ICustomSerialization, INamed
    {
        public uint UniqueId;

        public H3DMaterialFlags Flags;
        public H3DFragmentFlags FragmentFlags;

        public byte FrameAccessOffset;

        public H3DMaterialShader MaterialShader;

        [Inline, FixedLength(3)]
        public H3DTextureCoord[] TextureCoords;

        public ushort LightSetIndex;
        public ushort FogIndex;

        public RGBA EmissionColor;
        public RGBA AmbientColor;
        public RGBA DiffuseColor;
        public RGBA Specular0Color;
        public RGBA Specular1Color;
        public RGBA Constant0Color;
        public RGBA Constant1Color;
        public RGBA Constant2Color;
        public RGBA Constant3Color;
        public RGBA Constant4Color;
        public RGBA Constant5Color;
        public RGBA BlendColor;

        public float ColorScale;

        public H3DMaterialLUT Dist0LUT;
        public H3DMaterialLUT Dist1LUT;
        public H3DMaterialLUT FresnelLUT;
        public H3DMaterialLUT ReflecRLUT;
        public H3DMaterialLUT ReflecGLUT;
        public H3DMaterialLUT ReflecBLUT;

        private byte LayerConfig;

        public H3DTranslucencyLayer TranslucencyLayer
        {
            get { return (H3DTranslucencyLayer)BitUtils.GetBits(LayerConfig, 0, 4); }
            set { LayerConfig = BitUtils.SetBits(LayerConfig, (uint)value, 0, 4); }
        }

        public H3DTexCoordConfig TexCoordConfig
        {
            get { return (H3DTexCoordConfig)BitUtils.GetBits(LayerConfig, 4, 4); }
            set { LayerConfig = BitUtils.SetBits(LayerConfig, (uint)value, 4, 4); }
        }

        public H3DFresnelSelector FresnelSelector;

        public H3DBumpMode BumpMode;

        public byte BumpTexture;

        [Inline, FixedLength(6)]
        private uint[] LUTConfigCommands;

        public uint ConstantColors;

        public float PolygonOffsetUnit;

        [RepeatPointer]
        private uint[] FragmentShaderCommands;

        public string LUTDist0SamplerName;
        public string LUTDist1SamplerName;
        public string LUTFresnelSamplerName;
        public string LUTReflecRSamplerName;
        public string LUTReflecGSamplerName;
        public string LUTReflecBSamplerName;

        public string LUTDist0TableName;
        public string LUTDist1TableName;
        public string LUTFresnelTableName;
        public string LUTReflecRTableName;
        public string LUTReflecGTableName;
        public string LUTReflecBTableName;

        public string ShaderReference;
        public string ModelReference;

        public H3DMetaData MetaData;

        //LookUp Table
        [NonSerialized]
        public PICALUTInputAbs LUTInputAbs;

        [NonSerialized]
        public PICALUTInputSel LUTInputSel;

        [NonSerialized]
        public PICALUTInputScaleSel LUTInputScaleSel;

        //Fragment Lighting
        [NonSerialized]
        public PICAFaceCulling FaceCulling;

        [NonSerialized]
        public PICAColorOperation ColorOperation;

        [NonSerialized]
        public PICATexEnvStage[] TexEnvStages;

        [NonSerialized]
        public PICATexEnvColor TexEnvBufferColor;

        [NonSerialized]
        public PICABlendingFunction BlendingFunction;

        [NonSerialized]
        public PICALogicalOperation LogicalOperation;

        [NonSerialized]
        public PICAFragmentAlphaTest FragmentAlphaTest;

        [NonSerialized]
        public PICAStencilTest StencilTest;

        [NonSerialized]
        public PICAStencilOperation StencilOperation;

        [NonSerialized]
        public PICADepthColorMask DepthColorMask;

        [NonSerialized]
        public bool ColorBufferRead;

        [NonSerialized]
        public bool ColorBufferWrite;

        [NonSerialized]
        public bool StencilBufferRead;

        [NonSerialized]
        public bool StencilBufferWrite;

        [NonSerialized]
        public bool DepthBufferRead;

        [NonSerialized]
        public bool DepthBufferWrite;

        public string ObjectName { get { return null; } }

        public H3DMaterialParams()
        {
            TextureCoords = new H3DTextureCoord[3];
            TexEnvStages = new PICATexEnvStage[6];

            for (int Index = 0; Index < TexEnvStages.Length; Index++)
            {
                TexEnvStages[Index] = new PICATexEnvStage();
            }
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            PICACommandReader Reader;

            Reader = new PICACommandReader(LUTConfigCommands);

            while (Reader.HasCommand)
            {
                PICACommand Cmd = Reader.GetCommand();

                uint Param = Cmd.Parameters[0];

                switch (Cmd.Register)
                {
                    case PICARegister.GPUREG_LIGHTING_LUTINPUT_ABS: LUTInputAbs = new PICALUTInputAbs(Param); break;
                    case PICARegister.GPUREG_LIGHTING_LUTINPUT_SELECT: LUTInputSel = new PICALUTInputSel(Param); break;
                    case PICARegister.GPUREG_LIGHTING_LUTINPUT_SCALE: LUTInputScaleSel = new PICALUTInputScaleSel(Param); break;
                }
            }

            Reader = new PICACommandReader(FragmentShaderCommands);

            while (Reader.HasCommand)
            {
                PICACommand Cmd = Reader.GetCommand();

                uint Param = Cmd.Parameters[0];

                int Stage = ((int)Cmd.Register >> 3) & 7;

                if (Stage >= 6) Stage -= 2;

                switch (Cmd.Register)
                {
                    case PICARegister.GPUREG_FACECULLING_CONFIG: FaceCulling = (PICAFaceCulling)(Param & 3); break;

                    case PICARegister.GPUREG_TEXENV0_SOURCE:
                    case PICARegister.GPUREG_TEXENV1_SOURCE:
                    case PICARegister.GPUREG_TEXENV2_SOURCE:
                    case PICARegister.GPUREG_TEXENV3_SOURCE:
                    case PICARegister.GPUREG_TEXENV4_SOURCE:
                    case PICARegister.GPUREG_TEXENV5_SOURCE:
                        TexEnvStages[Stage].Source = new PICATexEnvSource(Param);
                        break;
                    case PICARegister.GPUREG_TEXENV0_OPERAND:
                    case PICARegister.GPUREG_TEXENV1_OPERAND:
                    case PICARegister.GPUREG_TEXENV2_OPERAND:
                    case PICARegister.GPUREG_TEXENV3_OPERAND:
                    case PICARegister.GPUREG_TEXENV4_OPERAND:
                    case PICARegister.GPUREG_TEXENV5_OPERAND:
                        TexEnvStages[Stage].Operand = new PICATexEnvOperand(Param);
                        break;
                    case PICARegister.GPUREG_TEXENV0_COMBINER:
                    case PICARegister.GPUREG_TEXENV1_COMBINER:
                    case PICARegister.GPUREG_TEXENV2_COMBINER:
                    case PICARegister.GPUREG_TEXENV3_COMBINER:
                    case PICARegister.GPUREG_TEXENV4_COMBINER:
                    case PICARegister.GPUREG_TEXENV5_COMBINER:
                        TexEnvStages[Stage].Combiner = new PICATexEnvCombiner(Param);
                        break;
                    case PICARegister.GPUREG_TEXENV0_COLOR:
                    case PICARegister.GPUREG_TEXENV1_COLOR:
                    case PICARegister.GPUREG_TEXENV2_COLOR:
                    case PICARegister.GPUREG_TEXENV3_COLOR:
                    case PICARegister.GPUREG_TEXENV4_COLOR:
                    case PICARegister.GPUREG_TEXENV5_COLOR:
                        TexEnvStages[Stage].Color = new PICATexEnvColor(Param);
                        break;
                    case PICARegister.GPUREG_TEXENV0_SCALE:
                    case PICARegister.GPUREG_TEXENV1_SCALE:
                    case PICARegister.GPUREG_TEXENV2_SCALE:
                    case PICARegister.GPUREG_TEXENV3_SCALE:
                    case PICARegister.GPUREG_TEXENV4_SCALE:
                    case PICARegister.GPUREG_TEXENV5_SCALE:
                        TexEnvStages[Stage].Scale = new PICATexEnvScale(Param);
                        break;

                    case PICARegister.GPUREG_COLOR_OPERATION: ColorOperation = new PICAColorOperation(Param); break;

                    case PICARegister.GPUREG_TEXENV_BUFFER_COLOR: TexEnvBufferColor = new PICATexEnvColor(Param); break;

                    case PICARegister.GPUREG_BLEND_FUNC: BlendingFunction = new PICABlendingFunction(Param); break;

                    case PICARegister.GPUREG_LOGIC_OP: LogicalOperation = (PICALogicalOperation)(Param & 0xf); break;

                    case PICARegister.GPUREG_FRAGOP_ALPHA_TEST: FragmentAlphaTest = new PICAFragmentAlphaTest(Param); break;

                    case PICARegister.GPUREG_STENCIL_TEST: StencilTest = new PICAStencilTest(Param); break;

                    case PICARegister.GPUREG_STENCIL_OP: StencilOperation = new PICAStencilOperation(Param); break;

                    case PICARegister.GPUREG_DEPTH_COLOR_MASK: DepthColorMask = new PICADepthColorMask(Param); break;

                    case PICARegister.GPUREG_COLORBUFFER_READ: ColorBufferRead = (Param & 0xf) == 0xf; break;

                    case PICARegister.GPUREG_COLORBUFFER_WRITE: ColorBufferWrite = (Param & 0xf) == 0xf; break;

                    case PICARegister.GPUREG_DEPTHBUFFER_READ:
                        StencilBufferRead = (Param & 1) != 0;
                        DepthBufferRead = (Param & 2) != 0;
                        break;

                    case PICARegister.GPUREG_DEPTHBUFFER_WRITE:
                        StencilBufferWrite = (Param & 1) != 0;
                        DepthBufferWrite = (Param & 2) != 0;
                        break;
                }
            }
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            //TODO
            PICACommandWriter Writer;

            Writer = new PICACommandWriter();

            for (int Stage = 0; Stage < 6; Stage++)
            {
                PICARegister Register = PICARegister.GPUREG_DUMMY;

                switch (Stage)
                {
                    case 0: Register = PICARegister.GPUREG_TEXENV0_SOURCE; break;
                    case 1: Register = PICARegister.GPUREG_TEXENV1_SOURCE; break;
                    case 2: Register = PICARegister.GPUREG_TEXENV2_SOURCE; break;
                    case 3: Register = PICARegister.GPUREG_TEXENV3_SOURCE; break;
                    case 4: Register = PICARegister.GPUREG_TEXENV4_SOURCE; break;
                    case 5: Register = PICARegister.GPUREG_TEXENV5_SOURCE; break;
                }

                Writer.SetCommand(Register, true, 0xf,
                    TexEnvStages[Stage].Source.ToUInt32(),
                    TexEnvStages[Stage].Operand.ToUInt32(),
                    TexEnvStages[Stage].Combiner.ToUInt32(),
                    TexEnvStages[Stage].Color.ToUInt32(),
                    TexEnvStages[Stage].Scale.ToUInt32());
            }

            return false;
        }
    }
}
