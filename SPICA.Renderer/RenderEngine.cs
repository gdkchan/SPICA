using OpenTK;
using OpenTK.Graphics.ES30;

using SPICA.Formats.CtrH3D;
using SPICA.Renderer.GUI;
using SPICA.Renderer.Shaders;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace SPICA.Renderer
{
    public class RenderEngine : IDisposable
    {
        private int Width, Height;

        private Shader MdlShader;
        private Shader GUIShader;

        private int MdlShaderHandle;
        private int GUIShaderHandle;

        private int ProjMtxLocation;
        private int ViewMtxLocation;

        private Matrix4 ProjMtx;
        private Matrix4 ViewMtx;

        private List<Model> Models;
        private List<Light> Lights;

        private List<GUIControl> Controls;

        public Vector4 SceneAmbient;

        public RenderEngine(int Width, int Height)
        {
            //Set initial and default values
            Models = new List<Model>();
            Lights = new List<Light>();

            Controls = new List<GUIControl>();

            SceneAmbient = new Vector4(0.1f);

            ViewMtx = Matrix4.Identity;

            //Create Shaders
            MdlShader = new Shader(
                GetEmbeddedString("SPICA.Shader.MdlVertexShader"),
                GetEmbeddedString("SPICA.Shader.MdlFragmentShader"));

            GUIShader = new Shader(
                GetEmbeddedString("SPICA.Shader.GUIVertexShader"),
                GetEmbeddedString("SPICA.Shader.GUIFragmentShader"));

            MdlShaderHandle = MdlShader.ShaderHandle;
            GUIShaderHandle = GUIShader.ShaderHandle;

            //Configure Model Shader
            GL.UseProgram(MdlShaderHandle);

            ProjMtxLocation = GL.GetUniformLocation(MdlShaderHandle, "ProjMatrix");
            ViewMtxLocation = GL.GetUniformLocation(MdlShaderHandle, "ViewMatrix");         

            GL.UniformMatrix4(ViewMtxLocation, false, ref ViewMtx);

            GL.Uniform1(GL.GetUniformLocation(MdlShaderHandle, "Texture0"), 0);
            GL.Uniform1(GL.GetUniformLocation(MdlShaderHandle, "Texture1"), 1);
            GL.Uniform1(GL.GetUniformLocation(MdlShaderHandle, "Texture2"), 2);

            GL.Uniform1(GL.GetUniformLocation(MdlShaderHandle, "TextureCube"), 3);

            GL.UniformBlockBinding(MdlShaderHandle, GL.GetUniformBlockIndex(MdlShaderHandle, "UBDist0"), 0);
            GL.UniformBlockBinding(MdlShaderHandle, GL.GetUniformBlockIndex(MdlShaderHandle, "UBDist1"), 1);
            GL.UniformBlockBinding(MdlShaderHandle, GL.GetUniformBlockIndex(MdlShaderHandle, "UBFresnel"), 2);
            GL.UniformBlockBinding(MdlShaderHandle, GL.GetUniformBlockIndex(MdlShaderHandle, "UBReflecR"), 3);
            GL.UniformBlockBinding(MdlShaderHandle, GL.GetUniformBlockIndex(MdlShaderHandle, "UBReflecG"), 4);
            GL.UniformBlockBinding(MdlShaderHandle, GL.GetUniformBlockIndex(MdlShaderHandle, "UBReflecB"), 5);

            GL.Uniform4(GL.GetUniformLocation(MdlShaderHandle, "SAmbient"), SceneAmbient);

            //Configure GUI Shader
            GL.UseProgram(GUIShaderHandle);

            GL.Uniform1(GL.GetUniformLocation(GUIShaderHandle, "UITexture"), 0);

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            UpdateResolution(Width, Height);
        }

        private string GetEmbeddedString(string Name)
        {
            Assembly Asm = Assembly.GetExecutingAssembly();

            using (Stream Text = Asm.GetManifestResourceStream(Name))
            {
                return new StreamReader(Text).ReadToEnd();
            }
        }
        
        public Model AddModel(H3D BaseModel, int ModelIndex = 0)
        {
            Model Model = new Model(BaseModel, ModelIndex, MdlShaderHandle);

            Models.Add(Model);

            return Model;
        }

        public void RemoveModel(Model Model)
        {
            Model.Dispose();

            Models.Remove(Model);
        }

        public void ClearModels()
        {
            foreach (Model Mdl in Models) Mdl.Dispose();

            Models.Clear();
        }

        public void Rotate(Vector3 Rotation)
        {
            ViewMtx *= Utils.EulerRotate(Rotation); UpdateView();
        }

        public void Translate(Vector3 Translation)
        {
            ViewMtx *= Matrix4.CreateTranslation(Translation); UpdateView();
        }

        public void RotateAbs(Vector3 Rotation)
        {
            ViewMtx = ViewMtx.ClearRotation() * Utils.EulerRotate(Rotation); UpdateView();
        }

        public void TranslateAbs(Vector3 Translation)
        {
            ViewMtx = ViewMtx.ClearTranslation() * Matrix4.CreateTranslation(Translation); UpdateView();
        }

        private void UpdateView()
        {
            GL.UseProgram(MdlShaderHandle);

            GL.UniformMatrix4(ViewMtxLocation, false, ref ViewMtx);
        }

        public void UpdateResolution(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;

            GL.Viewport(0, 0, Width, Height);

            foreach (GUIControl Ctrl in Controls) Ctrl.Resize();

            float AR = Width / (float)Height;

            ProjMtx = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI * 0.25f, AR, 0.1f, 1000);

            GL.UseProgram(MdlShaderHandle);

            GL.UniformMatrix4(ProjMtxLocation, false, ref ProjMtx);
        }

        public void SetBackgroundColor(Color Color)
        {
            GL.ClearColor(Color);
        }

        public void AddLight(Light Light)
        {
            Lights.Add(Light); UpdateLights();
        }

        public void RemoveLight(Light Light)
        {
            Lights.Remove(Light); UpdateLights();
        }

        public void ClearLights()
        {
            Lights.Clear(); UpdateLights();
        }

        private void UpdateLights()
        {
            GL.UseProgram(MdlShaderHandle);

            GL.Uniform1(GL.GetUniformLocation(MdlShaderHandle, "LightsCount"), Lights.Count);

            for (int Index = 0; Index < Lights.Count; Index++)
            {
                int LightPositionLocation = GL.GetUniformLocation(MdlShaderHandle, $"Lights[{Index}].Position");
                int LightAmbientLocation = GL.GetUniformLocation(MdlShaderHandle, $"Lights[{Index}].Ambient");
                int LightDiffuseLocation = GL.GetUniformLocation(MdlShaderHandle, $"Lights[{Index}].Diffuse");
                int LightSpecularLocation = GL.GetUniformLocation(MdlShaderHandle, $"Lights[{Index}].Specular");

                GL.Uniform3(LightPositionLocation, Lights[Index].Position);
                GL.Uniform4(LightAmbientLocation, Lights[Index].Ambient);
                GL.Uniform4(LightDiffuseLocation, Lights[Index].Diffuse);
                GL.Uniform4(LightSpecularLocation, Lights[Index].Specular);
            }
        }

        public void AddControl(GUIControl Control)
        {
            Controls.Add(Control);
        }

        public void RemoveControl(GUIControl Control)
        {
            Controls.Remove(Control);
        }

        public void RenderScene()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Clear(ClearBufferMask.StencilBufferBit);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            GL.UseProgram(MdlShaderHandle);

            foreach (Model Mdl in Models) Mdl.Render();

            GL.UseProgram(GUIShaderHandle);

            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.AlphaTest);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.BlendEquation(BlendEquationMode.FuncAdd);

            foreach (GUIControl Ctrl in Controls) Ctrl.Render();
        }

        private bool Disposed;

        protected virtual void Dispose(bool Disposing)
        {
            if (!Disposed)
            {
                MdlShader.Dispose();
                GUIShader.Dispose();

                foreach (Model Model in Models) Model.Dispose();

                foreach (GUIControl Ctrl in Controls) Ctrl.Dispose();

                Disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
