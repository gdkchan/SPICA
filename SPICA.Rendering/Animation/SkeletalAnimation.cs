using OpenTK;

using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Animation;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Rendering.SPICA_GL;

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SPICA.Rendering.Animation
{
    public class SkeletalAnimation : AnimationControl
    {
        private class Bone
        {
            public Vector3    Scale;
            public Vector3    EulerRotation;
            public Vector3    Translation;
            public Quaternion Rotation;
            public Bone       Parent;
            public int        ParentIndex;

            public void CalculateQuaternion()
            {
                double SX = Math.Sin(EulerRotation.X * 0.5f);
                double SY = Math.Sin(EulerRotation.Y * 0.5f);
                double SZ = Math.Sin(EulerRotation.Z * 0.5f);
                double CX = Math.Cos(EulerRotation.X * 0.5f);
                double CY = Math.Cos(EulerRotation.Y * 0.5f);
                double CZ = Math.Cos(EulerRotation.Z * 0.5f);

                double X = CZ * SX * CY - SZ * CX * SY;
                double Y = CZ * CX * SY + SZ * SX * CY;
                double Z = SZ * CX * CY - CZ * SX * SY;
                double W = CZ * CX * CY + SZ * SX * SY;

                Rotation = new Quaternion((float)X, (float)Y, (float)Z, (float)W);
            }
        }

        private H3DDict<H3DBone> Skeleton;

        private Bone[] FrameSkeleton;

        private Matrix4[] Transforms;

        private List<int> Indices;

        private List<H3DAnimationElement> Elements;

        private const string InvalidPrimitiveTypeEx = "Invalid Primitive type used on Skeleton Bone {0}!";

        public SkeletalAnimation(H3DDict<H3DBone> Skeleton)
        {
            this.Skeleton = Skeleton;

            FrameSkeleton = new Bone[Skeleton.Count];

            Transforms = new Matrix4[Skeleton.Count];

            for (int i = 0; i < Skeleton.Count; i++)
            {
                FrameSkeleton[i] = new Bone();

                FrameSkeleton[i].ParentIndex = Skeleton[i].ParentIndex;

                if (Skeleton[i].ParentIndex != -1)
                {
                    if (Skeleton[i].ParentIndex >= i)
                    {
                        Debug.WriteLine("[SPICA.Renderer|SkeletalAnimation] Skeleton is not properly sorted!");
                    }

                    FrameSkeleton[i].Parent = FrameSkeleton[Skeleton[i].ParentIndex];
                }
            }

            Indices = new List<int>();

            Elements = new List<H3DAnimationElement>();
        }

        public override void SetAnimations(IEnumerable<H3DAnimation> Animations)
        {
            for (int i = 0; i < Skeleton.Count; i++)
            {
                FrameSkeleton[i].Scale         = Skeleton[i].Scale      .ToVector3();
                FrameSkeleton[i].EulerRotation = Skeleton[i].Rotation   .ToVector3();
                FrameSkeleton[i].Translation   = Skeleton[i].Translation.ToVector3();

                FrameSkeleton[i].CalculateQuaternion();
            }

            ResetTransforms();

            Indices.Clear();
            Elements.Clear();

            float FC = 0;

            HashSet<string> UsedNames = new HashSet<string>();

            foreach (H3DAnimation Anim in Animations)
            {
                if (FC < Anim.FramesCount)
                    FC = Anim.FramesCount;

                foreach (H3DAnimationElement Elem in Anim.Elements)
                {
                    if (UsedNames.Contains(Elem.Name)) continue;

                    UsedNames.Add(Elem.Name);

                    int Index = Skeleton.FindIndex(Elem.Name);

                    if (Index != -1)
                    {
                        Indices.Add(Index);
                        Elements.Add(Elem);
                    }
                }
            }

            FramesCount = FC;
        }

        private void ResetTransforms()
        {
            for (int i = 0; i < Skeleton.Count; i++)
            {
                Transforms[i] = Skeleton[i].InverseTransform.ToMatrix4().Inverted();
            }
        }

        public Matrix4[] GetSkeletonTransforms()
        {
            if (State == AnimationState.Stopped)
            {
                ResetTransforms();
            }

            if (State != AnimationState.Playing || Elements.Count == 0)
            {
                return Transforms;
            }

            bool[] Skip = new bool[Transforms.Length];

            for (int i = 0; i < Elements.Count; i++)
            {
                int Index = Indices[i];

                Bone Bone = FrameSkeleton[Index];

                H3DAnimationElement Elem = Elements[i];

                switch (Elem.PrimitiveType)
                {
                    case H3DPrimitiveType.Transform:
                        SetBone((H3DAnimTransform)Elem.Content, Bone);

                        break;

                    case H3DPrimitiveType.QuatTransform:
                        SetBone((H3DAnimQuatTransform)Elem.Content, Bone);

                        break;

                    case H3DPrimitiveType.MtxTransform:
                        H3DAnimMtxTransform MtxTransform = (H3DAnimMtxTransform)Elem.Content;

                        Transforms[Index] = MtxTransform.GetTransform((int)Frame).ToMatrix4();

                        Skip[Index] = true;

                        break;

                    default: throw new InvalidOperationException(string.Format(InvalidPrimitiveTypeEx, Elem.Name));
                }
            }

            for (int Index = 0; Index < Transforms.Length; Index++)
            {
                if (Skip[Index]) continue;

                Bone Bone = FrameSkeleton[Index];

                bool ScaleCompensate = (Skeleton[Index].Flags & H3DBoneFlags.IsSegmentScaleCompensate) != 0;

                ScaleCompensate &= Bone.Parent != null;

                Transforms[Index]  = Matrix4.CreateScale(Bone.Scale);
                Transforms[Index] *= Matrix4.CreateFromQuaternion(Bone.Rotation);
                Transforms[Index] *= ScaleCompensate
                    ? Matrix4.CreateTranslation(Bone.Translation * Bone.Parent.Scale)
                    : Matrix4.CreateTranslation(Bone.Translation);

                if (ScaleCompensate)
                {
                    Transforms[Index] *= Matrix4.CreateScale(
                        1f / Bone.Parent.Scale.X,
                        1f / Bone.Parent.Scale.Y,
                        1f / Bone.Parent.Scale.Z);

                    Transforms[Index] *= Transforms[Bone.ParentIndex];
                }
                else if (Bone.Parent != null)
                {
                    Transforms[Index] *= Transforms[Bone.ParentIndex];
                }
            }

            return Transforms;
        }

        private void SetBone(H3DAnimTransform Transform, Bone Bone)
        {
            TrySetFrameValue(Transform.ScaleX,       ref Bone.Scale.X);
            TrySetFrameValue(Transform.ScaleY,       ref Bone.Scale.Y);
            TrySetFrameValue(Transform.ScaleZ,       ref Bone.Scale.Z);

            TrySetFrameValue(Transform.RotationX,    ref Bone.EulerRotation.X);
            TrySetFrameValue(Transform.RotationY,    ref Bone.EulerRotation.Y);
            TrySetFrameValue(Transform.RotationZ,    ref Bone.EulerRotation.Z);

            TrySetFrameValue(Transform.TranslationX, ref Bone.Translation.X);
            TrySetFrameValue(Transform.TranslationY, ref Bone.Translation.Y);
            TrySetFrameValue(Transform.TranslationZ, ref Bone.Translation.Z);

            if (Transform.RotationExists)
            {
                Bone.CalculateQuaternion();
            }
        }

        private void TrySetFrameValue(H3DFloatKeyFrameGroup Group, ref float Value)
        {
            if (Group.Exists)
            {
                Value = Group.GetFrameValue(Frame);
            }
        }

        private void SetBone(H3DAnimQuatTransform Transform, Bone Bone)
        {
            int Frame = (int)this.Frame;

            if (Frame != this.Frame)
            {
                SetBone(Transform, Bone, Frame, this.Frame - Frame);
            }
            else
            {
                SetBone(Transform, Bone, Frame);
            }
        }

        private void SetBone(H3DAnimQuatTransform Transform, Bone Bone, int Frame, float Weight)
        {
            if (Transform.HasScale)
            {
                Bone.Scale = Vector3.Lerp(
                    Transform.GetScaleValue(Frame + 0).ToVector3(),
                    Transform.GetScaleValue(Frame + 1).ToVector3(), Weight);
            }

            if (Transform.HasRotation)
            {
                Bone.Rotation = Quaternion.Slerp(
                    Transform.GetRotationValue(Frame + 0).ToQuaternion(),
                    Transform.GetRotationValue(Frame + 1).ToQuaternion(), Weight);
            }

            if (Transform.HasTranslation)
            {
                Bone.Translation = Vector3.Lerp(
                    Transform.GetTranslationValue(Frame + 0).ToVector3(),
                    Transform.GetTranslationValue(Frame + 1).ToVector3(), Weight);
            }
        }

        private void SetBone(H3DAnimQuatTransform Transform, Bone Bone, int Frame)
        {
            if (Transform.HasScale)
            {
                Bone.Scale = Transform.GetScaleValue(Frame).ToVector3();
            }

            if (Transform.HasRotation)
            {
                Bone.Rotation = Transform.GetRotationValue(Frame).ToQuaternion();
            }

            if (Transform.HasTranslation)
            {
                Bone.Translation = Transform.GetTranslationValue(Frame).ToVector3();
            }
        }
    }
}
