using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Animation;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.GFL2;
using SPICA.Formats.GFL2.Model;
using SPICA.Formats.GFL2.Motion;
using SPICA.Formats.GFL2.Shader;
using SPICA.Formats.GFL2.Texture;

using System.Collections.Generic;
using System.IO;

namespace SPICA.WinForms.Formats
{
    class GFPkmnModel
    {
        const uint GFModelConstant = 0x15122117;
        const uint GFTextureConstant = 0x15041213;
        const uint GFMotionConstant = 0x00060000;

        public static H3D OpenAsH3D(Stream Input, GFPackage.Header Header, PatriciaList<H3DBone> Skeleton = null)
        {
            H3D Output = default(H3D);

            BinaryReader Reader = new BinaryReader(Input);

            Input.Seek(Header.Entries[0].Address, SeekOrigin.Begin);

            uint MagicNum = Reader.ReadUInt32();

            switch (MagicNum)
            {
                case GFModelConstant:
                    GFModelPack MdlPack = new GFModelPack();

                    //High Poly Pokémon model
                    Input.Seek(Header.Entries[0].Address, SeekOrigin.Begin);

                    MdlPack.Models.Add(new GFModel(Reader, "PM_HighPoly"));

                    //Low Poly Pokémon model
                    Input.Seek(Header.Entries[1].Address, SeekOrigin.Begin);
                    
                    MdlPack.Models.Add(new GFModel(Reader, "PM_LowPoly"));

                    //Pokémon Shader package
                    Input.Seek(Header.Entries[2].Address, SeekOrigin.Begin);

                    GFPackage.Header PSHeader = GFPackage.GetPackageHeader(Input);

                    foreach (GFPackage.Entry Entry in PSHeader.Entries)
                    {
                        Input.Seek(Entry.Address, SeekOrigin.Begin);

                        MdlPack.FragShaders.Add(new GFFragShader(Reader));
                    }

                    Output = MdlPack.ToH3D();

                    break;

                case GFTextureConstant:
                    Output = new H3D();

                    foreach (GFPackage.Entry Entry in Header.Entries)
                    {
                        Input.Seek(Entry.Address, SeekOrigin.Begin);

                        Output.Textures.Add(new GFTexture(Reader).ToH3DTexture());
                    }

                    break;

                case GFMotionConstant:
                    Output = new H3D();

                    if (Skeleton == null) break;

                    for (int Index = 0; Index < Header.Entries.Length; Index++)
                    {
                        Input.Seek(Header.Entries[Index].Address, SeekOrigin.Begin);

                        if (Input.Position + 4 > Input.Length) break;
                        if (Reader.ReadUInt32() != GFMotionConstant) continue;

                        Input.Seek(-4, SeekOrigin.Current);

                        GFMotion Mot = new GFMotion(Reader, Index);

                        H3DAnimation SklAnim = Mot.ToH3DSkeletalAnimation(Skeleton);
                        H3DAnimation MatAnim = Mot.ToH3DMaterialAnimation();
                        H3DAnimation VisAnim = Mot.ToH3DVisibilityAnimation();

                        if (SklAnim != null)
                        {
                            SklAnim.Name = $"Motion_{Mot.Index}";

                            Output.SkeletalAnimations.Add(SklAnim);
                        }

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

                    break;
            }

            return Output;
        }
    }
}
