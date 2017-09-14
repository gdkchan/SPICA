using SPICA.Math3D;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace SPICA.Formats.CtrGfx.Model.Material
{
    [TypeChoice(0x08000000u, typeof(GfxMaterial))]
    public class GfxMaterial : GfxObject, ICustomSerialization
    {
        public GfxMaterialFlags Flags;

        public GfxTexCoordConfig   TexCoordConfig;
        public GfxTranslucencyKind TranslucencyKind;

        public GfxMaterialColor Colors;

        public GfxRasterization Rasterization;

        public GfxFragOp FragmentOperation;

        public int UsedTextureCoordsCount;

        [Inline, FixedLength(3)] public readonly GfxTextureCoord[]  TextureCoords;
        [Inline, FixedLength(3)] public readonly GfxTextureMapper[] TextureMappers;

        public GfxProcTextureMapper ProceduralTextureMapper;

        public readonly GfxShaderReference Shader;
        public readonly GfxFragShader      FragmentShader;

        public int ShaderProgramDescIndex;

        public readonly List<GfxShaderParam> ShaderParameters;

        public int LightSetIndex;
        public int FogIndex;

        private uint MaterialFlagsHash;
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

        public GfxMaterial()
        {
            TextureCoords  = new GfxTextureCoord[3];
            TextureMappers = new GfxTextureMapper[3];

            Shader         = new GfxShaderReference();
            FragmentShader = new GfxFragShader();

            ShaderParameters = new List<GfxShaderParam>();
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer) { }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            for (int i = 0; i < 3 && TextureMappers[i] != null; i++)
            {
                TextureMappers[i].MapperIndex = i;
            }

            for (int i = 0; i < 6 && FragmentShader.TextureEnvironments[i] != null; i++)
            {
                FragmentShader.TextureEnvironments[i].StageIndex = i;
            }

            CalcMaterialFlagsHash();
            CalcShaderParamsHash();
            CalcTextureCoordsHash();
            CalcTextureSamplersHash();
            CalcMaterialColorsHash();
            CalcRasterizationsHash();
            CalcFragLightHash();
            CalcFragLightLUTSampHash();
            CalcTextureEnvironmentHash();
            CalcAlphaTestHash();
            CalcFragOpHash();

            return false;
        }

        private void CalcMaterialFlagsHash()
        {
            using (MemoryStream MS = new MemoryStream())
            {
                BinaryWriter Writer = new BinaryWriter(MS);

                Writer.Write((uint)(Flags | GfxMaterialFlags.IsPolygonOffsetEnabled));

                MaterialFlagsHash = HashBuffer(MS.ToArray());
            }
        }

        private void CalcShaderParamsHash()
        {
            ShaderParamsHash = HashBuffer(new byte[0]); //TODO
        }

        private void CalcTextureCoordsHash()
        {
            using (MemoryStream MS = new MemoryStream())
            {
                BinaryWriter Writer = new BinaryWriter(MS);

                Writer.Write((uint)TexCoordConfig);

                for (int i = 0; i < 3; i++)
                {
                    Writer.Write(TextureCoords[i].GetBytes(i >= UsedTextureCoordsCount));
                    Writer.Write((uint)TexCoordConfig);
                }

                TextureCoordsHash = HashBuffer(MS.ToArray());
            }
        }

        private void CalcTextureSamplersHash()
        {
            uint[] Wraps = new uint[] { 2, 3, 0, 1 };

            using (MemoryStream MS = new MemoryStream())
            {
                BinaryWriter Writer = new BinaryWriter(MS);

                Writer.Write((uint)TexCoordConfig);

                foreach (GfxTextureMapper TexMapper in TextureMappers)
                {
                    if (TexMapper != null && TexMapper.Sampler is GfxTextureSamplerStd)
                    {
                        Writer.Write(TexMapper.BorderColor.ToVector4());
                        Writer.Write(Wraps[(uint)TexMapper.WrapU]);
                        Writer.Write(Wraps[(uint)TexMapper.WrapV]);
                        Writer.Write((float)TexMapper.MinLOD);
                        Writer.Write(TexMapper.LODBias);
                        Writer.Write((uint)TexMapper.GetMinFilter());
                        Writer.Write((uint)TexMapper.MagFilter);
                    }
                }

                TextureSamplersHash = HashBuffer(MS.ToArray());
            }
        }

        private void CalcMaterialColorsHash()
        {
            MaterialColorsHash = HashBuffer(Colors.GetBytes());
        }

        private void CalcRasterizationsHash()
        {
            RasterizationHash = HashBuffer(Rasterization.GetBytes());
        }

        private void CalcFragLightHash()
        {
            using (MemoryStream MS = new MemoryStream())
            {
                BinaryWriter Writer = new BinaryWriter(MS);

                byte FragLightEnb = (Flags & GfxMaterialFlags.IsFragmentLightingEnabled) != 0
                    ? (byte)1
                    : (byte)0;

                Writer.Write(FragmentShader.Lighting.GetBytes());
                Writer.Write(FragLightEnb);

                FragLightHash = HashBuffer(MS.ToArray());
            }   
        }

        private void CalcFragLightLUTSampHash()
        {
            FragLightLUTSampHash = HashBuffer(FragmentShader.LUTs.GetBytes());
        }

        private void CalcTextureEnvironmentHash()
        {
            using (MemoryStream MS = new MemoryStream())
            {
                BinaryWriter Writer = new BinaryWriter(MS);

                Writer.Write(FragmentShader.TexEnvBufferColor.ToVector4());

                foreach (GfxTexEnv TexEnv in FragmentShader.TextureEnvironments)
                {
                    Writer.Write(TexEnv.GetBytes());
                }

                TextureEnvironmentHash = HashBuffer(MS.ToArray());
            }
        }

        private void CalcAlphaTestHash()
        {
            AlphaTestHash = HashBuffer(FragmentShader.AlphaTest.GetBytes());
        }

        private void CalcFragOpHash()
        {
            FragOpHash = HashBuffer(FragmentOperation.GetBytes());
        }

        private uint HashBuffer(byte[] Input)
        {
            using (MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider())
            {
                byte[] Buffer = MD5.ComputeHash(Input);

                uint Hash = 0;

                for (int i = 0; i < Buffer.Length; i++)
                {
                    Hash ^= (uint)Buffer[i] << (i & 3) * 8;
                }

                return Hash != 0 ? Hash : 1;
            }
        }
    }
}
