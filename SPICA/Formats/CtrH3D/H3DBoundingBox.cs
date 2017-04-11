using SPICA.Math3D;

using System.Numerics;

namespace SPICA.Formats.CtrH3D
{
    public struct H3DBoundingBox
    {
        public Vector3   Center;
        public Matrix3x3 Orientation;
        public Vector3   Size;
    }
}
