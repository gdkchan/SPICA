using System.Xml.Serialization;

namespace SPICA.PICA.Commands
{
    public class PICATexEnvSource
    {
        [XmlAttribute] public PICATextureCombinerSource[] ColorSource;
        [XmlAttribute] public PICATextureCombinerSource[] AlphaSource;

        public PICATexEnvSource()
        {
            ColorSource = new PICATextureCombinerSource[3];
            AlphaSource = new PICATextureCombinerSource[3];
        }

        public PICATexEnvSource(uint Param)
        {
            ColorSource = new PICATextureCombinerSource[3];

            ColorSource[0] = (PICATextureCombinerSource)((Param >> 0) & 0xf);
            ColorSource[1] = (PICATextureCombinerSource)((Param >> 4) & 0xf);
            ColorSource[2] = (PICATextureCombinerSource)((Param >> 8) & 0xf);

            AlphaSource = new PICATextureCombinerSource[3];

            AlphaSource[0] = (PICATextureCombinerSource)((Param >> 16) & 0xf);
            AlphaSource[1] = (PICATextureCombinerSource)((Param >> 20) & 0xf);
            AlphaSource[2] = (PICATextureCombinerSource)((Param >> 24) & 0xf);
        }

        public uint ToUInt32()
        {
            uint Param = 0;

            Param |= ((uint)ColorSource[0] & 0xf) << 0;
            Param |= ((uint)ColorSource[1] & 0xf) << 4;
            Param |= ((uint)ColorSource[2] & 0xf) << 8;

            Param |= ((uint)AlphaSource[0] & 0xf) << 16;
            Param |= ((uint)AlphaSource[1] & 0xf) << 20;
            Param |= ((uint)AlphaSource[2] & 0xf) << 24;

            return Param;
        }
    }
}
