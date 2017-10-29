using SPICA.Formats.Common;
using SPICA.Formats.CtrH3D.LUT;
using SPICA.Math3D;
using SPICA.PICA;
using SPICA.PICA.Commands;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

using System;
using System.Collections.Generic;
using System.Numerics;

namespace SPICA.Formats.CtrH3D.Model.Material
{
    public class H3DMaterialParams : ICustomSerialization, INamed
    {
        private uint UniqueId;

        public H3DMaterialFlags Flags;
        public H3DFragmentFlags FragmentFlags;

        private byte FrameAccessOffset;

        public H3DMaterialShader MaterialShader;

        [Inline, FixedLength(3)] public readonly H3DTextureCoord[] TextureCoords;

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

        private uint Dist0SamplerPtr;
        private uint Dist1SamplerPtr;
        private uint FresnelSamplerPtr;
        private uint ReflecRSamplerPtr;
        private uint ReflecGSamplerPtr;
        private uint ReflecBSamplerPtr;

        private byte LayerConfig;

        public H3DTranslucencyKind TranslucencyKind
        {
            get => (H3DTranslucencyKind)BitUtils.GetBits(LayerConfig, 0, 4);
            set => LayerConfig = (byte)BitUtils.SetBits(LayerConfig, (int)value, 0, 4);
        }

        public H3DTexCoordConfig TexCoordConfig
        {
            get => (H3DTexCoordConfig)BitUtils.GetBits(LayerConfig, 4, 4);
            set => LayerConfig = (byte)BitUtils.SetBits(LayerConfig, (int)value, 4, 4);
        }

        public H3DFresnelSelector FresnelSelector;

        public H3DBumpMode BumpMode;

        public byte BumpTexture;

        [Inline, FixedLength(6)] private uint[] LUTConfigCommands;

        private int ConstantColors;

        public int Constant0Assignment
        {
            get => BitUtils.GetBits(ConstantColors, 0, 4);
            set => ConstantColors = BitUtils.SetBits(ConstantColors, value, 0, 4);
        }

        public int Constant1Assignment
        {
            get => BitUtils.GetBits(ConstantColors, 4, 4);
            set => ConstantColors = BitUtils.SetBits(ConstantColors, value, 4, 4);
        }

        public int Constant2Assignment
        {
            get => BitUtils.GetBits(ConstantColors, 8, 4);
            set => ConstantColors = BitUtils.SetBits(ConstantColors, value, 8, 4);
        }

        public int Constant3Assignment
        {
            get => BitUtils.GetBits(ConstantColors, 12, 4);
            set => ConstantColors = BitUtils.SetBits(ConstantColors, value, 12, 4);
        }

        public int Constant4Assignment
        {
            get => BitUtils.GetBits(ConstantColors, 16, 4);
            set => ConstantColors = BitUtils.SetBits(ConstantColors, value, 16, 4);
        }

        public int Constant5Assignment
        {
            get => BitUtils.GetBits(ConstantColors, 20, 4);
            set => ConstantColors = BitUtils.SetBits(ConstantColors, value, 20, 4);
        }

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

        [IfVersion(CmpOp.Gequal, 7)] public H3DMetaData MetaData;

        [Ignore] public PICALUTInAbs   LUTInputAbsolute;
        [Ignore] public PICALUTInSel   LUTInputSelection;
        [Ignore] public PICALUTInScale LUTInputScale;

        [Ignore] public readonly PICATexEnvStage[] TexEnvStages;

        [Ignore] public RGBA TexEnvBufferColor;

        [Ignore] public PICAColorOperation ColorOperation;

        [Ignore] public PICABlendFunction BlendFunction;

        [Ignore] public PICALogicalOp LogicalOperation;

        [Ignore] public PICAAlphaTest AlphaTest;

        [Ignore] public PICAStencilTest StencilTest;

        [Ignore] public PICAStencilOperation StencilOperation;

        [Ignore] public PICADepthColorMask DepthColorMask;

        [Ignore] public PICAFaceCulling FaceCulling;

        [Ignore] public bool ColorBufferRead;
        [Ignore] public bool ColorBufferWrite;

        [Ignore] public bool StencilBufferRead;
        [Ignore] public bool StencilBufferWrite;

        [Ignore] public bool DepthBufferRead;
        [Ignore] public bool DepthBufferWrite;

        [Ignore] public readonly float[] TextureSources;

        public string Name
        {
            get => ModelReference;
            set => ModelReference = value;
        }

        public Dictionary<uint, Vector4> VtxShaderUniforms { get; private set; }
        public Dictionary<uint, Vector4> GeoShaderUniforms { get; private set; }

