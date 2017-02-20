using System.Xml.Serialization;

namespace SPICA.PICA.Commands
{
    public struct PICALUTInAbs
    {
        [XmlAttribute] public bool Dist0Abs;
        [XmlAttribute] public bool Dist1Abs;
        [XmlAttribute] public bool SpecularAbs;
        [XmlAttribute] public bool FresnelAbs;
        [XmlAttribute] public bool ReflecRAbs;
        [XmlAttribute] public bool ReflecGAbs;
        [XmlAttribute] public bool ReflecBAbs;

        public PICALUTInAbs(uint Param)
        {
            Dist0Abs    = (Param & 0x00000002) == 0;
            Dist1Abs    = (Param & 0x00000020) == 0;
            SpecularAbs = (Param & 0x00000200) == 0;
            FresnelAbs  = (Param & 0x00002000) == 0;
            ReflecRAbs  = (Param & 0x00020000) == 0;
            ReflecGAbs  = (Param & 0x00200000) == 0;
            ReflecBAbs  = (Param & 0x02000000) == 0;
        }

        public uint ToUInt32()
        {
            uint Param = 0;

            Param |= (Dist0Abs    ? 0u : 1u) << 1;
            Param |= (Dist1Abs    ? 0u : 1u) << 5;
            Param |= (SpecularAbs ? 0u : 1u) << 9;
            Param |= (FresnelAbs  ? 0u : 1u) << 13;
            Param |= (ReflecRAbs  ? 0u : 1u) << 17;
            Param |= (ReflecGAbs  ? 0u : 1u) << 21;
            Param |= (ReflecBAbs  ? 0u : 1u) << 25;

            return Param;
        }
    }
}
