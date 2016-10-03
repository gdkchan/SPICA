using System;
using SPICA.Formats.H3D.Contents.Model.Material;
using SPICA.Math;
using SPICA.Serialization;
using SPICA.Serialization.BinaryAttributes;

namespace SPICA.Formats.H3D.Contents.Model
{
    class H3DModel : ICustomSerializer
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

        [FixedCount(8), CustomSerialization]
        public uint[] Unknown;

        [PointerOf("FaceCulling")]
        private uint FaceCullingAddress;

        [CountOf("FaceCulling")]
        private uint FaceCullingCount;

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
        public H3DFaceCulling[] FaceCulling;
        public H3DTreeNode[] SkeletonNameTree;
        public H3DSkeletonBone[] SkeletonBones;
        public H3DMetaData MetaData;

        [TargetSection("StringsSection")]
        public string Name;

        public object Serialize(BinarySerializer Serializer, string FName)
        {
            long Position = Serializer.BaseStream.Position;

            for (int Index = 0; Index < 8; Index++)
            {
                Serializer.AddPointer("SkeletonNameTree", this, Position, typeof(uint));
                Serializer.Relocator.AddPointer(Position);

                Position += 4;
            }

            return new uint[8];
        }
    }
}
