using SPICA.Formats.Utils;
using SPICA.Math3D;
using SPICA.Serialization.Attributes;

using System.Xml.Serialization;

namespace SPICA.Formats.CtrH3D.Model
{
    [Inline]
    public class H3DBone : INamed
    {
        public H3DBoneFlags Flags;

        public H3DBillboardMode BillboardMode
        {
            get { return (H3DBillboardMode)BitUtils.GetBits((uint)Flags, 16, 3); }
            set { Flags = (H3DBoneFlags)BitUtils.SetBits((uint)Flags, (uint)value, 16, 3); }
        }

        public short ParentIndex;

        private ushort Padding;

        public Vector3D Scale;
        public Vector3D Rotation;
        public Vector3D Translation;
        public Matrix3x4 InverseTransform;

        private string _Name;

        [XmlAttribute]
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public H3DMetaData MetaData;

        public Matrix3x4 Transform
        {
            get
            {
                Matrix3x4 Transform;

                Transform = Matrix3x4.Scale(Scale);
                Transform *= Matrix3x4.RotateX(Rotation.X);
                Transform *= Matrix3x4.RotateY(Rotation.Y);
                Transform *= Matrix3x4.RotateZ(Rotation.Z);
                Transform *= Matrix3x4.Translate(Translation);

                return Transform;
            }
        }

        public H3DBone()
        {
            InverseTransform = new Matrix3x4();
        }

        public void CalculateTransform(PatriciaList<H3DBone> Skeleton)
        {
            Matrix3x4 Transform = new Matrix3x4();

            H3DBone Bone = this;

            bool UniformScale = true;

            while (true)
            {
                Transform *= Bone.Transform;

                if (!Bone.Scale.IsOne) UniformScale = false;

                if (Bone.ParentIndex == -1) break;

                Bone = Skeleton[Bone.ParentIndex];
            }

            Flags = ParentIndex != -1 ? H3DBoneFlags.IsSegmentScaleCompensate : 0;

            if (UniformScale)       Flags |= H3DBoneFlags.IsScaleUniform;
            if (Scale.IsOne)        Flags |= H3DBoneFlags.IsScaleVolumeOne;
            if (Rotation.IsZero)    Flags |= H3DBoneFlags.IsRotationZero;
            if (Translation.IsZero) Flags |= H3DBoneFlags.IsTranslationZero;

            InverseTransform = Transform;
            InverseTransform.Invert();
        }
    }
}
