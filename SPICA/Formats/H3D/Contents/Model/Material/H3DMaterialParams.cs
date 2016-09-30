using SPICA.Formats.H3D.Contents.Model.Texture;

using SPICA.Math;
using SPICA.Serialization.BinaryAttributes;

namespace SPICA.Formats.H3D.Contents.Model.Material
{
    class H3DMaterialParams
    {
        public uint UId;
        public ushort Flags;
        public byte FragmentLightingFlags;
        public byte FrameBufferAddress;

        [PointerOf("MaterialShader")]
        private uint MaterialShaderAddress;

        [FixedCount(3)]
        public H3DTextureCoord[] TextureCoords;

        public ushort LightSetId;
        public ushort FogId;

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

        [PointerOf("MaterialLUTs")]
        private uint[] MaterialLUTAddress;

        public byte LayerConfig;
        public byte FresnelConfig;
        public byte BumpMode;
        public byte BumpTexture;

        [FixedCount(6)]
        public uint[] LUTConfigCommands;

        public uint ConstantColors;
        public float PolygonOffsetUnit;

        [PointerOf("FragmentShaderCommands")]
        private uint FragmentShaderCommandsAddress;

        [CountOf("FragmentShaderCommands")]
        private uint FragmentShaderCommandsCount;

        //???
        public uint FragmentShaderCommandsSource;

        [PointerOf("LUTTableNames")]
        private uint[] LUTTableNamesAddress;

        [PointerOf("LUTSamplerNames")]
        private uint[] LUTSamplerNamesAddress;

        [PointerOf("ShaderReference")]
        private uint ShaderReferenceAddress;

        [PointerOf("ModelReference")]
        private uint ModelReferenceAddress;

        [PointerOf("MetaData")]
        private uint MetaDataAddress;

        public H3DMaterialShader MaterialShader;

        [FixedCount(6)]
        public H3DMaterialLUT[] MaterialLUTs;

        [TargetSection("CommandsSection")]
        public uint[] FragmentShaderCommands;

        [TargetSection("StringsSection"), FixedCount(6)]
        public string[] LUTTableNames;

        [TargetSection("StringsSection"), FixedCount(6)]
        public string[] LUTSamplerNames;

        [TargetSection("StringsSection")] 
        public string ShaderReference;

        [TargetSection("StringsSection")]
        public string ModelReference;

        public H3DMetaData MetaData;
    }
}
