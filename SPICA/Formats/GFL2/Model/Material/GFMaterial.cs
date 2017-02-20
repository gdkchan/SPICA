using SPICA.Formats.Utils;
using SPICA.Math3D;
using SPICA.PICA;
using SPICA.PICA.Commands;

using System.Collections.Generic;
using System.IO;

namespace SPICA.Formats.GFL2.Model.Material
{
    public class GFMaterial
    {
        public string Name;

        public GFHashName SubMeshName;
        public GFHashName GeoShaderName;
        public GFHashName VtxShaderName;
        public GFHashName FragShaderName;

        public uint LUT0HashId;
        public uint LUT1HashId;
        public uint LUT2HashId;

        public sbyte BumpTexture;

        public byte Stage0ConstantIndex;
        public byte Stage1ConstantIndex;
        public byte Stage2ConstantIndex;
        public byte Stage3ConstantIndex;
        public byte Stage4ConstantIndex;
        public byte Stage5ConstantIndex;

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

        public int   EdgeType;
        public int   IDEdgeEnable;
        public int   EdgeID;
        public int   ProjectionType;
        public float RimPower;
        public float RimScale;
        public float PhongPower;
        public float PhongScale;
        public int   IDEdgeOffsetEnable;
        public int   EdgeMapAlphaMask;
        public int   BakeTexture0;
        public int   BakeTexture1;
        public int   BakeTexture2;
        public int   BakeConstant0;
        public int   BakeConstant1;
        public int   BakeConstant2;
        public int   BakeConstant3;
        public int   BakeConstant4;
        public int   BakeConstant5;
        public int   VertexShaderType;
        public float ShaderParam0;
        public float ShaderParam1;
        public float ShaderParam2;
        public float ShaderParam3;

        //Fragment Lighting
        public PICAColorOperation   ColorOperation;
        public PICABlendFunction    BlendFunction;
        public PICALogicalOperation LogicalOperation;
        public PICAAlphaTest        AlphaTest;
        public PICAStencilTest      StencilTest;
        public PICAStencilOperation StencilOperation;
        public PICADepthColorMask   DepthColorMask;
        public PICAFaceCulling      FaceCulling;

        //LookUp Table
        public PICALUTInAbs   LUTInAbs;
        public PICALUTInSel   LUTInSel;
        public PICALUTInScale LUTInScale;

        public bool ColorBufferRead;
        public bool ColorBufferWrite;

        public bool StencilBufferRead;
        public bool StencilBufferWrite;

        public bool DepthBufferRead;
        public bool DepthBufferWrite;

        public GFTextureCoord[] TextureCoords;

        public float[] TextureSources;

        public GFMaterial()
        {
            TextureCoords  = new GFTextureCoord[3];

            TextureSources = new float[3];
        }

