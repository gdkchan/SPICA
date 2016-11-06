using SPICA.Formats.CtrH3D.Model.Material.Texture;
using SPICA.Formats.CtrH3D.Texture;
using SPICA.Math3D;
using SPICA.PICA;
using SPICA.PICA.Commands;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;
using SPICA.Serialization.Serializer;

using System;

namespace SPICA.Formats.CtrH3D.Model.Material
{
    [Inline]
    public class H3DMaterial : ICustomSerialization, INamed
    {
        public H3DMaterialParams MaterialParams;

        public H3DTexture Texture0;
        public H3DTexture Texture1;
        public H3DTexture Texture2;

        private uint[] TextureCommands;

        [FixedLength(3)]
        public H3DTextureMapper[] TextureMappers;

        public string Texture0Name;
        public string Texture1Name;
        public string Texture2Name;

        public string Name;

        public string ObjectName { get { return Name; } }

        [NonSerialized]
        public bool[] EnabledTextures;

        [NonSerialized]
        public int[] TextureSources;

        //This is a default material with 1 texture and default settings
        public static H3DMaterial Default
        {
            get
            {
                H3DMaterial Output = new H3DMaterial();

                Output.MaterialParams.TextureCoords[0].Scale = new Vector2D(1, 1);

                Output.MaterialParams.AmbientColor = new RGBA(255, 255, 255, 255);
                Output.MaterialParams.DiffuseColor = new RGBA(0, 0, 0, 255);

                Output.MaterialParams.ColorScale = 1;

                Output.MaterialParams.TexEnvBufferColor = new PICATexEnvColor(0xff000000);

                Output.MaterialParams.LUTInputAbs.ReflecRAbs = true;
                Output.MaterialParams.LUTInputAbs.ReflecGAbs = true;
                Output.MaterialParams.LUTInputAbs.ReflecBAbs = true;

                Output.MaterialParams.LUTInputAbs.Dist0Abs = true;
                Output.MaterialParams.LUTInputAbs.Dist1Abs = true;

                Output.MaterialParams.LUTInputAbs.FresnelAbs = true;

                Output.MaterialParams.LUTInputAbs.SpecularAbs = true;

                Output.MaterialParams.DepthColorMask.Enabled = true;

                Output.MaterialParams.DepthColorMask.DepthFunc = PICATestFunc.LessThan;

                Output.MaterialParams.DepthColorMask.RedWrite = true;
                Output.MaterialParams.DepthColorMask.GreenWrite = true;
                Output.MaterialParams.DepthColorMask.BlueWrite = true;
                Output.MaterialParams.DepthColorMask.AlphaWrite = true;
                Output.MaterialParams.DepthColorMask.DepthWrite = true;

                for (int Stage = 0; Stage < 6; Stage++)
                {
                    Output.MaterialParams.TexEnvStages[Stage].Source.RGBSource[0] = PICATextureCombinerSource.Texture0;
                    Output.MaterialParams.TexEnvStages[Stage].Source.AlphaSource[0] = PICATextureCombinerSource.Texture0;
                }

                Output.MaterialParams.FaceCulling = PICAFaceCulling.BackFace;

                Output.MaterialParams.ColorBufferWrite = true;
                Output.MaterialParams.StencilBufferRead = true;
                Output.MaterialParams.StencilBufferWrite = true;
                Output.MaterialParams.DepthBufferRead = true;
                Output.MaterialParams.DepthBufferWrite = true;

                Output.TextureMappers[0].WrapU = H3DTextureWrap.Repeat;
                Output.TextureMappers[0].WrapV = H3DTextureWrap.Repeat;
                
                Output.EnabledTextures[0] = true;

                return Output;
            }
        }

