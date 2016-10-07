using SPICA.Math3D;
using SPICA.Serialization.Attributes;

namespace SPICA.Formats.H3D.Model.Material
{
    class H3DMaterialParams
    {
        public uint UId;
        public H3DMaterialFlags Flags;
        public H3DFragmentLightingFlags FragmentLightingFlags;
        public byte FrameAccessOffset;

        public H3DMaterialShader MaterialShader;

        [Inline, FixedLength(3)]
        public H3DTextureCoord[] TextureCoords;

        public ushort LightSetIndex;
        public ushort FogIndex;

        public RGBA EmissionColor;
        public RGBA AmbientColor;
        public RGBA DiffuseColor;
        public RGBA Specular0;
        public RGBA Specular1;
        public RGBA Constant0;
        public RGBA Constant1;
        public RGBA Constant2;
        public RGBA Constant3;
        public RGBA Constant4;
        public RGBA Constant5;
        public RGBA BlendColor;
        public float ColorScale;

        public H3DMaterialLUT Dist0LUT;
        public H3DMaterialLUT Dist1LUT;
        public H3DMaterialLUT FresnelLUT;
        public H3DMaterialLUT ReflecRLUT;
        public H3DMaterialLUT ReflecGLUT;
        public H3DMaterialLUT ReflecBLUT;

        public byte LayerConfig;
        public byte FresnelConfig;
        public byte BumpMode;
        public byte BumpTexture;

        [Inline, FixedLength(6)]
        public uint[] LUTConfigCommands;

        public uint ConstantColors;

        public float PolygonOffsetUnit;

        public uint[] FragmentShaderCommands;
        public uint FragmentShaderCommandsSource;

        public string LUTDist0TableName;
        public string LUTDist1TableName;
        public string LUTFresnelTableName;
        public string LUTReflecRTableName;
        public string LUTReflecGTableName;
        public string LUTReflecBTableName;

        public string LUTDist0SamplerName;
        public string LUTDist1SamplerName;
        public string LUTFresnelSamplerName;
        public string LUTReflecRSamplerName;
        public string LUTReflecGSamplerName;
        public string LUTReflecBSamplerName;

        public string ShaderReference;
        public string ModelReference;

        public H3DMetaData MetaData;
    }
}
