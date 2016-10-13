using SPICA.Math3D;
using SPICA.PICA;
using SPICA.PICA.Commands;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

using System;

namespace SPICA.Formats.H3D.Model.Material
{
    class H3DMaterialParams : ICustomSerialization
    {
        public uint UId;
        public H3DMaterialFlags Flags;
        public H3DFragLightFlags FragmentLightFlags;
        public byte FrameAccessOffset;

        public H3DMaterialShader MaterialShader;

        [Inline, FixedLength(3)]
        public H3DTextureCoord[] TextureCoords;

        public ushort LightSetIndex;
        public ushort FogIndex;

        public RGBA EmissionColor;
        public RGBA AmbientColor;
        public RGBA DiffuseColor;
        public RGBA Specular0;
        public RGBA Specular1;
        public RGBA Constant0;
        public RGBA Constant1;
        public RGBA Constant2;
        public RGBA Constant3;
        public RGBA Constant4;
        public RGBA Constant5;
        public RGBA BlendColor;

        public float ColorScale;

        public H3DMaterialLUT Dist0LUT;
        public H3DMaterialLUT Dist1LUT;
        public H3DMaterialLUT FresnelLUT;
        public H3DMaterialLUT ReflecRLUT;
        public H3DMaterialLUT ReflecGLUT;
        public H3DMaterialLUT ReflecBLUT;

        public byte LayerConfig;
        public byte FresnelConfig;
        public byte BumpMode;
        public byte BumpTexture;

        [Inline, FixedLength(6)]
        public uint[] LUTConfigCommands;

        public uint ConstantColors;

        public float PolygonOffsetUnit;

        [RepeatPointer]
        public uint[] FragmentShaderCommands;

        public string LUTDist0TableName;
        public string LUTDist1TableName;
        public string LUTFresnelTableName;
        public string LUTReflecRTableName;
        public string LUTReflecGTableName;
        public string LUTReflecBTableName;

        public string LUTDist0SamplerName;
        public string LUTDist1SamplerName;
        public string LUTFresnelSamplerName;
        public string LUTReflecRSamplerName;
        public string LUTReflecGSamplerName;
        public string LUTReflecBSamplerName;

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

        public H3DMaterialParams()
        {
            TexEnvStages = new PICATexEnvStage[6];

            for (int Index = 0; Index < TexEnvStages.Length; Index++)
            {
                TexEnvStages[Index] = new PICATexEnvStage();
            }
        }

        public void Deserialize(BinaryDeserializer Deserializer)
        {
            PICACommandReader Reader;

            Reader = new PICACommandReader(LUTConfigCommands);

            while (Reader.HasCommand)
            {
                PICACommand Cmd = Reader.GetCommand();

                uint Param = Cmd.Parameters[0];

                switch (Cmd.Register)
                {
                    case PICARegister.GPUREG_LIGHTING_LUTINPUT_ABS: LUTInputAbs = PICALUTInputAbs.FromParameter(Param); break;
                    case PICARegister.GPUREG_LIGHTING_LUTINPUT_SELECT: LUTInputSel = PICALUTInputSel.FromParameter(Param); break;
                    case PICARegister.GPUREG_LIGHTING_LUTINPUT_SCALE: LUTInputScaleSel = PICALUTInputScaleSel.FromParameter(Param); break;
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
                        TexEnvStages[Stage].Source = PICATexEnvSource.FromParameter(Param);
                        break;
                    case PICARegister.GPUREG_TEXENV0_OPERAND:
                    case PICARegister.GPUREG_TEXENV1_OPERAND:
                    case PICARegister.GPUREG_TEXENV2_OPERAND:
                    case PICARegister.GPUREG_TEXENV3_OPERAND:
                    case PICARegister.GPUREG_TEXENV4_OPERAND:
                    case PICARegister.GPUREG_TEXENV5_OPERAND:
                        TexEnvStages[Stage].Operand = PICATexEnvOperand.FromParameter(Param);
                        break;
                    case PICARegister.GPUREG_TEXENV0_COMBINER:
                    case PICARegister.GPUREG_TEXENV1_COMBINER:
                    case PICARegister.GPUREG_TEXENV2_COMBINER:
                    case PICARegister.GPUREG_TEXENV3_COMBINER:
                    case PICARegister.GPUREG_TEXENV4_COMBINER:
                    case PICARegister.GPUREG_TEXENV5_COMBINER:
                        TexEnvStages[Stage].Combiner = PICATexEnvCombiner.FromParameter(Param);
                        break;
                    case PICARegister.GPUREG_TEXENV0_COLOR:
                    case PICARegister.GPUREG_TEXENV1_COLOR:
                    case PICARegister.GPUREG_TEXENV2_COLOR:
                    case PICARegister.GPUREG_TEXENV3_COLOR:
                    case PICARegister.GPUREG_TEXENV4_COLOR:
                    case PICARegister.GPUREG_TEXENV5_COLOR:
                        TexEnvStages[Stage].Color = PICATexEnvColor.FromParameter(Param);
                        break;
                    case PICARegister.GPUREG_TEXENV0_SCALE:
                    case PICARegister.GPUREG_TEXENV1_SCALE:
                    case PICARegister.GPUREG_TEXENV2_SCALE:
                    case PICARegister.GPUREG_TEXENV3_SCALE:
                    case PICARegister.GPUREG_TEXENV4_SCALE:
                    case PICARegister.GPUREG_TEXENV5_SCALE:
                        TexEnvStages[Stage].Scale = PICATexEnvScale.FromParameter(Param);
                        break;

                    case PICARegister.GPUREG_COLOR_OPERATION: ColorOperation = PICAColorOperation.FromParameter(Param); break;

                    case PICARegister.GPUREG_TEXENV_BUFFER_COLOR: TexEnvBufferColor = PICATexEnvColor.FromParameter(Param); break;

                    case PICARegister.GPUREG_BLEND_FUNC: BlendingFunction = PICABlendingFunction.FromParameter(Param); break;

                    case PICARegister.GPUREG_LOGIC_OP: LogicalOperation = (PICALogicalOperation)(Param & 0xf); break;

                    case PICARegister.GPUREG_FRAGOP_ALPHA_TEST: FragmentAlphaTest = PICAFragmentAlphaTest.FromParameter(Param); break;

                    case PICARegister.GPUREG_STENCIL_TEST: StencilTest = PICAStencilTest.FromParameter(Param); break;

                    case PICARegister.GPUREG_STENCIL_OP: StencilOperation = PICAStencilOperation.FromParameter(Param); break;

                    case PICARegister.GPUREG_DEPTH_COLOR_MASK: DepthColorMask = PICADepthColorMask.FromParameter(Param); break;
                }
            }
        }

        public bool Serialize(BinarySerializer Serializer)
        {
            //TODO

            return false;
        }
    }
}
