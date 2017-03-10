using SPICA.Formats.CtrH3D.Model.Material.Texture;
using SPICA.Formats.Utils;
using SPICA.Math3D;
using SPICA.PICA;
using SPICA.PICA.Commands;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;
using System;
using System.Xml.Serialization;

namespace SPICA.Formats.CtrH3D.Model.Material
{
    public class H3DMaterialParams : ICustomSerialization, INamed
    {
        private uint UniqueId;

        public H3DMaterialFlags Flags;
        public H3DFragmentFlags FragmentFlags;

        private byte FrameAccessOffset;

        public H3DMaterialShader MaterialShader;

        [Inline, FixedLength(3)] public H3DTextureCoord[] TextureCoords;

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

        private byte LayerCfg;

        public H3DLayerConfig LayerConfig
        {
            get { return (H3DLayerConfig)BitUtils.GetBits(LayerCfg, 0, 4); }
            set { LayerCfg = BitUtils.SetBits(LayerCfg, (uint)value, 0, 4); }
        }

        public H3DTexCoordConfig TexCoordConfig
        {
            get { return (H3DTexCoordConfig)BitUtils.GetBits(LayerCfg, 4, 4); }
            set { LayerCfg = BitUtils.SetBits(LayerCfg, (uint)value, 4, 4); }
        }

        public H3DFresnelSelector FresnelSelector;

        public H3DBumpMode BumpMode;

        public byte BumpTexture;

        [Inline, FixedLength(6)] private uint[] LUTConfigCommands;

        private uint ConstantColors;

        [Ignore] public uint Constant0Assignment;
        [Ignore] public uint Constant1Assignment;
        [Ignore] public uint Constant2Assignment;
        [Ignore] public uint Constant3Assignment;
        [Ignore] public uint Constant4Assignment;
        [Ignore] public uint Constant5Assignment;

        public float PolygonOffsetUnit;

        [RepeatPointer] private uint[] FragmentShaderCommands;

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
        [Ignore] public PICALUTInAbs   LUTInAbs;
        [Ignore] public PICALUTInSel   LUTInSel;
        [Ignore] public PICALUTInScale LUTInScale;

        //Fragment Lighting
        [Ignore] public PICATexEnvStage[]    TexEnvStages;
        [Ignore] public PICATexEnvColor      TexEnvBufferColor;
        [Ignore] public PICAColorOperation   ColorOperation;
        [Ignore] public PICABlendFunction    BlendFunction;
        [Ignore] public PICALogicalOperation LogicalOperation;
        [Ignore] public PICAAlphaTest        AlphaTest;
        [Ignore] public PICAStencilTest      StencilTest;
        [Ignore] public PICAStencilOperation StencilOperation;
        [Ignore] public PICADepthColorMask   DepthColorMask;
        [Ignore] public PICAFaceCulling      FaceCulling;

        [Ignore] public bool ColorBufferRead;
        [Ignore] public bool ColorBufferWrite;

        [Ignore] public bool StencilBufferRead;
        [Ignore] public bool StencilBufferWrite;

        [Ignore] public bool DepthBufferRead;
        [Ignore] public bool DepthBufferWrite;

        [Ignore] public float[] TextureSources;

        [Ignore, XmlIgnore] internal H3DMaterial Parent;

        [XmlIgnore]
        public string Name
        {
            get { return Parent?.Name; }
            set { if (Parent != null) Parent.Name = value; }
        }

        public H3DMaterialParams()
        {
            TextureCoords = new H3DTextureCoord[3];
            TexEnvStages  = new PICATexEnvStage[6];

            for (int Index = 0; Index < TexEnvStages.Length; Index++)
            {
                TexEnvStages[Index] = new PICATexEnvStage();
            }

            TextureSources = new float[4];
        }

