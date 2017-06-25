using SPICA.Formats.Common;
using SPICA.Serialization.Attributes;

using System.Collections.Generic;

namespace SPICA.Formats.CtrGfx.Model.Material
{
    [TypeChoice(0x08000000u, typeof(GfxMaterial))]
    public class GfxMaterial : INamed
    {
        private GfxRevHeader Header;

        private string _Name;

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value ?? throw Exceptions.GetNullException("Name");
            }
        }

        public readonly GfxDict<GfxMetaData> MetaData;

        public GfxMaterialFlags Flags;

        public GfxTexCoordConfig   TexCoordConfig;
        public GfxTranslucencyKind TranslucencyKind;

        public GfxMaterialColor Colors;

        public GfxRasterization Rasterization;

        public GfxFragOp FragmentOperation;

        public int UsedTextureCoordsCount;

        [Inline, FixedLength(3)] public readonly GfxTextureCoord[]  TextureCoords;
        [Inline, FixedLength(3)] public readonly GfxTextureMapper[] TextureMappers;

        public readonly GfxProcTextureMapper ProceduralTextureMapper;

        public readonly GfxShaderReference Shader;

        public readonly GfxFragShader FragmentShader;

        public int ShaderProgramDescIndex;

        public readonly List<GfxShaderParam> ShaderParameters;

        public int LightSetIndex;
        public int FogIndex;

        private uint ShadeParamsHash;
        private uint ShaderParamsHash;
        private uint TextureCoordsHash;
        private uint TextureSamplersHash;
        private uint TextureMappersHash;
        private uint MaterialColorsHash;
        private uint RasterizationHash;
        private uint FragLightHash;
        private uint FragLightLUTHash;
        private uint FragLightLUTSampHash;
        private uint TextureEnvironmentHash;
        private uint AlphaTestHash;
        private uint FragOpHash;
        private uint UniqueId;
    }
}
