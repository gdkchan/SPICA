using SPICA.Math3D;

namespace SPICA.Formats.H3D.Model
{
    struct H3DBone
    {
        public uint Flags;

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
