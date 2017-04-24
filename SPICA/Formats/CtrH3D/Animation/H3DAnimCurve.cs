using SPICA.Formats.Common;
namespace SPICA.Formats.CtrH3D.Animation
{
    public struct H3DAnimCurve
    {
        public float StartFrame;
        public float EndFrame;

        public H3DLoopType PreRepeat;
        public H3DLoopType PostRepeat;

        public ushort CurveIndex;
    }
}
