using System;
using System.IO;
using System.Numerics;

namespace SPICA.Math3D
{
    public struct RGBA
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public static RGBA Black { get { return new RGBA(0, 0, 0, 255); } }

        public static RGBA White { get { return new RGBA(255, 255, 255, 255); } }

        public RGBA(byte R, byte G, byte B, byte A)
        {
            this.R = R;
            this.G = G;
            this.B = B;
            this.A = A;
        }

        public RGBA(uint Param)
        {
            R = (byte)(Param >>  0);
            G = (byte)(Param >>  8);
            B = (byte)(Param >> 16);
            A = (byte)(Param >> 24);
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
            uint Param;

            Param  = (uint)R << 0;
            Param |= (uint)G << 8;
            Param |= (uint)B << 16;
            Param |= (uint)A << 24;

            return Param;
        }

        private const float ByteToFloat = 1f / 255;

        public Vector4 ToVector4()
        {
            return new Vector4(
                (float)Math.Round(R * ByteToFloat, 2),
                (float)Math.Round(G * ByteToFloat, 2),
                (float)Math.Round(B * ByteToFloat, 2),
                (float)Math.Round(A * ByteToFloat, 2));
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
