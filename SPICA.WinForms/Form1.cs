using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;

using SPICA.Formats.CtrH3D;
using SPICA.Renderer;
using System;

namespace SPICA.WinForms
{
    public partial class Form1 : GameWindow
    {
        RenderEngine Renderer;

        Vector2 InitialRot;
        Vector2 InitialMov;
        Vector2 FinalMov;

        private float Zoom;

        private Model Model;

        public Form1()
            : base(640, 480,
            new GraphicsMode(), "OpenGL 3 Example", 0,
            DisplayDevice.Default, 3, 0,
            GraphicsContextFlags.ForwardCompatible | GraphicsContextFlags.Debug)
        { }

        protected override void OnLoad(System.EventArgs e)
        {
            VSync = VSyncMode.On;

            Renderer = new RenderEngine(Width, Height);

            Model = Renderer.AddModel(H3D.Open("D:\\may.bch"));
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {

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
                    Zoom += 0.5f;
                else
                    Zoom -= 0.5f;

                Renderer.TranslateAbs(new Vector3(-FinalMov.X, FinalMov.Y, Zoom));
            }

            base.OnMouseWheel(e);
        }
    }
}
