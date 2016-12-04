using System.Linq;

namespace SPICA.Formats.CtrH3D.Animation
{
    class H3DAnimBoolean
    {
        public float StartFrame;
        public float EndFrame;

        public H3DLoopType PreRepeat;
        public H3DLoopType PostRepeat;

        public ushort CurveIndex;

        public bool[] Values;

        public bool GetFrameValue(int Frame)
        {
            if (Frame < 0)
                return Values.First();
            else if (Frame >= Values.Length)
                return Values.Last();
            else
                return Values[Frame];
        }
    }
}
