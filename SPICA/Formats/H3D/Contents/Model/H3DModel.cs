using SPICA.Formats.H3D.Contents.Model.Material;
using SPICA.Math;
using SPICA.Serialization.BinaryAttributes;

namespace SPICA.Formats.H3D.Contents.Model
{
    class H3DModel
    {
        public byte Flags;
        public byte SkeletonScalingType;
        public ushort SilhouetteMaterialEntries;
        public Matrix3x4 WorldTransform;

        [PointerOf("Materials")]
        private uint MaterialsAddress;

        [CountOf("Materials"), CountOf("MaterialsNameTree", 1)]
        private uint MaterialsCount;

        [PointerOf("MaterialsNameTree")]
        private uint MaterialsNameTreeAddress;

        [PointerOf("Meshes")]
        private uint MeshesAddress;

        [CountOf("Meshes")]
        private uint MeshesCount;

        [FixedCount(10)]
        public uint[] Unknown;

        [PointerOf("SkeletonBones")]
        private uint SkeletonAddress;

        [CountOf("SkeletonBones"), CountOf("SkeletonNameTree", 1)]
        private uint SkeletonCount;

        [PointerOf("SkeletonNameTree")]
        private uint SkeletonNameTreeAddress;

        [PointerOf("MeshVisibilities")]
        private uint MeshVisibilitiesAddress;

        [CountOf("MeshVisibilities")]
        private uint MeshVisibilitiesCount;

        [PointerOf("Name")]
        private uint NameAddress;

        [CountOf("MeshesNameTree", 1)]
        private uint MeshesNameTreeCount;

        [PointerOf("MeshesNameTree")]
        private uint MeshesNameTreeAddress;

        //Should be zero
        private uint UserDefinedAddress;

        [PointerOf("MetaData")]
        private uint MetaDataAddress;

        public bool[] MeshVisibilities;
        public H3DTreeNode[] MeshesNameTree;
        public H3DTreeNode[] MaterialsNameTree;
        public H3DMaterial[] Materials;
        public H3DMesh[] Meshes;
        public H3DTreeNode[] SkeletonNameTree;
        public H3DSkeletonBone[] SkeletonBones;
        public H3DMetaData MetaData;

        [TargetSection("StringsSection")]
        public string Name;
    }
}
