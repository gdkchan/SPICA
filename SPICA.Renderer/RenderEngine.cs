using OpenTK;
using OpenTK.Graphics.ES30;

using SPICA.Formats.CtrH3D;
using SPICA.Renderer.Shaders;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace SPICA.Renderer
{
    public class RenderEngine : IDisposable
    {
        private int Width, Height;

        private int ShaderHandle;

        private int VertexShaderHandle;
        private int FragmentShaderHandle;

        private int ProjMtxLocation;
        private int ViewMtxLocation;
        internal int ModelMtxLocation;

        private Matrix4 ProjMtx;
        private Matrix4 ViewMtx;

        private List<Model> Models;
        private List<Light> Lights;

        public Vector4 SceneAmbient;

        public RenderEngine(int Width, int Height)
        {
            //Set initial and default values
            Models = new List<Model>();
            Lights = new List<Light>();

            SceneAmbient = new Vector4(0.1f);

            //Setup Shaders
            ShaderHandle = GL.CreateProgram();

            VertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            FragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(VertexShaderHandle, VertexShader.Code);
            GL.ShaderSource(FragmentShaderHandle, FragmentShader.Code);

            GL.CompileShader(VertexShaderHandle);
            GL.CompileShader(FragmentShaderHandle);

            GL.AttachShader(ShaderHandle, VertexShaderHandle);
            GL.AttachShader(ShaderHandle, FragmentShaderHandle);

            GL.LinkProgram(ShaderHandle);

            Debug.WriteLine("[RenderEngine] Shader compilation result (Vertex/Fragment/Shader):");

            Debug.WriteLine(GL.GetShaderInfoLog(VertexShaderHandle));
            Debug.WriteLine(GL.GetShaderInfoLog(FragmentShaderHandle));
            Debug.WriteLine(GL.GetProgramInfoLog(ShaderHandle));

            GL.UseProgram(ShaderHandle);

            //Setup Matrices
            ProjMtxLocation = GL.GetUniformLocation(ShaderHandle, "ProjMatrix");
            ViewMtxLocation = GL.GetUniformLocation(ShaderHandle, "ViewMatrix");
            ModelMtxLocation = GL.GetUniformLocation(ShaderHandle, "ModelMatrix");

            ViewMtx = Matrix4.Identity;

            GL.UniformMatrix4(ViewMtxLocation, false, ref ViewMtx);

            UpdateResolution(Width, Height);

            //Misc. stuff initialization
            GL.Uniform1(GL.GetUniformLocation(ShaderHandle, "Texture0"), 0);
            GL.Uniform1(GL.GetUniformLocation(ShaderHandle, "Texture1"), 1);
            GL.Uniform1(GL.GetUniformLocation(ShaderHandle, "Texture2"), 2);

            GL.Uniform1(GL.GetUniformLocation(ShaderHandle, "TextureCube"), 3);

            GL.UniformBlockBinding(ShaderHandle, GL.GetUniformBlockIndex(ShaderHandle, "UBDist0"), 0);
            GL.UniformBlockBinding(ShaderHandle, GL.GetUniformBlockIndex(ShaderHandle, "UBDist1"), 1);
            GL.UniformBlockBinding(ShaderHandle, GL.GetUniformBlockIndex(ShaderHandle, "UBFresnel"), 2);
            GL.UniformBlockBinding(ShaderHandle, GL.GetUniformBlockIndex(ShaderHandle, "UBReflecR"), 3);
            GL.UniformBlockBinding(ShaderHandle, GL.GetUniformBlockIndex(ShaderHandle, "UBReflecG"), 4);
            GL.UniformBlockBinding(ShaderHandle, GL.GetUniformBlockIndex(ShaderHandle, "UBReflecB"), 5);

            GL.Uniform4(GL.GetUniformLocation(ShaderHandle, "SAmbient"), SceneAmbient);

            GL.Enable(EnableCap.DepthTest);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
        }

        public void SetBackgroundColor(Color Color)
        {
            GL.ClearColor(Color);
        }
        
        public Model AddModel(H3D BaseModel, int ModelIndex = 0)
        {
            Model Model = new Model(this, BaseModel, ModelIndex, ShaderHandle);

            Models.Add(Model);

            return Model;
        }

        public void Rotate(Vector3 Rotation)
        {
            UpdateView(ViewMtx *= Utils.EulerRotate(Rotation));
        }

        public void Translate(Vector3 Translation)
        {
            UpdateView(ViewMtx *= Matrix4.CreateTranslation(Translation));
        }

        public void RotateAbs(Vector3 Rotation)
        {
            UpdateView(ViewMtx = (ViewMtx.ClearRotation() * Utils.EulerRotate(Rotation)));
        }

        public void TranslateAbs(Vector3 Translation)
        {
            UpdateView(ViewMtx = (ViewMtx.ClearTranslation() * Matrix4.CreateTranslation(Translation)));
        }

        private void UpdateView(Matrix4 Mtx)
        {
            GL.UniformMatrix4(ViewMtxLocation, false, ref ViewMtx);
        }

        public void RenderScene()
        {
            GL.Viewport(0, 0, Width, Height);

            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Clear(ClearBufferMask.StencilBufferBit);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            foreach (Model Model in Models) Model.Render();
        }
        
        public void UpdateResolution(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;

            float AR = Width / (float)Height;
            ProjMtx = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI * 0.25f, AR, 1, 1000);

            GL.UniformMatrix4(ProjMtxLocation, false, ref ProjMtx);
        }

        public void AddLight(Light Light)
        {
            Lights.Add(Light);
            UpdateLights();
        }

        public void RemoveLight(Light Light)
        {
            Lights.Remove(Light);
            UpdateLights();
        }

        public void ClearLights(Light Light)
        {
            Lights.Clear();
            UpdateLights();
        }

        private void UpdateLights()
        {
            GL.Uniform1(GL.GetUniformLocation(ShaderHandle, "LightsCount"), Lights.Count);

            for (int Index = 0; Index < Lights.Count; Index++)
            {
                int LightPositionLocation = GL.GetUniformLocation(ShaderHandle, $"Lights[{Index}].Position");
                int LightAmbientLocation = GL.GetUniformLocation(ShaderHandle, $"Lights[{Index}].Ambient");
                int LightDiffuseLocation = GL.GetUniformLocation(ShaderHandle, $"Lights[{Index}].Diffuse");
                int LightSpecularLocation = GL.GetUniformLocation(ShaderHandle, $"Lights[{Index}].Specular");

                GL.Uniform3(LightPositionLocation, Lights[Index].Position);
                GL.Uniform4(LightAmbientLocation, Lights[Index].Ambient);
                GL.Uniform4(LightDiffuseLocation, Lights[Index].Diffuse);
                GL.Uniform4(LightSpecularLocation, Lights[Index].Specular);
            }
        }

        private bool Disposed;

        protected virtual void Dispose(bool Disposing)
        {
            if (!Disposed)
            {
                GL.DeleteProgram(ShaderHandle);

                GL.DeleteShader(VertexShaderHandle);
                GL.DeleteShader(FragmentShaderHandle);

                foreach (Model Model in Models) Model.Dispose();

                Disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
