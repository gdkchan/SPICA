using SPICA.Formats.Utils;
using SPICA.Math3D;
using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrH3D.Model
{
    [Inline]
    public class H3DBone : INamed
    {
        public H3DBoneFlags Flags;

        public H3DBillboardMode BillboardMode
        {
            get { return (H3DBillboardMode)BitUtils.GetBits((uint)Flags, 16, 3); }
            set { Flags = (H3DBoneFlags)BitUtils.SetBits((uint)Flags, (uint)value, 16, 3); }
        }

        public short ParentIndex;

        private ushort Padding;

        public Vector3D Scale;
        public Vector3D Rotation;
        public Vector3D Translation;
        public Matrix3x4 InverseTransform;

        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public H3DMetaData MetaData;

        public Matrix3x4 Transform
        {
            get
            {
                Matrix3x4 Transform;

                Transform = Matrix3x4.Scale(Scale);
                Transform *= Matrix3x4.RotateX(Rotation.X);
                Transform *= Matrix3x4.RotateY(Rotation.Y);
                Transform *= Matrix3x4.RotateZ(Rotation.Z);
                Transform *= Matrix3x4.Translate(Translation);

                return Transform;
            }
        }

        public H3DBone()
        {
            InverseTransform = new Matrix3x4();
        }

        public Matrix3x4 CalculateTransform(PatriciaList<H3DBone> Skeleton)
        {
            Matrix3x4 Transform = new Matrix3x4();

            H3DBone Bone = this;

            while (true)
            {
                Transform *= Bone.Transform;

                if (Bone.ParentIndex == -1) break;

                Bone = Skeleton[Bone.ParentIndex];
            }

            return Transform;
        }
    }
}
