using OpenTK;
using OpenTK.Graphics;

using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Animation;
using SPICA.Renderer;
using SPICA.Renderer.Animation;
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
        #region Initialization
        private GLControl Viewport;
        private RenderEngine Renderer;

        private H3D SceneData;
        private Model Model;

        private Bitmap[] CachedTextures;

        private Vector2 InitialRot;
        private Vector2 InitialMov;
        private Vector2 FinalMov;

        private Vector3 MdlCenter;

        private float CamDist;
        private float Zoom;
        private float Step;

        private bool IgnoreClicks;

        private enum AnimType
        {
            None,
            Skeletal,
            Material,
            Visibility
        }

        private AnimType CurrAnimType;

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
            Viewport.MouseUp    += Viewport_MouseUp;
            Viewport.MouseWheel += Viewport_MouseWheel;
            Viewport.Resize     += Viewport_Resize;

            InitializeComponent();

            MainContainer.Panel1.Controls.Add(Viewport);

            TopMenu.Renderer = new ToolsRenderer(TopMenu.BackColor);
            TopIcons.Renderer = new ToolsRenderer(TopIcons.BackColor);
        }
        #endregion

        #region Viewport events
        private void Viewport_Load(object sender, EventArgs e)
        {
            //Note: Setting up OpenGL stuff only works after the control has loaded on the Form
            Viewport.MakeCurrent();

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

                UpdateViewport();
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

                UpdateViewport();
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

        private void Open(bool MergeMode)
        {
            IgnoreClicks = true;

            using (OpenFileDialog OpenDlg = new OpenFileDialog())
            {
                OpenDlg.Filter = "All files|*.*";
                OpenDlg.Multiselect = MergeMode;

                if (OpenDlg.ShowDialog() == DialogResult.OK)
                {
                    if (MergeMode)
                    {
                        if (SceneData == null) SceneData = new H3D();

                        for (int Index = 0; Index < OpenDlg.FileNames.Length; Index++)
                        {
                            H3D Data = FormatIdentifier.IdentifyAndOpen(OpenDlg.FileNames[Index]);

                            if (Data != null) SceneData.Merge(Data);
                        }

                        LoadScene();
                    }
                    else
                    {
                        SceneData = FormatIdentifier.IdentifyAndOpen(OpenDlg.FileName);

                        if (SceneData == null)
                            MessageBox.Show(
                                "Unsupported file format!",
                                "Can't open file!",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                        else
                            LoadScene();
                    }
                }
            }

            //Allow app to process click from the Open dialog that goes into the Viewport
            //This avoid the model from moving after opening a file on the dialog
            //(Note: The problem only happens if the dialog is on top of the Viewport)
            Application.DoEvents();

            IgnoreClicks = false;
        }

        private void LoadScene()
        {
            CacheTextures();

            SceneData.Textures.CollectionChanged += TexturesList_CollectionChanged;

            //Bind Lists to H3D contents
            ModelsList.Bind(SceneData.Models);
            TexturesList.Bind(SceneData.Textures);
            SklAnimsList.Bind(SceneData.SkeletalAnimations);
            MatAnimsList.Bind(SceneData.MaterialAnimations);

            //Model
            Renderer.ClearModels();

            Model = Renderer.AddModel(SceneData);

            //Camera
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

            //Animation
            Animator.Enabled = false;
            AnimSeekBar.Value = 0;
            AnimSeekBar.Maximum = 0;

            LblAnimSpeed.Text = string.Empty;
            LblAnimLoopMode.Text = string.Empty;

            CurrAnimType = AnimType.None;
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
            Model.ResetTransform();
            Model.TranslateAbs(MdlCenter);

            Zoom = MdlCenter.Z - CamDist * 2;
            Step = CamDist * 0.05f;

            InitialRot = Vector2.Zero;
            InitialMov = Vector2.Zero;
            FinalMov = Vector2.Zero;

            Renderer.ResetView();

            UpdateTranslation();

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

        private void TexturesList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CacheTextures();
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
        #endregion

        #region Animation timing
        private void Animator_Tick(object sender, EventArgs e)
        {
            Model.Animate();

            switch (CurrAnimType)
            {
                case AnimType.Skeletal: AnimSeekBar.Value = Model.SkeletalAnimation.Frame; break;
                case AnimType.Material: AnimSeekBar.Value = Model.MaterialAnimation.Frame; break;
            }

            Viewport.Invalidate();
        }

        private void EnableAnimator()
        {
            Animator.Enabled = true; UpdateSpeedLbl();
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
            AnimSeekBar.Value = 0;
            AnimSeekBar.Maximum = Anim.FramesCount;

            UpdateSpeedLbl();

            bool Loop = (Anim.AnimationFlags & H3DAnimationFlags.IsLooping) != 0;

            LblAnimLoopMode.Text = Loop ? "LOOP" : "1 GO";

            CurrAnimType = Type;
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

                UpdateSpeedLbl();
            }
        }

        private void AnimButtonSpeedUp_Click(object sender, EventArgs e)
        {
            if (Model != null)
            {
                Model.SkeletalAnimation.SpeedUp();
                Model.MaterialAnimation.SpeedUp();

                UpdateSpeedLbl();
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

        private void UpdateSpeedLbl()
        {
            LblAnimSpeed.Text = string.Format("{0:N2}x", Math.Abs(Model.SkeletalAnimation.Step));
        }
        #endregion
    }
}
