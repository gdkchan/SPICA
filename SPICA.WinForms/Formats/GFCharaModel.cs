using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Animation;
using SPICA.Formats.GFL2;
using SPICA.Formats.GFL2.Motion;
using SPICA.Formats.GFL2.Texture;

using System.IO;

namespace SPICA.WinForms.Formats
{
    static class GFCharaModel
    {
        public static H3D OpenAsH3D(Stream Input, GFPackage.Header Header)
        {
            H3D Output;

            //Model
            Input.Seek(Header.Entries[0].Address, SeekOrigin.Begin);

            GFModelPack MdlPack = new GFModelPack(Input);

            Output = MdlPack.ToH3D();

            //Animations
            Input.Seek(Header.Entries[1].Address, SeekOrigin.Begin);

            GFMotionPack MotPack = new GFMotionPack(Input);

            foreach (GFMotion Mot in MotPack)
            {
                H3DAnimation    SklAnim = Mot.ToH3DSkeletalAnimation(MdlPack.Models[0].Skeleton);
                H3DMaterialAnim MatAnim = Mot.ToH3DMaterialAnimation();
                H3DAnimation    VisAnim = Mot.ToH3DVisibilityAnimation();

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

            //Texture
            if (Header.Entries.Length > 3 && Header.Entries[3].Length >= 4)
            {
                Input.Seek(Header.Entries[3].Address, SeekOrigin.Begin);

                BinaryReader Reader = new BinaryReader(Input);

                uint MagicNum = Reader.ReadUInt32();

                if (MagicNum == 0x15041213)
                {
                    Input.Seek(-4, SeekOrigin.Current);

                    GFTexture Tex = new GFTexture(Reader);

                    Output.Textures.Add(Tex.ToH3DTexture());
                }
            }

            return Output;
        }
    }
}
