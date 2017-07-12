using OpenTK;
using OpenTK.Graphics;

using SPICA.Formats.CtrH3D.Light;

namespace SPICA.Rendering
{
    public class Light
    {
        public Vector3 Position;
        public Color4  Ambient;
        public Color4  Diffuse;
        public Color4  Specular;

        public bool Enabled;

        public Light() { }

        public Light(H3DLight Light)
        {
            //TODO
        }
    }
}
