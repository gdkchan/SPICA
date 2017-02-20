using SPICA.Formats.Utils;

using SPICA.PICA;
using SPICA.PICA.Commands;

using System.IO;

namespace SPICA.Formats.GFL2.Shader
{
    public class GFFragShader
    {
        public string Name;

        public PICATexEnvStage[] TexEnvStages;
        public PICATexEnvColor TexEnvBufferColor;

        public GFFragShader()
        {
            TexEnvStages = new PICATexEnvStage[6];

            for (int Index = 0; Index < TexEnvStages.Length; Index++)
            {
                TexEnvStages[Index] = new PICATexEnvStage();
            }
        }

        public GFFragShader(BinaryReader Reader) : this()
        {
            uint MagicNumber = Reader.ReadUInt32();
            uint ShaderCount = Reader.ReadUInt32();

            GFSection.SkipPadding(Reader);
            GFSection ShaderSection = new GFSection(Reader);

            Name = Reader.ReadPaddedString(0x40);

            uint Hash = Reader.ReadUInt32();
            uint Count = Reader.ReadUInt32();

            GFSection.SkipPadding(Reader);

            uint CommandsLength = Reader.ReadUInt32();
            uint CommandsCount = Reader.ReadUInt32();
            uint CommandsHash = Reader.ReadUInt32();
            uint Padding = Reader.ReadUInt32();

            string FileName = Reader.ReadPaddedString(0x40);

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
                        TexEnvStages[1].UpdateRGBBuffer   = (Param & 0x100)  != 0;
                        TexEnvStages[2].UpdateRGBBuffer   = (Param & 0x200)  != 0;
                        TexEnvStages[3].UpdateRGBBuffer   = (Param & 0x400)  != 0;
                        TexEnvStages[4].UpdateRGBBuffer   = (Param & 0x800)  != 0;

                        TexEnvStages[1].UpdateAlphaBuffer = (Param & 0x1000) != 0;
                        TexEnvStages[2].UpdateAlphaBuffer = (Param & 0x2000) != 0;
                        TexEnvStages[3].UpdateAlphaBuffer = (Param & 0x4000) != 0;
                        TexEnvStages[4].UpdateAlphaBuffer = (Param & 0x8000) != 0;
                        break;

                    case PICARegister.GPUREG_TEXENV_BUFFER_COLOR: TexEnvBufferColor = new PICATexEnvColor(Param); break;
                }
            }
        }
    }
}
