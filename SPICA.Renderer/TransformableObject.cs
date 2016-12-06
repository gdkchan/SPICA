using OpenTK;

namespace SPICA.Renderer
{
    public class TransformableObject
    {
        public Matrix4 Transform;

        public TransformableObject()
        {
            ResetTransform();
        }

        public void ResetTransform()
        {
            Transform = Matrix4.Identity;
        }

        public void Scale(Vector3 Scale)
        {
            Transform *= Matrix4.CreateScale(Scale);
        }

        public void Rotate(Vector3 Rotation)
        {
            Transform *= RenderUtils.EulerRotate(Rotation);
        }

        public void Translate(Vector3 Translation)
        {
            Transform *= Matrix4.CreateTranslation(Translation);
        }

        public void ScaleAbs(Vector3 Scale)
        {
            Transform = Transform.ClearScale() * Matrix4.CreateScale(Scale);
        }

        public void RotateAbs(Vector3 Rotation)
        {
            Transform = Transform.ClearRotation() * RenderUtils.EulerRotate(Rotation);
        }

        public void TranslateAbs(Vector3 Translation)
        {
            Transform = Transform.ClearTranslation() * Matrix4.CreateTranslation(Translation);
        }
    }
}