        private void GenerateUniqueId()
        {
            FNVHash HashGen = new FNVHash();

            HashGen.Hash((ushort)(Flags | H3DMaterialFlags.IsParamCommandSourceAccessible));
            HashGen.Hash((byte)FragmentFlags);

            foreach (H3DTextureCoord TexCoord in TextureCoords)
            {
                HashGen.Hash((byte)TexCoord.Flags);
                HashGen.Hash((byte)TexCoord.TransformType);
                HashGen.Hash((byte)TexCoord.MappingType);
                HashGen.Hash(TexCoord.ReferenceCameraIndex);
                HashGen.Hash(TexCoord.Scale.X);
                HashGen.Hash(TexCoord.Scale.Y);
                HashGen.Hash(TexCoord.Rotation);
                HashGen.Hash(TexCoord.Translation.X);
                HashGen.Hash(TexCoord.Translation.Y);
            }

            HashGen.Hash(LightSetIndex);
            HashGen.Hash(FogIndex);
            HashGen.Hash(EmissionColor.ToUInt32());
            HashGen.Hash(AmbientColor.ToUInt32());
            HashGen.Hash(DiffuseColor.ToUInt32());
            HashGen.Hash(Specular0Color.ToUInt32());
            HashGen.Hash(Specular1Color.ToUInt32());
            HashGen.Hash(Constant0Color.ToUInt32());
            HashGen.Hash(Constant1Color.ToUInt32());
            HashGen.Hash(Constant2Color.ToUInt32());
            HashGen.Hash(Constant3Color.ToUInt32());
            HashGen.Hash(Constant4Color.ToUInt32());
            HashGen.Hash(Constant5Color.ToUInt32());
            HashGen.Hash(BlendColor.ToUInt32());
            HashGen.Hash(ColorScale);
            HashGen.Hash(LayerCfg);
            HashGen.Hash((byte)FresnelSelector);
            HashGen.Hash((byte)BumpMode);
            HashGen.Hash(BumpTexture);
            HashGen.Hash(LUTConfigCommands);
            HashGen.Hash(ConstantColors);
            HashGen.Hash(PolygonOffsetUnit);
            HashGen.Hash(FragmentShaderCommands);

            HashGen.Hash(LUTDist0TableName);
            HashGen.Hash(LUTDist1TableName);
            HashGen.Hash(LUTFresnelTableName);
            HashGen.Hash(LUTReflecRTableName);
            HashGen.Hash(LUTReflecGTableName);
            HashGen.Hash(LUTReflecBTableName);

            HashGen.Hash(LUTDist0SamplerName);
            HashGen.Hash(LUTDist1SamplerName);
            HashGen.Hash(LUTFresnelSamplerName);
            HashGen.Hash(LUTReflecRSamplerName);
            HashGen.Hash(LUTReflecGSamplerName);
            HashGen.Hash(LUTReflecBSamplerName);

            HashGen.Hash(ShaderReference);

            UniqueId = HashGen.HashCode;
        }

