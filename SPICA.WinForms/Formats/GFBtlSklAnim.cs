using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Animation;
using SPICA.Formats.GFL;
using SPICA.Formats.GFL.Motion;

using System.IO;

namespace SPICA.WinForms.Formats
{
    class GFBtlSklAnim
    {
        public static H3D OpenAsH3D(Stream Input, GFPackage.Header Header)
        {
            H3D Output = new H3D
            {
                ConverterVersion      = 42607,
                BackwardCompatibility = 0x21,
                ForwardCompatibility  = 0x21,
                Flags                 = H3DFlags.IsFromNewConverter
            };

            //Animations
            Input.Seek(Header.Entries[0].Address, SeekOrigin.Begin);

            GFMotionPack MotPack = new GFMotionPack(Input);

            foreach (GFMotion Mot in MotPack)
            {
                H3DAnimation SklAnim = Mot.ToH3DSkeletalAnimation();

                SklAnim.Name = $"Motion_{Mot.Index}";

                Output.SkeletalAnimations.Add(SklAnim);
            }

            return Output;
        }
    }
}
