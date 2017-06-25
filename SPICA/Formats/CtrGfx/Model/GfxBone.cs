using SPICA.Formats.Common;
using SPICA.Math3D;

using System.Numerics;

namespace SPICA.Formats.CtrGfx.Model
{
    public class GfxBone : INamed
    {
        private string _Name;

        public string Name
        {
            get => _Name;
            set => _Name = value ?? throw Exceptions.GetNullException("Name");
        }

        public GfxBoneFlags Flags;

        public int Index;
        public int ParentIndex;

        public GfxBone Parent;
        public GfxBone Child;
        public GfxBone PrevSibling;
        public GfxBone NextSibling;

        public Vector3 Scale;
        public Vector3 Rotation;
        public Vector3 Translation;

        public Matrix3x4 LocalTransform;
        public Matrix3x4 WorldTransform;
        public Matrix3x4 InvWorldTransform;

        public GfxBillboardMode BillboardMode;

        public GfxDict<GfxMetaData> MetaData;
    }
}
