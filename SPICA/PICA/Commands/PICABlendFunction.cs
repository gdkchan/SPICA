using System.Xml.Serialization;

namespace SPICA.PICA.Commands
{
    public struct PICABlendFunction
    {
        [XmlAttribute] public PICABlendEquation ColorEquation;
        [XmlAttribute] public PICABlendEquation AlphaEquation;

        [XmlAttribute] public PICABlendFunc ColorSourceFunc;
        [XmlAttribute] public PICABlendFunc ColorDestFunc;

        [XmlAttribute] public PICABlendFunc AlphaSourceFunc;
        [XmlAttribute] public PICABlendFunc AlphaDestFunc;

        public PICABlendFunction(uint Param)
        {
            ColorEquation = (PICABlendEquation)((Param >> 0) & 7);
            AlphaEquation = (PICABlendEquation)((Param >> 8) & 7);

            ColorSourceFunc = (PICABlendFunc)((Param >> 16) & 0xf);
            ColorDestFunc   = (PICABlendFunc)((Param >> 20) & 0xf);

            AlphaSourceFunc = (PICABlendFunc)((Param >> 24) & 0xf);
            AlphaDestFunc   = (PICABlendFunc)((Param >> 28) & 0xf);
        }

        public uint ToUInt32()
        {
            uint Param = 0;

            Param |= ((uint)ColorEquation & 7) << 0;
            Param |= ((uint)AlphaEquation & 7) << 8;

            Param |= ((uint)ColorSourceFunc & 0xf) << 16;
            Param |= ((uint)ColorDestFunc   & 0xf) << 20;

            Param |= ((uint)AlphaSourceFunc & 0xf) << 24;
            Param |= ((uint)AlphaDestFunc   & 0xf) << 28;

            return Param;
        }
    }
}
