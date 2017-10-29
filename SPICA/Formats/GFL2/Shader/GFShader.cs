using SPICA.Formats.Common;
using SPICA.Math3D;
using SPICA.PICA;
using SPICA.PICA.Commands;
using SPICA.PICA.Shader;

using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace SPICA.Formats.GFL2.Shader
{
    public class GFShader
    {
        public string Name;

        public readonly PICATexEnvStage[] TexEnvStages;

        public RGBA TexEnvBufferColor;

        public ShaderProgram VtxShader;
        public ShaderProgram GeoShader;

        public uint[]  Executable;
        public ulong[] Swizzles;

        public bool HasVertexShader => VtxShader != null;

        public Dictionary<uint, Vector4> VtxShaderUniforms { get; private set; }
        public Dictionary<uint, Vector4> GeoShaderUniforms { get; private set; }

        public GFShader()
        {
            TexEnvStages = new PICATexEnvStage[6];

            for (int Index = 0; Index < TexEnvStages.Length; Index++)
            {
                TexEnvStages[Index] = new PICATexEnvStage();
            }
        }

        public GFShader(BinaryReader Reader) : this()
        {
            uint MagicNumber = Reader.ReadUInt32();
            uint ShaderCount = Reader.ReadUInt32();

            GFSection.SkipPadding(Reader.BaseStream);

            GFSection ShaderSection = new GFSection(Reader);

            Name = Reader.ReadPaddedString(0x40);

            uint Hash  = Reader.ReadUInt32();
            uint Count = Reader.ReadUInt32();

            GFSection.SkipPadding(Reader.BaseStream);

            uint CommandsLength = Reader.ReadUInt32();
            uint CommandsCount  = Reader.ReadUInt32();
            uint CommandsHash   = Reader.ReadUInt32();
            uint Padding        = Reader.ReadUInt32();

            string FileName = Reader.ReadPaddedString(0x40);

            uint[] Commands = new uint[CommandsLength >> 2];

            for (int Index = 0; Index < Commands.Length; Index++)
            {
                Commands[Index] = Reader.ReadUInt32();
            }

            uint[] OutMap = new uint[7];

            List<uint>  ShaderExecutable = new List<uint>();
            List<ulong> ShaderSwizzles   = new List<ulong>();

            PICACommandReader CmdReader = new PICACommandReader(Commands);

            while (CmdReader.HasCommand)
            {
                PICACommand Cmd = CmdReader.GetCommand();

                uint Param = Cmd.Parameters[0];

                int Stage = ((int)Cmd.Register >> 3) & 7;

                if (Stage >= 6) Stage -= 2;

                switch (Cmd.Register)
                {
                    /* Shader */

                    case PICARegister.GPUREG_SH_OUTMAP_O0: OutMap[0] = Param; break;
                    case PICARegister.GPUREG_SH_OUTMAP_O1: OutMap[1] = Param; break;
                    case PICARegister.GPUREG_SH_OUTMAP_O2: OutMap[2] = Param; break;
                    case PICARegister.GPUREG_SH_OUTMAP_O3: OutMap[3] = Param; break;
                    case PICARegister.GPUREG_SH_OUTMAP_O4: OutMap[4] = Param; break;
                    case PICARegister.GPUREG_SH_OUTMAP_O5: OutMap[5] = Param; break;
                    case PICARegister.GPUREG_SH_OUTMAP_O6: OutMap[6] = Param; break;

                    /* Fragment Shader */

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

                    case PICARegister.GPUREG_TEXENV_UPDATE_BUFFER: PICATexEnvStage.SetUpdateBuffer(TexEnvStages, Param); break;

                    case PICARegister.GPUREG_TEXENV_BUFFER_COLOR: TexEnvBufferColor = new RGBA(Param); break;

                    /* Geometry Shader */

                    case PICARegister.GPUREG_GSH_ENTRYPOINT:
                        if (GeoShader == null)
                            GeoShader = new ShaderProgram();

                        GeoShader.MainOffset = Param & 0xffff;
                        break;

                    /* Vertex Shader */

                    case PICARegister.GPUREG_VSH_CODETRANSFER_DATA0:
                    case PICARegister.GPUREG_VSH_CODETRANSFER_DATA1:
                    case PICARegister.GPUREG_VSH_CODETRANSFER_DATA2:
                    case PICARegister.GPUREG_VSH_CODETRANSFER_DATA3:
                    case PICARegister.GPUREG_VSH_CODETRANSFER_DATA4:
                    case PICARegister.GPUREG_VSH_CODETRANSFER_DATA5:
                    case PICARegister.GPUREG_VSH_CODETRANSFER_DATA6:
                    case PICARegister.GPUREG_VSH_CODETRANSFER_DATA7:
                        ShaderExecutable.AddRange(Cmd.Parameters);
                        break;

                    case PICARegister.GPUREG_VSH_OPDESCS_DATA0:
                    case PICARegister.GPUREG_VSH_OPDESCS_DATA1:
                    case PICARegister.GPUREG_VSH_OPDESCS_DATA2:
                    case PICARegister.GPUREG_VSH_OPDESCS_DATA3:
                    case PICARegister.GPUREG_VSH_OPDESCS_DATA4:
                    case PICARegister.GPUREG_VSH_OPDESCS_DATA5:
                    case PICARegister.GPUREG_VSH_OPDESCS_DATA6:
                    case PICARegister.GPUREG_VSH_OPDESCS_DATA7:
                        for (int i = 0; i < Cmd.Parameters.Length; i++)
                        {
                            ShaderSwizzles.Add(Cmd.Parameters[i]);
                        }
                        break;

                    case PICARegister.GPUREG_VSH_ENTRYPOINT:
                        if (VtxShader == null)
                            VtxShader = new ShaderProgram();

                        VtxShader.MainOffset = Param & 0xffff;
                        break;
                }
            }

            Executable = ShaderExecutable.ToArray();
            Swizzles   = ShaderSwizzles.ToArray();

            for (int i = 0; i < OutMap.Length; i++)
            {
                if (OutMap[i] == 0) continue;

                ShaderOutputReg Reg = new ShaderOutputReg();

                for (int j = 0; j < 4; j++)
                {
                    uint Value = (OutMap[i] >> j * 8) & 0x1f;

                    if (Value != 0x1f)
                    {
                        Reg.Mask |= 1u << j;

                        if (Value < 0x4)
                            Reg.Name = ShaderOutputRegName.Position;
                        else if (Value < 0x8)
                            Reg.Name = ShaderOutputRegName.QuatNormal;
                        else if (Value < 0xc)
                            Reg.Name = ShaderOutputRegName.Color;
                        else if (Value < 0xe)
                            Reg.Name = ShaderOutputRegName.TexCoord0;
                        else if (Value < 0x10)
                            Reg.Name = ShaderOutputRegName.TexCoord1;
                        else if (Value < 0x11)
                            Reg.Name = ShaderOutputRegName.TexCoord0W;
                        else if (Value < 0x12)
                            Reg.Name = ShaderOutputRegName.Generic;
                        else if (Value < 0x16)
                            Reg.Name = ShaderOutputRegName.View;
                        else if (Value < 0x18)
                            Reg.Name = ShaderOutputRegName.TexCoord2;
                        else
                            Reg.Name = ShaderOutputRegName.Generic;
                    }
                }

                if (VtxShader != null)
                    VtxShader.OutputRegs[i] = Reg;

                if (GeoShader != null)
                    GeoShader.OutputRegs[i] = Reg;
            }

            HashSet<uint> Dsts = new HashSet<uint>();

            uint LblId = 0;

            for (uint i = 0; i < Executable.Length; i++)
            {
                ShaderOpCode OpCode = (ShaderOpCode)(Executable[i] >> 26);

                if (OpCode == ShaderOpCode.Call ||
                    OpCode == ShaderOpCode.CallC ||
                    OpCode == ShaderOpCode.CallU ||
                    OpCode == ShaderOpCode.JmpC ||
                    OpCode == ShaderOpCode.JmpU)
                {
                    uint Dst = (Executable[i] >> 10) & 0xfff;

                    if (!Dsts.Contains(Dst))
                    {
                        Dsts.Add(Dst);

                        string Name = "label_" + Dst.ToString("x4");

                        ShaderLabel Label = new ShaderLabel()
                        {
                            Id     = LblId++,
                            Offset = Dst,
                            Length = 0,
                            Name   = Name
                        };

                        if (VtxShader != null)
                            VtxShader.Labels.Add(Label);

                        if (GeoShader != null)
                            GeoShader.Labels.Add(Label);
                    }
                }
            }

            MakeArray(VtxShader?.Vec4Uniforms, "v_c");
            MakeArray(GeoShader?.Vec4Uniforms, "g_c");

            FindProgramEnd(VtxShader, Executable);
            FindProgramEnd(GeoShader, Executable);

            VtxShaderUniforms = CmdReader.GetAllVertexShaderUniforms();
            GeoShaderUniforms = CmdReader.GetAllGeometryShaderUniforms();
        }

        private void MakeArray(ShaderUniformVec4[] Uniforms, string Name)
        {
            //This is necessary because it's almost impossible to know what
            //is supposed to be an array without the SHBin information.
            //So we just make the entire thing an array to allow indexing.
            if (Uniforms != null)
            {
                for (int i = 0; i < Uniforms.Length; i++)
                {
                    Uniforms[i].Name        = Name;
                    Uniforms[i].IsArray     = true;
                    Uniforms[i].ArrayIndex  = i;
                    Uniforms[i].ArrayLength = Uniforms.Length;
                }
            }
        }

        private void FindProgramEnd(ShaderProgram Program, uint[] Executable)
        {
            if (Program != null)
            {
                for (uint i = Program.MainOffset; i < Executable.Length; i++)
                {
                    if ((ShaderOpCode)(Executable[i] >> 26) == ShaderOpCode.End)
                    {
                        Program.EndMainOffset = i;

                        break;
                    }
                }
            }
        }

        public ShaderBinary ToShaderBinary()
        {
            ShaderBinary Output = new ShaderBinary();

            Output.Executable = Executable;
            Output.Swizzles   = Swizzles;

            if (VtxShader != null) Output.Programs.Add(VtxShader);
            if (GeoShader != null) Output.Programs.Add(GeoShader);

            return Output;
        }
    }
}
