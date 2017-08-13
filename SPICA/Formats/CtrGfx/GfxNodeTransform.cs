using SPICA.Math3D;

using System.Numerics;

namespace SPICA.Formats.CtrGfx
{
    public class GfxNodeTransform : GfxNode
    {
        public Vector3 TransformScale;
        public Vector3 TransformRotation;
        public Vector3 TransformTranslation;

        public Matrix3x4 LocalTransform;
        public Matrix3x4 WorldTransform;
    }
}
