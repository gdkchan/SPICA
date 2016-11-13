using SPICA.Formats.CtrH3D.Model;
using SPICA.Math3D;

using System;
using System.Collections.Generic;

namespace SPICA.Formats.CtrH3D.Animation
{
    public class H3DAnimation : INamed
    {
        public string Name;

        public H3DAnimationFlags AnimationFlags;

        public float FramesCount;

        public List<H3DAnimationElement> Elements;

        public H3DMetaData MetaData;

        public string ObjectName { get { return Name; } }

        public Matrix3x4[] GetSkeletonTransform(PatriciaList<H3DBone> Skeleton, float Frame)
        {
            PatriciaList<H3DBone> FrameSkeleton = new PatriciaList<H3DBone>();

            foreach (H3DBone Bone in Skeleton)
            {
                H3DBone B = new H3DBone
                {
                    Name = Bone.Name,

                    ParentIndex = Bone.ParentIndex,

                    Translation = Bone.Translation,
                    Rotation = Bone.Rotation,
                    Scale = Bone.Scale
                };

                int Elem = Elements.FindIndex(x => x.Name == Bone.Name);

                if (Elem != -1)
                {
                    H3DAnimationElement Element = Elements[Elem];

                    switch (Element.PrimitiveType)
                    {
                        case H3DAnimPrimitiveType.Transform:
                            H3DAnimTransform Transform = (H3DAnimTransform)Element.Content;

                            if (Transform.ScaleX.HasData)       B.Scale.X       = Transform.ScaleX.GetFrameValue(Frame);
                            if (Transform.ScaleY.HasData)       B.Scale.Y       = Transform.ScaleY.GetFrameValue(Frame);
                            if (Transform.ScaleZ.HasData)       B.Scale.Z       = Transform.ScaleZ.GetFrameValue(Frame);

                            if (Transform.RotationX.HasData)    B.Rotation.X    = Transform.RotationX.GetFrameValue(Frame);
                            if (Transform.RotationY.HasData)    B.Rotation.Y    = Transform.RotationY.GetFrameValue(Frame);
                            if (Transform.RotationZ.HasData)    B.Rotation.Z    = Transform.RotationZ.GetFrameValue(Frame);

                            if (Transform.TranslationX.HasData) B.Translation.X = Transform.TranslationX.GetFrameValue(Frame);
                            if (Transform.TranslationY.HasData) B.Translation.Y = Transform.TranslationY.GetFrameValue(Frame);
                            if (Transform.TranslationZ.HasData) B.Translation.Z = Transform.TranslationZ.GetFrameValue(Frame);
                            break;

                        default: throw new NotImplementedException();
                    }
                }

                FrameSkeleton.Add(B);
            }

            Matrix3x4[] Output = new Matrix3x4[Skeleton.Count];

            for (int Index = 0; Index < Skeleton.Count; Index++)
            {
                Output[Index] = FrameSkeleton[Index].CalculateTransform(FrameSkeleton);
            }

            return Output;
        }
    }
}
