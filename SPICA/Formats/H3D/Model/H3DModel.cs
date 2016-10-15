using SPICA.Formats.H3D.Model.Material;
using SPICA.Formats.H3D.Model.Mesh;
using SPICA.Math3D;
using SPICA.Serialization.Attributes;

using System.Collections.Generic;

namespace SPICA.Formats.H3D.Model
{
    class H3DModel : INamed
    {
        public H3DModelFlags Flags;
        public H3DBoneScaling BoneScaling;

        public ushort SilhouetteMaterialsCount;

        public Matrix3x4 WorldTransform;

        public PatriciaList<H3DMaterial> Materials;

        public List<H3DMesh> Meshes;

        [Range]
        public List<H3DMesh> MeshesLayer0;

        [Range]
        public List<H3DMesh> MeshesLayer1;

        [Range]
        public List<H3DMesh> MeshesLayer2;

        [Range]
        public List<H3DMesh> MeshesLayer3;

        public List<H3DSubMeshCulling> SubMeshCullings;

        public PatriciaList<H3DBone> Skeleton;

        public List<bool> MeshNodesVisibility;

        public string Name;

        public string ObjectName { get { return Name; } }

        public int MeshNodesCount;

        public PatriciaTree MeshNodesTree;

        public uint UserDefinedAddress;

        public H3DMetaData MetaData;
    }
}
