using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Animation;
using SPICA.Formats.GFL;
using SPICA.Formats.GFL.Motion;

using System.IO;

namespace SPICA.WinForms.Formats
{
    class GFOWCharaModel
    {
        public static H3D OpenAsH3D(Stream Input, GFPackage.Header Header)
        {
            H3D Output;

            //Model
            byte[] Buffer = new byte[Header.Entries[0].Length];

            Input.Seek(Header.Entries[0].Address, SeekOrigin.Begin);

            Input.Read(Buffer, 0, Buffer.Length);

            using (MemoryStream MS = new MemoryStream(Buffer))
            {
                Output = H3D.Open(MS);
            }

            //Skeletal Animations
            Input.Seek(Header.Entries[1].Address, SeekOrigin.Begin);

            GFMotionPack MotPack = new GFMotionPack(Input);

            foreach (GFMotion Mot in MotPack)
            {
                H3DAnimation SklAnim = Mot.ToH3DSkeletalAnimation();

                SklAnim.Name = $"Motion_{Mot.Index}";

                Output.SkeletalAnimations.Add(SklAnim);
            }

            //Material Animations
            if (Header.Entries[2].Length > 0)
            {
                Input.Seek(Header.Entries[2].Address, SeekOrigin.Begin);

                byte[] Data = new byte[Header.Entries[2].Length];

                Input.Read(Data, 0, Data.Length);

                H3D MatAnims = H3D.Open(Data);

                Output.Merge(MatAnims);
            }

            return Output;
        }
    }
}
