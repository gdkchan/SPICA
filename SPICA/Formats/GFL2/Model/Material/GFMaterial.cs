using SPICA.Formats.Common;
using SPICA.Math3D;
using SPICA.PICA;
using SPICA.PICA.Commands;

using System.IO;
using System.Numerics;

namespace SPICA.Formats.GFL2.Model.Material
{
    public class GFMaterial : INamed
    {
        private const string MagicStr = "material";

        public string Name
        {
            get => MaterialName;
            set => MaterialName = value;
        }

        public string MaterialName;
        public string ShaderName;
        public string VtxShaderName;
        public string FragShaderName;

        public uint LUT0HashId;
        public uint LUT1HashId;
        public uint LUT2HashId;

        public sbyte BumpTexture;

        public byte Constant0Assignment;
        public byte Constant1Assignment;
        public byte Constant2Assignment;
        public byte Constant3Assignment;
        public byte Constant4Assignment;
        public byte Constant5Assignment;

        public RGBA Constant0Color;
        public RGBA Constant1Color;
        public RGBA Constant2Color;
        public RGBA Constant3Color;
        public RGBA Constant4Color;
        public RGBA Constant5Color;
        public RGBA Specular0Color;
        public RGBA Specular1Color;
        public RGBA BlendColor;
        public RGBA EmissionColor;
        public RGBA AmbientColor;
        public RGBA DiffuseColor;

        public int EdgeType;
        public int IDEdgeEnable;
        public int EdgeID;

        public int ProjectionType;

        public float RimPower;
        public float RimScale;
        public float PhongPower;
        public float PhongScale;

        public int IDEdgeOffsetEnable;

        public int EdgeMapAlphaMask;

        public int BakeTexture0;
        public int BakeTexture1;
        public int BakeTexture2;
        public int BakeConstant0;
        public int BakeConstant1;
        public int BakeConstant2;
        public int BakeConstant3;
        public int BakeConstant4;
        public int BakeConstant5; 

        public int VertexShaderType;

        public float ShaderParam0;
        public float ShaderParam1;
        public float ShaderParam2;
        public float ShaderParam3;

        public int RenderPriority;
        public int RenderLayer;

        public PICAColorOperation ColorOperation;

        public PICABlendFunction BlendFunction;

        public PICALogicalOp LogicalOperation;

        public PICAAlphaTest AlphaTest;

        public PICAStencilTest StencilTest;

        public PICAStencilOperation StencilOperation;

        public PICADepthColorMask DepthColorMask;

        public PICAFaceCulling FaceCulling;

        public PICALUTInAbs   LUTInputAbsolute;
        public PICALUTInSel   LUTInputSelection;
        public PICALUTInScale LUTInputScale;

        public bool ColorBufferRead;
        public bool ColorBufferWrite;

        public bool StencilBufferRead;
        public bool StencilBufferWrite;

        public bool DepthBufferRead;
        public bool DepthBufferWrite;

        public readonly GFTextureCoord[] TextureCoords;

        public readonly RGBA[] BorderColor;

        public readonly float[] TextureSources;

        public GFMaterial()
        {
            TextureCoords  = new GFTextureCoord[3];

            BorderColor = new RGBA[3];

            TextureSources = new float[4];
        }

