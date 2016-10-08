using SPICA.Math3D;
using SPICA.Utils;

namespace SPICA.Formats.H3D.Model
{
    struct H3DBone
    {
        public H3DBoneFlags Flags;

        public H3DBillboardMode BillboardMode
        {
            get { return (H3DBillboardMode)BitUtils.GetBits((uint)Flags, 16, 3); }
            set { Flags = (H3DBoneFlags)BitUtils.SetBits((uint)Flags, (uint)value, 16, 3); }
        }

        public short ParentIndex;

        public ushort Padding;

        public Vector3D Scale;
        public Vector3D Rotation;
        public Vector3D Translation;
        public Matrix3x4 InverseTransform;

        public string Name;

        public H3DMetaData MetaData;
    }
}
