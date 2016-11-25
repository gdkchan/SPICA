using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Animation;
using SPICA.Formats.GFL2;
using SPICA.Formats.GFL2.Motion;

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
                H3DAnimation SklAnim = Mot.ToH3DSkeletalAnimation(MdlPack.Models[0].Skeleton);
                H3DAnimation MatAnim = Mot.ToH3DMaterialAnimation();

                SklAnim.Name = $"SklAnim_{Mot.Index}";
                MatAnim.Name = $"MatAnim_{Mot.Index}";

                Output.SkeletalAnimations.Add(SklAnim);
                Output.MaterialAnimations.Add(MatAnim);
            }

            return Output;
        }
    }
}
