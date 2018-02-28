using SPICA.Formats.CtrGfx;
using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Animation;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.Generic.StudioMdl;
using SPICA.Formats.Generic.WavefrontOBJ;
using SPICA.Formats.GFL2;
using SPICA.Formats.GFL2.Model;
using SPICA.Formats.GFL2.Motion;
using SPICA.Formats.GFL2.Texture;
using SPICA.Formats.ModelBinary;
using SPICA.Formats.MTFramework.Model;
using SPICA.Formats.MTFramework.Shader;
using SPICA.Formats.MTFramework.Texture;

using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace SPICA.WinForms.Formats
{
    static class FormatIdentifier
    {
        public static H3D IdentifyAndOpen(string FileName, H3DDict<H3DBone> Skeleton = null)
        {
            //Formats that can by identified by extensions
            string FilePath = Path.GetDirectoryName(FileName);

            switch (Path.GetExtension(FileName).ToLower())
            {
                case ".smd": return new SMD(FileName).ToH3D(FilePath);
                case ".obj": return new OBJ(FileName).ToH3D(FilePath);
                case ".mbn":
                    using (FileStream Input = new FileStream(FileName, FileMode.Open))
                    {
                        H3D BaseScene = H3D.Open(File.ReadAllBytes(FileName.Replace(".mbn", ".bch")));

                        MBn ModelBinary = new MBn(new BinaryReader(Input), BaseScene);

                        return ModelBinary.ToH3D();
                    }
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
                        return H3D.Open(Reader.ReadBytes((int)FS.Length));
                    }
                    else if (Magic.StartsWith("MOD"))
                    {
                        return LoadMTModel(Reader, FileName, Path.GetDirectoryName(FileName));
                    }
                    else if (Magic.StartsWith("TEX"))
                    {
                        return new MTTexture(Reader, Path.GetFileNameWithoutExtension(FileName)).ToH3D();
                    }
                    else if (Magic.StartsWith("MFX"))
                    {
                        MTShader = new MTShaderEffects(Reader);
                    }
                    else if (Magic.StartsWith("CGFX"))
                    {
                        return Gfx.Open(FS);
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
                                case "BS": Output = GFBtlSklAnim.OpenAsH3D(FS, PackHeader, Skeleton); break;
                                case "CM": Output = GFCharaModel.OpenAsH3D(FS, PackHeader); break;
                                case "GR": Output = GFOWMapModel.OpenAsH3D(FS, PackHeader); break;
                                case "MM": Output = GFOWCharaModel.OpenAsH3D(FS, PackHeader); break;
                                case "PC": Output = GFPkmnModel.OpenAsH3D(FS, PackHeader, Skeleton); break;
                                case "PT": Output = GFPackedTexture.OpenAsH3D(FS, PackHeader, 0); break;
                                case "PK":
                                case "PB":
                                    Output = GFPkmnSklAnim.OpenAsH3D(FS, PackHeader, Skeleton); break;
                            }
                        }
                        else
                        {
                            switch (MagicNum)
                            {
                                case 0x15122117:
                                    Output = new H3D();

                                    Output.Models.Add(new GFModel(Reader, "Model").ToH3DModel());

                                    break;

                                case 0x15041213:
                                    Output = new H3D();

                                    Output.Textures.Add(new GFTexture(Reader).ToH3DTexture());

                                    break;

                                case 0x00010000: Output = new GFModelPack(Reader).ToH3D(); break;
                                case 0x00060000:
                                    if (Skeleton != null)
                                    {
                                        Output = new H3D();

                                        GFMotion Motion = new GFMotion(Reader, 0);

                                        H3DAnimation    SklAnim = Motion.ToH3DSkeletalAnimation(Skeleton);
                                        H3DMaterialAnim MatAnim = Motion.ToH3DMaterialAnimation();
                                        H3DAnimation    VisAnim = Motion.ToH3DVisibilityAnimation();

                                        if (SklAnim != null) Output.SkeletalAnimations.Add(SklAnim);
                                        if (MatAnim != null) Output.MaterialAnimations.Add(MatAnim);
                                        if (VisAnim != null) Output.VisibilityAnimations.Add(VisAnim);
                                    }

                                    break;
                            }
                        }
                    }
                }
            }

            return Output;
        }

        private static MTShaderEffects MTShader;

        private static H3D LoadMTModel(BinaryReader Reader, string ModelFile, string MRLSearchPath)
        {
            if (MTShader != null)
            {
                MTMaterials MRLData = new MTMaterials();

                foreach (string File in Directory.GetFiles(MRLSearchPath))
                {
                    if (File == ModelFile || !File.StartsWith(ModelFile.Substring(0, ModelFile.LastIndexOf('.'))))
                    {
                        continue;
                    }

                    using (FileStream Input = new FileStream(File, FileMode.Open))
                    {
                        if (Input.Length < 4) continue;

                        byte[] Magic = new byte[4];

                        Input.Read(Magic, 0, Magic.Length);

                        if (Encoding.ASCII.GetString(Magic) == "MRL\0")
                        {
                            Input.Seek(0, SeekOrigin.Begin);

                            MRLData.Materials.AddRange(new MTMaterials(Input, MTShader).Materials);
                        }
                    }
                }

                return new MTModel(Reader, MRLData, MTShader).ToH3D();
            }
            else
            {
                DialogResult Result = MessageBox.Show(
                    "A *.lfx shader is necessary to load this format." + Environment.NewLine +
                    "Do you want to open the shader file now?",
                    "Shader file missing",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (Result == DialogResult.Yes)
                {
                    using (OpenFileDialog OpenDlg = new OpenFileDialog())
                    {
                        OpenDlg.Filter = "MT Mobile Shader|*.lfx";

                        if (OpenDlg.ShowDialog() == DialogResult.OK)
                        {
                            LoadMTShader(OpenDlg.FileName);

                            return LoadMTModel(Reader, ModelFile, MRLSearchPath);
                        }
                    }
                }
            }

            return null;
        }

        private static void LoadMTShader(string FileName)
        {
            using (FileStream FS = new FileStream(FileName, FileMode.Open))
            {
                MTShader = new MTShaderEffects(FS);
            }
        }
    }
}
