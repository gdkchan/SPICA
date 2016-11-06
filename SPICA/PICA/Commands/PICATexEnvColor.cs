using SPICA.Math3D;

namespace SPICA.PICA.Commands
{
    public struct PICATexEnvColor
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

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

        public RGBA ToRGBA()
        {
            return new RGBA(R, G, B, A);
        }
    }
}
