using OpenTK;
using OpenTK.Graphics;

using SPICA.Formats;
using SPICA.Formats.CtrH3D;
using SPICA.Renderer;
using SPICA.WinForms.Formats;
using SPICA.WinForms.GUI;
using SPICA.WinForms.GUI.Animation;
using SPICA.WinForms.GUI.Viewport;
using SPICA.WinForms.Properties;

using System;
using System.Drawing;
using System.Windows.Forms;

namespace SPICA.WinForms
{
    public partial class FrmMain : Form
    {
        #region Declarations
        private AllAnimations Animations;
        private Vector2       InitialMov;
        private Matrix4       MdlCenter;
        private Matrix4       MdlTrans;
        private RenderEngine  Renderer;
        private H3D           Scene;
        private GridLines     UIGrid;
        private AxisLines     UIAxis;
        private GLControl     Viewport;

        private float Dimension;

        private bool IgnoreClicks;
        #endregion

        #region Initialization/Termination
        public FrmMain()
        {
            //We need to add the control here cause we need to call the constructor with Graphics Mode
            //This enables the higher precision Depth Buffer and a Stencil Buffer
            Viewport = new GLControl(new GraphicsMode(new ColorFormat(32), 24, 8),
                3, 3, GraphicsContextFlags.ForwardCompatible);

            Viewport.BackColor = Color.Gray;
            Viewport.Dock      = DockStyle.Fill;
            Viewport.Location  = Point.Empty;
            Viewport.Name      = "Viewport";
            Viewport.Size      = new Size(256, 256);
            Viewport.TabIndex  = 0;
            Viewport.VSync     = true;

            Viewport.Load       += Viewport_Load;
            Viewport.Paint      += Viewport_Paint;
            Viewport.MouseDown  += Viewport_MouseDown;
            Viewport.MouseMove  += Viewport_MouseMove;
            Viewport.MouseWheel += Viewport_MouseWheel;
            Viewport.Resize     += Viewport_Resize;

            InitializeComponent();

            MainContainer.Panel1.Controls.Add(Viewport);

            Animations = new AllAnimations();

            TopMenu.Renderer   = new ToolsRenderer(TopMenu.BackColor);
            TopIcons.Renderer  = new ToolsRenderer(TopIcons.BackColor);
            SideIcons.Renderer = new ToolsRenderer(SideIcons.BackColor);
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

            Dimension = 100f;

            UIGrid = new GridLines();
            UIAxis = new AxisLines();

            MdlCenter = Matrix4.Identity;
            MdlTrans  = Matrix4.CreateTranslation(0, 0, -Dimension);

            UpdateTransforms();

            /*
             * Load settings (needs to be done here cause some of then changes values related to the renderer,
             * and those needs that OpenTK have been properly initialized).
             */
            SettingsManager.BindBool("RenderShowGrid", ToggleGrid, MenuShowGrid, ToolButtonShowGrid);
            SettingsManager.BindBool("RenderShowAxis", ToggleAxis, MenuShowAxis, ToolButtonShowAxis);
            SettingsManager.BindBool("UIShowSideMenu", ToggleSide, MenuShowSide, ToolButtonShowSide);
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
                Vector3 Translation = MdlTrans.ExtractTranslation();

                MdlTrans.Row3.Xyz = Vector3.Zero;

                if ((e.Button & MouseButtons.Left) != 0)
                {
                    float Y = (float)(((e.X - InitialMov.X) / Width)  * Math.PI);
                    float X = (float)(((e.Y - InitialMov.Y) / Height) * Math.PI);

                    MdlTrans *= Matrix4.CreateRotationX(X) * Matrix4.CreateRotationY(Y);
                }

                if ((e.Button & MouseButtons.Right) != 0)
                {
                    float X = (InitialMov.X - e.X) * Dimension * 0.005f;
                    float Y = (InitialMov.Y - e.Y) * Dimension * 0.005f;

                    MdlTrans.Row3.Xyz += new Vector3(-X, Y, 0);
                }

                MdlTrans.Row3.Xyz += Translation;

                InitialMov = new Vector2(e.X, e.Y);

                UpdateTransforms();
            }
        }

