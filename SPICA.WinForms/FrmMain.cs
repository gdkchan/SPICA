using OpenTK;
using OpenTK.Graphics;

using SPICA.Formats.CtrH3D;
using SPICA.Renderer;
using SPICA.WinForms.Formats;
using SPICA.WinForms.GUI;

using System;
using System.Collections.Specialized;
using System.Drawing;
using System.Windows.Forms;

namespace SPICA.WinForms
{
    public partial class FrmMain : Form
    {
        RenderEngine Renderer;

        private H3D SceneData;

        private Bitmap[] CachedTextures;

        Vector2 InitialRot;
        Vector2 InitialMov;
        Vector2 FinalMov;

        private Vector3 MdlCenter;

        private float CamDist;
        private float Zoom;
        private float Step;

        private Model Model;

        GLControl Viewport;

        private bool IgnoreClicks;

        public FrmMain()
        {
            InitializeComponent();

            Toolkit.Init();

            Viewport = new GLControl(new GraphicsMode(new ColorFormat(32), 24, 8));

            Viewport.VSync = true;
            Viewport.Dock = DockStyle.Fill;
            
            Viewport.MouseDown  += Viewport_MouseDown;
            Viewport.MouseUp    += Viewport_MouseUp;
            Viewport.MouseMove  += Viewport_MouseMove;
            Viewport.MouseWheel += Viewport_MouseWheel;
            Viewport.Paint      += Viewport_Paint;
            Viewport.Resize     += Viewport_Resize;

            MainContainer.Panel1.Controls.Add(Viewport);

            TopMenu.Renderer = new ToolsRenderer(TopMenu.BackColor);
            TopIcons.Renderer = new ToolsRenderer(TopIcons.BackColor);
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            Renderer = new RenderEngine(Viewport.Width, Viewport.Height);

            Renderer.SetBackgroundColor(Color.Gray);
        }

        private void Viewport_MouseDown(object sender, MouseEventArgs e)
        {
            if (!IgnoreClicks)
            {
                switch (e.Button)
                {
                    case MouseButtons.Left: InitialRot = new Vector2(e.X, e.Y); break;
                    case MouseButtons.Right: InitialMov = new Vector2(e.X, e.Y); break;
                }
            }
        }

