using SPICA.Formats.Common;
using SPICA.Formats.CtrGfx.Model.Material;
using SPICA.Formats.CtrGfx.Model.Mesh;
using SPICA.Math3D;
using SPICA.PICA.Commands;

using System.Collections.Generic;
using System.Numerics;

namespace SPICA.Formats.CtrGfx.Model
{
    public class GfxModel : INamed
    {
        private GfxVersion Revision;

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

        public readonly GfxDict<GfxMetaData> MetaData;

        public uint Unk0; //TODO: Check this

        public uint Flags2; //Flags of what?

        public int ChildsCount; //What is a model "child"? The meshes?

        public uint Unused;

        public readonly GfxDict<GfxAnimationsGroup> AnimationsGroup;

        public Vector3 TransformScale;
        public Vector3 TransformRotation;
        public Vector3 TransformTranslation;

        public Matrix3x4 LocalTransform;
        public Matrix3x4 WorldTransform;

        public readonly List<GfxMesh> Meshes;

        public readonly GfxDict<GfxMaterial> Materials;

        public readonly List<GfxShape> Shapes;

        public readonly GfxDict<GfxMeshNode> MeshNodes;

        public uint Flags3;

        public PICAFaceCulling FaceCulling;

        public int LayerId;

        public GfxSkeleton Skeleton;

        public GfxModel()
        {
            MetaData = new GfxDict<GfxMetaData>();

            AnimationsGroup = new GfxDict<GfxAnimationsGroup>();

            Meshes = new List<GfxMesh>();

            Materials = new GfxDict<GfxMaterial>();

            Shapes = new List<GfxShape>();

            MeshNodes = new GfxDict<GfxMeshNode>();
        }
    }
}
