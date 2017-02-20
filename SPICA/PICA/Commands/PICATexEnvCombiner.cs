using System.Xml.Serialization;

namespace SPICA.PICA.Commands
{
    public struct PICATexEnvCombiner
    {
        [XmlAttribute] public PICATextureCombinerMode ColorCombiner;
        [XmlAttribute] public PICATextureCombinerMode AlphaCombiner;

        public PICATexEnvCombiner(uint Param)
        {
            ColorCombiner = (PICATextureCombinerMode)((Param >> 0) & 0xf);
            AlphaCombiner = (PICATextureCombinerMode)((Param >> 16) & 0xf);
        }

        public uint ToUInt32()
        {
            uint Param = 0;

            Param |= ((uint)ColorCombiner & 0xf) << 0;
            Param |= ((uint)AlphaCombiner & 0xf) << 16;

            return Param;
        }
    }
}