        public RGBA GetConstant(int Stage)
        {
            if (Stage >= 0 && Stage < 6)
            {
                PackConstantAssignments();

                switch ((ConstantColors >> Stage * 4) & 0xf)
                {
                    default:
                    case 0: return Constant0Color;
                    case 1: return Constant1Color;
                    case 2: return Constant2Color;
                    case 3: return Constant3Color;
                    case 4: return Constant4Color;
                    case 5: return Constant5Color;
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("Expected Stage in 0-5 range!");
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
                    case PICARegister.GPUREG_LIGHTING_LUTINPUT_ABS:    LUTInAbs   = new PICALUTInAbs(Param);   break;
                    case PICARegister.GPUREG_LIGHTING_LUTINPUT_SELECT: LUTInSel   = new PICALUTInSel(Param);   break;
                    case PICARegister.GPUREG_LIGHTING_LUTINPUT_SCALE:  LUTInScale = new PICALUTInScale(Param); break;
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

                    case PICARegister.GPUREG_TEXENV_UPDATE_BUFFER:
                        TexEnvStages[1].UpdateColorBuffer = (Param & 0x100)  != 0;
                        TexEnvStages[2].UpdateColorBuffer = (Param & 0x200)  != 0;
                        TexEnvStages[3].UpdateColorBuffer = (Param & 0x400)  != 0;
                        TexEnvStages[4].UpdateColorBuffer = (Param & 0x800)  != 0;

                        TexEnvStages[1].UpdateAlphaBuffer = (Param & 0x1000) != 0;
                        TexEnvStages[2].UpdateAlphaBuffer = (Param & 0x2000) != 0;
                        TexEnvStages[3].UpdateAlphaBuffer = (Param & 0x4000) != 0;
                        TexEnvStages[4].UpdateAlphaBuffer = (Param & 0x8000) != 0;
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

                    case PICARegister.GPUREG_COLORBUFFER_READ:  ColorBufferRead  = (Param & 0xf) == 0xf; break;
                    case PICARegister.GPUREG_COLORBUFFER_WRITE: ColorBufferWrite = (Param & 0xf) == 0xf; break;

                    case PICARegister.GPUREG_DEPTHBUFFER_READ:
                        StencilBufferRead = (Param & 1) != 0;
                        DepthBufferRead   = (Param & 2) != 0;
                        break;
                    case PICARegister.GPUREG_DEPTHBUFFER_WRITE:
                        StencilBufferWrite = (Param & 1) != 0;
                        DepthBufferWrite   = (Param & 2) != 0;
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
            TextureSources[3] = Uniform[10].W;

            UnpackConstantAssignments();
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            PICACommandWriter Writer;

            Writer = new PICACommandWriter();

            Writer.SetCommand(PICARegister.GPUREG_LIGHTING_LUTINPUT_ABS,    LUTInAbs.ToUInt32());
            Writer.SetCommand(PICARegister.GPUREG_LIGHTING_LUTINPUT_SELECT, LUTInSel.ToUInt32());
            Writer.SetCommand(PICARegister.GPUREG_LIGHTING_LUTINPUT_SCALE,  LUTInScale.ToUInt32());

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

            FrameAccessOffset = (byte)Writer.Index;

            Writer.SetCommand(PICARegister.GPUREG_COLORBUFFER_READ, ColorBufferRead ? 0xfu : 0u, 1);
            Writer.SetCommand(PICARegister.GPUREG_COLORBUFFER_WRITE, ColorBufferWrite ? 0xfu : 0u, 1);
            Writer.SetCommand(PICARegister.GPUREG_DEPTHBUFFER_READ, StencilBufferRead, DepthBufferRead);
            Writer.SetCommand(PICARegister.GPUREG_DEPTHBUFFER_WRITE, StencilBufferWrite, DepthBufferWrite);

            Matrix3x4[] TexMtx = new Matrix3x4[3];

            for (int Unit = 0; Unit < 3; Unit++)
            {
                if (Parent.EnabledTextures[Unit])
                    TexMtx[Unit] = TextureCoords[Unit].Transform;
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
                IOUtils.ToUInt32(TextureSources[3]),
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

            PackConstantAssignments();

            GenerateUniqueId();

            return false;
        }

        private void UnpackConstantAssignments()
        {
            Constant0Assignment = (ConstantColors >>  0) & 0xf;
            Constant1Assignment = (ConstantColors >>  4) & 0xf;
            Constant2Assignment = (ConstantColors >>  8) & 0xf;
            Constant3Assignment = (ConstantColors >> 12) & 0xf;
            Constant4Assignment = (ConstantColors >> 16) & 0xf;
            Constant5Assignment = (ConstantColors >> 20) & 0xf;
        }

        private void PackConstantAssignments()
        {
            ConstantColors =
                (Constant0Assignment <<  0) |
                (Constant1Assignment <<  4) |
                (Constant2Assignment <<  8) |
                (Constant3Assignment << 12) |
                (Constant4Assignment << 16) |
                (Constant5Assignment << 20);
        }
    }
}
