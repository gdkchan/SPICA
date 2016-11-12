using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.LUT;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.Formats.CtrH3D.Texture;
using SPICA.Formats.GFL2.Model;
using SPICA.Formats.GFL2.Shader;
using SPICA.Formats.GFL2.Texture;
using SPICA.Formats.GFL2.Utils;

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SPICA.Formats.GFL2
{
    class GFModelPack
    {
        private enum Section
        {
            Model,
            Texture,
            VtxShader,
            GeoShader,
            FragShader
        }

        public List<GFModel>      Models;
        public List<GFTexture>    Textures;
        public List<GFFragShader> FragShaders;

        public GFModelPack()
        {
            Models      = new List<GFModel>();
            Textures    = new List<GFTexture>();
            FragShaders = new List<GFFragShader>();
        }

        public GFModelPack(string FileName)
        {
            using (FileStream FS = new FileStream(FileName, FileMode.Open))
            {
                GFModelPackImpl(new BinaryReader(FS));
            }
        }

        public GFModelPack(Stream Input)
        {
            GFModelPackImpl(new BinaryReader(Input));
        }

        public GFModelPack(BinaryReader Reader)
        {
            GFModelPackImpl(Reader);
        }

        private void GFModelPackImpl(BinaryReader Reader)
        {
            Models      = new List<GFModel>();
            Textures    = new List<GFTexture>();
            FragShaders = new List<GFFragShader>();

            uint MagicNumber = Reader.ReadUInt32();

            uint[] Counts = new uint[5];

            for (int Index = 0; Index < Counts.Length; Index++)
            {
                Counts[Index] = Reader.ReadUInt32();
            }

            long PointersAddr = Reader.BaseStream.Position;

            for (int Sect = 0; Sect < Counts.Length; Sect++)
            {
                for (int Entry = 0; Entry < Counts[Sect]; Entry++)
                {
                    Reader.BaseStream.Seek(PointersAddr + Entry * 4, SeekOrigin.Begin);
                    Reader.BaseStream.Seek(Reader.ReadUInt32(), SeekOrigin.Begin);

                    string Name = GFString.ReadLength(Reader, Reader.ReadByte());
                    uint Address = Reader.ReadUInt32();

                    Reader.BaseStream.Seek(Address, SeekOrigin.Begin);

                    switch ((Section)Sect)
                    {
                        case Section.Model: Models.Add(new GFModel(Reader, Name)); break;
                        case Section.Texture: Textures.Add(new GFTexture(Reader)); break;
                        case Section.FragShader: FragShaders.Add(new GFFragShader(Reader)); break;
                    }
                }

                PointersAddr += Counts[Sect] * 4;
            }
        }

        public H3D ToH3D()
        {
            H3D Output = new H3D();

            H3DLUT L = new H3DLUT();

            L.Name = GFModel.DefaultLUTName;

            for (int MdlIndex = 0; MdlIndex < Models.Count; MdlIndex++)
            {
                GFModel Model = Models[MdlIndex];
                H3DModel Mdl = Model.ToH3DModel();

                for (int MatIndex = 0; MatIndex < Model.Materials.Count; MatIndex++)
                {
                    H3DMaterialParams Params = Mdl.Materials[MatIndex].MaterialParams;
                    GFHashName FragShaderName = Model.Materials[MatIndex].FragShaderName;
                    GFFragShader FragShader = FragShaders.First(x => x.Name == FragShaderName.Name);

                    Params.TexEnvStages      = FragShader.TexEnvStages;
                    Params.TexEnvBufferColor = FragShader.TexEnvBufferColor;
                }

                foreach (GFLUT LUT in Model.LUTs)
                {
                    L.Samplers.Add(new H3DLUTSampler
                    {
                        Name = LUT.Name,
                        Type = LUT.Type,
                        Table = LUT.Table
                    });
                }

                Output.Models.Add(Mdl);
            }

            Output.LUTs.Add(L);

            Output.CopyMaterials();

            foreach (GFTexture Texture in Textures)
            {
                Output.Textures.Add(new H3DTexture
                {
                    Name = Texture.Name,
                    RawBufferXPos = Texture.RawBuffer,
                    Width = Texture.Width,
                    Height = Texture.Height,
                    Format = Texture.Format.ToPICATextureFormat(),
                    MipmapSize = (byte)Texture.MipmapSize
                });
            }

            return Output;
        }
    }
}
