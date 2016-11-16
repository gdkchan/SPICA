using OpenTK;
using OpenTK.Graphics;

namespace SPICA.Renderer
{
    public struct Light
    {
        public Vector3 Position;
        public Color4 Ambient;
        public Color4 Diffuse;
        public Color4 Specular;
    }
}
