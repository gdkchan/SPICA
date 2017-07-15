using OpenTK;

namespace SPICA.Rendering
{
    public struct BoundingBox
    {
        public Vector3 Center;
        public Vector3 Size;

        public BoundingBox(Vector3 Center, Vector3 Size)
        {
            this.Center = Center;
            this.Size   = Size;
        }
    }
}
