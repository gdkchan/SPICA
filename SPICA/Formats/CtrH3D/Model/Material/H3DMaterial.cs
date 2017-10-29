using SPICA.Formats.Common;
using SPICA.Formats.CtrH3D.Texture;
using SPICA.Math3D;
using SPICA.PICA;
using SPICA.PICA.Commands;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;
using SPICA.Serialization.Serializer;

using System;
using System.Numerics;

namespace SPICA.Formats.CtrH3D.Model.Material
{
    [Inline]
    public class H3DMaterial : ICustomSerialization, INamed
    {
        public readonly H3DMaterialParams MaterialParams;

        public H3DTexture Texture0;
        public H3DTexture Texture1;
        public H3DTexture Texture2;

        private uint[] TextureCommands;

        [FixedLength(3), IfVersion(CmpOp.Gequal, 0x21)] public readonly H3DTextureMapper[] TextureMappers;

        /*
         * Older BCH versions had the Texture Mappers stored directly within the Material data.
         * Newer versions (see above) uses a pointer and stores it somewhere else instead.
         */
        [FixedLength(3), IfVersion(CmpOp.Less, 0x21), Inline] private H3DTextureMapper[] TextureMappersCompat;

        public string Texture0Name;
        public string Texture1Name;
        public string Texture2Name;

        private string _Name;

        public string Name
        {
            get => _Name;
            set => _Name = value ?? throw Exceptions.GetNullException("Name");
        }

        [Ignore] public readonly bool[] EnabledTextures;

        public static H3DMaterial GetSimpleMaterial(
            string ModelName,
            string MaterialName,
            string TextureName,
            string ShaderName = "DefaultShader",
            int    ShaderIndex = 0)
        {
            H3DMaterial Output = new H3DMaterial();

            Output.MaterialParams.EmissionColor  = RGBA.White;
            Output.MaterialParams.AmbientColor   = RGBA.White;
            Output.MaterialParams.DiffuseColor   = RGBA.White;
            Output.MaterialParams.Specular0Color = RGBA.White;
            Output.MaterialParams.Specular1Color = RGBA.White;
            Output.MaterialParams.Constant0Color = RGBA.White;
            Output.MaterialParams.Constant1Color = RGBA.White;
            Output.MaterialParams.Constant2Color = RGBA.White;
            Output.MaterialParams.Constant3Color = RGBA.White;
            Output.MaterialParams.Constant4Color = RGBA.White;
            Output.MaterialParams.Constant5Color = RGBA.White;
            Output.MaterialParams.BlendColor     = RGBA.White;

            Output.MaterialParams.ColorScale = 1;

            Output.MaterialParams.TexEnvBufferColor = RGBA.White;

            Output.MaterialParams.ColorOperation.BlendMode   = PICABlendMode.Blend;
            Output.MaterialParams.BlendFunction.ColorSrcFunc = PICABlendFunc.One;
            Output.MaterialParams.BlendFunction.ColorDstFunc = PICABlendFunc.Zero;
            Output.MaterialParams.BlendFunction.AlphaSrcFunc = PICABlendFunc.One;
            Output.MaterialParams.BlendFunction.AlphaDstFunc = PICABlendFunc.Zero;

            Output.MaterialParams.DepthColorMask.Enabled = true;

            Output.MaterialParams.DepthColorMask.DepthFunc = PICATestFunc.Lequal;

            Output.MaterialParams.DepthColorMask.RedWrite   = true;
            Output.MaterialParams.DepthColorMask.GreenWrite = true;
            Output.MaterialParams.DepthColorMask.BlueWrite  = true;
            Output.MaterialParams.DepthColorMask.AlphaWrite = true;
            Output.MaterialParams.DepthColorMask.DepthWrite = true;

            Output.MaterialParams.ColorBufferRead  = false;
            Output.MaterialParams.ColorBufferWrite = true;

            Output.MaterialParams.StencilBufferRead  = false;
            Output.MaterialParams.StencilBufferWrite = false;

            Output.MaterialParams.DepthBufferRead  = true;
            Output.MaterialParams.DepthBufferWrite = true;

            Output.MaterialParams.TexEnvStages[0] = PICATexEnvStage.Texture0;
            Output.MaterialParams.TexEnvStages[1] = PICATexEnvStage.PassThrough;
            Output.MaterialParams.TexEnvStages[2] = PICATexEnvStage.PassThrough;
            Output.MaterialParams.TexEnvStages[3] = PICATexEnvStage.PassThrough;
            Output.MaterialParams.TexEnvStages[4] = PICATexEnvStage.PassThrough;
            Output.MaterialParams.TexEnvStages[5] = PICATexEnvStage.PassThrough;

            Output.TextureMappers[0].WrapU = PICATextureWrap.Repeat;
            Output.TextureMappers[0].WrapV = PICATextureWrap.Repeat;

            Output.TextureMappers[0].MinFilter = H3DTextureMinFilter.NearestMipmapLinear;
            Output.TextureMappers[0].MagFilter = H3DTextureMagFilter.Linear;

            Output.TextureMappers[0].BorderColor = RGBA.White;

            Output.MaterialParams.TextureCoords[0].Flags = H3DTextureCoordFlags.IsDirty;
            Output.MaterialParams.TextureCoords[0].ReferenceCameraIndex = -1;
            Output.MaterialParams.TextureCoords[0].Scale = Vector2.One;

            Output.MaterialParams.ShaderReference = $"{ShaderIndex}@{ShaderName}";
            Output.MaterialParams.ModelReference = $"{MaterialName}@{ModelName}";

            Output.EnabledTextures[0] = true;

            Output.Name         = MaterialName;
            Output.Texture0Name = TextureName;

            return Output;
        }

