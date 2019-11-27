using SPICA.Formats;
using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.Generic.COLLADA;
using SPICA.Formats.Generic.StudioMdl;
using SPICA.Formats.GFL2.Model;
using SPICA.Rendering;

using System.IO;
using System.Windows.Forms;

namespace SPICA.WinForms.Formats
{
    class FileIO
    {
        public static H3D Merge(string[] FileNames, Renderer Renderer, H3D Scene = null)
        {
            if (Scene == null)
            {
                //Renderer.DeleteAll();

                Scene = new H3D();
            }

            int OpenFiles = 0;

            using (FrmLoading Form = new FrmLoading(FileNames.Length))
            foreach (string FileName in FileNames)
            {
                Form.Proceed(FileName);
                
                H3DDict<H3DBone> Skeleton = null;

                if (Scene.Models.Count > 0) Skeleton = Scene.Models[0].Skeleton;

                H3D Data = FormatIdentifier.IdentifyAndOpen(FileName, Skeleton);

                if (Data != null)
                {
                    Scene.Merge(Data);

                    Renderer.Merge(Data);

                    OpenFiles++;
                }
            }

            if (OpenFiles == 0)
            {
                MessageBox.Show(
                    "Unsupported file format!",
                    "Can't open file!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }

            return Scene;
        }

        public static void Save(H3D Scene, SceneState State)
        {
            if (Scene == null)
            {
                MessageBox.Show(
                    "Please load a file first!",
                    "No data",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);

                return;
            }

            using (SaveFileDialog SaveDlg = new SaveFileDialog())
            {
                SaveDlg.Filter = 
                    "COLLADA 1.4.1|*.dae|" +
                    "Valve StudioMdl|*.smd|" +
                    "Binary Ctr H3D|*.bch";

                SaveDlg.FileName = "Model";

                if (SaveDlg.ShowDialog() == DialogResult.OK)
                {
                    int MdlIndex  = State.ModelIndex;
                    int AnimIndex = State.SklAnimIndex;

                    switch (SaveDlg.FilterIndex)
                    {
                        case 1: new DAE(Scene, MdlIndex, AnimIndex).Save(SaveDlg.FileName); break;
                        case 2: new SMD(Scene, MdlIndex, AnimIndex).Save(SaveDlg.FileName); break;
                        case 3: H3D.Save(SaveDlg.FileName, Scene); break;
                    }
                }
            }
        }

        public static void Export(H3D Scene, int Index = -1)
        {
            if (Index != -1)
            {
                //Export one
                using (SaveFileDialog SaveDlg = new SaveFileDialog())
                {
                    SaveDlg.Filter = "Portable Network Graphics|*.png|"	+
									 "GFTexture|*.*;*.pc;*.bin";
                    SaveDlg.FileName = Scene.Textures[Index].Name;

					if (SaveDlg.ShowDialog() == DialogResult.OK)
                    {
						switch (SaveDlg.FilterIndex) {
							case 1:	//PNG
								TextureManager.GetTexture(Index).Save(SaveDlg.FileName);
								break;
							case 2:	//GFTexture
								new GFPackedTexture(Scene, Index).Save(SaveDlg.FileName);
								break;
						}
                    }
                }
            }
            else
            {
                //Export all (or don't export if format can only export a single item)
                using (FolderBrowserDialog FolderDlg = new FolderBrowserDialog())
                {
                    if (FolderDlg.ShowDialog() == DialogResult.OK)
                    {
                        for (int i = 0; i < Scene.Textures.Count; i++)
                        {
                            string FileName = Path.Combine(FolderDlg.SelectedPath, $"{Scene.Textures[i].Name}.png");

                            TextureManager.GetTexture(i).Save(FileName);
                        }
                    }
                }
            }
        }
    }
}
