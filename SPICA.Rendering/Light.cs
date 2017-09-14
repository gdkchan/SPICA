using OpenTK;
using OpenTK.Graphics;

using SPICA.Formats.CtrH3D.Light;
using SPICA.PICA.Commands;
using SPICA.Rendering.SPICA_GL;

using System;

namespace SPICA.Rendering
{
    public class Light
    {
        public Vector3 Position;
        public Vector3 Direction;
        public Color4  Ambient;
        public Color4  Diffuse;
        public Color4  Specular0;
        public Color4  Specular1;

        public LightType Type;

        public int   AngleLUTInput;
        public float AngleLUTScale;

        public float AttenuationScale;
        public float AttenuationBias;

        public string AngleLUTTableName;
        public string AngleLUTSamplerName;

        public string DistanceLUTTableName;
        public string DistanceLUTSamplerName;

        public bool Enabled;
        public bool DistAttEnabled;
        public bool TwoSidedDiffuse;
        public bool Directional;

        public Light() { }

        public Light(H3DLight Light)
        {
            Matrix4 Transform =
                Matrix4.CreateScale(Light.TransformScale.ToVector3()) *
                Matrix4.CreateRotationX(Light.TransformRotation.X) *
                Matrix4.CreateRotationY(Light.TransformRotation.Y) *
                Matrix4.CreateRotationZ(Light.TransformRotation.Z) *
                Matrix4.CreateTranslation(Light.TransformTranslation.ToVector3());

            Position = Vector4.Transform(Transform, Vector4.UnitW).Xyz;

            Enabled = Light.IsEnabled;

            DistAttEnabled  = (Light.Flags & H3DLightFlags.HasDistanceAttenuation) != 0;
            TwoSidedDiffuse = (Light.Flags & H3DLightFlags.IsTwoSidedDiffuse)      != 0;

            AngleLUTInput = (int)Light.LUTInput;
            AngleLUTScale =      Light.LUTScale.ToSingle();

            if (Light.Content is H3DFragmentLight FragmentLight)
            {
                Direction = FragmentLight.Direction.ToVector3();

                Ambient   = FragmentLight.AmbientColor.ToColor4();
                Diffuse   = FragmentLight.DiffuseColor.ToColor4();
                Specular0 = FragmentLight.Specular0Color.ToColor4();
                Specular1 = FragmentLight.Specular1Color.ToColor4();

                float AttDiff =
                    FragmentLight.AttenuationEnd -
                    FragmentLight.AttenuationStart;

                AttDiff = Math.Max(AttDiff, 0.01f);

                AttenuationScale = 1f / AttDiff;
                AttenuationBias = -FragmentLight.AttenuationStart / AttDiff;

                AngleLUTTableName   = FragmentLight.AngleLUTTableName;
                AngleLUTSamplerName = FragmentLight.AngleLUTSamplerName;

                DistanceLUTTableName   = FragmentLight.DistanceLUTTableName;
                DistanceLUTSamplerName = FragmentLight.DistanceLUTSamplerName;

                Directional = Light.Type == H3DLightType.FragmentDir;

                /*
                 * Note: Directional lights doesn't need a position, because they
                 * only specify the direction the light is pointing to.
                 * The Shader expects the direction to be on the Position vector
                 * on this case. The Direction vector is only used for the SpotLight
                 * direction on the Shader.
                 */
                if (Directional)
                {
                    Position = Direction;
                }
            }
        }
    }
}
