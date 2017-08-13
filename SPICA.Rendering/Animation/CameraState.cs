using OpenTK;

namespace SPICA.Rendering.Animation
{
    public class CameraState
    {
        public Vector3 Scale;
        public Vector3 Rotation;
        public Vector3 Translation;
        public Vector3 UpVector;
        public Vector3 Target;
        public Vector3 ViewRotation;

        public float ZNear;
        public float ZFar;
        public float Twist;
    }
}