        public GFMaterial(BinaryReader Reader) : this()
        {
            GFSection MaterialSection = new GFSection(Reader);

            long Position = Reader.BaseStream.Position;

            MaterialName   = new GFHashName(Reader).Name;
            ShaderName     = new GFHashName(Reader).Name;
            VtxShaderName  = new GFHashName(Reader).Name;
            FragShaderName = new GFHashName(Reader).Name;

            LUT0HashId = Reader.ReadUInt32();
            LUT1HashId = Reader.ReadUInt32();
            LUT2HashId = Reader.ReadUInt32();

            Reader.ReadUInt32(); //16 bytes padding

            BumpTexture = Reader.ReadSByte();

            Constant0Assignment = Reader.ReadByte();
            Constant1Assignment = Reader.ReadByte();
            Constant2Assignment = Reader.ReadByte();
            Constant3Assignment = Reader.ReadByte();
            Constant4Assignment = Reader.ReadByte();
            Constant5Assignment = Reader.ReadByte();

            Reader.ReadByte(); //8 bytes padding

            Constant0Color = new RGBA(Reader);
            Constant1Color = new RGBA(Reader);
            Constant2Color = new RGBA(Reader);
            Constant3Color = new RGBA(Reader);
            Constant4Color = new RGBA(Reader);
            Constant5Color = new RGBA(Reader);
            Specular0Color = new RGBA(Reader);
            Specular1Color = new RGBA(Reader);
            BlendColor     = new RGBA(Reader);
            EmissionColor  = new RGBA(Reader);
            AmbientColor   = new RGBA(Reader);
            DiffuseColor   = new RGBA(Reader);

            EdgeType     = Reader.ReadInt32();
            IDEdgeEnable = Reader.ReadInt32();
            EdgeID       = Reader.ReadInt32();

            ProjectionType = Reader.ReadInt32();

            RimPower   = Reader.ReadSingle();
            RimScale   = Reader.ReadSingle();
            PhongPower = Reader.ReadSingle();
            PhongScale = Reader.ReadSingle();

            IDEdgeOffsetEnable = Reader.ReadInt32();

            EdgeMapAlphaMask = Reader.ReadInt32();

            BakeTexture0  = Reader.ReadInt32();
            BakeTexture1  = Reader.ReadInt32();
            BakeTexture2  = Reader.ReadInt32();
            BakeConstant0 = Reader.ReadInt32();
            BakeConstant1 = Reader.ReadInt32();
            BakeConstant2 = Reader.ReadInt32();
            BakeConstant3 = Reader.ReadInt32();
            BakeConstant4 = Reader.ReadInt32();
            BakeConstant5 = Reader.ReadInt32();

            VertexShaderType = Reader.ReadInt32();

            ShaderParam0 = Reader.ReadSingle();
            ShaderParam1 = Reader.ReadSingle();
            ShaderParam2 = Reader.ReadSingle();
            ShaderParam3 = Reader.ReadSingle();

            uint UnitsCount = Reader.ReadUInt32();

            for (int Unit = 0; Unit < UnitsCount; Unit++)
            {
                TextureCoords[Unit] = new GFTextureCoord(Reader);
            }

            GFSection.SkipPadding(Reader.BaseStream);

            uint CommandsLength = Reader.ReadUInt32();

            RenderPriority = Reader.ReadInt32();
            Reader.ReadUInt32(); //Seems to be a 24 bits value.
            RenderLayer = Reader.ReadInt32();
            Reader.ReadUInt32(); //LUT 0 (Reflection R?) hash again?
            Reader.ReadUInt32(); //LUT 1 (Reflection G?) hash again?
            Reader.ReadUInt32(); //LUT 2 (Reflection B?) hash again?
            Reader.ReadUInt32(); //Another hash?

            uint[] Commands = new uint[CommandsLength >> 2];

            for (int Index = 0; Index < Commands.Length; Index++)
            {
                Commands[Index] = Reader.ReadUInt32();
            }

            PICACommandReader CmdReader = new PICACommandReader(Commands);

            while (CmdReader.HasCommand)
            {
                PICACommand Cmd = CmdReader.GetCommand();

                uint Param = Cmd.Parameters[0];

                switch (Cmd.Register)
                {
                    case PICARegister.GPUREG_TEXUNIT0_BORDER_COLOR: BorderColor[0] = new RGBA(Param); break;
                    case PICARegister.GPUREG_TEXUNIT1_BORDER_COLOR: BorderColor[1] = new RGBA(Param); break;
                    case PICARegister.GPUREG_TEXUNIT2_BORDER_COLOR: BorderColor[2] = new RGBA(Param); break;

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

                    case PICARegister.GPUREG_LIGHTING_LUTINPUT_ABS:    LUTInputAbsolute  = new PICALUTInAbs(Param);   break;
                    case PICARegister.GPUREG_LIGHTING_LUTINPUT_SELECT: LUTInputSelection = new PICALUTInSel(Param);   break;
                    case PICARegister.GPUREG_LIGHTING_LUTINPUT_SCALE:  LUTInputScale     = new PICALUTInScale(Param); break;
                }
            }

            TextureSources[0] = CmdReader.VtxShaderUniforms[0].X;
            TextureSources[1] = CmdReader.VtxShaderUniforms[0].Y;
            TextureSources[2] = CmdReader.VtxShaderUniforms[0].Z;
            TextureSources[3] = CmdReader.VtxShaderUniforms[0].W;

            Reader.BaseStream.Seek(Position + MaterialSection.Length, SeekOrigin.Begin);
        }