        public H3DMaterialParams()
        {
            TextureCoords = new H3DTextureCoord[3];
            TexEnvStages  = new PICATexEnvStage[6];

            for (int Index = 0; Index < TexEnvStages.Length; Index++)
            {
                TexEnvStages[Index] = new PICATexEnvStage();
            }

            TextureSources = new float[4];

            VtxShaderUniforms = new Dictionary<uint, Vector4>();
            GeoShaderUniforms = new Dictionary<uint, Vector4>();
        }

        private void GenerateUniqueId()
        {
            FNV1a FNV = new FNV1a();

            FNV.Hash((ushort)(Flags | H3DMaterialFlags.IsParamCommandSourceAccessible));
            FNV.Hash((byte)FragmentFlags);

            foreach (H3DTextureCoord TexCoord in TextureCoords)
            {
                FNV.Hash((byte)TexCoord.Flags);
                FNV.Hash((byte)TexCoord.TransformType);
                FNV.Hash((byte)TexCoord.MappingType);
                FNV.Hash(TexCoord.ReferenceCameraIndex);
                FNV.Hash(TexCoord.Scale.X);
                FNV.Hash(TexCoord.Scale.Y);
                FNV.Hash(TexCoord.Rotation);
                FNV.Hash(TexCoord.Translation.X);
                FNV.Hash(TexCoord.Translation.Y);
            }

            FNV.Hash(LightSetIndex);
            FNV.Hash(FogIndex);
            FNV.Hash(EmissionColor.ToUInt32());
            FNV.Hash(AmbientColor.ToUInt32());
            FNV.Hash(DiffuseColor.ToUInt32());
            FNV.Hash(Specular0Color.ToUInt32());
            FNV.Hash(Specular1Color.ToUInt32());
            FNV.Hash(Constant0Color.ToUInt32());
            FNV.Hash(Constant1Color.ToUInt32());
            FNV.Hash(Constant2Color.ToUInt32());
            FNV.Hash(Constant3Color.ToUInt32());
            FNV.Hash(Constant4Color.ToUInt32());
            FNV.Hash(Constant5Color.ToUInt32());
            FNV.Hash(BlendColor.ToUInt32());
            FNV.Hash(ColorScale);
            FNV.Hash(LayerConfig);
            FNV.Hash((byte)FresnelSelector);
            FNV.Hash((byte)BumpMode);
            FNV.Hash(BumpTexture);
            FNV.Hash(LUTConfigCommands);
            FNV.Hash(ConstantColors);
            FNV.Hash(PolygonOffsetUnit);
            FNV.Hash(FragmentShaderCommands);

            FNV.Hash(LUTDist0TableName);
            FNV.Hash(LUTDist1TableName);
            FNV.Hash(LUTFresnelTableName);
            FNV.Hash(LUTReflecRTableName);
            FNV.Hash(LUTReflecGTableName);
            FNV.Hash(LUTReflecBTableName);

            FNV.Hash(LUTDist0SamplerName);
            FNV.Hash(LUTDist1SamplerName);
            FNV.Hash(LUTFresnelSamplerName);
            FNV.Hash(LUTReflecRSamplerName);
            FNV.Hash(LUTReflecGSamplerName);
            FNV.Hash(LUTReflecBSamplerName);

            FNV.Hash(ShaderReference);

            UniqueId = FNV.HashCode;
        }

