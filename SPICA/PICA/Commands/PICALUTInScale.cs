using System.Xml.Serialization;

namespace SPICA.PICA.Commands
{
    public struct PICALUTInScale
    {
        [XmlAttribute] public PICALUTScale Dist0Scale;
        [XmlAttribute] public PICALUTScale Dist1Scale;
        [XmlAttribute] public PICALUTScale SpecularScale;
        [XmlAttribute] public PICALUTScale FresnelScale;
        [XmlAttribute] public PICALUTScale ReflecRScale;
        [XmlAttribute] public PICALUTScale ReflecGScale;
        [XmlAttribute] public PICALUTScale ReflecBScale;

        public PICALUTInScale(uint Param)
        {
            Dist0Scale    = (PICALUTScale)((Param >> 0)  & 7);
            Dist1Scale    = (PICALUTScale)((Param >> 4)  & 7);
            SpecularScale = (PICALUTScale)((Param >> 8)  & 7);
            FresnelScale  = (PICALUTScale)((Param >> 12) & 7);
            ReflecRScale  = (PICALUTScale)((Param >> 16) & 7);
            ReflecGScale  = (PICALUTScale)((Param >> 20) & 7);
            ReflecBScale  = (PICALUTScale)((Param >> 24) & 7);
        }

        public uint ToUInt32()
        {
            uint Param = 0;

            Param |= ((uint)Dist0Scale    & 7) << 0;
            Param |= ((uint)Dist1Scale    & 7) << 4;
            Param |= ((uint)SpecularScale & 7) << 8;
            Param |= ((uint)FresnelScale  & 7) << 12;
            Param |= ((uint)ReflecRScale  & 7) << 16;
            Param |= ((uint)ReflecGScale  & 7) << 20;
            Param |= ((uint)ReflecBScale  & 7) << 24;

            return Param;
        }
    }
}
