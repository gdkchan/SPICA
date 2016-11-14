using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;

using SPICA.Formats.CtrH3D;
using SPICA.Formats.GFL2;
using SPICA.Renderer;

using System;

namespace SPICA.WinForms
{
    public partial class FrmMain : GameWindow
    {
        RenderEngine Renderer;

        Vector2 InitialRot;
        Vector2 InitialMov;
        Vector2 FinalMov;

        private float Zoom;
        private float Step;

        private Model Model;

        public FrmMain() : base(800, 600, new GraphicsMode(new ColorFormat(32), 24, 8))
        {
            Title = "SPICA";
        }

        protected override void OnResize(EventArgs e)
        {
            Renderer.UpdateResolution(Width, Height);
        }

        protected override void OnLoad(EventArgs e)
        {
            VSync = VSyncMode.On;

            Renderer = new RenderEngine(Width, Height);

            Model = Renderer.AddModel(H3D.Open("D:\\may.bch"));
            //Model = Renderer.AddModel(new GFModelPack("D:\\suntest.bin").ToH3D());

            Tuple<Vector3, float> CenterMax = Model.GetCenterMaxXY();

            Vector3 Center = -CenterMax.Item1;
            float Maximum = CenterMax.Item2;

            Model.TranslateAbs(Center);
            Zoom = Center.Z - Maximum * 2;
            Step = Maximum * 0.05f;

            UpdateTranslation();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            Model.Animate();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Renderer.RenderScene();
            SwapBuffers();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButton.Left: InitialRot = new Vector2(e.X, e.Y); break;
                case MouseButton.Right: InitialMov = new Vector2(e.X, e.Y); break;
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Right)
            {
                float X = (InitialMov.X - e.X) + FinalMov.X;
                float Y = (InitialMov.Y - e.Y) + FinalMov.Y;

                FinalMov = new Vector2(X, Y);
            }

            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if (e.Mouse.LeftButton == ButtonState.Pressed)
            {
                float RY = (float)(((e.X - InitialRot.X) / Width) * Math.PI);
                float RX = (float)(((e.Y - InitialRot.Y) / Height) * Math.PI);

                Model.Rotate(new Vector3(RX, RY, 0));
            }

            if (e.Mouse.RightButton == ButtonState.Pressed)
            {
                float TX = (InitialMov.X - e.X) + FinalMov.X;
                float TY = (InitialMov.Y - e.Y) + FinalMov.Y;

                Renderer.TranslateAbs(new Vector3(-TX, TY, Zoom));
            }

            InitialRot = new Vector2(e.X, e.Y);

            base.OnMouseMove(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (e.Mouse.RightButton == ButtonState.Released)
            {
                if (e.Delta > 0)
                    Zoom += Step;
                else
                    Zoom -= Step;

                UpdateTranslation();
            }

            base.OnMouseWheel(e);
        }

        private void UpdateTranslation()
        {
            Renderer.TranslateAbs(new Vector3(-FinalMov.X, FinalMov.Y, Zoom));
        }
    }
}
