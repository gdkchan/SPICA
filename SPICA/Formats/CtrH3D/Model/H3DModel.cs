using SPICA.Formats.Common;
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
        public H3DModelFlags  Flags;
        public H3DBoneScaling BoneScaling;

        public ushort SilhouetteMaterialsCount;

        public Matrix3x4 WorldTransform;

        public readonly H3DPatriciaList<H3DMaterial> Materials;

        public readonly List<H3DMesh> Meshes;

        [Range] public readonly List<H3DMesh> MeshesLayer0;
        [Range] public readonly List<H3DMesh> MeshesLayer1;
        [Range] public readonly List<H3DMesh> MeshesLayer2;

        /*
         * Very old versions of the BCH format only supported 3 layers.
         * Newer versions added another layer supporting up to 4.
         */
        [Range, IfVersion(CmpOp.Gequal, 7)] public readonly List<H3DMesh> MeshesLayer3;

        public readonly List<H3DSubMeshCulling> SubMeshCullings;

        public readonly H3DPatriciaList<H3DBone> Skeleton;

        public readonly List<bool> MeshNodesVisibility;

        private string _Name;

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value ?? throw Exceptions.GetNullException("Name");
            }
        }

        public int MeshNodesCount;

        public H3DPatriciaTree MeshNodesTree;

        private uint UserDefinedAddress;

        public H3DMetaData MetaData;

        public H3DModel()
        {
            WorldTransform = new Matrix3x4();

            Materials = new H3DPatriciaList<H3DMaterial>();

            Meshes = new List<H3DMesh>();

            MeshesLayer0 = new List<H3DMesh>();
            MeshesLayer1 = new List<H3DMesh>();
            MeshesLayer2 = new List<H3DMesh>();
            MeshesLayer3 = new List<H3DMesh>();

            SubMeshCullings = new List<H3DSubMeshCulling>();

            Skeleton = new H3DPatriciaList<H3DBone>();

            MeshNodesVisibility = new List<bool>();

            UserDefinedAddress = 0; //SBZ, set by program on 3DS
        }

        public void AddMesh(H3DMesh Mesh, int Layer = 0, int Priority = 0)
        {
            Mesh.Parent   = this;
            Mesh.Layer    = (uint)Layer;
            Mesh.Priority = (uint)Priority;

            Meshes.Add(Mesh);

            switch (Layer)
            {
                case 0: MeshesLayer0.Add(Mesh); break;
                case 1: MeshesLayer1.Add(Mesh); break;
                case 2: MeshesLayer2.Add(Mesh); break;
                case 3: MeshesLayer3.Add(Mesh); break;

                default: throw new ArgumentOutOfRangeException("Invalid Layer! Expected 0, 1, 2 or 3!");
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
