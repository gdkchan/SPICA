using SPICA.Formats.CtrH3D.Model.Material.Texture;
using SPICA.Math3D;
using SPICA.PICA;
using SPICA.PICA.Commands;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;
using SPICA.Utils;

using System;

namespace SPICA.Formats.CtrH3D.Model.Material
{
    public class H3DMaterialParams : ICustomSerialization, INamed
    {
        //Original tool uses FNV hash with some of contents here to generate this Id
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
        public PICATexEnvStage[] TexEnvStages;

        [NonSerialized]
        public PICATexEnvColor TexEnvBufferColor;

        [NonSerialized]
        public PICAColorOperation ColorOperation;

        [NonSerialized]
        public PICABlendFunction BlendFunction;

        [NonSerialized]
        public PICALogicalOperation LogicalOperation;

        [NonSerialized]
        public PICAAlphaTest AlphaTest;

        [NonSerialized]
        public PICAStencilTest StencilTest;

        [NonSerialized]
        public PICAStencilOperation StencilOperation;

        [NonSerialized]
        public PICADepthColorMask DepthColorMask;

        [NonSerialized]
        public PICAFaceCulling FaceCulling;

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

        [NonSerialized]
        public float[] TextureSources;

        [NonSerialized]
        internal H3DMaterial Parent;

        public string ObjectName { get { return Parent?.Name; } }

        public H3DMaterialParams()
        {
            TextureCoords = new H3DTextureCoord[3];
            TexEnvStages = new PICATexEnvStage[6];

            for (int Index = 0; Index < TexEnvStages.Length; Index++)
            {
                TexEnvStages[Index] = new PICATexEnvStage();
            }

            TextureSources = new float[3];
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

            int UniformIndex = 0;

            Vector4D[] Uniform = new Vector4D[96];

            while (Reader.HasCommand)
            {
                PICACommand Cmd = Reader.GetCommand();

                uint Param = Cmd.Parameters[0];

                int Stage = ((int)Cmd.Register >> 3) & 7;

                if (Stage >= 6) Stage -= 2;

                switch (Cmd.Register)
                {
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

                    case PICARegister.GPUREG_TEXENV_BUFFER_COLOR: TexEnvBufferColor = new PICATexEnvColor(Param); break;

                    case PICARegister.GPUREG_COLOR_OPERATION: ColorOperation = new PICAColorOperation(Param); break;

                    case PICARegister.GPUREG_BLEND_FUNC: BlendFunction = new PICABlendFunction(Param); break;

                    case PICARegister.GPUREG_LOGIC_OP: LogicalOperation = (PICALogicalOperation)(Param & 0xf); break;

                    case PICARegister.GPUREG_FRAGOP_ALPHA_TEST: AlphaTest = new PICAAlphaTest(Param); break;

                    case PICARegister.GPUREG_STENCIL_TEST: StencilTest = new PICAStencilTest(Param); break;

                    case PICARegister.GPUREG_STENCIL_OP: StencilOperation = new PICAStencilOperation(Param); break;

                    case PICARegister.GPUREG_DEPTH_COLOR_MASK: DepthColorMask = new PICADepthColorMask(Param); break;

                    case PICARegister.GPUREG_FACECULLING_CONFIG: FaceCulling = (PICAFaceCulling)(Param & 3); break;

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

                    case PICARegister.GPUREG_VSH_FLOATUNIFORM_INDEX: UniformIndex = (int)((Param & 0xff) << 2); break;
                    case PICARegister.GPUREG_VSH_FLOATUNIFORM_DATA0:
                    case PICARegister.GPUREG_VSH_FLOATUNIFORM_DATA1:
                    case PICARegister.GPUREG_VSH_FLOATUNIFORM_DATA2:
                    case PICARegister.GPUREG_VSH_FLOATUNIFORM_DATA3:
                    case PICARegister.GPUREG_VSH_FLOATUNIFORM_DATA4:
                    case PICARegister.GPUREG_VSH_FLOATUNIFORM_DATA5:
                    case PICARegister.GPUREG_VSH_FLOATUNIFORM_DATA6:
                    case PICARegister.GPUREG_VSH_FLOATUNIFORM_DATA7:
                        for (int i = 0; i < Cmd.Parameters.Length; i++)
                        {
                            int j = UniformIndex >> 2;
                            int k = (UniformIndex++ & 3) ^ 3;

                            Uniform[j][k] = IOUtils.ToSingle(Cmd.Parameters[i]);
                        }
                        break;
                }
            }

            //Those define the mapping used by each texture
            /*
             * Values:
             * 0-2 = UVMap[0-2]
             * 3 = CameraCubeEnvMap aka SkyBox
             * 4 = CameraSphereEnvMap aka SkyDome
             * 5 = ProjectionMap?
             */
            TextureSources[0] = Uniform[10].X;
            TextureSources[1] = Uniform[10].Y;
            TextureSources[2] = Uniform[10].Z;
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            PICACommandWriter Writer;

            Writer = new PICACommandWriter();

            Writer.SetCommand(PICARegister.GPUREG_LIGHTING_LUTINPUT_ABS, LUTInputAbs.ToUInt32());
            Writer.SetCommand(PICARegister.GPUREG_LIGHTING_LUTINPUT_SELECT, LUTInputSel.ToUInt32());
            Writer.SetCommand(PICARegister.GPUREG_LIGHTING_LUTINPUT_SCALE, LUTInputScaleSel.ToUInt32());

            LUTConfigCommands = Writer.GetBuffer();

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

                Writer.SetCommand(Register, true,
                    TexEnvStages[Stage].Source.ToUInt32(),
                    TexEnvStages[Stage].Operand.ToUInt32(),
                    TexEnvStages[Stage].Combiner.ToUInt32(),
                    TexEnvStages[Stage].Color.ToUInt32(),
                    TexEnvStages[Stage].Scale.ToUInt32());
            }

