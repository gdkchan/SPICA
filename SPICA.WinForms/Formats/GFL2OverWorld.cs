using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Animation;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.GFL2;
using SPICA.Formats.GFL2.Model;
using SPICA.Formats.GFL2.Motion;
using SPICA.Formats.GFL2.Shader;
using SPICA.Formats.GFL2.Texture;

using System.IO;

namespace SPICA.WinForms.Formats
{
    class GFL2OverWorld
    {
        const uint GFModelConstant = 0x15122117;
        const uint GFTextureConstant = 0x15041213;
        const uint GFMotionConstant = 0x00060000;

        public static H3D OpenAsH3D(Stream Input, GFPackage.Header Header, H3DDict<H3DBone> Skeleton = null)
        {
            BinaryReader Reader = new BinaryReader(Input);

            GFModelPack MdlPack = new GFModelPack();
            GFMotionPack MotPack = new GFMotionPack();

            ReadModelsBG(Header.Entries[0], Reader, MdlPack); //Textures
            ReadModelsBG(Header.Entries[1], Reader, MdlPack); //Shaders
            ReadModelsBG(Header.Entries[2], Reader, MdlPack); //Models
            ReadModelsBG(Header.Entries[3], Reader, MdlPack); //Models?
            ReadModelsBG(Header.Entries[4], Reader, MdlPack); //More models
            ReadAnimsBG(Header.Entries[5], Reader, MotPack); //Animations
            ReadAnimsBG(Header.Entries[6], Reader, MotPack); //More animations

            H3D Output = MdlPack.ToH3D();

            foreach (GFMotion Mot in MotPack)
            {
                H3DMaterialAnim MatAnim = Mot.ToH3DMaterialAnimation();
                H3DAnimation    VisAnim = Mot.ToH3DVisibilityAnimation();

                if (MatAnim != null)
                {
                    MatAnim.Name = $"Motion_{Mot.Index}";

                    Output.MaterialAnimations.Add(MatAnim);
                }

                if (VisAnim != null)
                {
                    VisAnim.Name = $"Motion_{Mot.Index}";

                    Output.VisibilityAnimations.Add(VisAnim);
                }
            }

            return Output;
        }

        private static void ReadModelsBG(GFPackage.Entry File, BinaryReader Reader, GFModelPack MdlPack)
        {
            if (File.Length < 0x80) return;

            Reader.BaseStream.Seek(File.Address, SeekOrigin.Begin);

            GFPackage.Header Header = GFPackage.GetPackageHeader(Reader.BaseStream);

            foreach (GFPackage.Entry Entry in Header.Entries)
            {
                if (Entry.Length < 4) continue;

                Reader.BaseStream.Seek(Entry.Address, SeekOrigin.Begin);

                uint MagicNum = Reader.ReadUInt32();

                switch (MagicNum)
                {
                    case GFModelConstant:
                        Reader.BaseStream.Seek(-4, SeekOrigin.Current);

                        MdlPack.Models.Add(new GFModel(Reader, $"Model_{MdlPack.Models.Count}"));

                        break;

                    case GFTextureConstant:
                        uint Count = Reader.ReadUInt32();

                        string Signature = string.Empty;

                        for (int i = 0; i < 8; i++)
                        {
                            byte Value = Reader.ReadByte();

                            if (Value < 0x20 || Value > 0x7e) break;

                            Signature += (char)Value;
                        }

                        Reader.BaseStream.Seek(Entry.Address, SeekOrigin.Begin);

                        if (Signature == "texture")
                            MdlPack.Textures.Add(new GFTexture(Reader));
                        else
                            MdlPack.Shaders.Add(new GFShader(Reader));

                        break;
                }
            }
        }

        private static void ReadAnimsBG(GFPackage.Entry File, BinaryReader Reader, GFMotionPack MotPack)
        {
            if (File.Length < 0x80) return;

            Reader.BaseStream.Seek(File.Address, SeekOrigin.Begin);

            GFPackage.Header Header = GFPackage.GetPackageHeader(Reader.BaseStream);

            foreach (GFPackage.Entry Entry in Header.Entries)
            {
                if (Entry.Length < 4) continue;

                Reader.BaseStream.Seek(Entry.Address, SeekOrigin.Begin);

                uint MagicNum = Reader.ReadUInt32();

                if (MagicNum == GFMotionConstant)
                {
                    Reader.BaseStream.Seek(-4, SeekOrigin.Current);

                    MotPack.Add(new GFMotion(Reader, MotPack.Count));
                }
            }
        }
    }
}
