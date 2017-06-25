using SPICA.Formats.Common;
using SPICA.Formats.CtrGfx.Model.AnimGroup;
using SPICA.Formats.CtrGfx.Model.Material;
using SPICA.Formats.CtrGfx.Model.Mesh;
using SPICA.Math3D;
using SPICA.PICA.Commands;
using SPICA.Serialization.Attributes;

using System.Collections.Generic;
using System.Numerics;

namespace SPICA.Formats.CtrGfx.Model
{
    [TypeChoice(0x40000012u, typeof(GfxModel))]
    [TypeChoice(0x40000092u, typeof(GfxModelSkeletal))]
    public class GfxModel : INamed
    {
        private GfxRevHeader Header;

        private string _Name;

        public string Name
        {
            get => _Name;
            set => _Name = value ?? throw Exceptions.GetNullException("Name");
        }

        public readonly GfxDict<GfxMetaData> MetaData;

        private uint BranchVisible;

        private bool _IsBranchVisible;

        public bool IsBranchVisible
        {
            get => _IsBranchVisible;
            set
            {
                _IsBranchVisible = value;

                BranchVisible = (uint)BitUtils.SetBit(BranchVisible, value, 0);
            }
        }

        public readonly List<GfxModel> Childs;

        public readonly GfxDict<GfxAnimGroup> AnimationsGroup;

        public Vector3 TransformScale;
        public Vector3 TransformRotation;
        public Vector3 TransformTranslation;

        public Matrix3x4 LocalTransform;
        public Matrix3x4 WorldTransform;

        public readonly List<GfxMesh> Meshes;

        public readonly GfxDict<GfxMaterial> Materials;

        public readonly List<GfxShape> Shapes;

        public readonly GfxDict<GfxMeshNodeVisibility> MeshNodeVisibilities;

        public GfxModelFlags Flags;

        public PICAFaceCulling FaceCulling;

        public int LayerId;

        public GfxModel()
        {
            MetaData = new GfxDict<GfxMetaData>();

            AnimationsGroup = new GfxDict<GfxAnimGroup>();

            Meshes = new List<GfxMesh>();

            Materials = new GfxDict<GfxMaterial>();

            Shapes = new List<GfxShape>();

            MeshNodeVisibilities = new GfxDict<GfxMeshNodeVisibility>();
        }
    }
}
