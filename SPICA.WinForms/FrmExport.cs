using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.Formats.CtrH3D.Texture;
using SPICA.Formats.Generic.COLLADA;
using SPICA.Formats.Generic.StudioMdl;
using SPICA.WinForms.Formats;

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace SPICA.WinForms
{
    public partial class FrmExport : Form
    {
        public FrmExport()
        {
            InitializeComponent();
        }

        private void FrmExport_Load(object sender, EventArgs e)
        {
            CmbFormat.SelectedIndex = 0;
        }

        private void BtnBrowseIn_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog Browser = new FolderBrowserDialog())
            {
                if (Browser.ShowDialog() == DialogResult.OK) TxtInputFolder.Text = Browser.SelectedPath;
            }
        }

        private void BtnBrowseOut_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog Browser = new FolderBrowserDialog())
            {
                if (Browser.ShowDialog() == DialogResult.OK) TxtOutFolder.Text = Browser.SelectedPath;
            }
        }

        private void BtnConvert_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(TxtInputFolder.Text))
            {
                MessageBox.Show(
                    "Input folder not found!",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (!Directory.Exists(TxtOutFolder.Text))
            {
                MessageBox.Show(
                    "Output folder not found!",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            string[] Files = Directory.GetFiles(TxtInputFolder.Text);

            Array.Sort(Files, StringComparer.OrdinalIgnoreCase);

            List<Tuple<string, H3D>> Inputs = new List<Tuple<string, H3D>>();
            Dictionary<string, H3DTexture> SharedTextures = new Dictionary<string, H3DTexture>(StringComparer.Ordinal);
            H3DDict<H3DBone> Skeleton = null;

            foreach (string FileName in Files)
            {
                H3D Data = FormatIdentifier.IdentifyAndOpen(FileName, Skeleton);

                Inputs.Add(Tuple.Create(FileName, Data));

                if (Data == null) continue;

                if (Skeleton == null && Data.Models.Count > 0)
                {
                    Skeleton = Data.Models[0].Skeleton;
                }

                foreach (H3DTexture Texture in Data.Textures)
                {
                    if (!SharedTextures.ContainsKey(Texture.Name))
                    {
                        SharedTextures.Add(Texture.Name, Texture);
                    }
                }
            }

            bool ExportModels = ChkExportModels.Checked;
            bool ExportAnims = ChkExportAnimations.Checked;
            bool ExportTexs = ChkExportTextures.Checked;
            bool PrefixNames = ChkPrefixNames.Checked;

            int Format = CmbFormat.SelectedIndex;

            int FileIndex = 0;

            //TODO: Use Parallel loop for more speed and keep UI responsive
            foreach (Tuple<string, H3D> Input in Inputs)
            {
                string File = Input.Item1;
                H3D Data = Input.Item2;

                if (Data != null)
                {
                    foreach (H3DModel Model in Data.Models)
                    foreach (H3DMaterial Material in Model.Materials)
                    {
                        string[] TextureNames = { Material.Texture0Name, Material.Texture1Name, Material.Texture2Name };

                        foreach (string TextureName in TextureNames)
                        {
                            if (TextureName != null && !Data.Textures.Contains(TextureName) &&
                                SharedTextures.TryGetValue(TextureName, out H3DTexture Texture))
                            {
                                Data.Textures.Add(Texture);
                            }
                        }
                    }

                    string BaseName = PrefixNames ? Path.GetFileNameWithoutExtension(File) + "_" : string.Empty;

                    BaseName = Path.Combine(TxtOutFolder.Text, BaseName);

                    if (!PrefixNames) BaseName += Path.DirectorySeparatorChar;

                    if (ExportModels)
                    {
                        for (int Index = 0; Index < Data.Models.Count; Index++)
                        {
                            string FileName = BaseName + Data.Models[Index].Name;

                            switch (Format)
                            {
                                case 0:
                                    new DAE(Data, Index, -1, new DAEExportOptions
                                    {
                                        ExportTextures = ExportTexs
                                    }).Save(FileName + ".dae");
                                    break;
                                case 1: new SMD(Data, Index).Save(FileName + ".smd"); break;
                            }
                        }
                    }

                    if (ExportAnims && Data.Models.Count > 0)
                    {
                        for (int Index = 0; Index < Data.SkeletalAnimations.Count; Index++)
                        {
                            string FileName = BaseName + Data.Models[0].Name + "_" + Data.SkeletalAnimations[Index].Name;

                            switch (Format)
                            {
                                case 0:
                                    new DAE(Data, 0, Index, new DAEExportOptions
                                    {
                                        ExportTextures = ExportTexs
                                    }).Save(FileName + ".dae");
                                    break;
                                case 1: new SMD(Data, 0, Index).Save(FileName + ".smd"); break;
                            }
                        }
                    }

                    if (ExportTexs)
                    {
                        foreach (H3DTexture Tex in Data.Textures)
                        {
                            Tex.ToBitmap().Save(Path.Combine(TxtOutFolder.Text, Tex.Name + ".png"));
                        }
                    }
                }

                float Progress = ++FileIndex;

                Progress = (Progress / Files.Length) * 100;

                ProgressConv.Value = (int)Progress;

                Application.DoEvents();
            }
        }
    }
}
