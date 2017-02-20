using System.Xml.Serialization;

namespace SPICA.PICA.Commands
{
    public class PICATexEnvOperand
    {
        [XmlAttribute] public PICATextureCombinerColorOp[] ColorOp;
        [XmlAttribute] public PICATextureCombinerAlphaOp[] AlphaOp;

        public PICATexEnvOperand()
        {
            ColorOp = new PICATextureCombinerColorOp[3];
            AlphaOp = new PICATextureCombinerAlphaOp[3];
        }

        public PICATexEnvOperand(uint Param)
        {
            ColorOp = new PICATextureCombinerColorOp[3];

            ColorOp[0] = (PICATextureCombinerColorOp)((Param >> 0) & 0xf);
            ColorOp[1] = (PICATextureCombinerColorOp)((Param >> 4) & 0xf);
            ColorOp[2] = (PICATextureCombinerColorOp)((Param >> 8) & 0xf);

            AlphaOp = new PICATextureCombinerAlphaOp[3];

            AlphaOp[0] = (PICATextureCombinerAlphaOp)((Param >> 12) & 7);
            AlphaOp[1] = (PICATextureCombinerAlphaOp)((Param >> 16) & 7);
            AlphaOp[2] = (PICATextureCombinerAlphaOp)((Param >> 20) & 7);
        }

        public uint ToUInt32()
        {
            uint Param = 0;

            Param |= ((uint)ColorOp[0] & 0xf) << 0;
            Param |= ((uint)ColorOp[1] & 0xf) << 4;
            Param |= ((uint)ColorOp[2] & 0xf) << 8;

            Param |= ((uint)AlphaOp[0] & 0xf) << 12;
            Param |= ((uint)AlphaOp[1] & 0xf) << 16;
            Param |= ((uint)AlphaOp[2] & 0xf) << 20;

            return Param;
        }
    }
}
