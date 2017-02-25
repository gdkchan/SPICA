using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Animation;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.Generic.StudioMdl;
using SPICA.Formats.GFL2;
using SPICA.Formats.GFL2.Motion;

using System.IO;
using System.Text;

namespace SPICA.WinForms.Formats
{
    static class FormatIdentifier
    {
        public static H3D IdentifyAndOpen(string FileName, PatriciaList<H3DBone> Skeleton = null)
        {
            //Formats that can by identified by extensions
            string FilePath = Path.GetDirectoryName(FileName);

            switch (Path.GetExtension(FileName).ToLower())
            {
                case ".smd": return new SMD(FileName).ToH3D(FilePath);
            }

            //Formats that can only be indetified by "magic numbers"
            H3D Output = null;

            using (FileStream FS = new FileStream(FileName, FileMode.Open))
            {
                if (FS.Length > 4)
                {
                    BinaryReader Reader = new BinaryReader(FS);

                    uint MagicNum = Reader.ReadUInt32();

                    FS.Seek(-4, SeekOrigin.Current);

                    string Magic = Encoding.ASCII.GetString(Reader.ReadBytes(4));

                    FS.Seek(0, SeekOrigin.Begin);

                    if (Magic.StartsWith("BCH"))
                    {
                        FS.Close();

                        return H3D.Open(FileName);
                    }
                    else
                    {
                        if (GFPackage.IsValidPackage(FS))
                        {
                            GFPackage.Header PackHeader = GFPackage.GetPackageHeader(FS);

                            switch (PackHeader.Magic)
                            {
                                case "AD": Output = GFPackedTexture.OpenAsH3D(FS, PackHeader, 1); break;
                                case "BG": Output = GFL2OverWorld.OpenAsH3D(FS, PackHeader, Skeleton); break;
                                case "BS": Output = GFBtlSklAnim.OpenAsH3D(FS, PackHeader); break;
                                case "CM": Output = GFCharaModel.OpenAsH3D(FS, PackHeader); break;
                                case "GR": Output = GFOWMapModel.OpenAsH3D(FS, PackHeader); break;
                                case "MM": Output = GFOWCharaModel.OpenAsH3D(FS, PackHeader); break;
                                case "PC": Output = GFPkmnModel.OpenAsH3D(FS, PackHeader, Skeleton); break;
                                case "PT": Output = GFPackedTexture.OpenAsH3D(FS, PackHeader, 0); break;
                            }
                        }
                        else
                        {
                            switch (MagicNum)
                            {
                                case 0x00010000: Output = new GFModelPack(Reader).ToH3D(); break;
                                case 0x00060000:
                                    if (Skeleton != null)
                                    {
                                        Output = new H3D();

                                        GFMotion Motion = new GFMotion(Reader, 0);

                                        H3DAnimation SklAnim = Motion.ToH3DSkeletalAnimation(Skeleton);
                                        H3DAnimation MatAnim = Motion.ToH3DMaterialAnimation();
                                        H3DAnimation VisAnim = Motion.ToH3DVisibilityAnimation();

                                        if (SklAnim != null) Output.SkeletalAnimations.Add(SklAnim);
                                        if (MatAnim != null) Output.SkeletalAnimations.Add(MatAnim);
                                        if (VisAnim != null) Output.SkeletalAnimations.Add(VisAnim);
                                    }

                                    break;
                            }
                        }
                    }
                }
            }

            return Output;
        }
    }
}