        private void Viewport_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                float Step = e.Delta > 0
                    ?  Dimension * 0.025f
                    : -Dimension * 0.025f;

                MdlTrans *= Matrix4.CreateTranslation(0, 0, Step);

                UpdateTransforms();
            }
        }

        private void Viewport_Paint(object sender, PaintEventArgs e)
        {
            Renderer.Clear();

            UIGrid.Render(Renderer.Set3DColorShader());

            if (ModelsList.SelectedIndex != -1 && ModelsList.SelectedIndex < Renderer.Models.Count)
            {
                Renderer.Render(ModelsList.SelectedIndex);
            }

            UIAxis.Render(Renderer.Set3DColorShader());

            Viewport.SwapBuffers();
        }

        private void Viewport_Resize(object sender, EventArgs e)
        {
            Renderer?.Resize(Viewport.Width, Viewport.Height);

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
        #endregion

        #region Tool buttons and Menus
        private void ToolButtonOpen_Click(object sender, EventArgs e)
        {
            Open(false);
        }

        private void ToolButtonMerge_Click(object sender, EventArgs e)
        {
            Open(Scene != null);
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

        private void ToggleSide()
        {
            MainContainer.Panel2Collapsed = !ToolButtonShowSide.Checked;
        }

        private void ToolButtonExport_Click(object sender, EventArgs e)
        {
            FileIO.Export(Scene, TexturesList.SelectedIndex);
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
                    Scene = MergeMode
                        ? FileIO.Merge(OpenDlg.FileNames, Renderer, Scene)
                        : FileIO.Merge(OpenDlg.FileNames, Renderer);

                    if (!MergeMode)
                    {
                        TextureManager.Textures = Scene.Textures;

                        MdlCenter = Matrix4.Identity;

                        Animations.Scene = Scene;

                        ModelsList.Bind(Scene.Models);
                        TexturesList.Bind(Scene.Textures);
                        SklAnimsList.Bind(Scene.SkeletalAnimations);
                        MatAnimsList.Bind(Scene.MaterialAnimations);

                        if (Scene.Models.Count > 0)
                        {
                            ModelsList.Select(0);
                        }

                        Animator.Enabled     = false;
                        AnimSeekBar.Value    = 0;
                        AnimSeekBar.Maximum  = 0;
                        LblAnimSpeed.Text    = string.Empty;
                        LblAnimLoopMode.Text = string.Empty;

                        InitialMov = Vector2.Zero;

                        float ZDist = Dimension < RenderEngine.ClipDistance ? -Dimension : 0;

                        MdlTrans = Matrix4.CreateTranslation(0, 0, ZDist);

                        UpdateTransforms();
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
            FileIO.Save(Scene, new SceneState
            {
                ModelIndex   = ModelsList.SelectedIndex,
                SklAnimIndex = SklAnimsList.SelectedIndex,
                MatAnimIndex = MatAnimsList.SelectedIndex
            });
        }

        private void UpdateTransforms()
        {
            if (ModelsList.SelectedIndex != -1)
            {
                Renderer.Models[ModelsList.SelectedIndex].Transform = MdlCenter * MdlTrans;
            }

            UIGrid.Transform = MdlCenter * MdlTrans;
            UIAxis.Transform = MdlCenter.ClearTranslation() * MdlTrans;

            UpdateViewport();
        }
        #endregion

        #region Side menu events
        private void ModelsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ModelsList.SelectedIndex != -1)
            {
                Animations.Model = Renderer.Models[ModelsList.SelectedIndex];

                Tuple<Vector3, Vector3> CD = Renderer.Models[ModelsList.SelectedIndex].GetCenterDim();

                Dimension = 0;

                Dimension = Math.Max(Dimension, Math.Abs(CD.Item2.X));
                Dimension = Math.Max(Dimension, Math.Abs(CD.Item2.Y));
                Dimension = Math.Max(Dimension, Math.Abs(CD.Item2.Z));

                Dimension *= 2;

                Renderer.Lights.Clear();

                Renderer.Lights.Add(new Light
                {
                    Position = new Vector3(0, CD.Item1.Y, Dimension),
                    Ambient  = new Color4(0.0f, 0.0f, 0.0f, 1.0f),
                    Diffuse  = new Color4(0.8f, 0.8f, 0.8f, 1.0f),
                    Specular = new Color4(0.2f, 0.2f, 0.2f, 1.0f),
                    Enabled  = true
                });

                Renderer.Models[ModelsList.SelectedIndex].UpdateUniforms();

                MdlCenter = Matrix4.CreateTranslation(-CD.Item1);
            }

            UpdateViewport();
        }

        private void TexturesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int Index = TexturesList.SelectedIndex;

            if (Index != -1)
            {
                TexturePreview.Image = TextureManager.GetTexture(Index);

                TextureInfo.Text = string.Format("{0} {1}x{2} {3}",
                    Scene.Textures[Index].MipmapSize,
                    Scene.Textures[Index].Width,
                    Scene.Textures[Index].Height,
                    Scene.Textures[Index].Format);
            }
            else
            {
                TexturePreview.Image = null;

                TextureInfo.Text = string.Empty;
            }
        }
        #endregion

        #region Animation related + playback controls
        private void Animator_Tick(object sender, EventArgs e)
        {
            Animations.AdvanceFrame();

            UpdateSeekBar();

            Viewport.Invalidate();
        }

        private void SetAnimation(int Index, AnimationType Type)
        {
            if (Index != -1)
                Animations.SetAnimation(Index, Type);
            else
                Animations.Reset();

            AnimSeekBar.Maximum = Animations.FramesCount;

            UpdateSeekBar();
            UpdateAnimLbls();
        }

        private void UpdateAnimLbls()
        {
            LblAnimSpeed.Text = $"{Math.Abs(Animations.Step).ToString("N2")}x";

            LblAnimLoopMode.Text = Animations.IsLooping ? "LOOP" : "1 GO";
        }

        private void UpdateSeekBar()
        {
            AnimSeekBar.Value = Animations.Frame;
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

        private void SklAnimsList_Selected(object sender, EventArgs e)
        {
            SetAnimation(SklAnimsList.SelectedIndex, AnimationType.Skeletal);
        }

        private void MatAnimsList_Selected(object sender, EventArgs e)
        {
            SetAnimation(MatAnimsList.SelectedIndex, AnimationType.Material);
        }

        private void AnimButtonPlayBackward_Click(object sender, EventArgs e)
        {
            Animations.Play(-Math.Abs(Animations.Step)); EnableAnimator();
        }

        private void AnimButtonPlayForward_Click(object sender, EventArgs e)
        {
            Animations.Play(Math.Abs(Animations.Step)); EnableAnimator();
        }

        private void AnimButtonPause_Click(object sender, EventArgs e)
        {
            Animations.Pause(); DisableAnimator();
        }

        private void AnimButtonStop_Click(object sender, EventArgs e)
        {
            DisableAnimator();

            AnimSeekBar.Value = 0;

            Animations.Stop();
        }

        private void AnimButtonSlowDown_Click(object sender, EventArgs e)
        {
            Animations.SlowDown(); UpdateAnimLbls();
        }

        private void AnimButtonSpeedUp_Click(object sender, EventArgs e)
        {
            Animations.SpeedUp(); UpdateAnimLbls();
        }

        private void AnimButtonPrev_Click(object sender, EventArgs e)
        {
            switch (Animations.Type)
            {
                case AnimationType.Skeletal: SklAnimsList.SelectUp(); break;
                case AnimationType.Material: MatAnimsList.SelectUp(); break;
            }
        }

        private void AnimButtonNext_Click(object sender, EventArgs e)
        {
            switch (Animations.Type)
            {
                case AnimationType.Skeletal: SklAnimsList.SelectDown(); break;
                case AnimationType.Material: MatAnimsList.SelectDown(); break;
            }
        }

        private void AnimSeekBar_Seek(object sender, EventArgs e)
        {
            Animations.Pause();

            Animations.Frame = AnimSeekBar.Value;

            UpdateViewport();
        }

        private void AnimSeekBar_MouseUp(object sender, MouseEventArgs e)
        {
            Animations.Play();
        }
        #endregion
    }
}
