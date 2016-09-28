using SPICA.Math;
using SPICA.Serialization.BinaryAttributes;

namespace SPICA.Formats.H3D.Contents.Model
{
    struct H3DSkeletonBone
    {
        public uint Flags;
        public short ParentIndex;
        public ushort Padding;
        public Vector3D Scale;
        public Vector3D Rotation;
        public Vector3D Translation;
        public Matrix3x4 InverseTransform;

        [PointerOf("Name")]
        private uint NameAddress;

        [PointerOf("MetaData")]
        private uint MetaDataAddress;

        [TargetSection("StringsSection")]
        public string Name;

        public H3DMetaData MetaData;
    }
}
