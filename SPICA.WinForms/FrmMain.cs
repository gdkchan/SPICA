using OpenTK;
using OpenTK.Graphics;

using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Animation;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.Generic.XML;
using SPICA.Formats.Generic.COLLADA;
using SPICA.Formats.Generic.StudioMdl;
using SPICA.Renderer;
using SPICA.Renderer.Animation;
using SPICA.WinForms.Formats;
using SPICA.WinForms.GUI;
using SPICA.WinForms.Properties;
using SPICA.WinForms.RenderExtensions;

using System;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SPICA.WinForms
{
    public partial class FrmMain : Form
    {
        #region Declarations
        private RenderEngine Renderer;
        private GLControl Viewport;
        private GridLines UIGrid;
        private AxisLines UIAxis;
        private Vector2 InitialMov;
        private Vector2 CurrentRot;
        private Vector2 CurrentMov;
        private Model Model;
        private H3D SceneData;
        private Bitmap[] CachedTextures;

        private float Dimension;

        private float Zoom;

        private bool IgnoreClicks;

        private enum AnimType
        {
            None,
            Skeletal,
            Material,
            Visibility
        }

        private AnimType CurrAnimType;
        #endregion

        #region Initialization/Termination
        public FrmMain()
        {
            //We need to add the control here cause we need to call the constructor with Graphics Mode
            //This enables the higher precision Depth Buffer and a Stencil Buffer
            Viewport = new GLControl(new GraphicsMode(new ColorFormat(32), 24, 8));

            Viewport.BackColor = Color.Gray;
            Viewport.Dock = DockStyle.Fill;
            Viewport.Location = Point.Empty;
            Viewport.Name = "Viewport";
            Viewport.Size = new Size(256, 256);
            Viewport.TabIndex = 0;
            Viewport.VSync = true;

            Viewport.Load       += Viewport_Load;
            Viewport.Paint      += Viewport_Paint;
            Viewport.MouseDown  += Viewport_MouseDown;
            Viewport.MouseMove  += Viewport_MouseMove;
            Viewport.MouseWheel += Viewport_MouseWheel;
            Viewport.Resize     += Viewport_Resize;

            InitializeComponent();

            MainContainer.Panel1.Controls.Add(Viewport);

            TopMenu.Renderer      = new ToolsRenderer(TopMenu.BackColor);
            TopIcons.Renderer     = new ToolsRenderer(TopIcons.BackColor);
            TexturesMenu.Renderer = new ToolsRenderer(TexturesMenu.BackColor);
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.Save();
        }
        #endregion

        #region Viewport events
        private void Viewport_Load(object sender, EventArgs e)
        {
            //Note: Setting up OpenGL stuff only works after the control has loaded on the Form
            Viewport.MakeCurrent();

            Renderer = new RenderEngine(Viewport.Width, Viewport.Height);

            Renderer.SetBackgroundColor(Color.Gray);

            UIGrid = new GridLines();
            UIAxis = new AxisLines();

            Renderer.BeforeDraw += UIGrid.Render;
            Renderer.AfterDraw += UIAxis.Render;

            Dimension =  100f;
            Zoom      = -100f;

            UpdateViewport();

            /*
             * Load settings (needs to be done here cause some of then changes values related to the renderer,
             * and those needs that OpenTK have been properly initialized.
             */
            SettingsManager.BindBool("RenderShowGrid", ToggleGrid, MenuShowGrid, ToolButtonShowGrid);
            SettingsManager.BindBool("RenderShowAxis", ToggleAxis, MenuShowAxis, ToolButtonShowAxis);
        }

        private void Viewport_MouseDown(object sender, MouseEventArgs e)
        {
            if (!IgnoreClicks && (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right))
            {
                InitialMov = new Vector2(e.X, e.Y);
            }
        }

        private void Viewport_MouseMove(object sender, MouseEventArgs e)
        {
            if (!IgnoreClicks)
            {
                if (e.Button == MouseButtons.Left)
                {
                    CurrentRot.Y = (float)(((e.X - InitialMov.X) / Width) * Math.PI);
                    CurrentRot.X = (float)(((e.Y - InitialMov.Y) / Height) * Math.PI);

                    UpdateTransforms();
                }
                else if (e.Button == MouseButtons.Right)
                {
                    CurrentMov.X = (InitialMov.X - e.X) * Dimension * 0.005f;
                    CurrentMov.Y = (InitialMov.Y - e.Y) * Dimension * 0.005f;

                    UpdateTransforms();
                }

                InitialMov = new Vector2(e.X, e.Y);
            }
        }

        private void Viewport_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                float Step = Dimension * 0.025f;

                if (e.Delta > 0)
                    Zoom += Step;
                else
                    Zoom -= Step;

                UpdateTransforms();
            }
        }

        private void Viewport_Paint(object sender, PaintEventArgs e)
        {
            Renderer.RenderScene();

            Viewport.SwapBuffers();
        }

        private void Viewport_Resize(object sender, EventArgs e)
        {
            Renderer?.UpdateResolution(Viewport.Width, Viewport.Height);

            UpdateViewport();
        }
        #endregion

        #region Menu items
        private void MenuOpenFile_Click(object sender, EventArgs e)
        {
            ToolButtonOpen_Click(sender, e);
        }

        private void MenuMergeFiles_Click(object sender, EventArgs e)
        {
            ToolButtonMerge_Click(sender, e);
        }

        private void MenuBatchExport_Click(object sender, EventArgs e)
        {
            new FrmExport().Show();
        }

        private void MenuTexExport_Click(object sender, EventArgs e)
        {
            if (TexturesList.SelectedIndex != -1)
            {
                using (SaveFileDialog SaveDlg = new SaveFileDialog())
                {
                    SaveDlg.Filter = "Portable Network Graphics|*.png";
                    SaveDlg.FileName = $"{TexturesList[TexturesList.SelectedIndex]}.png";

                    if (SaveDlg.ShowDialog() == DialogResult.OK)
                    {
                        CachedTextures[TexturesList.SelectedIndex].Save(SaveDlg.FileName);
                    }
                }
            }
        }

        private void MenuTexExportAll_Click(object sender, EventArgs e)
        {
            if (TexturesList.Count > 0)
            {
                using (FolderBrowserDialog FolderDlg = new FolderBrowserDialog())
                {
                    if (FolderDlg.ShowDialog() == DialogResult.OK)
                    {
                        int Index = 0;

                        foreach (Bitmap Img in CachedTextures)
                        {
                            Img.Save(Path.Combine(FolderDlg.SelectedPath, $"{TexturesList[Index++]}.png"));
                        }
                    }
                }
            }
        }
        #endregion

        #region Tool buttons
        private void ToolButtonOpen_Click(object sender, EventArgs e)
        {
            Open(false);
        }

        private void ToolButtonMerge_Click(object sender, EventArgs e)
        {
            Open(true);
        }

        private void ToolButtonSave_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void ToggleGrid()
        {
            UIGrid.Visible = ToolButtonShowGrid.Checked; UpdateViewport();
        }

        private void ToggleAxis()
        {
            UIAxis.Visible = ToolButtonShowAxis.Checked; UpdateViewport();
        }

        private void Open(bool MergeMode)
        {
            IgnoreClicks = true;

            using (OpenFileDialog OpenDlg = new OpenFileDialog())
            {
                OpenDlg.Filter = "All files|*.*";
                OpenDlg.Multiselect = true;

                if (OpenDlg.ShowDialog() == DialogResult.OK && OpenDlg.FileNames.Length > 0)
                {
                    if (!MergeMode) SceneData = null;

                    for (int Index = 0; Index < OpenDlg.FileNames.Length; Index++)
                    {
                        //We need to ensure that the Skeleton we will use is the one from the lastest loaded Model
                        //Otherwise the wrong Skeleton (from the last loaded model) may be incorrectly used
                        PatriciaList<H3DBone> Skeleton = null;

                        if (SceneData != null && SceneData.Models.Count > 0)
                        {
                            Skeleton = SceneData.Models[0].Skeleton;
                        }

                        //The Skeleton is used to convert animations to the H3D format that the Renderer understands
                        //On H3D data the Skeleton is not necessary and is not used, because it doesn't need conversions
                        H3D Data = FormatIdentifier.IdentifyAndOpen(OpenDlg.FileNames[Index], Skeleton);

                        if (Data != null)
                        {
                            if ((MergeMode && SceneData != null) || Index > 0)
                                SceneData.Merge(Data);
                            else
                                SceneData = Data;
                        }
                    }

                    if (SceneData != null)
                    {
                        SceneData.Textures.CollectionChanged += TexturesList_CollectionChanged;

                        ModelsList.Bind(SceneData.Models);
                        TexturesList.Bind(SceneData.Textures);
                        SklAnimsList.Bind(SceneData.SkeletalAnimations);
                        MatAnimsList.Bind(SceneData.MaterialAnimations);

                        Renderer.ClearModels();

                        Model = Renderer.AddModel(SceneData);

                        if (SceneData.Models.Count > 0) ModelsList.Select(0);

                        Animator.Enabled     = false;
                        AnimSeekBar.Value    = 0;
                        AnimSeekBar.Maximum  = 0;
                        LblAnimSpeed.Text    = string.Empty;
                        LblAnimLoopMode.Text = string.Empty;
                        CurrAnimType         = AnimType.None;

                        CacheTextures();
                        ResetView();
                    }
                    else
                    {
                        MessageBox.Show(
                            "Unsupported file format!",
                            "Can't open file!",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                    }
                }
            }

            //Allow app to process click from the Open dialog that goes into the Viewport
            //This avoid the model from moving after opening a file on the dialog
            //(Note: The problem only happens if the dialog is on top of the Viewport)
            Application.DoEvents();

            IgnoreClicks = false;

            UpdateViewport();
        }

        private void Save()
        {
            if (SceneData == null)
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
                    int MdlIndex = ModelsList.SelectedIndex;
                    int AnimIndex = SklAnimsList.SelectedIndex;

                    switch (SaveDlg.FilterIndex)
                    {
                        case 1: new DAE(SceneData, MdlIndex, AnimIndex).Save(SaveDlg.FileName); break;
                        case 2: new SMD(SceneData, MdlIndex, AnimIndex).Save(SaveDlg.FileName); break;
                        case 3: new H3DXML(SceneData).Save(SaveDlg.FileName); break;
                    }
                }
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

        private void ResetView()
        {
            InitialMov = Vector2.Zero;
            CurrentRot = Vector2.Zero;
            CurrentMov = Vector2.Zero;

            Model.ResetTransform();
            UIGrid.ResetTransform();
            UIAxis.ResetTransform();

            Tuple<Vector3, Vector3> CenterDim = Model.GetCenterDim(ModelsList.SelectedIndex);

            Vector3 Center = CenterDim.Item1;
            Vector3 Dim    = CenterDim.Item2;

            Dimension = Math.Max(Math.Max(Math.Abs(Dim.Y), Math.Abs(Dim.Z)) * 2, 100f);

            Renderer.ClearLights();

            Renderer.AddLight(new Light
            {
                Position = new Vector3(0, Center.Y, Dimension),
                Ambient  = new Color4(0.0f, 0.0f, 0.0f, 1f),
                Diffuse  = new Color4(0.8f, 0.8f, 0.8f, 1f),
                Specular = new Color4(0.5f, 0.5f, 0.5f, 1f)
            });

            Model.TranslateAbs(-Center);
            UIGrid.TranslateAbs(-Center);

            Zoom = (Dimension < RenderEngine.ClipDistance ? -Dimension : 0);

            Renderer.ResetView();

            UpdateTransforms();
        }

        private void UpdateTransforms()
        {
            Vector3 Translation = new Vector3(-CurrentMov.X, CurrentMov.Y, 0);
            Vector3 Rotation    = new Vector3( CurrentRot.X, CurrentRot.Y, 0);

            Model?.Translate(Translation);
            UIGrid.Translate(Translation);

            Model?.Rotate(Rotation);
            UIGrid.Rotate(Rotation);

            UIAxis.Rotate(Rotation);

            CurrentMov = Vector2.Zero;
            CurrentRot = Vector2.Zero;

            Renderer.TranslateAbs(new Vector3(0, 0, Zoom));

            UpdateViewport();
        }
        #endregion

        #region Side menu events
        private void ModelsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ModelsList.SelectedIndex != -1 && Model != null)
            {
                Model.CurrentModelIndex = ModelsList.SelectedIndex;

                ResetView();
            }
        }

        private void TexturesList_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) TexturesMenu.Show(Cursor.Position);
        }

        private void TexturesList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CacheTextures();
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
        #endregion

        #region Animation timing
        private void Animator_Tick(object sender, EventArgs e)
        {
            Model.Animate();

            UpdateSeekBar();

            Viewport.Invalidate();
        }

        private void UpdateSeekBar()
        {
            switch (CurrAnimType)
            {
                case AnimType.Skeletal: AnimSeekBar.Value = Model.SkeletalAnimation.Frame; break;
                case AnimType.Material: AnimSeekBar.Value = Model.MaterialAnimation.Frame; break;
            }
        }

        private void EnableAnimator()
        {
            Animator.Enabled = true; UpdateAnimLbls();
        }

        private void DisableAnimator()
        {
            Animator.Enabled = false; Viewport.Invalidate();
        }

        private void UpdateViewport()
        {
            if (!Animator.Enabled) Viewport.Invalidate();
        }
        #endregion

        #region Animation related + playback controls
        private void SklAnimsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Here we automatically select materials with a matching name for convenience
            //If an material anim with matching name isn't found, then it is disabled
            if (SklAnimsList.SelectedIndex != -1)
            {
                H3DAnimation SklAnim = SceneData.SkeletalAnimations[SklAnimsList.SelectedIndex];

                Model.SkeletalAnimation.SetAnimation(SklAnim);

                MatAnimsList.SelectedIndex = FindAndSetAnim(
                    SceneData.MaterialAnimations,
                    Model.MaterialAnimation,
                    SklAnim.Name);

                SetAnimationControls(SklAnim, AnimType.Skeletal);
            }
        }

        private int FindAndSetAnim(PatriciaList<H3DAnimation> Src, AnimControl Tgt, string Name)
        {
            int AnimIndex = Src.FindIndex(Name);

            Tgt.SetAnimation(AnimIndex != -1 ? Src[AnimIndex] : null);

            return AnimIndex;
        }

        private void MatAnimsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MatAnimsList.SelectedIndex != -1)
            {
                SklAnimsList.SelectedIndex = -1;

                H3DAnimation MatAnim = SceneData.MaterialAnimations[MatAnimsList.SelectedIndex];

                Model.SkeletalAnimation.SetAnimation(null);
                Model.MaterialAnimation.SetAnimation(MatAnim);

                SetAnimationControls(MatAnim, AnimType.Material);
            }
        }

        private void SetAnimationControls(H3DAnimation Anim, AnimType Type)
        {
            AnimSeekBar.Maximum = Anim.FramesCount;

            CurrAnimType = Type;

            UpdateSeekBar();
            UpdateAnimLbls();
        }

        private void AnimButtonPlayBackward_Click(object sender, EventArgs e)
        {
            if (Model != null)
            {
                Model.SkeletalAnimation.Play(-1);
                Model.MaterialAnimation.Play(-1);

                EnableAnimator();
            }
        }

        private void AnimButtonPlayForward_Click(object sender, EventArgs e)
        {
            if (Model != null)
            {
                Model.SkeletalAnimation.Play(1);
                Model.MaterialAnimation.Play(1);

                EnableAnimator();
            }
        }

        private void AnimButtonPause_Click(object sender, EventArgs e)
        {
            if (Model != null)
            {
                Model.SkeletalAnimation.Pause();
                Model.MaterialAnimation.Pause();

                DisableAnimator();
            }
        }

        private void AnimButtonStop_Click(object sender, EventArgs e)
        {
            if (Model != null)
            {
                Model.SkeletalAnimation.Stop();
                Model.MaterialAnimation.Stop();

                DisableAnimator();

                AnimSeekBar.Value = 0;

                Model.UpdateAnimationTransforms();
            }
        }

        private void AnimButtonSlowDown_Click(object sender, EventArgs e)
        {
            if (Model != null)
            {
                Model.SkeletalAnimation.SlowDown();
                Model.MaterialAnimation.SlowDown();

                UpdateAnimLbls();
            }
        }

        private void AnimButtonSpeedUp_Click(object sender, EventArgs e)
        {
            if (Model != null)
            {
                Model.SkeletalAnimation.SpeedUp();
                Model.MaterialAnimation.SpeedUp();

                UpdateAnimLbls();
            }
        }

        private void AnimButtonPrev_Click(object sender, EventArgs e)
        {
            switch (CurrAnimType)
            {
                case AnimType.Skeletal: SklAnimsList.SelectUp(); break;
                case AnimType.Material: MatAnimsList.SelectUp(); break;
            }
        }

        private void AnimButtonNext_Click(object sender, EventArgs e)
        {
            switch (CurrAnimType)
            {
                case AnimType.Skeletal: SklAnimsList.SelectDown(); break;
                case AnimType.Material: MatAnimsList.SelectDown(); break;
            }
        }

        private void AnimSeekBar_Seek(object sender, EventArgs e)
        {
            if (Model != null)
            {
                Model.SkeletalAnimation.Pause();
                Model.MaterialAnimation.Pause();

                Model.SkeletalAnimation.Frame = AnimSeekBar.Value;
                Model.MaterialAnimation.Frame = AnimSeekBar.Value;

                Model.UpdateAnimationTransforms();

                UpdateViewport();
            }
        }

        private void AnimSeekBar_MouseUp(object sender, MouseEventArgs e)
        {
            Model?.SkeletalAnimation.Play();
            Model?.MaterialAnimation.Play();
        }

        private void UpdateAnimLbls()
        {
            LblAnimSpeed.Text = $"{Math.Abs(Model.SkeletalAnimation.Step).ToString("N2")}x";

            bool Loop = false;

            switch (CurrAnimType)
            {
                case AnimType.Skeletal: Loop = Model.SkeletalAnimation.IsLooping; break;
                case AnimType.Material: Loop = Model.MaterialAnimation.IsLooping; break;
            }

            LblAnimLoopMode.Text = Loop ? "LOOP" : "1 GO";
        }
        #endregion

        private void MenuNormalMapTangent_Click(object sender, EventArgs e)
        {
            Renderer.ObjectSpaceNormalMap = false;
            UpdateViewport();
        }

        private void MenuNormalMapObject_Click(object sender, EventArgs e)
        {
            Renderer.ObjectSpaceNormalMap = true;
            UpdateViewport();
        }
    }
}
