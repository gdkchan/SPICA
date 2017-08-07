using SPICA.Math3D;

using System.Collections.Generic;

namespace SPICA.Formats.CtrGfx.Animation
{
    public class GfxAnimMtxTransform
    {
        public readonly List<Matrix3x4> Frames;

        public GfxAnimMtxTransform()
        {
            Frames = new List<Matrix3x4>();
        }
    }
}
