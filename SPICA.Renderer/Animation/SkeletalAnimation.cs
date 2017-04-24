using OpenTK;

using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Animation;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Renderer.SPICA_GL;

using System;

namespace SPICA.Renderer.Animation
{
    public class SkeletalAnimation : AnimationControl
    {
        private struct Bone
        {
            public int ParentIndex;

            public Vector3    Scale;
            public Vector3    Rotation;
            public Vector3    Translation;
            public Quaternion QuatRotation;

            public bool IsQuatRotation;
            public bool HasMtxTransform;
        }

        private const string InvalidPrimitiveTypeEx = "Invalid Primitive type used on Skeleton Bone {0}!";

        public Matrix4[] GetSkeletonTransforms(PatriciaList<H3DBone> Skeleton)
        {
            Matrix4[] Output = new Matrix4[Skeleton.Count];
            Bone[] FrameSkeleton = new Bone[Skeleton.Count];

            int Index = 0;

            foreach (H3DBone Bone in Skeleton)
            {
                Bone B = new Bone
                {
                    ParentIndex = Bone.ParentIndex,

                    Scale       = Bone.Scale.ToVector3(),
                    Rotation    = Bone.Rotation.ToVector3(),
                    Translation = Bone.Translation.ToVector3()
                };

                int Elem = BaseAnimation?.Elements.FindIndex(x => x.Name == Bone.Name) ?? -1;

                if (Elem != -1 && State != AnimationState.Stopped)
                {
                    H3DAnimationElement Element = BaseAnimation.Elements[Elem];

                    switch (Element.PrimitiveType)
                    {
                        case H3DAnimPrimitiveType.Transform:
                            SetBone((H3DAnimTransform)Element.Content, ref B);

                            break;

                        case H3DAnimPrimitiveType.QuatTransform:
                            SetBone((H3DAnimQuatTransform)Element.Content, ref B);

                            break;

                        case H3DAnimPrimitiveType.MtxTransform:
                            H3DAnimMtxTransform MtxTransform = (H3DAnimMtxTransform)Element.Content;

                            Output[Index] = MtxTransform.GetTransform((int)Frame).ToMatrix4();

                            B.HasMtxTransform = true;

                            break;

                        default: throw new InvalidOperationException(string.Format(InvalidPrimitiveTypeEx, Bone.Name));
                    }
                }

                FrameSkeleton[Index++] = B;
            }

            for (int i = 0; i < Skeleton.Count; i++)
            {
                int p, b = i;

                if (FrameSkeleton[b].HasMtxTransform) continue;

                bool ScaleCompensate = Skeleton[i].IsSegmentScaleCompensate;

                Output[i] = Matrix4.CreateScale(FrameSkeleton[b].Scale);

                do
                {
                    if (FrameSkeleton[b].IsQuatRotation)
                        Output[i] *= Matrix4.CreateFromQuaternion(FrameSkeleton[b].QuatRotation);
                    else
                        Output[i] *= RenderUtils.EulerRotate(FrameSkeleton[b].Rotation);

                    p = FrameSkeleton[b].ParentIndex;

                    /*
                     * Scale is inherited when Scale Compensate is not specified.
                     * Otherwise Scale only applies to the bone where it is set and child bones doesn't inherit it.
                     */
                    Vector3 Scale = p != -1 && ScaleCompensate ? FrameSkeleton[p].Scale : Vector3.One;

                    Output[i] *= Matrix4.CreateTranslation(Scale * FrameSkeleton[b].Translation);

                    if (p != -1 && !ScaleCompensate) Output[i] *= Matrix4.CreateScale(FrameSkeleton[p].Scale);
                }
                while ((b = p) != -1);
            }

            return Output;
        }

        private void SetBone(H3DAnimTransform Transform, ref Bone B)
        {
            if (Transform.ScaleX.HasData)       B.Scale.X       = Transform.ScaleX.GetFrameValue(Frame);
            if (Transform.ScaleY.HasData)       B.Scale.Y       = Transform.ScaleY.GetFrameValue(Frame);
            if (Transform.ScaleZ.HasData)       B.Scale.Z       = Transform.ScaleZ.GetFrameValue(Frame);

            if (Transform.RotationX.HasData)    B.Rotation.X    = Transform.RotationX.GetFrameValue(Frame);
            if (Transform.RotationY.HasData)    B.Rotation.Y    = Transform.RotationY.GetFrameValue(Frame);
            if (Transform.RotationZ.HasData)    B.Rotation.Z    = Transform.RotationZ.GetFrameValue(Frame);

            if (Transform.TranslationX.HasData) B.Translation.X = Transform.TranslationX.GetFrameValue(Frame);
            if (Transform.TranslationY.HasData) B.Translation.Y = Transform.TranslationY.GetFrameValue(Frame);
            if (Transform.TranslationZ.HasData) B.Translation.Z = Transform.TranslationZ.GetFrameValue(Frame);
        }

        private void SetBone(H3DAnimQuatTransform Transform, ref Bone B)
        {
            int IntFrame = (int)Frame;

            float Weight = Frame - IntFrame;

            if (Transform.HasScale)
            {
                Vector3 LHS = Transform.GetScaleValue(IntFrame + 0).ToVector3();
                Vector3 RHS = Transform.GetScaleValue(IntFrame + 1).ToVector3();

                B.Scale = Vector3.Lerp(LHS, RHS, Weight);
            }

            if (B.IsQuatRotation = Transform.HasRotation)
            {
                Quaternion LHS = Transform.GetRotationValue(IntFrame + 0).ToQuaternion();
                Quaternion RHS = Transform.GetRotationValue(IntFrame + 1).ToQuaternion();

                B.QuatRotation = Quaternion.Slerp(LHS, RHS, Weight);
            }

            if (Transform.HasTranslation)
            {
                Vector3 LHS = Transform.GetTranslationValue(IntFrame + 0).ToVector3();
                Vector3 RHS = Transform.GetTranslationValue(IntFrame + 1).ToVector3();

                B.Translation = Vector3.Lerp(LHS, RHS, Weight);
            }
        }
    }
}
