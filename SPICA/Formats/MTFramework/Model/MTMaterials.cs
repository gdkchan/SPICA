using SPICA.Formats.Common;
using SPICA.Formats.MTFramework.Shader;

using System.Collections.Generic;
using System.IO;

namespace SPICA.Formats.MTFramework.Model
{
    public class MTMaterials
    {
        public readonly List<MTMaterial> Materials;

        public MTMaterials()
        {
            Materials = new List<MTMaterial>();
        }

        public MTMaterials(Stream Input, MTShaderEffects Shader) : this(new BinaryReader(Input), Shader) { }

        public MTMaterials(BinaryReader Reader, MTShaderEffects Shader) : this()
        {
            string Magic = Reader.ReadPaddedString(4);

            uint Version        = Reader.ReadUInt32();
            uint MaterialsCount = Reader.ReadUInt32();
            uint TexLUTsCount   = Reader.ReadUInt32();
            uint MFXShaderHash  = Reader.ReadUInt32();

            if (Version == 0xc)
            {
                //Version 0xC (Street Fighter) have values like 0x1b here, unknown what it is
                //It also have an extra section (Address located after MaterialsAddress), use is unknown
                Reader.ReadUInt32();
            }

            uint TexLUTsAddress   = Reader.ReadUInt32();
            uint MaterialsAddress = Reader.ReadUInt32();

            for (int Index = 0; Index < MaterialsCount; Index++)
            {
                long BaseAddr = MaterialsAddress + Index * 0x3c;

                /*
                 * Note: It's just amazing how they can't keep the same format even on the same platform.
                 * Yea, different games have those values in different order inside the struct.
                 * TODO: Figure out exact version numbers where stuff changes (or the closest based on released games).
                 */
                Reader.BaseStream.Seek(BaseAddr + 4, SeekOrigin.Begin);

                uint MaterialNameHash  = Reader.ReadUInt32();
                uint TextureDescLength = Reader.ReadUInt32();

                Reader.BaseStream.Seek(BaseAddr + 0x34, SeekOrigin.Begin);

                uint TextureDescAddress = Reader.ReadUInt32();
                uint MatConfig1Address  = Reader.ReadUInt32();

                Reader.BaseStream.Seek(BaseAddr + (Version < 0x20 ? 0x10 : 0xc), SeekOrigin.Begin);

                uint AlphaBlendHash   = Reader.ReadUInt32(); //Alpha blending related config
                uint DepthStencilHash = Reader.ReadUInt32(); //Depth Buffer and Stencil Buffer config
                uint MeshScissorHash  = Reader.ReadUInt32(); //Mesh and Scissor stuff (Culling, Z-Bias, ...)
                byte TextureDescCount = Reader.ReadByte();

                string DiffuseName = null;

                for (int DescIndex = 0; DescIndex < TextureDescCount; DescIndex++)
                {
                    /*
                     * Bits 00-03 = Entry type
                     * Bits 04-15 = Memory garbage
                     * Bits 16-31 = Unknown
                     * Entry types:
                     * 1 = Combiner
                     * 2 = Sampler
                     * 3 = Texture map
                     */
                    Reader.BaseStream.Seek(TextureDescAddress + DescIndex * 0xc, SeekOrigin.Begin);

                    byte EntryType = (byte)(Reader.ReadUInt32() & 0xf);

                    switch (EntryType)
                    {
                        case 3:
                            /*
                             * This can be either a Texture name or a LUT.
                             * LUTs seems to be interpolated from some quantized values.
                             * It's possible that they use Hermite to interpolate the values,
                             * or maybe it's just Linear but it only have 16 values so the step is pretty large.
                             */
                            int TextureIndex = Reader.ReadInt32() - 1;

                            if (TextureIndex != -1)
                            {
                                MTTextureMap TextureMap = Shader.GetDescriptor<MTTextureMap>(Reader.ReadUInt32());

                                Reader.BaseStream.Seek(TexLUTsAddress + TextureIndex * 0x4c + 0xc, SeekOrigin.Begin);

                                switch (TextureMap.Type)
                                {
                                    case "BaseMap":       DiffuseName = Reader.ReadPaddedString(0x40); break;
                                    case "LutToon":       ReadLUT(Reader); break;
                                    case "LutShininess":  ReadLUT(Reader); break;
                                    case "LutShininessR": ReadLUT(Reader); break;
                                    case "LutShininessG": ReadLUT(Reader); break;
                                    case "LutShininessB": ReadLUT(Reader); break;
                                    case "LutFresnel":    ReadLUT(Reader); break;
                                }
                            }
                            break;
                    }
                }

                Materials.Add(new MTMaterial()
                {
                    NameHash         = MaterialNameHash,
                    AlphaBlend       = Shader.GetDescriptor<MTAlphaBlend>(AlphaBlendHash),
                    DepthStencil     = Shader.GetDescriptor<MTDepthStencil>(DepthStencilHash),
                    Texture0Name     = DiffuseName
                });
            }
        }

        private void ReadLUT(BinaryReader Reader)
        {
            /*
             * 3x UInt32 ? Textures seems to have those also
             * 4x UInt32 Small values
             * 4x Float like 0, 1, 16, 0
             * 8x Fixed Point 0.0.16 (LUT neg values?)
             * 8x Fixed Point 0.0.16 (LUT pos values?)
             * TODO: Actually read and use this stuff
             * Needs Combiners first otherwise LUTs are useless
             */
        }
    }
}
