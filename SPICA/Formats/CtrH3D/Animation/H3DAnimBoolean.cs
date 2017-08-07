using System.Collections.Generic;
using System.Linq;

namespace SPICA.Formats.CtrH3D.Animation
{
    public class H3DAnimBoolean : H3DAnimationCurve
    {
        public readonly List<bool> Values;

        public H3DAnimBoolean()
        {
            Values = new List<bool>();
        }

        public bool GetFrameValue(int Frame)
        {
            if (Frame < 0)
                return Values.First();
            else if (Frame >= Values.Count)
                return Values.Last();
            else
                return Values[Frame];
        }
    }
}
