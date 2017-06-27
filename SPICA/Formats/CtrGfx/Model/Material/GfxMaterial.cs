using SPICA.Formats.Common;
using SPICA.Math3D;
using SPICA.PICA.Commands;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace SPICA.Formats.CtrGfx.Model.Material
{
    [TypeChoice(0x08000000u, typeof(GfxMaterial))]
    public class GfxMaterial : INamed, ICustomSerialization
    {
        private GfxRevHeader Header;

        private string _Name;

        public string Name
        {
            get => _Name;
            set => _Name = value ?? throw Exceptions.GetNullException("Name");
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
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer) { }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            for (int i = 0; i < 3 && TextureMappers[i] != null; i++)
            {
                TextureMappers[i].MapperIndex = i;
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
            MaterialFlagsHash = HashBuffer(UInt32ToBuffer((uint)(Flags | GfxMaterialFlags.IsPolygonOffsetEnabled)));
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

                Writer.Write(FragmentShader.Lighting.GetBytes());
                Writer.Write((byte)((Flags & GfxMaterialFlags.IsFragmentLightingEnabled) != 0 ? 1 : 0));

                FragLightHash = HashBuffer(MS.ToArray());
            }   
        }

        private void CalcFragLightLUTSampHash()
        {
            FragLightLUTSampHash = HashBuffer(FragmentShader.LUTs.GetBytes());
        }

        private void CalcTextureEnvironmentHash()
        {
            //FIXME: This hash is different from the original. Not that it makes any difference anyway.
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
            FragOpHash = HashBuffer(FragmentShader.AlphaTest.GetBytes());
        }

        private byte[] UInt32ToBuffer(uint Value)
        {
            byte[] Buffer = BitConverter.GetBytes(Value);

            if (!BitConverter.IsLittleEndian) Array.Reverse(Buffer);

            return Buffer;
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

        internal static uint GetTestFunc(PICATestFunc TestFunc)
        {
            //Too much to ask to use the same values the GPU use?
            //Really hate to convert between those stupid inconsistent enumerations.
            //TODO: Axe this since the hash value doesn't matter, the game isn't going to verify this.
            //It's only necessary to be unique per material setting.
            switch (TestFunc)
            {
                case PICATestFunc.Never:    return 0;
                case PICATestFunc.Always:   return 1;
                case PICATestFunc.Less:     return 2;
                case PICATestFunc.Lequal:   return 3;
                case PICATestFunc.Equal:    return 4;
                case PICATestFunc.Gequal:   return 5;
                case PICATestFunc.Greater:  return 6;
                case PICATestFunc.Notequal: return 7;
            }

            return 0;
        }
    }
}
