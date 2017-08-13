using OpenTK;

using SPICA.Formats.CtrH3D.Camera;
using SPICA.Rendering.Animation;
using SPICA.Rendering.SPICA_GL;

using System;

namespace SPICA.Rendering
{
    public class Camera
    {
        public Vector3 Scale;
        public Vector3 Rotation;
        public Vector3 Translation;

        public Vector3 UpVector;
        public Vector3 Target;

        public Matrix4 ProjectionMatrix;
        public Matrix4 ViewMatrix;

        public CameraAnimation Animation;

        private Renderer Renderer;

        private H3DCamera BaseCamera;

        public Camera(Renderer Renderer)
        {
            this.Renderer = Renderer;

            Animation = new CameraAnimation();

            RecalculateMatrices();
        }

        public void Set(H3DCamera Camera)
        {
            BaseCamera = Camera;

            Animation.SetCamera(Camera);

            RecalculateMatrices();
        }

        public void RecalculateMatrices()
        {
            CameraState CamState = Animation.GetCameraState();

            float AspectRatio = (float)Renderer.Width / Renderer.Height;

            if (BaseCamera != null)
            {
                Matrix4 Transform =
                    Matrix4.CreateScale(CamState.Scale) *
                    Matrix4.CreateRotationX(CamState.Rotation.X) *
                    Matrix4.CreateRotationY(CamState.Rotation.Y) *
                    Matrix4.CreateRotationZ(CamState.Rotation.Z) *
                    Matrix4.CreateTranslation(CamState.Translation);

                if (BaseCamera.View is H3DCameraViewLookAt LookAtView)
                {
                    Vector3 Eye    = Transform.Row3.Xyz;
                    Vector3 Up     = CamState.UpVector;
                    Vector3 Target = CamState.Target;

                    if ((BaseCamera.Flags & H3DCameraFlags.IsInheritingUpRotation) != 0)
                    {
                        Up = Vector3.Transform(new Matrix3(Transform), Up);
                    }

                    if ((BaseCamera.Flags & H3DCameraFlags.IsInheritingTargetRotation) != 0)
                    {
                        Target = Vector3.Transform(new Matrix3(Transform), Target);
                    }

                    if ((BaseCamera.Flags & H3DCameraFlags.IsInheritingTargetTranslation) != 0)
                    {
                        Target += Eye;
                    }

                    ViewMatrix = Matrix4.LookAt(Eye, Target, Up);
                }

                if (BaseCamera.Projection is H3DCameraProjectionPerspective PerspProj)
                {
                    ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
                        PerspProj.FOVY,
                        AspectRatio,
                        PerspProj.ZNear,
                        PerspProj.ZFar);
                }
                else if (BaseCamera.Projection is H3DCameraProjectionOrthogonal OrthoProj)
                {
                    ProjectionMatrix = Matrix4.CreateOrthographic(
                        Renderer.Width,
                        Renderer.Height,
                        OrthoProj.ZNear,
                        OrthoProj.ZFar);
                }
            }
            else
            {
                ViewMatrix = Matrix4.Identity;

                ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
                    (float)Math.PI * 0.25f,
                    AspectRatio,
                    0.25f,
                    100000f);
            }
        }
    }
}
