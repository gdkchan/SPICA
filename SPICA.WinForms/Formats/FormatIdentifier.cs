using SPICA.Formats.CtrH3D;

using System.IO;
using System.Text;

namespace SPICA.WinForms.Formats
{
    static class FormatIdentifier
    {
        public static H3D IdentifyAndOpen(string FileName)
        {
            H3D Output = null;

            using (FileStream FS = new FileStream(FileName, FileMode.Open))
            {
                if (FS.Length > 4)
                {
                    BinaryReader Reader = new BinaryReader(FS);

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
                                case "CM": Output = GFCharaModel.OpenAsH3D(FS, PackHeader); break;
                            }
                        }
                    }
                }
            }

            return Output;
        }
    }
}
