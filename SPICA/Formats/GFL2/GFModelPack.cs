using SPICA.Formats.Common;
using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.LUT;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.Formats.GFL2.Model;
using SPICA.Formats.GFL2.Shader;
using SPICA.Formats.GFL2.Texture;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace SPICA.Formats.GFL2
{
    public class GFModelPack
    {
        private enum Section
        {
            Model,
            Texture,
            Unknown2,
            Unknown3,
            Shader
        }

        public readonly List<GFModel>   Models;
        public readonly List<GFTexture> Textures;
        public readonly List<GFShader>  Shaders;

        public GFModelPack()
        {
            Models   = new List<GFModel>();
            Textures = new List<GFTexture>();
            Shaders  = new List<GFShader>();
        }

        public GFModelPack(Stream Input) : this(new BinaryReader(Input)) { }

        public GFModelPack(BinaryReader Reader) : this()
        {
            long Position = Reader.BaseStream.Position;

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
                    Reader.BaseStream.Seek(Position + Reader.ReadUInt32(), SeekOrigin.Begin);

                    string Name = Reader.ReadByteLengthString();
                    uint Address = Reader.ReadUInt32();

                    Reader.BaseStream.Seek(Position + Address, SeekOrigin.Begin);

                    switch ((Section)Sect)
                    {
                        case Section.Model:   Models.Add(new GFModel(Reader, Name)); break;
                        case Section.Texture: Textures.Add(new GFTexture(Reader));   break;
                        case Section.Shader:  Shaders.Add(new GFShader(Reader));     break;
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
                GFModel  Model = Models[MdlIndex];
                H3DModel Mdl   = Model.ToH3DModel();

                for (int MatIndex = 0; MatIndex < Model.Materials.Count; MatIndex++)
                {
                    H3DMaterialParams Params = Mdl.Materials[MatIndex].MaterialParams;

                    string FragShaderName = Model.Materials[MatIndex].FragShaderName;
                    string VtxShaderName  = Model.Materials[MatIndex].VtxShaderName;

                    GFShader FragShader = Shaders.FirstOrDefault(x => x.Name == FragShaderName);
                    GFShader VtxShader  = Shaders.FirstOrDefault(x => x.Name == VtxShaderName);

                    if (FragShader != null)
                    {
                        Params.TexEnvBufferColor = FragShader.TexEnvBufferColor;

                        Array.Copy(FragShader.TexEnvStages, Params.TexEnvStages, 6);
                    }

                    if (VtxShader != null)
                    {
                        foreach (KeyValuePair<uint, Vector4> KV in VtxShader.VtxShaderUniforms)
                        {
                            Params.VtxShaderUniforms.Add(KV.Key, KV.Value);
                        }

                        foreach (KeyValuePair<uint, Vector4> KV in VtxShader.GeoShaderUniforms)
                        {
                            Params.GeoShaderUniforms.Add(KV.Key, KV.Value);
                        }
                    }
                }

                foreach (GFLUT LUT in Model.LUTs)
                {
                    L.Samplers.Add(new H3DLUTSampler()
                    {
                        Name  = LUT.Name,
                        Table = LUT.Table
                    });
                }

                Output.Models.Add(Mdl);
            }

            Output.LUTs.Add(L);

            Output.CopyMaterials();

            foreach (GFTexture Texture in Textures)
            {
                Output.Textures.Add(Texture.ToH3DTexture());
            }

            return Output;
        }
    }
}
