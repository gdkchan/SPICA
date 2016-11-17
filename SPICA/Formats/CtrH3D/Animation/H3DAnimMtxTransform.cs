using SPICA.Math3D;

using System.Collections.Generic;
using System.Linq;

namespace SPICA.Formats.CtrH3D.Animation
{
    class H3DAnimMtxTransform
    {
        public float StartFrame;
        public float EndFrame;

        public H3DLoopType PreRepeat;
        public H3DLoopType PostRepeat;

        public ushort CurveIndex;

        public List<Matrix3x4> Frames;

        public Matrix3x4 GetTransform(int Frame)
        {
            if (Frames.Count > 0)
            {
                if (Frame < 0)
                    return Frames.First();
                else if (Frame >= Frames.Count)
                    return Frames.Last();
                else
                    return Frames[Frame];
            }
            else
            {
                return default(Matrix3x4);
            }
        }
    }
}
