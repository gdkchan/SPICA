using System;

namespace SPICA.PICA.Commands
{
    public enum PICALUTInputScale
    {
        One = 0,
        Two = 1,
        Four = 2,
        Eight = 3,
        Quarter = 6,
        Half = 7
    }

    public static class PICALUTInputScaleExtensions
    {
        public static float ToFloat(this PICALUTInputScale Scale)
        {
            switch (Scale)
            {
                case PICALUTInputScale.One: return 1;
                case PICALUTInputScale.Two: return 2;
                case PICALUTInputScale.Four: return 4;
                case PICALUTInputScale.Eight: return 8;
                case PICALUTInputScale.Quarter: return 0.25f;
                case PICALUTInputScale.Half: return 0.5f;

                default: throw new ArgumentException("Invaid Scale value!");
            }
        }
    }
}
