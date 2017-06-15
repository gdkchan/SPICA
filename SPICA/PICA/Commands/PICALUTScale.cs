using System;

namespace SPICA.PICA.Commands
{
    public enum PICALUTScale : uint
    {
        One = 0,
        Two = 1,
        Four = 2,
        Eight = 3,
        Quarter = 6,
        Half = 7
    }

    public static class PICALUTScaleExtensions
    {
        public static float ToSingle(this PICALUTScale Scale)
        {
            switch (Scale)
            {
                case PICALUTScale.One:     return 1;
                case PICALUTScale.Two:     return 2;
                case PICALUTScale.Four:    return 4;
                case PICALUTScale.Eight:   return 8;
                case PICALUTScale.Quarter: return 0.25f;
                case PICALUTScale.Half:    return 0.5f;

                default: throw new ArgumentException("Invalid Scale value!");
            }
        }
    }
}