        public int GetConstantIndex(int Stage)
        {
            if (Stage >= 0 && Stage < 6)
            {
                return (ConstantColors >> Stage * 4) & 0xf;
            }

            throw new ArgumentOutOfRangeException(nameof(Stage));
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
                    case PICARegister.GPUREG_LIGHTING_LUTINPUT_ABS:    LUTInputAbsolute  = new PICALUTInAbs(Param);   break;
                    case PICARegister.GPUREG_LIGHTING_LUTINPUT_SELECT: LUTInputSelection = new PICALUTInSel(Param);   break;
                    case PICARegister.GPUREG_LIGHTING_LUTINPUT_SCALE:  LUTInputScale     = new PICALUTInScale(Param); break;
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
                        TexEnvStages[Stage].Color = new RGBA(Param);
                        break;

                    case PICARegister.GPUREG_TEXENV0_SCALE:
                    case PICARegister.GPUREG_TEXENV1_SCALE:
                    case PICARegister.GPUREG_TEXENV2_SCALE:
                    case PICARegister.GPUREG_TEXENV3_SCALE:
                    case PICARegister.GPUREG_TEXENV4_SCALE:
                    case PICARegister.GPUREG_TEXENV5_SCALE:
                        TexEnvStages[Stage].Scale = new PICATexEnvScale(Param);
                        break;

                    case PICARegister.GPUREG_TEXENV_UPDATE_BUFFER:  PICATexEnvStage.SetUpdateBuffer(TexEnvStages, Param); break;

                    case PICARegister.GPUREG_TEXENV_BUFFER_COLOR: TexEnvBufferColor = new RGBA(Param); break;

                    case PICARegister.GPUREG_COLOR_OPERATION: ColorOperation = new PICAColorOperation(Param); break;

                    case PICARegister.GPUREG_BLEND_FUNC: BlendFunction = new PICABlendFunction(Param); break;

                    case PICARegister.GPUREG_LOGIC_OP: LogicalOperation = (PICALogicalOp)(Param & 0xf); break;

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
            TextureSources[0] = Reader.VtxShaderUniforms[10].X;
            TextureSources[1] = Reader.VtxShaderUniforms[10].Y;
            TextureSources[2] = Reader.VtxShaderUniforms[10].Z;
            TextureSources[3] = Reader.VtxShaderUniforms[10].W;

            VtxShaderUniforms = Reader.GetAllVertexShaderUniforms();
            GeoShaderUniforms = Reader.GetAllGeometryShaderUniforms();
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            PICACommandWriter Writer;

            Writer = new PICACommandWriter();

            Writer.SetCommand(PICARegister.GPUREG_LIGHTING_LUTINPUT_ABS,    LUTInputAbsolute.ToUInt32());
            Writer.SetCommand(PICARegister.GPUREG_LIGHTING_LUTINPUT_SELECT, LUTInputSelection.ToUInt32());
            Writer.SetCommand(PICARegister.GPUREG_LIGHTING_LUTINPUT_SCALE,  LUTInputScale.ToUInt32());

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

            uint UpdateBuffer = PICATexEnvStage.GetUpdateBuffer(TexEnvStages);

            Writer.SetCommand(PICARegister.GPUREG_TEXENV_UPDATE_BUFFER, UpdateBuffer, 2);

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

            Writer.SetCommand(PICARegister.GPUREG_COLORBUFFER_READ,  ColorBufferRead  ? 0xfu : 0u, 1);
            Writer.SetCommand(PICARegister.GPUREG_COLORBUFFER_WRITE, ColorBufferWrite ? 0xfu : 0u, 1);

            Writer.SetCommand(PICARegister.GPUREG_DEPTHBUFFER_READ,  StencilBufferRead,  DepthBufferRead);
            Writer.SetCommand(PICARegister.GPUREG_DEPTHBUFFER_WRITE, StencilBufferWrite, DepthBufferWrite);

            Matrix3x4[] TexMtx = new Matrix3x4[3];

            for (int Unit = 0; Unit < 3; Unit++)
            {
                if (TextureCoords[Unit].Scale != Vector2.Zero)
                {
                    TexMtx[Unit] = TextureCoords[Unit].GetTransform();
                }
                else
                {
                    TexMtx[Unit] = new Matrix3x4(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                }
            }

            Writer.SetCommand(PICARegister.GPUREG_VSH_FLOATUNIFORM_INDEX, 0x8000000bu);

            Writer.SetCommand(PICARegister.GPUREG_VSH_FLOATUNIFORM_DATA0, false,
                TexMtx[0].M41, TexMtx[0].M31, TexMtx[0].M21, TexMtx[0].M11,
                TexMtx[0].M42, TexMtx[0].M32, TexMtx[0].M22, TexMtx[0].M12,
                TexMtx[0].M43, TexMtx[0].M33, TexMtx[0].M23, TexMtx[0].M13,
                TexMtx[1].M41, TexMtx[1].M31, TexMtx[1].M21, TexMtx[1].M11,
                TexMtx[1].M42, TexMtx[1].M32, TexMtx[1].M22, TexMtx[1].M12,
                TexMtx[1].M43, TexMtx[1].M33, TexMtx[1].M23, TexMtx[1].M13,
                TexMtx[2].M41, TexMtx[2].M31, TexMtx[2].M21, TexMtx[2].M11,
                TexMtx[2].M42, TexMtx[2].M32, TexMtx[2].M22, TexMtx[2].M12);

            Writer.SetCommand(PICARegister.GPUREG_VSH_FLOATUNIFORM_INDEX, true, 0x80000013u,
                IOUtils.ToUInt32(TextureCoords[1].Translation.Y),
                IOUtils.ToUInt32(TextureCoords[1].Translation.X),
                IOUtils.ToUInt32(TextureCoords[0].Translation.Y),
                IOUtils.ToUInt32(TextureCoords[0].Translation.X));      

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

            GenerateUniqueId();

            return false;
        }
    }
}
