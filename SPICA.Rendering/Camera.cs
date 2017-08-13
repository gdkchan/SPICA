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

                Vector3 Eye = Transform.Row3.Xyz;

                if (BaseCamera.View is H3DCameraViewLookAt LookAtView)
                {
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
                else if (BaseCamera.View is H3DCameraViewAim AimView)
                {
                    Vector3 Target = CamState.Target;
                    Vector3 EyeDir = Vector3.Normalize(Eye - Target);
                    Vector3 Twist  = Vector3.Normalize(new Vector3(EyeDir.Z, 0, -EyeDir.X));
                    Vector3 Cross  = Vector3.Cross(EyeDir, Twist);

                    float ST = (float)Math.Sin(CamState.Twist);
                    float CT = (float)Math.Cos(CamState.Twist);

                    Vector3 Up = new Vector3(
                        Cross.X * CT - Twist.X * ST,
                        Cross.Y * CT - Twist.Y * ST,
                        Cross.Z * CT - Twist.Z * ST);

                    if ((BaseCamera.Flags & H3DCameraFlags.IsInheritingTargetRotation) != 0)
                    {
                        Up     = Vector3.Transform(new Matrix3(Transform), Up);
                        Target = Vector3.Transform(new Matrix3(Transform), Target);
                    }

                    if ((BaseCamera.Flags & H3DCameraFlags.IsInheritingTargetTranslation) != 0)
                    {
                        Target += Eye;
                    }

                    ViewMatrix = Matrix4.LookAt(Eye, Target, Up);
                }
                else if (BaseCamera.View is H3DCameraViewRotation RotView)
                {
                    Matrix3 Rotation =
                        Matrix3.CreateRotationZ(CamState.ViewRotation.Z) *
                        Matrix3.CreateRotationX(CamState.ViewRotation.X) *
                        Matrix3.CreateRotationY(CamState.ViewRotation.Y);

                    Vector3 Up     = Vector3.UnitY;
                    Vector3 Target = Vector3.Transform(Rotation, new Vector3(0, 0, -1));

                    if ((BaseCamera.Flags & H3DCameraFlags.IsInheritingTargetRotation) != 0)
                    {
                        Up     = Vector3.Transform(new Matrix3(Transform), Up);
                        Target = Vector3.Transform(new Matrix3(Transform), Target);
                    }

                    Target += Eye;

                    ViewMatrix = Matrix4.LookAt(Eye, Target, Up);
                }

                if (BaseCamera.Projection is H3DCameraProjectionPerspective PerspProj)
                {
                    ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
                        PerspProj.FOVY,
                        AspectRatio,
                        CamState.ZNear,
                        CamState.ZFar);
                }
                else if (BaseCamera.Projection is H3DCameraProjectionOrthogonal OrthoProj)
                {
                    ProjectionMatrix = Matrix4.CreateOrthographic(
                        Renderer.Width,
                        Renderer.Height,
                        CamState.ZNear,
                        CamState.ZFar);
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
