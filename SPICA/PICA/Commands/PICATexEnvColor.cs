using System.Xml.Serialization;

namespace SPICA.PICA.Commands
{
    public struct PICATexEnvColor
    {
        [XmlAttribute] public byte R;
        [XmlAttribute] public byte G;
        [XmlAttribute] public byte B;
        [XmlAttribute] public byte A;

        public PICATexEnvColor(uint Param)
        {
            R = (byte)(Param >> 0);
            G = (byte)(Param >> 8);
            B = (byte)(Param >> 16);
            A = (byte)(Param >> 24);
        }

        public uint ToUInt32()
        {
            uint Param = 0;

            Param |= (uint)R << 0;
            Param |= (uint)G << 8;
            Param |= (uint)B << 16;
            Param |= (uint)A << 24;

            return Param;
        }

        public override string ToString()
        {
            return $"R: {R} G: {G} B: {B} A: {A}";
        }
    }
}
