using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.Formats.CtrH3D.Model.Mesh;
using SPICA.Math3D;
using SPICA.Serialization.Attributes;

using System;
using System.Collections.Generic;

namespace SPICA.Formats.CtrH3D.Model
{
    public class H3DModel : INamed
    {
        public H3DModelFlags Flags;
        public H3DBoneScaling BoneScaling;

        public ushort SilhouetteMaterialsCount;

        public Matrix3x4 WorldTransform;

        public PatriciaList<H3DMaterial> Materials;

        public List<H3DMesh> Meshes;

        [Range] public List<H3DMesh> MeshesLayer0;
        [Range] public List<H3DMesh> MeshesLayer1;
        [Range] public List<H3DMesh> MeshesLayer2;
        [Range] public List<H3DMesh> MeshesLayer3;

        public List<H3DSubMeshCulling> SubMeshCullings;

        public PatriciaList<H3DBone> Skeleton;

        public List<bool> MeshNodesVisibility;

        public string Name;

        public string ObjectName { get { return Name; } }

        public int MeshNodesCount;

        public PatriciaTree MeshNodesTree;

        private uint UserDefinedAddress;

        public H3DMetaData MetaData;

        public H3DModel()
        {
            WorldTransform = new Matrix3x4();

            Materials = new PatriciaList<H3DMaterial>();

            Meshes = new List<H3DMesh>();

            MeshesLayer0 = new List<H3DMesh>();
            MeshesLayer1 = new List<H3DMesh>();
            MeshesLayer2 = new List<H3DMesh>();
            MeshesLayer3 = new List<H3DMesh>();

            SubMeshCullings = new List<H3DSubMeshCulling>();

            Skeleton = new PatriciaList<H3DBone>();

            MeshNodesVisibility = new List<bool>();
        }

        public void AddMesh(H3DMesh Mesh, int Layer = 0)
        {
            Mesh.Parent = this;

            Meshes.Add(Mesh);

            switch (Layer)
            {
                case 0: MeshesLayer0.Add(Mesh); break;
                case 1: MeshesLayer1.Add(Mesh); break;
                case 2: MeshesLayer2.Add(Mesh); break;
                case 3: MeshesLayer3.Add(Mesh); break;

                default: throw new IndexOutOfRangeException("Invalid Layer! Expected 0, 1, 2 or 3!");
            }
        }

        public void AddMeshes(IEnumerable<H3DMesh> Meshes)
        {
            foreach (H3DMesh Mesh in Meshes) AddMesh(Mesh);
        }

        public void AddMeshes(params H3DMesh[] Meshes)
        {
            foreach (H3DMesh Mesh in Meshes) AddMesh(Mesh);
        }

        public void RemoveMesh(H3DMesh Mesh)
        {
            if (Meshes.Remove(Mesh))
            {
                MeshesLayer0.Remove(Mesh);
                MeshesLayer1.Remove(Mesh);
                MeshesLayer2.Remove(Mesh);
                MeshesLayer3.Remove(Mesh);
            }
        }

        public void ClearMeshes()
        {
            Meshes.Clear();

            MeshesLayer0.Clear();
            MeshesLayer1.Clear();
            MeshesLayer2.Clear();
            MeshesLayer3.Clear();
        }
    }
}
