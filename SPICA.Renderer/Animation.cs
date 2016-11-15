using OpenTK;

using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Animation;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Renderer.SPICA_GL;

using System;

namespace SPICA.Renderer
{
    public struct Animation
    {
        public float Frame;
        public float Step;

        private H3DAnimation BaseAnimation;

        public bool HasData { get { return BaseAnimation != null; } }

        public Animation(H3DAnimation BaseAnimation, float Frame, float Step)
        {
            this.Frame = Frame;
            this.Step = Step;

            this.BaseAnimation = BaseAnimation;
        }

        public void AdvanceFrame()
        {
            if (BaseAnimation != null)
            {
                Frame += Step;

                if (Frame >= BaseAnimation.FramesCount)
                {
                    Frame -= BaseAnimation.FramesCount;
                }
            }
        }

        private struct Bone
        {
            public int ParentIndex;

            public Vector3 Scale;
            public Vector3 Rotation;
            public Vector3 Translation;

            public Quaternion QuatRotation;

            public bool IsQuatRotation;
        }

        public Matrix4[] GetSkeletonTransform(PatriciaList<H3DBone> Skeleton)
        {
            Matrix4[] Output = new Matrix4[Skeleton.Count];
            Bone[] FrameSkeleton = new Bone[Skeleton.Count];

            int Index = 0;

            foreach (H3DBone Bone in Skeleton)
            {
                Bone B = new Bone
                {
                    ParentIndex = Bone.ParentIndex,

                    Scale       = GLConverter.ToVector3(Bone.Scale),
                    Rotation    = GLConverter.ToVector3(Bone.Rotation),
                    Translation = GLConverter.ToVector3(Bone.Translation)
                };

                if (BaseAnimation != null)
                {
                    int Elem = BaseAnimation.Elements.FindIndex(x => x.Name == Bone.Name);

                    if (Elem != -1)
                    {
                        H3DAnimationElement Element = BaseAnimation.Elements[Elem];

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

                            case H3DAnimPrimitiveType.QuatTransform:
                                H3DAnimQuatTransform QuatTransform = (H3DAnimQuatTransform)Element.Content;

                                int IntFrame = (int)Frame;
                                float Weight = Frame - IntFrame;

                                if (QuatTransform.HasScale)
                                {
                                    Vector3 L = GLConverter.ToVector3(QuatTransform.GetScaleValue(IntFrame + 0));
                                    Vector3 R = GLConverter.ToVector3(QuatTransform.GetScaleValue(IntFrame + 1));

                                    B.Scale = Vector3.Lerp(L, R, Weight);
                                }

                                if (B.IsQuatRotation = QuatTransform.HasRotation)
                                {
                                    Quaternion L = GLConverter.ToQuaternion(QuatTransform.GetRotationValue(IntFrame + 0));
                                    Quaternion R = GLConverter.ToQuaternion(QuatTransform.GetRotationValue(IntFrame + 1));

                                    B.QuatRotation = Quaternion.Slerp(L, R, Weight);
                                }

                                if (QuatTransform.HasTranslation)
                                {
                                    Vector3 L = GLConverter.ToVector3(QuatTransform.GetTranslationValue(IntFrame + 0));
                                    Vector3 R = GLConverter.ToVector3(QuatTransform.GetTranslationValue(IntFrame + 1));

                                    B.Translation = Vector3.Lerp(L, R, Weight);
                                }
                                break;

                            default: throw new NotImplementedException();
                        }
                    }
                }

                FrameSkeleton[Index++] = B;
            }

            for (Index = 0; Index < Skeleton.Count; Index++)
            {
                Bone B = FrameSkeleton[Index];

                Output[Index] = Matrix4.CreateScale(B.Scale);

                while (true)
                {
                    if (B.IsQuatRotation)
                        Output[Index] *= Matrix4.CreateFromQuaternion(B.QuatRotation);
                    else
                        Output[Index] *= Utils.EulerRotate(B.Rotation);

                    Vector3 Translation = B.Translation;

                    if (B.ParentIndex != -1) Translation *= FrameSkeleton[B.ParentIndex].Scale;

                    Output[Index] *= Matrix4.CreateTranslation(Translation);

                    if (B.ParentIndex == -1) break;

                    B = FrameSkeleton[B.ParentIndex];
                }
            }

            AdvanceFrame();

            return Output;
        }
    }
}