        public H3DMaterial()
        {
            MaterialParams = new H3DMaterialParams { Parent = this };
            TextureMappers = new H3DTextureMapper[3];

            EnabledTextures = new bool[4];
            TextureSources = new int[4];
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            MaterialParams.Parent = this;

            PICACommandReader Reader = new PICACommandReader(TextureCommands);

            while (Reader.HasCommand)
            {
                PICACommand Cmd = Reader.GetCommand();

                uint Param = Cmd.Parameters[0];

                switch (Cmd.Register)
                {
                    case PICARegister.GPUREG_TEXUNIT_CONFIG:
                        EnabledTextures[0] = (Param & 0x1) != 0;
                        EnabledTextures[1] = (Param & 0x2) != 0;
                        EnabledTextures[2] = (Param & 0x4) != 0;
                        TextureSources[3] = (int)((Param >> 8) & 3);
                        EnabledTextures[3] = (Param & 0x400) != 0;
                        TextureSources[2] = (int)((Param >> 13) & 1);
                        break;
                }
            }
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            //The original tool seems to add those (usually unused) names with the silhouette suffix
            Serializer.Strings.Values.Add(new RefValue
            {
                Position = -1,
                Value = Name + "-silhouette"
            });

            PICACommandWriter Writer = new PICACommandWriter();

            uint TexUnitConfig = 0x00011000u;

            TexUnitConfig |= (EnabledTextures[0] ? 1u : 0u) << 0;
            TexUnitConfig |= (EnabledTextures[1] ? 1u : 0u) << 1;
            TexUnitConfig |= (EnabledTextures[2] ? 1u : 0u) << 2;
            TexUnitConfig |= ((uint)TextureSources[3] & 3) << 8;
            TexUnitConfig |= (EnabledTextures[3] ? 1u : 0u) << 10;
            TexUnitConfig |= ((uint)TextureSources[2] & 1) << 13;

            Writer.SetCommands(PICARegister.GPUREG_TEXUNIT_CONFIG, false, 0, 0, 0, 0);
            Writer.SetCommand(PICARegister.GPUREG_TEXUNIT_CONFIG, TexUnitConfig);

            for (int Unit = 0; Unit < 3; Unit++)
            {
                switch (Unit)
                {
                    case 0:
                        Writer.SetCommand(PICARegister.GPUREG_TEXUNIT0_BORDER_COLOR, 0);
                        Writer.SetCommand(PICARegister.GPUREG_TEXUNIT0_DIM, 0);
                        Writer.SetCommand(PICARegister.GPUREG_TEXUNIT0_PARAM, 0x2220);
                        Writer.SetCommand(PICARegister.GPUREG_TEXUNIT0_LOD, 0);
                        Writer.SetCommand(PICARegister.GPUREG_TEXUNIT0_ADDR1, 0);
                        Writer.SetCommand(PICARegister.GPUREG_TEXUNIT0_TYPE, 0xc);
                        break;

                    case 1:
                        Writer.SetCommand(PICARegister.GPUREG_TEXUNIT1_BORDER_COLOR, 0);
                        Writer.SetCommand(PICARegister.GPUREG_TEXUNIT1_DIM, 0);
                        Writer.SetCommand(PICARegister.GPUREG_TEXUNIT1_PARAM, 0x2220);
                        Writer.SetCommand(PICARegister.GPUREG_TEXUNIT1_LOD, 0);
                        Writer.SetCommand(PICARegister.GPUREG_TEXUNIT1_ADDR, 0);
                        Writer.SetCommand(PICARegister.GPUREG_TEXUNIT1_TYPE, 0xc);
                        break;

                    case 2:
                        Writer.SetCommand(PICARegister.GPUREG_TEXUNIT2_BORDER_COLOR, 0);
                        Writer.SetCommand(PICARegister.GPUREG_TEXUNIT2_DIM, 0);
                        Writer.SetCommand(PICARegister.GPUREG_TEXUNIT2_PARAM, 0x2220);
                        Writer.SetCommand(PICARegister.GPUREG_TEXUNIT2_LOD, 0);
                        Writer.SetCommand(PICARegister.GPUREG_TEXUNIT2_ADDR, 0);
                        Writer.SetCommand(PICARegister.GPUREG_TEXUNIT2_TYPE, 0xc);
                        break;
                }
            }

            Writer.WriteEnd();

            TextureCommands = Writer.GetBuffer();

            return false;
        }
    }
}
