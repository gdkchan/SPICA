using SPICA.Formats.GFL2.Utils;
using SPICA.Math3D;
using SPICA.PICA;
using SPICA.PICA.Commands;
using System;
using System.IO;

namespace SPICA.Formats.GFL2.Model.Material
{
    class GFMaterial
    {
        public string Name;

        public uint LUT0HashId;
        public uint LUT1HashId;
        public uint LUT2HashId;

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

        public Vector4D ShaderParams;

        //LookUp Table
        [NonSerialized]
        public PICALUTInputAbs LUTInputAbs;

        [NonSerialized]
        public PICALUTInputSel LUTInputSel;

        [NonSerialized]
        public PICALUTInputScaleSel LUTInputScaleSel;

        //Fragment Lighting
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

        public GFTextureCoord[] TextureCoords;

        public GFMaterial()
        {
            TextureCoords = new GFTextureCoord[3];
        }

        public GFMaterial(BinaryReader Reader, string MaterialName)
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

            LUT0HashId = Reader.ReadUInt32();
            LUT1HashId = Reader.ReadUInt32();
            LUT2HashId = Reader.ReadUInt32();

            Reader.ReadUInt32(); //TODO: Figure out
            Reader.ReadUInt32(); //TODO: Figure out
            Reader.ReadUInt32(); //TODO: Figure out

            Constant0Color = new RGBA(Reader);
            Constant1Color = new RGBA(Reader);
            Constant2Color = new RGBA(Reader);
            Constant3Color = new RGBA(Reader);
            Constant4Color = new RGBA(Reader);
            Constant5Color = new RGBA(Reader);

            Reader.ReadUInt32(); //TODO: Figure out
            Reader.ReadUInt32(); //TODO: Figure out
            Reader.ReadUInt32(); //TODO: Figure out

            EmissionColor = new RGBA(Reader);
            AmbientColor = new RGBA(Reader);
            DiffuseColor = new RGBA(Reader);

            Reader.BaseStream.Seek(0x10, SeekOrigin.Current);

            RimPower = Reader.ReadSingle();
            RimScale = Reader.ReadSingle();
            PhongPower = Reader.ReadSingle();
            PhongScale = Reader.ReadSingle();

            Reader.BaseStream.Seek(0x30, SeekOrigin.Current);

            ShaderParams = new Vector4D(Reader);

            uint UnitsCount = Reader.ReadUInt32();

            TextureCoords = new GFTextureCoord[3];

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
    }
}
