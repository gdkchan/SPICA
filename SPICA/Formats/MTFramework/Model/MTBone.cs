using System.Numerics;

namespace SPICA.Formats.MTFramework.Model
{
    public class MTBone
    {
        public sbyte ParentIndex;
        public sbyte OppositeIndex;

        public float ChildDistance;
        public float ParentDistance;

        public Vector3 Position;

        public Matrix4x4 WorldTransform;
        public Matrix4x4 LocalTransform;
    }
}
