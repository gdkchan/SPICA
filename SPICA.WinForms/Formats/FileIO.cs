using SPICA.Formats;
using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.Generic.COLLADA;
using SPICA.Formats.Generic.StudioMdl;
using SPICA.Formats.Generic.XML;
using SPICA.Renderer;

using System.Windows.Forms;

namespace SPICA.WinForms.Formats
{
    class FileIO
    {
        public static H3D Merge(string[] FileNames, RenderEngine Renderer, H3D Scene = null)
        {
            if (Scene == null)
            {
                Renderer.DeleteAll();

                Scene = new H3D();
            }

            int OpenFiles = 0;

            foreach (string FileName in FileNames)
            {
                PatriciaList<H3DBone> Skeleton = null;

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
                SaveDlg.Filter = "COLLADA 1.4.1|*.dae|Valve StudioMdl|*.smd|H3D as XML|*.xml";
                SaveDlg.FileName = "Model";

                if (SaveDlg.ShowDialog() == DialogResult.OK)
                {
                    int MdlIndex  = State.ModelIndex;
                    int AnimIndex = State.SklAnimIndex;

                    switch (SaveDlg.FilterIndex)
                    {
                        case 1: new DAE(Scene, MdlIndex, AnimIndex).Save(SaveDlg.FileName); break;
                        case 2: new SMD(Scene, MdlIndex, AnimIndex).Save(SaveDlg.FileName); break;
                        case 3: new H3DXML(Scene).Save(SaveDlg.FileName); break;
                    }
                }
            }
        }
    }
}
