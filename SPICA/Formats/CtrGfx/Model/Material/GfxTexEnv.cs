using SPICA.Math3D;
using SPICA.PICA;
using SPICA.PICA.Commands;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

using System.IO;

namespace SPICA.Formats.CtrGfx.Model.Material
{
    [Inline]
    public class GfxTexEnv : ICustomSerialization
    {
        public GfxTexEnvConstant Constant;

        [Inline, FixedLength(6)] private uint[] Commands;

        [Ignore] public readonly PICATexEnvStage Stage;

        [Ignore] internal int StageIndex;

        public GfxTexEnv()
        {
            Stage = new PICATexEnvStage();
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            PICACommandReader Reader = new PICACommandReader(Commands);

            while (Reader.HasCommand)
            {
                PICACommand Cmd = Reader.GetCommand();

                uint Param = Cmd.Parameters[0];

                switch (Cmd.Register)
                {
                    case PICARegister.GPUREG_TEXENV0_SOURCE:
                    case PICARegister.GPUREG_TEXENV1_SOURCE:
                    case PICARegister.GPUREG_TEXENV2_SOURCE:
                    case PICARegister.GPUREG_TEXENV3_SOURCE:
                    case PICARegister.GPUREG_TEXENV4_SOURCE:
                    case PICARegister.GPUREG_TEXENV5_SOURCE:
                        Stage.Source = new PICATexEnvSource(Param);
                        break;

                    case PICARegister.GPUREG_TEXENV0_OPERAND:
                    case PICARegister.GPUREG_TEXENV1_OPERAND:
                    case PICARegister.GPUREG_TEXENV2_OPERAND:
                    case PICARegister.GPUREG_TEXENV3_OPERAND:
                    case PICARegister.GPUREG_TEXENV4_OPERAND:
                    case PICARegister.GPUREG_TEXENV5_OPERAND:
                        Stage.Operand = new PICATexEnvOperand(Param);
                        break;

                    case PICARegister.GPUREG_TEXENV0_COMBINER:
                    case PICARegister.GPUREG_TEXENV1_COMBINER:
                    case PICARegister.GPUREG_TEXENV2_COMBINER:
                    case PICARegister.GPUREG_TEXENV3_COMBINER:
                    case PICARegister.GPUREG_TEXENV4_COMBINER:
                    case PICARegister.GPUREG_TEXENV5_COMBINER:
                        Stage.Combiner = new PICATexEnvCombiner(Param);
                        break;

                    case PICARegister.GPUREG_TEXENV0_COLOR:
                    case PICARegister.GPUREG_TEXENV1_COLOR:
                    case PICARegister.GPUREG_TEXENV2_COLOR:
                    case PICARegister.GPUREG_TEXENV3_COLOR:
                    case PICARegister.GPUREG_TEXENV4_COLOR:
                    case PICARegister.GPUREG_TEXENV5_COLOR:
                        Stage.Color = new RGBA(Param);
                        break;

                    case PICARegister.GPUREG_TEXENV0_SCALE:
                    case PICARegister.GPUREG_TEXENV1_SCALE:
                    case PICARegister.GPUREG_TEXENV2_SCALE:
                    case PICARegister.GPUREG_TEXENV3_SCALE:
                    case PICARegister.GPUREG_TEXENV4_SCALE:
                    case PICARegister.GPUREG_TEXENV5_SCALE:
                        Stage.Scale = new PICATexEnvScale(Param);
                        break;
                }
            }
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            RebuildCommands();

            return false;
        }

        internal byte[] GetBytes()
        {
            using (MemoryStream MS = new MemoryStream())
            {
                BinaryWriter Writer = new BinaryWriter(MS);

                Writer.Write((uint)Constant);
                Writer.Write(GetGLSrc(Stage.Source.Color[0]));
                Writer.Write(GetGLSrc(Stage.Source.Color[1]));
                Writer.Write(GetGLSrc(Stage.Source.Color[2]));
                Writer.Write(GetGLOp(Stage.Operand.Color[0]));
                Writer.Write(GetGLOp(Stage.Operand.Color[1]));
                Writer.Write(GetGLOp(Stage.Operand.Color[2]));
                Writer.Write(GetGLSrc(Stage.Source.Alpha[0]));
                Writer.Write(GetGLSrc(Stage.Source.Alpha[1]));
                Writer.Write(GetGLSrc(Stage.Source.Alpha[2]));
                Writer.Write(GetGLOp(Stage.Operand.Alpha[0]));
                Writer.Write(GetGLOp(Stage.Operand.Alpha[1]));
                Writer.Write(GetGLOp(Stage.Operand.Alpha[2]));

                RebuildCommands();

                foreach (uint Cmd in Commands)
                {
                    Writer.Write(Cmd);
                }

                return MS.ToArray();
            }
        }

