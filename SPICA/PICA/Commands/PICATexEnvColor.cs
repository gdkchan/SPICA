using SPICA.Math3D;

namespace SPICA.PICA.Commands
{
    public class PICATexEnvColor : RGBA
    {
        public PICATexEnvColor() { }

        public PICATexEnvColor(uint Param)
        {
            R = (byte)(Param >> 0);
            G = (byte)(Param >> 8);
            B = (byte)(Param >> 16);
            A = (byte)(Param >> 24);
        }
    }
}
