using OpenTK;
using OpenTK.Graphics;

namespace SPICA.Rendering.Animation
{
    public class MaterialState
    {
        public Matrix4[] Transforms;

        public Color4 Emission;
        public Color4 Ambient;
        public Color4 Diffuse;
        public Color4 Specular0;
        public Color4 Specular1;
        public Color4 Constant0;
        public Color4 Constant1;
        public Color4 Constant2;
        public Color4 Constant3;
        public Color4 Constant4;
        public Color4 Constant5;

        public string Texture0Name;
        public string Texture1Name;
        public string Texture2Name;

        public MaterialState()
        {
            Transforms = new Matrix4[3];
        }
    }
}
