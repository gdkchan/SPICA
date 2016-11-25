using OpenTK;
using OpenTK.Graphics;

using SPICA.Formats.CtrH3D;
using SPICA.Renderer;
using SPICA.WinForms.Formats;
using SPICA.WinForms.GUI;

using System;
using System.Drawing;
using System.Windows.Forms;

namespace SPICA.WinForms
{
    public partial class FrmMain : Form
    {
        RenderEngine Renderer;

        Vector2 InitialRot;
        Vector2 InitialMov;
        Vector2 FinalMov;

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

            for (int Index = 0; Index < 100; Index++)
            {
                ModelsList.AddItem($"Item_{Index}");
            }

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

        private void UpdateTranslation()
        {
            Renderer.TranslateAbs(new Vector3(-FinalMov.X, FinalMov.Y, Zoom));
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
                    H3D BCH = FormatIdentifier.IdentifyAndOpen(OpenDlg.FileName);

                    if (BCH == null)
                    {
                        MessageBox.Show(
                            "Unsupported file format!",
                            "Can't open file!",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);

                        return;
                    }

                    Renderer.ClearModels();

                    Model = Renderer.AddModel(BCH);

                    Tuple<Vector3, float> CenterMax = Model.GetCenterMaxXY();

                    Vector3 Center = -CenterMax.Item1;
                    float Maximum = CenterMax.Item2;

                    Model.TranslateAbs(Center);
                    Zoom = Center.Z - Maximum * 2;
                    Step = Maximum * 0.05f;

                    Renderer.ClearLights();

                    Renderer.AddLight(new Light
                    {
                        Position = new Vector3(0, -Center.Y, -Zoom),
                        Ambient = new Color4(0f, 0f, 0f, 0f),
                        Diffuse = new Color4(0.5f, 0.5f, 0.5f, 1f),
                        Specular = new Color4(0.8f, 0.8f, 0.8f, 1f)
                    });

                    InitialRot = Vector2.Zero;
                    InitialMov = Vector2.Zero;
                    FinalMov = Vector2.Zero;

                    UpdateTranslation();
                }
            }

            //Allow app to process click from the Open dialog that goes into the Viewport
            //This avoid the model from moving after opening a file on the dialog
            //(Note: The problem only happens if the dialog is on top of the Viewport)
            Application.DoEvents();

            IgnoreClicks = false;

            Viewport.Invalidate();
        }
    }
}
