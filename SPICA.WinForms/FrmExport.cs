using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.CtrH3D.Texture;
using SPICA.Formats.Generic.COLLADA;
using SPICA.Formats.Generic.StudioMdl;
using SPICA.WinForms.Formats;

using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPICA.WinForms {
    public partial class FrmExport : Form {
        public FrmExport() {
            InitializeComponent();
        }

        private void FrmExport_Load(object sender, EventArgs e) {
            CmbFormat.SelectedIndex = 0;
        }

        private void BtnBrowseIn_Click(object sender, EventArgs e) {
            using (FolderBrowserDialog Browser = new FolderBrowserDialog()) {
                if (Browser.ShowDialog() == DialogResult.OK) TxtInputFolder.Text = Browser.SelectedPath;
            }
        }
        private void BtnBrowseAnimationIn_Click(object sender, EventArgs e) {
            using (FolderBrowserDialog Browser = new FolderBrowserDialog()) {
                if (Browser.ShowDialog() == DialogResult.OK) TxtInputAnimationFolder.Text = Browser.SelectedPath;
            }
        }
        private void BtnBrowseOut_Click(object sender, EventArgs e) {
            using (FolderBrowserDialog Browser = new FolderBrowserDialog()) {
                if (Browser.ShowDialog() == DialogResult.OK) TxtOutFolder.Text = Browser.SelectedPath;
            }
        }

        private void BtnConvert_Click(object sender, EventArgs e) {
            if (!Directory.Exists(TxtInputFolder.Text)) {
                MessageBox.Show(
                    "Input folder not found!",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (!Directory.Exists(TxtOutFolder.Text)) {
                MessageBox.Show(
                    "Output folder not found!",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            string[] Files = Directory.GetFiles(TxtInputFolder.Text);
            string[] AnimationFiles = Directory.GetFiles(TxtInputAnimationFolder.Text);

            bool ExportModels = ChkExportModels.Checked;
            bool ExportAnims = ChkExportAnimations.Checked;
            bool ExportTexs = ChkExportTextures.Checked;
            bool PrefixNames = ChkPrefixNames.Checked;

            int Format = CmbFormat.SelectedIndex;

            int FileIndex = 0;

            int CountFiles = 0;
            //TODO: Use Parallel loop for more speed and keep UI responsive
            foreach (string File in Files) {
                H3D Data = FormatIdentifier.IdentifyAndOpen(File);
                if (Data != null) {
                    string BaseName = PrefixNames ? Path.GetFileNameWithoutExtension(File) + "_" : string.Empty;

                    BaseName = Path.Combine(TxtOutFolder.Text, BaseName);

                    if (!PrefixNames) BaseName += Path.DirectorySeparatorChar;

                    if (ExportModels) {
                        for (int Index = 0; Index < Data.Models.Count; Index++) {
                            string FileName = BaseName + Data.Models[Index].Name;

                            switch (Format) {
                                case 0: new DAE(Data, Index).Save(FileName + ".dae"); break;
                                case 1: new SMD(Data, Index).Save(FileName + ".smd"); break;
                            }
                        }
                    }
                    if (ExportAnims) {
                        string Filename = Path.GetFileName(File);
                        string ModelFolder = File.Replace(Filename, "");

                        foreach (string AnimationFile in AnimationFiles) {
                            if (AnimationFile.Contains(Filename)) {
                                string AnimationFolder = AnimationFile.Replace(Filename, "");
                                string[] MergedFiles = new string[] { Path.Combine(ModelFolder, Filename), Path.Combine(AnimationFolder, Filename) };
                                H3D MergedData = FileIO.Merge(MergedFiles, new Rendering.Renderer(1, 1), Data);
                                for (int Index = 0; Index < MergedData.SkeletalAnimations.Count; Index++) {
                                    string FileName = BaseName + MergedData.Models[0].Name + "_" + MergedData.SkeletalAnimations[Index].Name;
                                    switch (Format) {
                                        case 0: new DAE(MergedData, 0, Index).Save(FileName + ".dae"); break;
                                        case 1: new SMD(MergedData, 0, Index).Save(FileName + ".smd"); break;
                                    }
                                }
                            }
                        }
                    }

                    if (ExportTexs) {
                        foreach (H3DTexture Tex in Data.Textures) {
                            Tex.ToBitmap().Save(Path.Combine(TxtOutFolder.Text, Tex.Name + ".png"));
                        }
                    }
                }

                float Progress = ++FileIndex;

                Progress = (Progress / Files.Length) * 100;

                ProgressConv.Value = (int)Progress;

                Application.DoEvents();
                CountFiles++;
            }
        }
    }
}
