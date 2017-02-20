using System.Xml.Serialization;

namespace SPICA.PICA.Commands
{
    public struct PICAColorOperation
    {
        [XmlAttribute] public PICAFragOpMode FragOpMode;
        [XmlAttribute] public PICABlendMode BlendMode;

        public PICAColorOperation(uint Param)
        {
            FragOpMode = (PICAFragOpMode)((Param >> 0) & 3);
            BlendMode = (PICABlendMode)((Param >> 8) & 1);
        }

        public uint ToUInt32()
        {
            uint Param = 0xe4u << 16;

            Param |= ((uint)FragOpMode & 3) << 0;
            Param |= ((uint)BlendMode  & 1) << 8;

            return Param;
        }
    }
}
