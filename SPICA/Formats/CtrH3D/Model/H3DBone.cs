using SPICA.Formats.Common;
using SPICA.Math3D;
using SPICA.Serialization.Attributes;

using System.Numerics;

namespace SPICA.Formats.CtrH3D.Model
{
    [Inline]
    public class H3DBone : INamed
    {
        private H3DBoneFlags Flags;

        public bool IsSegmentScaleCompensate
        {
            get
            {
                return (Flags & H3DBoneFlags.IsSegmentScaleCompensate) != 0;
            }
            set
            {
                if (value)
                    Flags |= H3DBoneFlags.IsSegmentScaleCompensate;
                else
                    Flags &= ~H3DBoneFlags.IsSegmentScaleCompensate;
            }
        }

        public H3DBillboardMode BillboardMode
        {
            get
            {
                return (H3DBillboardMode)BitUtils.GetBits((uint)Flags, 16, 3);
            }
            set
            {
                Flags = (H3DBoneFlags)BitUtils.SetBits((uint)Flags, (uint)value, 16, 3);
            }
        }

        [Padding(4)] public short ParentIndex;

        public Vector3   Scale;
        public Vector3   Rotation;
        public Vector3   Translation;
        public Matrix3x4 InverseTransform;

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

        public H3DMetaData MetaData;

        public Matrix4x4 Transform
        {
            get
            {
                Matrix4x4 Transform;

                Transform  = Matrix4x4.CreateScale(Scale);
                Transform *= Matrix4x4.CreateRotationX(Rotation.X);
                Transform *= Matrix4x4.CreateRotationY(Rotation.Y);
                Transform *= Matrix4x4.CreateRotationZ(Rotation.Z);
                Transform *= Matrix4x4.CreateTranslation(Translation);

                return Transform;
            }
        }

        public H3DBone()
        {
            InverseTransform = new Matrix3x4();
        }

        public H3DBone(
            Vector3 Translation,
            Vector3 Rotation,
            Vector3 Scale,
            string Name,
            short Parent) : this()
        {
            this.Translation = Translation;
            this.Rotation    = Rotation;
            this.Scale       = Scale;
            this.Name        = Name;

            ParentIndex = Parent;
        }

        public Matrix4x4 GetWorldTransform(H3DDict<H3DBone> Skeleton)
        {
            Matrix4x4 Transform = Matrix4x4.Identity;

            H3DBone Bone = this;

            while (true)
            {
                Transform *= Bone.Transform;

                if (Bone.ParentIndex == -1) break;

                Bone = Skeleton[Bone.ParentIndex];
            }

            return Transform;
        }

        public void CalculateTransform(H3DDict<H3DBone> Skeleton)
        {
            Matrix4x4 Transform = Matrix4x4.Identity;

            H3DBone Bone = this;

            while (true)
            {
                Transform *= Bone.Transform;

                if (Bone.ParentIndex == -1) break;

                Bone = Skeleton[Bone.ParentIndex];
            }

            H3DBoneFlags Mask =
                H3DBoneFlags.IsScaleUniform |
                H3DBoneFlags.IsScaleVolumeOne |
                H3DBoneFlags.IsRotationZero |
                H3DBoneFlags.IsTranslationZero;

            Flags &= ~Mask;

            bool ScaleUniform = Scale.X == Scale.Y && Scale.X == Scale.Z;

            if (ScaleUniform)                Flags  = H3DBoneFlags.IsScaleUniform;
            if (Scale       == Vector3.One)  Flags |= H3DBoneFlags.IsScaleVolumeOne;
            if (Rotation    == Vector3.Zero) Flags |= H3DBoneFlags.IsRotationZero;
            if (Translation == Vector3.Zero) Flags |= H3DBoneFlags.IsTranslationZero;

            Matrix4x4 Inverse;
            Matrix4x4.Invert(Transform, out Inverse);

            InverseTransform = new Matrix3x4(Inverse);
        }
    }
}