        private uint GetGLSrc(PICATextureCombinerSource Source)
        {
            switch (Source)
            {
                case PICATextureCombinerSource.Texture0:               return 0x84c0; //Texture 0
                case PICATextureCombinerSource.Texture1:               return 0x84c1; //Texture 1
                case PICATextureCombinerSource.Texture2:               return 0x84c2; //Texture 2
                case PICATextureCombinerSource.Texture3:               return 0x84c3; //Texture 3
                case PICATextureCombinerSource.Constant:               return 0x8576; //Constant
                case PICATextureCombinerSource.PrimaryColor:           return 0x8577; //Primary Color
                case PICATextureCombinerSource.Previous:               return 0x8578; //Previous
                case PICATextureCombinerSource.PreviousBuffer:         return 0x8579; //Does not exist on OpenGL
                case PICATextureCombinerSource.FragmentPrimaryColor:   return 0x6210; //Does not exist on OpenGL
                case PICATextureCombinerSource.FragmentSecondaryColor: return 0x6211; //Does not exist on OpenGL
            }

            return 0;
        }

        private uint GetGLOp(PICATextureCombinerColorOp Source)
        {
            switch (Source)
            {
                case PICATextureCombinerColorOp.Color:         return 0x0300; //Source Color
                case PICATextureCombinerColorOp.OneMinusColor: return 0x0301; //1 - Source Color
                case PICATextureCombinerColorOp.Alpha:         return 0x0302; //Source Alpha
                case PICATextureCombinerColorOp.OneMinusAlpha: return 0x0303; //1 - Source Alpha
                case PICATextureCombinerColorOp.Red:           return 0x8580; //Src0 RGB
                case PICATextureCombinerColorOp.Green:         return 0x8581; //Src1 RGB
                case PICATextureCombinerColorOp.Blue:          return 0x8582; //Src2 RGB
                case PICATextureCombinerColorOp.OneMinusRed:   return 0x8583; //Does not exist on OpenGL
                case PICATextureCombinerColorOp.OneMinusGreen: return 0x8584; //Does not exist on OpenGL
                case PICATextureCombinerColorOp.OneMinusBlue:  return 0x8585; //Does not exist on OpenGL
            }

            return 0;
        }

        private uint GetGLOp(PICATextureCombinerAlphaOp Source)
        {
            switch (Source)
            {
                case PICATextureCombinerAlphaOp.Alpha:         return 0x0302; //Source Alpha
                case PICATextureCombinerAlphaOp.OneMinusAlpha: return 0x0303; //1 - Source Alpha
                case PICATextureCombinerAlphaOp.Red:           return 0x8580; //Src0 RGB
                case PICATextureCombinerAlphaOp.Green:         return 0x8581; //Src1 RGB
                case PICATextureCombinerAlphaOp.Blue:          return 0x8582; //Src2 RGB
                case PICATextureCombinerAlphaOp.OneMinusRed:   return 0x8583; //Does not exist on OpenGL
                case PICATextureCombinerAlphaOp.OneMinusGreen: return 0x8584; //Does not exist on OpenGL
                case PICATextureCombinerAlphaOp.OneMinusBlue:  return 0x8585; //Does not exist on OpenGL
            }

            return 0;
        }

        private void RebuildCommands()
        {
            PICACommandWriter Writer = new PICACommandWriter();

            PICARegister Register = PICARegister.GPUREG_DUMMY;

            switch (StageIndex)
            {
                case 0: Register = PICARegister.GPUREG_TEXENV0_SOURCE; break;
                case 1: Register = PICARegister.GPUREG_TEXENV1_SOURCE; break;
                case 2: Register = PICARegister.GPUREG_TEXENV2_SOURCE; break;
                case 3: Register = PICARegister.GPUREG_TEXENV3_SOURCE; break;
                case 4: Register = PICARegister.GPUREG_TEXENV4_SOURCE; break;
                case 5: Register = PICARegister.GPUREG_TEXENV5_SOURCE; break;
            }

            Writer.SetCommand(Register, true,
                Stage.Source.ToUInt32(),
                Stage.Operand.ToUInt32(),
                Stage.Combiner.ToUInt32(),
                Stage.Color.ToUInt32(),
                Stage.Scale.ToUInt32());

            Commands = Writer.GetBuffer();
        }
    }
}