            Writer.SetCommand(PICARegister.GPUREG_TEXENV_UPDATE_BUFFER, false, 2);
            Writer.SetCommand(PICARegister.GPUREG_TEXENV_BUFFER_COLOR, TexEnvBufferColor.ToUInt32());
            Writer.SetCommand(PICARegister.GPUREG_COLOR_OPERATION, ColorOperation.ToUInt32(), 3);
            Writer.SetCommand(PICARegister.GPUREG_BLEND_FUNC, BlendFunction.ToUInt32());
            Writer.SetCommand(PICARegister.GPUREG_FRAGOP_ALPHA_TEST, AlphaTest.ToUInt32(), 3);
            Writer.SetCommand(PICARegister.GPUREG_STENCIL_TEST, StencilTest.ToUInt32());
            Writer.SetCommand(PICARegister.GPUREG_STENCIL_OP, StencilOperation.ToUInt32());
            Writer.SetCommand(PICARegister.GPUREG_DEPTH_COLOR_MASK, DepthColorMask.ToUInt32());
            Writer.SetCommand(PICARegister.GPUREG_FACECULLING_CONFIG, (uint)FaceCulling);
            Writer.SetCommand(PICARegister.GPUREG_FRAMEBUFFER_FLUSH, true);
            Writer.SetCommand(PICARegister.GPUREG_FRAMEBUFFER_INVALIDATE, true);
            Writer.SetCommand(PICARegister.GPUREG_COLORBUFFER_READ, ColorBufferRead ? 0xfu : 0u, 1);
            Writer.SetCommand(PICARegister.GPUREG_COLORBUFFER_WRITE, ColorBufferWrite ? 0xfu : 0u, 1);
            Writer.SetCommand(PICARegister.GPUREG_DEPTHBUFFER_READ, StencilBufferRead, DepthBufferRead);
            Writer.SetCommand(PICARegister.GPUREG_DEPTHBUFFER_WRITE, StencilBufferWrite, DepthBufferWrite);

            Matrix3x4[] TexMtx = new Matrix3x4[3];

            for (int Unit = 0; Unit < 3; Unit++)
            {
                if (Parent.EnabledTextures[Unit])
                    TexMtx[Unit] = Matrix3x4.RotateZ(TextureCoords[Unit].Rotation);
                else
                    TexMtx[Unit] = Matrix3x4.Empty;
            }

            Writer.SetCommand(PICARegister.GPUREG_VSH_FLOATUNIFORM_INDEX, 0x8000000bu);

            Writer.SetCommand(PICARegister.GPUREG_VSH_FLOATUNIFORM_DATA0, false,
                TexMtx[0].M14, TexMtx[0].M13, TexMtx[0].M12, TexMtx[0].M11,
                TexMtx[0].M24, TexMtx[0].M23, TexMtx[0].M22, TexMtx[0].M21,
                TexMtx[0].M34, TexMtx[0].M33, TexMtx[0].M32, TexMtx[0].M31,
                TexMtx[1].M14, TexMtx[1].M13, TexMtx[1].M12, TexMtx[1].M11,
                TexMtx[1].M24, TexMtx[1].M23, TexMtx[1].M22, TexMtx[1].M21,
                TexMtx[1].M34, TexMtx[1].M33, TexMtx[1].M32, TexMtx[1].M31,
                TexMtx[2].M14, TexMtx[2].M13, TexMtx[2].M12, TexMtx[2].M11,
                TexMtx[2].M24, TexMtx[2].M23, TexMtx[2].M22, TexMtx[2].M21);

            Writer.SetCommand(PICARegister.GPUREG_VSH_FLOATUNIFORM_INDEX, true, 0x80000013u, 0, 0, 0, 0);

            Writer.SetCommand(PICARegister.GPUREG_VSH_FLOATUNIFORM_INDEX, true, 0x8000000au,
                IOUtils.ToUInt32(0),
                IOUtils.ToUInt32(TextureSources[2]),
                IOUtils.ToUInt32(TextureSources[1]),
                IOUtils.ToUInt32(TextureSources[0]));

            Writer.SetCommand(PICARegister.GPUREG_VSH_FLOATUNIFORM_INDEX, true, 0x80000014u,
                IOUtils.ToUInt32(ColorScale),
                IOUtils.ToUInt32(AmbientColor.B / 255f),
                IOUtils.ToUInt32(AmbientColor.G / 255f),
                IOUtils.ToUInt32(AmbientColor.R / 255f));

            Writer.SetCommand(PICARegister.GPUREG_VSH_FLOATUNIFORM_INDEX, true, 0x80000015u,
                IOUtils.ToUInt32(DiffuseColor.A / 255f),
                IOUtils.ToUInt32(DiffuseColor.B / 255f),
                IOUtils.ToUInt32(DiffuseColor.G / 255f),
                IOUtils.ToUInt32(DiffuseColor.R / 255f));

            Writer.SetCommand(PICARegister.GPUREG_VSH_FLOATUNIFORM_INDEX, true, 0x80000055u,
                IOUtils.ToUInt32(1),
                IOUtils.ToUInt32(8),
                IOUtils.ToUInt32(1),
                IOUtils.ToUInt32(8));

            Writer.WriteEnd();

            FragmentShaderCommands = Writer.GetBuffer();

            return false;
        }
    }
}