        public H3DMaterial()
        {
            MaterialParams = new H3DMaterialParams();
            TextureMappers = new H3DTextureMapper[3];
            TextureMappersCompat = null;

            EnabledTextures = new bool[4];
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            PICACommandReader Reader = new PICACommandReader(TextureCommands);

            while (Reader.HasCommand)
            {
                PICACommand Cmd = Reader.GetCommand();

                uint Param = Cmd.Parameters[0];

                switch (Cmd.Register)
                {
                    case PICARegister.GPUREG_TEXUNIT_CONFIG:
                        EnabledTextures[0] = (Param & 0x001) != 0;
                        EnabledTextures[1] = (Param & 0x002) != 0;
                        EnabledTextures[2] = (Param & 0x004) != 0;
                        EnabledTextures[3] = (Param & 0x400) != 0;
                        break;
                }
            }

            if (TextureMappersCompat != null)
            {
                Array.Copy(TextureMappersCompat, TextureMappers, 3);
            }
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            //The original tool seems to add those (usually unused) names with the silhouette suffix
            Serializer.Sections[(uint)H3DSectionId.Strings].Values.Add(new RefValue()
            {
                Position = -1,
                Value    = $"{Name}-silhouette"
            });

            PICACommandWriter Writer = new PICACommandWriter();

            uint TexUnitConfig = 0x00011000u;

            TexUnitConfig |= (EnabledTextures[0] ? 1u : 0u) << 0;
            TexUnitConfig |= (EnabledTextures[1] ? 1u : 0u) << 1;
            TexUnitConfig |= (EnabledTextures[2] ? 1u : 0u) << 2;
            TexUnitConfig |= (EnabledTextures[3] ? 1u : 0u) << 10;

            Writer.SetCommands(PICARegister.GPUREG_TEXUNIT_CONFIG, false, 0, 0, 0, 0);
            Writer.SetCommand(PICARegister.GPUREG_TEXUNIT_CONFIG, TexUnitConfig);

            Writer.SetCommand(PICARegister.GPUREG_TEXUNIT0_BORDER_COLOR, 0);
            Writer.SetCommand(PICARegister.GPUREG_TEXUNIT0_DIM, 0);
            Writer.SetCommand(PICARegister.GPUREG_TEXUNIT0_PARAM, 0x2220);
            Writer.SetCommand(PICARegister.GPUREG_TEXUNIT0_LOD, 0);
            Writer.SetCommand(PICARegister.GPUREG_TEXUNIT0_ADDR1, 0);
            Writer.SetCommand(PICARegister.GPUREG_TEXUNIT0_TYPE, 0xc);

            Writer.SetCommand(PICARegister.GPUREG_TEXUNIT1_BORDER_COLOR, 0);
            Writer.SetCommand(PICARegister.GPUREG_TEXUNIT1_DIM, 0);
            Writer.SetCommand(PICARegister.GPUREG_TEXUNIT1_PARAM, 0x2220);
            Writer.SetCommand(PICARegister.GPUREG_TEXUNIT1_LOD, 0);
            Writer.SetCommand(PICARegister.GPUREG_TEXUNIT1_ADDR, 0);
            Writer.SetCommand(PICARegister.GPUREG_TEXUNIT1_TYPE, 0xc);

            Writer.SetCommand(PICARegister.GPUREG_TEXUNIT2_BORDER_COLOR, 0);
            Writer.SetCommand(PICARegister.GPUREG_TEXUNIT2_DIM, 0);
            Writer.SetCommand(PICARegister.GPUREG_TEXUNIT2_PARAM, 0x2220);
            Writer.SetCommand(PICARegister.GPUREG_TEXUNIT2_LOD, 0);
            Writer.SetCommand(PICARegister.GPUREG_TEXUNIT2_ADDR, 0);
            Writer.SetCommand(PICARegister.GPUREG_TEXUNIT2_TYPE, 0xc);

            Writer.WriteEnd();

            TextureCommands = Writer.GetBuffer();

            if (TextureMappersCompat != null)
            {
                Array.Copy(TextureMappers, TextureMappersCompat, 3);
            }

            return false;
        }
    }
}
