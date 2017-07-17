using OpenTK;
using OpenTK.Graphics;

using SPICA.Formats;
using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Animation;
using SPICA.Rendering;
using SPICA.Rendering.Animation;
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
        private GLControl Viewport;
        private GridLines UIGrid;
        private AxisLines UIAxis;

        private Vector2 InitialMov;
        private Vector3 MdlCenter;
        private Matrix4 Transform;

        private Renderer Renderer;
        private Shader   Shader;

        private H3DAnimation SklAnim;
        private H3DAnimation MatAnim;

        private AnimationControl AnimCtrl;

        private H3D Scene;

        private float Dimension;

        private bool IgnoreClicks;

        private const float MinDim = 100;
        #endregion

        #region Initialization/Termination
        public FrmMain()
        {
            //We need to add the control here cause we need to call the constructor with Graphics Mode.
            //This enables the higher precision Depth Buffer and a Stencil Buffer.
            Viewport = new GLControl(new GraphicsMode(32, 24, 8), 3, 3, GraphicsContextFlags.ForwardCompatible);

            Viewport.Dock  = DockStyle.Fill;
            Viewport.Name  = "Viewport";
            Viewport.VSync = true;

            Viewport.Load       += Viewport_Load;
            Viewport.Paint      += Viewport_Paint;
            Viewport.MouseDown  += Viewport_MouseDown;
            Viewport.MouseMove  += Viewport_MouseMove;
            Viewport.MouseWheel += Viewport_MouseWheel;
            Viewport.Resize     += Viewport_Resize;

            InitializeComponent();

            MainContainer.Panel1.Controls.Add(Viewport);

            AnimCtrl = new AnimationControl();

            TopMenu.Renderer   = new ToolsRenderer(TopMenu.BackColor);
            TopIcons.Renderer  = new ToolsRenderer(TopIcons.BackColor);
            SideIcons.Renderer = new ToolsRenderer(SideIcons.BackColor);
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Renderer.Dispose();

            Shader.Dispose();

            Settings.Default.Save();
        }
        #endregion

        #region Viewport events
        private void Viewport_Load(object sender, EventArgs e)
        {
            //Note: Setting up OpenGL stuff only works after the control has loaded on the Form.
            Viewport.MakeCurrent();

            Renderer = new Renderer(Viewport.Width, Viewport.Height);

            Renderer.SetBackgroundColor(Color.Gray);

            Shader = new Shader();

            UIGrid = new GridLines(Renderer, Shader);
            UIAxis = new AxisLines(Renderer, Shader);

            ResetTransforms();
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
                Vector3 Translation = Transform.Row3.Xyz;

                Transform.Row3.Xyz = Vector3.Zero;

                if ((e.Button & MouseButtons.Left) != 0)
                {
                    float X = (float)(((e.X - InitialMov.X) / Width)  * Math.PI);
                    float Y = (float)(((e.Y - InitialMov.Y) / Height) * Math.PI);

                    Transform *= Matrix4.CreateRotationX(Y) * Matrix4.CreateRotationY(X);
                }

                if ((e.Button & MouseButtons.Right) != 0)
                {
                    float X = (InitialMov.X - e.X) * Dimension * 0.005f;
                    float Y = (InitialMov.Y - e.Y) * Dimension * 0.005f;

                    Transform.Row3.Xyz += new Vector3(-X, Y, 0);
                }

                Transform.Row3.Xyz += Translation;

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

                Transform *= Matrix4.CreateTranslation(0, 0, Step);

                UpdateTransforms();
            }
        }

        private void Viewport_Paint(object sender, PaintEventArgs e)
        {
            Renderer.Clear();

            UIGrid.Render();

            foreach (int i in ModelsList.SelectedIndices)
            {
                Renderer.Models[i].Render();
            }

            UIAxis.Render();

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
                    if (!MergeMode)
                    {
                        Renderer.DeleteAll();

                        Renderer.Lights.Add(new Light
                        {
                            Ambient  = Color4.Black,
                            Diffuse  = Color4.Gainsboro,
                            Specular = Color4.Gainsboro,
                            Enabled  = true
                        });

                        ResetTransforms();

                        Scene = FileIO.Merge(OpenDlg.FileNames, Renderer);

                        TextureManager.Textures = Scene.Textures;

                        ModelsList.Bind(Scene.Models);
                        TexturesList.Bind(Scene.Textures);
                        SklAnimsList.Bind(Scene.SkeletalAnimations);
                        MatAnimsList.Bind(Scene.MaterialAnimations);

                        Animator.Enabled     = false;
                        LblAnimSpeed.Text    = string.Empty;
                        LblAnimLoopMode.Text = string.Empty;
                        AnimSeekBar.Value    = 0;
                        AnimSeekBar.Maximum  = 0;
                        AnimCtrl.Frame       = 0;
                        AnimCtrl.Animation   = null;
                        SklAnim              = null;
                        MatAnim              = null;

                        if (Scene.Models.Count > 0)
                        {
                            ModelsList.Select(0);
                        }
                        else
                        {
                            UpdateTransforms();
                        }
                    }
                    else
                    {
                        Scene = FileIO.Merge(OpenDlg.FileNames, Renderer, Scene);
                    }
                }
            }

            //Allow app to process click from the Open dialog that goes into the Viewport.
            //This avoid the model from moving after opening a file on the dialog.
            //(Note: The problem only happens if the dialog is on top of the Viewport).
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

        private void ResetTransforms()
        {
            MdlCenter = Vector3.Zero;

            Dimension = MinDim;

            Transform =
                Matrix4.CreateRotationY((float)Math.PI * 0.25f) *
                Matrix4.CreateRotationX((float)Math.PI * 0.25f) *
                Matrix4.CreateTranslation(0, 0, -Dimension * 2);
        }

        private void UpdateTransforms()
        {
            Matrix4 Centered = Matrix4.CreateTranslation(MdlCenter) * Transform;

            foreach (int i in ModelsList.SelectedIndices)
            {
                Renderer.Models[i].Transform = Centered;
            }

            UIGrid.Transform = Centered;
            UIAxis.Transform = Transform;

            UpdateViewport();
        }
        #endregion

        #region Side menu events
        private void ModelsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int SelectedIndices = ModelsList.SelectedIndices.Length;

            if (SelectedIndices > 0)
            {
                int Index = ModelsList.SelectedIndex;

                BoundingBox AABB = Renderer.Models[Index].GetModelAABB();

                MdlCenter = -AABB.Center;

                Dimension = MinDim * 0.5f;

                Dimension = Math.Max(Dimension, Math.Abs(AABB.Size.X));
                Dimension = Math.Max(Dimension, Math.Abs(AABB.Size.Y));
                Dimension = Math.Max(Dimension, Math.Abs(AABB.Size.Z));

                Dimension *= 2;

                Transform = Matrix4.CreateTranslation(0, 0, -Dimension);

                Renderer.Lights[0].Position.Y = AABB.Center.Y;
                Renderer.Lights[0].Position.Z = Dimension;

                foreach (int i in ModelsList.SelectedIndices)
                {
                    Renderer.Models[i].UpdateUniforms();
                }

                UpdateTransforms();

                SyncAnimationStates();
            }
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
            AnimCtrl.AdvanceFrame();

            AnimSeekBar.Value = AnimCtrl.Frame;

            SyncAnimationStates();

            Viewport.Invalidate();
        }

        private void SetAnimation(int Index, AnimationType Type)
        {
            if (Index == -1) return;

            switch (Type)
            {
                case AnimationType.Skeletal:
                    SklAnim = AnimCtrl.Animation = Scene.SkeletalAnimations[Index];
                    break;

                case AnimationType.Material:
                    MatAnim = AnimCtrl.Animation = Scene.MaterialAnimations[Index];
                    break;
            }

            if (Type == AnimationType.Skeletal)
            {
                int MIndex = Scene.MaterialAnimations.FindIndex(AnimCtrl.Animation.Name);

                if (MIndex != -1) MatAnimsList.Select(MIndex);
            }

            AnimCtrl.Frame = 0;

            AnimSeekBar.Value   = AnimCtrl.Frame;
            AnimSeekBar.Maximum = AnimCtrl.FramesCount;

            UpdateAnimLbls();
            SyncAnimationStates();
        }

        private void SyncAnimationStates()
        {
            foreach (int i in ModelsList.SelectedIndices)
            {
                Renderer.Models[i].SkeletalAnim.CopyState(AnimCtrl);
                Renderer.Models[i].MaterialAnim.CopyState(AnimCtrl);

                Renderer.Models[i].SkeletalAnim.Animation = SklAnim;
                Renderer.Models[i].MaterialAnim.Animation = MatAnim;

                Renderer.Models[i].UpdateAnimationTransforms();
            }
        }

        private void UpdateAnimLbls()
        {
            LblAnimSpeed.Text = $"{Math.Abs(AnimCtrl.Step).ToString("N2")}x";

            LblAnimLoopMode.Text = AnimCtrl.IsLooping ? "LOOP" : "1 GO";
        }

        private void EnableAnimator()
        {
            Animator.Enabled = true;

            UpdateAnimLbls();

            SyncAnimationStates();
        }

        private void DisableAnimator()
        {
            Animator.Enabled = false;

            Viewport.Invalidate();

            SyncAnimationStates();
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
            AnimCtrl.Play(-Math.Abs(AnimCtrl.Step)); EnableAnimator();
        }

        private void AnimButtonPlayForward_Click(object sender, EventArgs e)
        {
            AnimCtrl.Play(Math.Abs(AnimCtrl.Step)); EnableAnimator();
        }

        private void AnimButtonPause_Click(object sender, EventArgs e)
        {
            AnimCtrl.Pause(); DisableAnimator();
        }

        private void AnimButtonStop_Click(object sender, EventArgs e)
        {
            AnimCtrl.Stop();

            DisableAnimator();

            AnimSeekBar.Value = 0;
        }

        private void AnimButtonSlowDown_Click(object sender, EventArgs e)
        {
            AnimCtrl.SlowDown();

            UpdateAnimLbls();

            SyncAnimationStates();
        }

        private void AnimButtonSpeedUp_Click(object sender, EventArgs e)
        {
            AnimCtrl.SpeedUp();

            UpdateAnimLbls();

            SyncAnimationStates();
        }

        private void AnimButtonPrev_Click(object sender, EventArgs e)
        {
            if (SklAnimsList.SelectedIndex != -1)
                SklAnimsList.SelectUp();
        }

        private void AnimButtonNext_Click(object sender, EventArgs e)
        {
            if (SklAnimsList.SelectedIndex != -1)
                SklAnimsList.SelectDown();
        }

        private void AnimSeekBar_Seek(object sender, EventArgs e)
        {
            AnimCtrl.Pause();

            AnimCtrl.Frame = AnimSeekBar.Value;

            UpdateViewport();

            SyncAnimationStates();
        }

        private void AnimSeekBar_MouseUp(object sender, MouseEventArgs e)
        {
            AnimCtrl.Play();

            SyncAnimationStates();
        }
        #endregion
    }
}
