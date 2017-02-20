using System.Xml.Serialization;

namespace SPICA.PICA.Commands
{
    public struct PICATexEnvScale
    {
        [XmlAttribute] public PICATextureCombinerScale ColorScale;
        [XmlAttribute] public PICATextureCombinerScale AlphaScale;

        public PICATexEnvScale(uint Param)
        {
            ColorScale = (PICATextureCombinerScale)((Param >> 0) & 3);
            AlphaScale = (PICATextureCombinerScale)((Param >> 16) & 3);
        }

        public uint ToUInt32()
        {
            uint Param = 0;

            Param |= ((uint)ColorScale & 3) << 0;
            Param |= ((uint)AlphaScale & 3) << 16;

            return Param;
        }
    }
}
