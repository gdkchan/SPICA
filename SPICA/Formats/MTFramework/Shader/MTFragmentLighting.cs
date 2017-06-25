using SPICA.Math3D;
using SPICA.PICA.Commands;

using System.IO;

namespace SPICA.Formats.MTFramework.Shader
{
    public class MTFragmentLighting
    {
        public PICALUTInAbs      LUTInAbs;
        public PICALUTInSel      LUTInSel;
        public PICALUTInScale    LUTInScale;
        public PICATexEnvStage[] TexEnvStages;
        public RGBA              TexEnvBufferColor;

        public MTFragmentLighting(BinaryReader Reader)
        {
            Reader.BaseStream.Seek(0x4c, SeekOrigin.Current);

            uint TexEnvStagesAddress = Reader.ReadUInt32();

            Reader.BaseStream.Seek(TexEnvStagesAddress + 0xc, SeekOrigin.Begin);

            TexEnvStages = new PICATexEnvStage[6];

            for (int Stage = 0; Stage < 6; Stage++)
            {
                TexEnvStages[Stage] = new PICATexEnvStage()
                {
                    Source   = new PICATexEnvSource(Reader.ReadUInt32()),
                    Operand  = new PICATexEnvOperand(Reader.ReadUInt32()),
                    Combiner = new PICATexEnvCombiner(Reader.ReadUInt32()),
                    Color    = new RGBA(Reader.ReadUInt32()),
                    Scale    = new PICATexEnvScale(Reader.ReadUInt32())
                };
            }
        }
    }
}