        private void Viewport_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && !IgnoreClicks)
            {
                float X = (InitialMov.X - e.X) + FinalMov.X;
                float Y = (InitialMov.Y - e.Y) + FinalMov.Y;

                FinalMov = new Vector2(X, Y);
            }
        }

        private void Viewport_MouseMove(object sender, MouseEventArgs e)
        {
            if (!IgnoreClicks)
            {
                if (e.Button == MouseButtons.Left)
                {
                    float RY = (float)(((e.X - InitialRot.X) / Width) * Math.PI);
                    float RX = (float)(((e.Y - InitialRot.Y) / Height) * Math.PI);

                    Model?.Rotate(new Vector3(RX, RY, 0));
                }

                if (e.Button == MouseButtons.Right)
                {
                    float TX = (InitialMov.X - e.X) + FinalMov.X;
                    float TY = (InitialMov.Y - e.Y) + FinalMov.Y;

                    Renderer.TranslateAbs(new Vector3(-TX, TY, Zoom));
                }

                InitialRot = new Vector2(e.X, e.Y);

                Viewport.Invalidate();
            }
        }

        private void Viewport_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                if (e.Delta > 0)
                    Zoom += Step;
                else
                    Zoom -= Step;

                UpdateTranslation();

                Viewport.Invalidate();
            }
        }

        private void Viewport_Paint(object sender, PaintEventArgs e)
        {
            Viewport.MakeCurrent();

            Model?.Animate();

            Renderer.RenderScene();

            Viewport.SwapBuffers();
        }

        private void Viewport_Resize(object sender, EventArgs e)
        {
            Renderer?.UpdateResolution(Viewport.Width, Viewport.Height);

            Viewport.Invalidate();
        }

        //Menu items
        private void MenuOpenFile_Click(object sender, EventArgs e)
        {
            ToolButtonOpen_Click(sender, e);
        }

        //Tool buttons
        private void ToolButtonOpen_Click(object sender, EventArgs e)
        {
            IgnoreClicks = true;

            using (OpenFileDialog OpenDlg = new OpenFileDialog())
            {
                OpenDlg.Filter = "All files|*.*";

                if (OpenDlg.ShowDialog() == DialogResult.OK)
                {
                    SceneData = FormatIdentifier.IdentifyAndOpen(OpenDlg.FileName);

                    if (SceneData == null)
                    {
                        MessageBox.Show(
                            "Unsupported file format!",
                            "Can't open file!",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);

                        return;
                    }

                    //Bind Lists to H3D contents
                    CacheTextures();

                    SceneData.Textures.CollectionChanged +=
                        delegate (object _sender, NotifyCollectionChangedEventArgs _e)
                        {
                            CacheTextures();
                        };

                    ModelsList.Bind(SceneData.Models);
                    TexturesList.Bind(SceneData.Textures);
                    SklAnimsList.Bind(SceneData.SkeletalAnimations);

                    //Setup Renderer
                    Renderer.ClearModels();

                    Model = Renderer.AddModel(SceneData);

                    Tuple<Vector3, float> CenterMax = Model.GetCenterMaxXY();

                    MdlCenter = -CenterMax.Item1;
                    CamDist = CenterMax.Item2;

                    Renderer.ClearLights();

                    Renderer.AddLight(new Light
                    {
                        Position = new Vector3(
                            0, 
                            -MdlCenter.Y, 
                            -(MdlCenter.Z - CamDist * 2)),
                        Ambient = new Color4(0f, 0f, 0f, 0f),
                        Diffuse = new Color4(0.5f, 0.5f, 0.5f, 1f),
                        Specular = new Color4(0.8f, 0.8f, 0.8f, 1f)
                    });

                    if (SceneData.Models.Count > 0) ModelsList.Select(0);

                    ResetView();
                }
            }

            //Allow app to process click from the Open dialog that goes into the Viewport
            //This avoid the model from moving after opening a file on the dialog
            //(Note: The problem only happens if the dialog is on top of the Viewport)
            Application.DoEvents();

            IgnoreClicks = false;

            Viewport.Invalidate();
        }

        private void ModelsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ModelsList.SelectedIndex != -1 && Model != null)
            {
                Model.CurrentModelIndex = ModelsList.SelectedIndex;

                ResetView();
            }
        }

        private void ResetView()
        {
            Model.ResetTransform();

            Model.TranslateAbs(MdlCenter);
            Zoom = MdlCenter.Z - CamDist * 2;
            Step = CamDist * 0.05f;

            InitialRot = Vector2.Zero;
            InitialMov = Vector2.Zero;
            FinalMov = Vector2.Zero;

            Renderer.ResetView();

            UpdateTranslation();

            Viewport.Invalidate();
        }

        private void UpdateTranslation()
        {
            Renderer.TranslateAbs(new Vector3(-FinalMov.X, FinalMov.Y, Zoom));
        }

        private void TexturesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int Index = TexturesList.SelectedIndex;

            if (Index != -1)
            {
                TexturePreview.Image = CachedTextures[Index];
                TextureInfo.Text = string.Format("{0} {1}x{2} {3}",
                    SceneData.Textures[Index].MipmapSize,
                    SceneData.Textures[Index].Width,
                    SceneData.Textures[Index].Height,
                    SceneData.Textures[Index].Format);
            }
            else
            {
                TexturePreview.Image = null;
                TextureInfo.Text = string.Empty;
            }
        }

        private void CacheTextures()
        {
            if (CachedTextures != null)
            {
                foreach (Bitmap Img in CachedTextures) Img.Dispose();
            }

            //Cache the textures converted to bitmap to avoid making the conversion over and over again
            //The cache needs to be updated when the original collection changes
            CachedTextures = new Bitmap[SceneData.Textures.Count];

            for (int Index = 0; Index < CachedTextures.Length; Index++)
            {
                CachedTextures[Index] = SceneData.Textures[Index].ToBitmap();
            }
        }
    }
}
