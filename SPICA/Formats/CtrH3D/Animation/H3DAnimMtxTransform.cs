using SPICA.Math3D;
using SPICA.Serialization.Attributes;

using System.Collections.Generic;
using System.Linq;

namespace SPICA.Formats.CtrH3D.Animation
{
    public class H3DAnimMtxTransform : H3DAnimationCurve
    {
        public readonly List<Matrix3x4> Frames;

        public H3DAnimMtxTransform()
        {
            Frames = new List<Matrix3x4>();
        }

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
