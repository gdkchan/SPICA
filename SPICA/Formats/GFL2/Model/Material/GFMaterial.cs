using SPICA.Formats.GFL2.Utils;
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

        public RGBA Constant0Color;
        public RGBA Constant1Color;
        public RGBA Constant2Color;
        public RGBA Constant3Color;
        public RGBA Constant4Color;
        public RGBA Constant5Color;

        public RGBA EmissionColor;
        public RGBA AmbientColor;
        public RGBA DiffuseColor;
        public RGBA Specular0Color;
        public RGBA Specular1Color;
        
        public RGBA BlendColor;

        public float RimPower;
        public float RimScale;
        public float PhongPower;
        public float PhongScale;

        public bool IDEdgeOffsetEnable;
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

        public uint VertexShaderType;

        public float ShaderParam0;
        public float ShaderParam1;
        public float ShaderParam2;
        public float ShaderParam3;

        //LookUp Table
        public PICALUTInputAbs LUTInputAbs;

        public PICALUTInputSel LUTInputSel;

        public PICALUTInputScaleSel LUTInputScaleSel;

        //Fragment Lighting
        public PICAColorOperation ColorOperation;

        public PICABlendFunction BlendFunction;

        public PICALogicalOperation LogicalOperation;

        public PICAAlphaTest AlphaTest;

        public PICAStencilTest StencilTest;

        public PICAStencilOperation StencilOperation;

        public PICADepthColorMask DepthColorMask;

        public PICAFaceCulling FaceCulling;

        public bool ColorBufferRead;
        public bool ColorBufferWrite;

        public bool StencilBufferRead;
        public bool StencilBufferWrite;

        public bool DepthBufferRead;
        public bool DepthBufferWrite;

        public GFTextureCoord[] TextureCoords;

        public GFMaterial()
        {
            TextureCoords = new GFTextureCoord[3];
        }

        public GFMaterial(BinaryReader Reader, string MaterialName) : this()
        {
            Name = MaterialName;

            GFSection MaterialSection = new GFSection(Reader);

            long Position = Reader.BaseStream.Position;

            GFHashName[] Names = new GFHashName[4];

            for (int i = 0; i < Names.Length; i++)
            {
                uint Hash = Reader.ReadUInt32();
                string Name = GFString.ReadLength(Reader, Reader.ReadByte());

                Names[i] = new GFHashName(Hash, Name);
            }

            SubMeshName = Names[0];
            GeoShaderName = Names[1];
            VtxShaderName = Names[2];
            FragShaderName = Names[3];

            LUT0HashId = Reader.ReadUInt32();
            LUT1HashId = Reader.ReadUInt32();
            LUT2HashId = Reader.ReadUInt32();

            uint unk0 = Reader.ReadUInt32(); //TODO: Figure out

            BumpTexture = (sbyte)Reader.ReadUInt32();

            uint unk2 = Reader.ReadUInt32(); //TODO: Figure out

            Constant0Color = new RGBA(Reader);
            Constant1Color = new RGBA(Reader);
            Constant2Color = new RGBA(Reader);
            Constant3Color = new RGBA(Reader);
            Constant4Color = new RGBA(Reader);
            Constant5Color = new RGBA(Reader);

            uint unk3 = Reader.ReadUInt32(); //TODO: Figure out
            uint unk4 = Reader.ReadUInt32(); //TODO: Figure out
            uint unk5 = Reader.ReadUInt32(); //TODO: Figure out

            //System.Diagnostics.Debug.WriteLine(Name + " - " + unk0 + " " + unk1 + " " + unk2 + " " + unk3 + " " + unk4 + " " + unk5);

            EmissionColor = new RGBA(Reader);
            AmbientColor = new RGBA(Reader);
            DiffuseColor = new RGBA(Reader);

            Reader.BaseStream.Seek(0x10, SeekOrigin.Current);

            RimPower = Reader.ReadSingle();
            RimScale = Reader.ReadSingle();
            PhongPower = Reader.ReadSingle();
            PhongScale = Reader.ReadSingle();

            IDEdgeOffsetEnable = (Reader.ReadUInt32() & 1) != 0;
            EdgeMapAlphaMask = Reader.ReadInt32();

            BakeTexture0 = Reader.ReadInt32();
            BakeTexture1 = Reader.ReadInt32();
            BakeTexture2 = Reader.ReadInt32();

            BakeConstant0 = Reader.ReadInt32();
            BakeConstant1 = Reader.ReadInt32();
            BakeConstant2 = Reader.ReadInt32();
            BakeConstant3 = Reader.ReadInt32();
            BakeConstant4 = Reader.ReadInt32();
            BakeConstant5 = Reader.ReadInt32();

            VertexShaderType = Reader.ReadUInt32();

            ShaderParam0 = Reader.ReadSingle();
            ShaderParam1 = Reader.ReadSingle();
            ShaderParam2 = Reader.ReadSingle();
            ShaderParam3 = Reader.ReadSingle();

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

            while (CmdReader.HasCommand)
            {
                PICACommand Cmd = CmdReader.GetCommand();

                uint Param = Cmd.Parameters[0];

                switch (Cmd.Register)
                {
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
                        DepthBufferRead   = (Param & 2) != 0;
                        break;
                    case PICARegister.GPUREG_DEPTHBUFFER_WRITE:
                        StencilBufferWrite = (Param & 1) != 0;
                        DepthBufferWrite   = (Param & 2) != 0;
                        break;

                    case PICARegister.GPUREG_LIGHTING_LUTINPUT_ABS: LUTInputAbs = new PICALUTInputAbs(Param); break;
                    case PICARegister.GPUREG_LIGHTING_LUTINPUT_SELECT: LUTInputSel = new PICALUTInputSel(Param); break;
                    case PICARegister.GPUREG_LIGHTING_LUTINPUT_SCALE: LUTInputScaleSel = new PICALUTInputScaleSel(Param); break;

                    case PICARegister.GPUREG_TEXUNIT0_BORDER_COLOR: TextureCoords[0].BorderColor = new PICATexEnvColor(Param); break;
                    case PICARegister.GPUREG_TEXUNIT1_BORDER_COLOR: TextureCoords[1].BorderColor = new PICATexEnvColor(Param); break;
                    case PICARegister.GPUREG_TEXUNIT2_BORDER_COLOR: TextureCoords[2].BorderColor = new PICATexEnvColor(Param); break;
                }
            }

            Reader.BaseStream.Seek(Position + MaterialSection.Length, SeekOrigin.Begin);
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
