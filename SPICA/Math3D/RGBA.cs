using System.IO;
using System.Xml.Serialization;

namespace SPICA.Math3D
{
    public struct RGBA
    {
        [XmlAttribute] public byte R;
        [XmlAttribute] public byte G;
        [XmlAttribute] public byte B;
        [XmlAttribute] public byte A;

        public static RGBA Black { get { return new RGBA(0, 0, 0, 255); } }

        public static RGBA Gray { get { return new RGBA(127, 127, 127, 255); } }

        public static RGBA White { get { return new RGBA(255, 255, 255, 255); } }

        public RGBA(byte R, byte G, byte B, byte A)
        {
            this.R = R;
            this.G = G;
            this.B = B;
            this.A = A;
        }

        public RGBA(BinaryReader Reader)
        {
            R = Reader.ReadByte();
            G = Reader.ReadByte();
            B = Reader.ReadByte();
            A = Reader.ReadByte();
        }

        public override string ToString()
        {
            return $"R: {R} G: {G} B: {B} A: {A}";
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

        public void Write(BinaryWriter Writer)
        {
            Writer.Write(R);
            Writer.Write(G);
            Writer.Write(B);
            Writer.Write(A);
        }
    }
}
