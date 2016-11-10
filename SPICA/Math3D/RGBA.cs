using System.IO;

namespace SPICA.Math3D
{
    public struct RGBA
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

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
            return string.Format("R: {0} G: {1} B: {2} A: {3}", R, G, B, A);
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
