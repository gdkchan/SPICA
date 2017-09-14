using SPICA.Formats.CtrH3D.Light;
using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrGfx.Light
{
    [TypeChoice(0x400000a2, typeof(GfxFragmentLight))]
    [TypeChoice(0x40000122, typeof(GfxHemisphereLight))]
    [TypeChoice(0x40000222, typeof(GfxVertexLight))]
    [TypeChoice(0x40000422, typeof(GfxAmbientLight))]
    public class GfxLight : GfxNodeTransform
    {
        public bool IsEnabled;

        public H3DLight ToH3DLight()
        {
            H3DLight Output = new H3DLight() { Name = Name };

            Output.IsEnabled = IsEnabled;

            Output.TransformScale       = TransformScale;
            Output.TransformRotation    = TransformRotation;
            Output.TransformTranslation = TransformTranslation;

            if (this is GfxHemisphereLight HemisphereLight)
            {
                Output.Type = H3DLightType.Hemisphere;

                Output.Content = new H3DHemisphereLight()
                {
                    SkyColor    = HemisphereLight.SkyColor,
                    GroundColor = HemisphereLight.GroundColor,
                    Direction   = HemisphereLight.Direction,
                    LerpFactor  = HemisphereLight.LerpFactor
                };
            }
            else if (this is GfxAmbientLight AmbientLight)
            {
                Output.Type = H3DLightType.Ambient;

                Output.Content = new H3DAmbientLight()
                {
                    Color = AmbientLight.Color
                };
            }
            else if (this is GfxVertexLight VertexLight)
            {
                Output.Type = H3DLightType.Vertex | (H3DLightType)VertexLight.Type;

                Output.Content = new H3DVertexLight()
                {
                    AmbientColor         = VertexLight.AmbientColor,
                    DiffuseColor         = VertexLight.DiffuseColor,
                    Direction            = VertexLight.Direction,
                    AttenuationConstant  = VertexLight.AttenuationConstant,
                    AttenuationLinear    = VertexLight.AttenuationLinear,
                    AttenuationQuadratic = VertexLight.AttenuationQuadratic,
                    SpotExponent         = VertexLight.SpotExponent,
                    SpotCutOffAngle      = VertexLight.SpotCutOffAngle
                };
            }
            else if (this is GfxFragmentLight FragmentLight)
            {
                Output.Type = H3DLightType.Fragment | (H3DLightType)FragmentLight.Type;

                Output.LUTInput = FragmentLight.AngleSampler?.Input ?? 0;
                Output.LUTScale = FragmentLight.AngleSampler?.Scale ?? 0;

                Output.Content = new H3DFragmentLight()
                {
                    AmbientColor           = FragmentLight.AmbientColor,
                    DiffuseColor           = FragmentLight.DiffuseColor,
                    Specular0Color         = FragmentLight.Specular0Color,
                    Specular1Color         = FragmentLight.Specular1Color,
                    Direction              = FragmentLight.Direction,
                    AttenuationStart       = FragmentLight.AttenuationStart,
                    AttenuationEnd         = FragmentLight.AttenuationEnd,
                    DistanceLUTTableName   = FragmentLight.DistanceSampler?.TableName,
                    DistanceLUTSamplerName = FragmentLight.DistanceSampler?.SamplerName,
                    AngleLUTTableName      = FragmentLight.AngleSampler?.Sampler.TableName,
                    AngleLUTSamplerName    = FragmentLight.AngleSampler?.Sampler.SamplerName
                };
            }

            return Output;
        }
    }
}
