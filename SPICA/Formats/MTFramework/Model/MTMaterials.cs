using SPICA.Formats.Common;
using SPICA.Formats.MTFramework.Shader;

using System.Collections.Generic;
using System.IO;

namespace SPICA.Formats.MTFramework.Model
{
    class MTMaterials
    {
        public List<MTMaterial> Materials;

        public MTMaterials()
        {
            Materials = new List<MTMaterial>();
        }

        public MTMaterials(Stream Input, MTShaderEffects Shader) : this(new BinaryReader(Input), Shader) { }

        public MTMaterials(BinaryReader Reader, MTShaderEffects Shader) : this()
        {
            string Magic = Reader.ReadPaddedString(4);

            uint Version          = Reader.ReadUInt32();
            uint MaterialsCount   = Reader.ReadUInt32();
            uint TexConfigCount   = Reader.ReadUInt32();
            uint MFXShaderHash    = Reader.ReadUInt32();
            uint TexConfigAddress = Reader.ReadUInt32();
            uint MaterialsAddress = Reader.ReadUInt32();

            for (int Index = 0; Index < MaterialsCount; Index++)
            {
                Reader.BaseStream.Seek(MaterialsAddress + Index * 0x3c, SeekOrigin.Begin);

                uint Unknown00        = Reader.ReadUInt32();
                uint MaterialNameHash = Reader.ReadUInt32();
                uint Unknown01        = Reader.ReadUInt32();
                uint AlphaBlendHash   = Reader.ReadUInt32();
                uint DepthStencilHash = Reader.ReadUInt32();

                Reader.BaseStream.Seek(0x20, SeekOrigin.Current);

                uint MatConfig0Address = Reader.ReadUInt32();
                uint MatConfig1Address = Reader.ReadUInt32();

                Reader.BaseStream.Seek(MatConfig0Address + 0x10, SeekOrigin.Begin);

                int Texture0Index = Reader.ReadInt32() - 1;

                Reader.BaseStream.Seek(TexConfigAddress + Texture0Index * 0x4c + 0xc, SeekOrigin.Begin);

                string Texture0Name = Reader.ReadPaddedString(0x40);

                Materials.Add(new MTMaterial
                {
                    NameHash     = MaterialNameHash,
                    AlphaBlend   = Shader.GetDescriptor<MTAlphaBlendConfig>(AlphaBlendHash),
                    DepthStencil = Shader.GetDescriptor<MTDepthStencilConfig>(DepthStencilHash),
                    Texture0Name = Texture0Name
                });
            }
        }
    }
}
