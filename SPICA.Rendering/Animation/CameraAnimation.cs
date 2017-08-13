using OpenTK;

using SPICA.Formats.CtrH3D.Animation;
using SPICA.Formats.CtrH3D.Camera;
using SPICA.Rendering.SPICA_GL;

using System.Collections.Generic;

namespace SPICA.Rendering.Animation
{
    public class CameraAnimation : AnimationControl
    {
        private CameraState CamState;

        private H3DCamera BaseCamera;

        private List<H3DAnimationElement> Elements;

        public CameraAnimation()
        {
            CamState = new CameraState();

            Elements = new List<H3DAnimationElement>();
        }

        public void SetCamera(H3DCamera Camera)
        {
            BaseCamera = Camera;

            ResetState();
        }

        private void ResetState()
        {
            if (BaseCamera != null)
            {
                CamState.Scale       = BaseCamera.TransformScale      .ToVector3();
                CamState.Rotation    = BaseCamera.TransformRotation   .ToVector3();
                CamState.Translation = BaseCamera.TransformTranslation.ToVector3();

                if (BaseCamera.View is H3DCameraViewLookAt LookAtView)
                {
                    CamState.UpVector = LookAtView.UpVector      .ToVector3();
                    CamState.Target   = LookAtView.TargetPosition.ToVector3();
                }
            }
        }

        public override void SetAnimations(IEnumerable<H3DAnimation> Animations)
        {
            Elements.Clear();

            float FC = 0;

            HashSet<string> UsedNames = new HashSet<string>();

            foreach (H3DAnimation Anim in Animations)
            {
                if (FC < Anim.FramesCount)
                    FC = Anim.FramesCount;

                foreach (H3DAnimationElement Elem in Anim.Elements)
                {
                    Elements.Add(Elem);
                }
            }

            FramesCount = FC;
        }

        public CameraState GetCameraState()
        {
            if (State == AnimationState.Stopped)
            {
                ResetState();
            }

            if (State != AnimationState.Playing || Elements.Count == 0)
            {
                return CamState;
            }

            for (int i = 0; i < Elements.Count; i++)
            {
                H3DAnimationElement Elem = Elements[i];

                if (Elem.PrimitiveType == H3DPrimitiveType.Transform)
                {
                    SetStateTransform((H3DAnimTransform)Elem.Content);
                }
                else if (Elem.PrimitiveType == H3DPrimitiveType.Vector3D)
                {
                    H3DAnimVector3D Vector = (H3DAnimVector3D)Elem.Content;

                    switch (Elem.TargetType)
                    {
                        case H3DTargetType.CameraUpVector:  SetVector3(Vector, ref CamState.UpVector); break;
                        case H3DTargetType.CameraTargetPos: SetVector3(Vector, ref CamState.Target);   break;
                    }
                }
            }

            return CamState;
        }

        private void SetStateTransform(H3DAnimTransform Transform)
        {
            TrySetFrameValue(Transform.ScaleX,       ref CamState.Scale.X);
            TrySetFrameValue(Transform.ScaleY,       ref CamState.Scale.Y);
            TrySetFrameValue(Transform.ScaleZ,       ref CamState.Scale.Z);

            TrySetFrameValue(Transform.RotationX,    ref CamState.Rotation.X);
            TrySetFrameValue(Transform.RotationY,    ref CamState.Rotation.Y);
            TrySetFrameValue(Transform.RotationZ,    ref CamState.Rotation.Z);

            TrySetFrameValue(Transform.TranslationX, ref CamState.Translation.X);
            TrySetFrameValue(Transform.TranslationY, ref CamState.Translation.Y);
            TrySetFrameValue(Transform.TranslationZ, ref CamState.Translation.Z);
        }

        private void TrySetFrameValue(H3DFloatKeyFrameGroup Group, ref float Value)
        {
            if (Group.Exists)
            {
                Value = Group.GetFrameValue(Frame);
            }
        }

        private void SetVector3(H3DAnimVector3D Vector, ref Vector3 Target)
        {
            if (Vector.X.Exists) Target.X = Vector.X.GetFrameValue(Frame);
            if (Vector.Y.Exists) Target.Y = Vector.Y.GetFrameValue(Frame);
            if (Vector.Z.Exists) Target.Z = Vector.Z.GetFrameValue(Frame);
        }
    }
}
