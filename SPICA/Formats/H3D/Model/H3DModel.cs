using SPICA.Formats.H3D.Model.Material;
using SPICA.Formats.H3D.Model.Mesh;
using SPICA.Math3D;

using System.Collections.Generic;

namespace SPICA.Formats.H3D.Model
{
    class H3DModel
    {
        public H3DModelFlags Flags;
        public H3DSkeletonScalingType SkeletonScalingType;

        public ushort SilhouetteMaterialsCount;

        public Matrix3x4 WorldTransform;

        public PatriciaList<H3DMaterial> Materials;

        public List<H3DMesh> Meshes;
        public RangeList<H3DMesh> MeshesLayer0;
        public RangeList<H3DMesh> MeshesLayer1;
        public RangeList<H3DMesh> MeshesLayer2;
        public RangeList<H3DMesh> MeshesLayer3;
        public List<H3DSubMeshCulling> SubMeshCullings;
        public PatriciaList<H3DBone> Skeleton;
        public List<bool> MeshVisibilities;

        public string Name;

        public int MeshesCount;

        public PatriciaTree MeshesTree;

        public uint UserDefinedAddress;

        public H3DMetaData MetaData;
    }
}