        public GFMaterial(BinaryReader Reader, string MaterialName) : this()
        {
            Name = MaterialName;

            GFSection MaterialSection = new GFSection(Reader);

            long Position = Reader.BaseStream.Position;

            GFHashName[] Names = new GFHashName[4];

            for (int i = 0; i < Names.Length; i++)
            {
                Names[i] = new GFHashName(Reader);
            }

            SubMeshName    = Names[0];
            GeoShaderName  = Names[1];
            VtxShaderName  = Names[2];
            FragShaderName = Names[3];

            LUT0HashId = Reader.ReadUInt32();
            LUT1HashId = Reader.ReadUInt32();
            LUT2HashId = Reader.ReadUInt32();

            Reader.ReadUInt32(); //This seems to be always 0

            BumpTexture = (sbyte)Reader.ReadUInt16();

            Stage0ConstantIndex = Reader.ReadByte();
            Stage1ConstantIndex = Reader.ReadByte();
            Stage2ConstantIndex = Reader.ReadByte();
            Stage3ConstantIndex = Reader.ReadByte();
            Stage4ConstantIndex = Reader.ReadByte();
            Stage5ConstantIndex = Reader.ReadByte();

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

            EdgeType           = Reader.ReadInt32();
            IDEdgeEnable       = Reader.ReadInt32();
            EdgeID             = Reader.ReadInt32();
            ProjectionType     = Reader.ReadInt32();
            RimPower           = Reader.ReadSingle();
            RimScale           = Reader.ReadSingle();
            PhongPower         = Reader.ReadSingle();
            PhongScale         = Reader.ReadSingle();
            IDEdgeOffsetEnable = Reader.ReadInt32();
            EdgeMapAlphaMask   = Reader.ReadInt32();
            BakeTexture0       = Reader.ReadInt32();
            BakeTexture1       = Reader.ReadInt32();
            BakeTexture2       = Reader.ReadInt32();
            BakeConstant0      = Reader.ReadInt32();
            BakeConstant1      = Reader.ReadInt32();
            BakeConstant2      = Reader.ReadInt32();
            BakeConstant3      = Reader.ReadInt32();
            BakeConstant4      = Reader.ReadInt32();
            BakeConstant5      = Reader.ReadInt32();
            VertexShaderType   = Reader.ReadInt32();
            ShaderParam0       = Reader.ReadSingle();
            ShaderParam1       = Reader.ReadSingle();
            ShaderParam2       = Reader.ReadSingle();
            ShaderParam3       = Reader.ReadSingle();

            uint UnitsCount = Reader.ReadUInt32();

            for (int Unit = 0; Unit < UnitsCount; Unit++)
            {
                TextureCoords[Unit] = new GFTextureCoord(Reader);
            }

            GFSection.SkipPadding(Reader);

            uint CommandsLength = Reader.ReadUInt32();

            Reader.BaseStream.Seek(0x1c, SeekOrigin.Current);

            uint[] Commands = new uint[CommandsLength >> 2];

            for (int Index = 0; Index < Commands.Length; Index++)
            {
                Commands[Index] = Reader.ReadUInt32();
            }

            PICACommandReader CmdReader = new PICACommandReader(Commands);

            int UniformIndex = 0;

            Vector4D[] Uniform = new Vector4D[96];

            while (CmdReader.HasCommand)
            {
                PICACommand Cmd = CmdReader.GetCommand();

                uint Param = Cmd.Parameters[0];

                switch (Cmd.Register)
                {
                    case PICARegister.GPUREG_TEXUNIT0_BORDER_COLOR: TextureCoords[0].BorderColor = new PICATexEnvColor(Param); break;
                    case PICARegister.GPUREG_TEXUNIT1_BORDER_COLOR: TextureCoords[1].BorderColor = new PICATexEnvColor(Param); break;
                    case PICARegister.GPUREG_TEXUNIT2_BORDER_COLOR: TextureCoords[2].BorderColor = new PICATexEnvColor(Param); break;

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

                    case PICARegister.GPUREG_LIGHTING_LUTINPUT_ABS:    LUTInAbs   = new PICALUTInAbs(Param);   break;
                    case PICARegister.GPUREG_LIGHTING_LUTINPUT_SELECT: LUTInSel   = new PICALUTInSel(Param);   break;
                    case PICARegister.GPUREG_LIGHTING_LUTINPUT_SCALE:  LUTInScale = new PICALUTInScale(Param); break;

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

            Reader.BaseStream.Seek(Position + MaterialSection.Length, SeekOrigin.Begin);

            TextureSources[0] = Uniform[0].X;
            TextureSources[1] = Uniform[0].Y;
            TextureSources[2] = Uniform[0].Z;
        }

        public static List<GFMaterial> ReadList(BinaryReader Reader, GFHashName[] Names)
        {
            List<GFMaterial> Output = new List<GFMaterial>();

            foreach (GFHashName HN in Names)
            {
                Output.Add(new GFMaterial(Reader, HN.Name));
            }

            return Output;
        }
    }
}