        public void Write(BinaryWriter Writer)
        {
            long StartPosition = Writer.BaseStream.Position;

            new GFSection(MagicStr).Write(Writer);            

            new GFHashName(MaterialName)  .Write(Writer);
            new GFHashName(ShaderName)    .Write(Writer);
            new GFHashName(VtxShaderName) .Write(Writer);
            new GFHashName(FragShaderName).Write(Writer);

            Writer.Write(LUT0HashId);
            Writer.Write(LUT1HashId);
            Writer.Write(LUT2HashId);

            Writer.Write(0u);

            Writer.Write(BumpTexture);

            Writer.Write(Constant0Assignment);
            Writer.Write(Constant1Assignment);
            Writer.Write(Constant2Assignment);
            Writer.Write(Constant3Assignment);
            Writer.Write(Constant4Assignment);
            Writer.Write(Constant5Assignment);

            Writer.Write((byte)0);

            Constant0Color.Write(Writer);
            Constant1Color.Write(Writer);
            Constant2Color.Write(Writer);
            Constant3Color.Write(Writer);
            Constant4Color.Write(Writer);
            Constant5Color.Write(Writer);
            Specular0Color.Write(Writer);
            Specular1Color.Write(Writer);
            BlendColor    .Write(Writer);
            EmissionColor .Write(Writer);
            AmbientColor  .Write(Writer);
            DiffuseColor  .Write(Writer);

            Writer.Write(EdgeType);
            Writer.Write(IDEdgeEnable);
            Writer.Write(EdgeID);

            Writer.Write(ProjectionType);

            Writer.Write(RimPower);
            Writer.Write(RimScale);
            Writer.Write(PhongPower);
            Writer.Write(PhongScale);

            Writer.Write(IDEdgeOffsetEnable);

            Writer.Write(EdgeMapAlphaMask);

            Writer.Write(BakeTexture0);
            Writer.Write(BakeTexture1);
            Writer.Write(BakeTexture2);
            Writer.Write(BakeConstant0);
            Writer.Write(BakeConstant1);
            Writer.Write(BakeConstant2);
            Writer.Write(BakeConstant3);
            Writer.Write(BakeConstant4);
            Writer.Write(BakeConstant5);

            Writer.Write(VertexShaderType);

            Writer.Write(ShaderParam0);
            Writer.Write(ShaderParam1);
            Writer.Write(ShaderParam2);
            Writer.Write(ShaderParam3);

            int UnitsCount =
                TextureCoords[2].Name != null ? 3 :
                TextureCoords[1].Name != null ? 2 :
                TextureCoords[0].Name != null ? 1 : 0;

            Writer.Write((uint)UnitsCount);

            float[] TexMtx = new float[UnitsCount * 12];

            for (int Unit = 0; Unit < UnitsCount; Unit++)
            {
                TextureCoords[Unit].Write(Writer);

                Matrix3x4 Mtx = TextureCoords[Unit].GetTransform();

                TexMtx[Unit * 12 + 0]  = Mtx.M41;
                TexMtx[Unit * 12 + 1]  = Mtx.M31;
                TexMtx[Unit * 12 + 2]  = Mtx.M21;
                TexMtx[Unit * 12 + 3]  = Mtx.M11;
                TexMtx[Unit * 12 + 4]  = Mtx.M42;
                TexMtx[Unit * 12 + 5]  = Mtx.M32;
                TexMtx[Unit * 12 + 6]  = Mtx.M22;
                TexMtx[Unit * 12 + 7]  = Mtx.M12;
                TexMtx[Unit * 12 + 8]  = Mtx.M43;
                TexMtx[Unit * 12 + 9]  = Mtx.M33;
                TexMtx[Unit * 12 + 10] = Mtx.M23;
                TexMtx[Unit * 12 + 11] = Mtx.M13;
            }

            Writer.Align(0x10, 0xff);

            PICACommandWriter CmdWriter = new PICACommandWriter();

            CmdWriter.SetCommand(PICARegister.GPUREG_VSH_FLOATUNIFORM_INDEX, true, 0x80000000u,
                IOUtils.ToUInt32(TextureSources[3]),
                IOUtils.ToUInt32(TextureSources[2]),
                IOUtils.ToUInt32(TextureSources[1]),
                IOUtils.ToUInt32(TextureSources[0]));

            CmdWriter.SetCommand(PICARegister.GPUREG_VSH_FLOATUNIFORM_INDEX, 0x80000001u);

            CmdWriter.SetCommand(PICARegister.GPUREG_VSH_FLOATUNIFORM_DATA0, false, TexMtx);    

            CmdWriter.SetCommand(PICARegister.GPUREG_FACECULLING_CONFIG, (uint)FaceCulling);

            CmdWriter.SetCommand(PICARegister.GPUREG_COLOR_OPERATION, ColorOperation.ToUInt32(), 3);

            CmdWriter.SetCommand(PICARegister.GPUREG_BLEND_FUNC, BlendFunction.ToUInt32());

            if (BlendFunction.ColorSrcFunc != PICABlendFunc.One  ||
                BlendFunction.ColorDstFunc != PICABlendFunc.Zero ||
                BlendFunction.AlphaSrcFunc != PICABlendFunc.One  ||
                BlendFunction.AlphaDstFunc != PICABlendFunc.Zero)
            {
                CmdWriter.SetCommand(PICARegister.GPUREG_LOGIC_OP, (uint)LogicalOperation);

                CmdWriter.SetCommand(PICARegister.GPUREG_BLEND_COLOR, BlendColor.ToUInt32() | 0xff000000u);
            }

            CmdWriter.SetCommand(PICARegister.GPUREG_FRAGOP_ALPHA_TEST, AlphaTest.ToUInt32(), 3);

            CmdWriter.SetCommand(PICARegister.GPUREG_STENCIL_TEST, StencilTest.ToUInt32());

            CmdWriter.SetCommand(PICARegister.GPUREG_STENCIL_OP, StencilOperation.ToUInt32());

            CmdWriter.SetCommand(PICARegister.GPUREG_DEPTH_COLOR_MASK, DepthColorMask.ToUInt32());

            CmdWriter.SetCommand(PICARegister.GPUREG_DEPTHMAP_ENABLE, true);
            CmdWriter.SetCommand(PICARegister.GPUREG_DEPTHMAP_SCALE, PICAVectorFloat24.GetWord24(-1));
            CmdWriter.SetCommand(PICARegister.GPUREG_DEPTHMAP_OFFSET, 0u);

            CmdWriter.SetCommand(PICARegister.GPUREG_FRAMEBUFFER_FLUSH, true);
            CmdWriter.SetCommand(PICARegister.GPUREG_FRAMEBUFFER_INVALIDATE, true);

            CmdWriter.SetCommand(PICARegister.GPUREG_COLORBUFFER_READ,  ColorBufferRead  ? 0xfu : 0u, 1);
            CmdWriter.SetCommand(PICARegister.GPUREG_COLORBUFFER_WRITE, ColorBufferWrite ? 0xfu : 0u, 1);

            CmdWriter.SetCommand(PICARegister.GPUREG_DEPTHBUFFER_READ,  StencilBufferRead,  DepthBufferRead);
            CmdWriter.SetCommand(PICARegister.GPUREG_DEPTHBUFFER_WRITE, StencilBufferWrite, DepthBufferWrite);

            uint TexUnitConfig = 0x00011000u;

            TexUnitConfig |= (TextureCoords[0].Name != null ? 1u : 0u) << 0;
            TexUnitConfig |= (TextureCoords[1].Name != null ? 1u : 0u) << 1;
            TexUnitConfig |= (TextureCoords[2].Name != null ? 1u : 0u) << 2;

            CmdWriter.SetCommands(PICARegister.GPUREG_TEXUNIT_CONFIG, false, 0, 0, 0, 0);
            CmdWriter.SetCommand(PICARegister.GPUREG_TEXUNIT_CONFIG, TexUnitConfig);

            CmdWriter.SetCommand(PICARegister.GPUREG_TEXUNIT0_BORDER_COLOR, BorderColor[0].ToUInt32());
            CmdWriter.SetCommand(PICARegister.GPUREG_TEXUNIT1_BORDER_COLOR, BorderColor[1].ToUInt32());
            CmdWriter.SetCommand(PICARegister.GPUREG_TEXUNIT2_BORDER_COLOR, BorderColor[2].ToUInt32());

            CmdWriter.SetCommand(PICARegister.GPUREG_LIGHTING_LUTINPUT_ABS,    LUTInputAbsolute.ToUInt32());
            CmdWriter.SetCommand(PICARegister.GPUREG_LIGHTING_LUTINPUT_SELECT, LUTInputSelection.ToUInt32());
            CmdWriter.SetCommand(PICARegister.GPUREG_LIGHTING_LUTINPUT_SCALE,  LUTInputScale.ToUInt32());

            CmdWriter.WriteEnd();

            uint[] Commands = CmdWriter.GetBuffer();

            Writer.Write((uint)(Commands.Length * 4));
            Writer.Write(RenderPriority);
            Writer.Write(0xd5738d); //FIXME: This number changes (depending on the cmd buff?)
            Writer.Write(RenderLayer);
            Writer.Write(LUT0HashId);
            Writer.Write(LUT1HashId);
            Writer.Write(LUT2HashId);
            Writer.Write(0xcd20dd3du); //TODO: Figure out what this means.

            foreach (uint Cmd in Commands)
            {
                Writer.Write(Cmd);
            }

            Writer.Write(0ul);
            Writer.Write(0ul);

            long EndPosition = Writer.BaseStream.Position;

            Writer.BaseStream.Seek(StartPosition + 8, SeekOrigin.Begin);

            Writer.Write((uint)(EndPosition - StartPosition - 0x10));

            Writer.BaseStream.Seek(EndPosition, SeekOrigin.Begin);
        }
    }
}
