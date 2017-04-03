using SPICA.Math3D;

namespace SPICA.Formats.MTFramework.Model
{
    class MTBone
    {
        public sbyte ParentIndex;
        public sbyte OppositeIndex;

        public float ChildDistance;
        public float ParentDistance;

        public Vector3D Position;

        public Matrix4x4 WorldTransform;
        public Matrix4x4 LocalTransform;
    }
}
