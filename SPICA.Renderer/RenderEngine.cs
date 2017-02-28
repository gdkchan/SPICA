using OpenTK;
using OpenTK.Graphics.ES30;

using SPICA.Renderer.GUI;
using SPICA.Renderer.Shaders;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace SPICA.Renderer
{
    public class RenderEngine : TransformableObject, IDisposable
    {
        public const float ClipDistance = 100000f;

        public event EventHandler BeforeDraw;
        public event EventHandler AfterDraw;

        public ShaderManager MdlShader;
        public ShaderManager GUIShader;

        public List<Model> Models;
        public List<Light> Lights;

        public List<GUIControl> Controls;

        public Vector4 SceneAmbient;

        public Matrix4 ProjectionMatrix;

        public bool ObjectSpaceNormalMap;

        private int ProjMtxLocation;
        private int ViewMtxLocation;

        private int Width;
        private int Height;

        public RenderEngine(int Width, int Height)
        {
            //Set initial and default values
            Models = new List<Model>();
            Lights = new List<Light>();

            Controls = new List<GUIControl>();

            SceneAmbient = new Vector4(0.1f);

            //Create Shaders
            MdlShader = new ShaderManager(
                GetEmbeddedString("SPICA.Shader.MdlVertexShader"),
                GetEmbeddedString("SPICA.Shader.MdlFragmentShader"));

            GUIShader = new ShaderManager(
                GetEmbeddedString("SPICA.Shader.GUIVertexShader"),
                GetEmbeddedString("SPICA.Shader.GUIFragmentShader"));

            //Configure Model Shader
            GL.UseProgram(MdlShader.Handle);

            ProjMtxLocation = GL.GetUniformLocation(MdlShader.Handle, "ProjMatrix");
            ViewMtxLocation = GL.GetUniformLocation(MdlShader.Handle, "ViewMatrix");

            int MdlMtxLocation = GL.GetUniformLocation(MdlShader.Handle, "ModelMatrix");

            GL.UniformMatrix4(ViewMtxLocation, false, ref Transform);
            GL.UniformMatrix4(MdlMtxLocation, false, ref Transform);

            GL.Uniform1(GL.GetUniformLocation(MdlShader.Handle, "Texture[0]"), 0);
            GL.Uniform1(GL.GetUniformLocation(MdlShader.Handle, "Texture[1]"), 1);
            GL.Uniform1(GL.GetUniformLocation(MdlShader.Handle, "Texture[2]"), 2);

            GL.Uniform1(GL.GetUniformLocation(MdlShader.Handle, "TextureCube"), 3);

            GL.UniformBlockBinding(MdlShader.Handle, GL.GetUniformBlockIndex(MdlShader.Handle, "UBDist0"), 0);
            GL.UniformBlockBinding(MdlShader.Handle, GL.GetUniformBlockIndex(MdlShader.Handle, "UBDist1"), 1);
            GL.UniformBlockBinding(MdlShader.Handle, GL.GetUniformBlockIndex(MdlShader.Handle, "UBFresnel"), 2);
            GL.UniformBlockBinding(MdlShader.Handle, GL.GetUniformBlockIndex(MdlShader.Handle, "UBReflecR"), 3);
            GL.UniformBlockBinding(MdlShader.Handle, GL.GetUniformBlockIndex(MdlShader.Handle, "UBReflecG"), 4);
            GL.UniformBlockBinding(MdlShader.Handle, GL.GetUniformBlockIndex(MdlShader.Handle, "UBReflecB"), 5);

            GL.Uniform4(GL.GetUniformLocation(MdlShader.Handle, "SAmbient"), SceneAmbient);

            //Configure GUI Shader
            GL.UseProgram(GUIShader.Handle);

            GL.Uniform1(GL.GetUniformLocation(GUIShader.Handle, "UITexture"), 0);

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

        public void UpdateResolution(int Width, int Height)
        {
            this.Width  = Width;
            this.Height = Height;

            GL.Viewport(0, 0, Width, Height);

            foreach (GUIControl Ctrl in Controls)
            {
                Ctrl.Resize();
            }

            float AR = Width / (float)Height;

            ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI * 0.25f, AR, 0.25f, ClipDistance);

            GL.UseProgram(MdlShader.Handle);

            GL.UniformMatrix4(ProjMtxLocation, false, ref ProjectionMatrix);
        }

        public void SetBackgroundColor(Color Color)
        {
            GL.ClearColor(Color);
        }

        public void RenderScene()
        {
            GL.Clear(
                ClearBufferMask.ColorBufferBit |
                ClearBufferMask.StencilBufferBit |
                ClearBufferMask.DepthBufferBit);

            BeforeDraw?.Invoke(this, EventArgs.Empty);

            GL.UseProgram(MdlShader.Handle);

            int ObjNormalMapLocation = GL.GetUniformLocation(MdlShader.Handle, "ObjNormalMap");
            int LightsCountLocation = GL.GetUniformLocation(MdlShader.Handle, "LightsCount");

            GL.Uniform1(ObjNormalMapLocation, ObjectSpaceNormalMap ? 1 : 0);
            GL.Uniform1(LightsCountLocation, Lights.Count);

            for (int Index = 0; Index < Lights.Count; Index++)
            {
                int LightPositionLocation = GL.GetUniformLocation(MdlShader.Handle, $"Lights[{Index}].Position");
                int LightAmbientLocation  = GL.GetUniformLocation(MdlShader.Handle, $"Lights[{Index}].Ambient");
                int LightDiffuseLocation  = GL.GetUniformLocation(MdlShader.Handle, $"Lights[{Index}].Diffuse");
                int LightSpecularLocation = GL.GetUniformLocation(MdlShader.Handle, $"Lights[{Index}].Specular");

                GL.Uniform3(LightPositionLocation, Lights[Index].Position);
                GL.Uniform4(LightAmbientLocation,  Lights[Index].Ambient);
                GL.Uniform4(LightDiffuseLocation,  Lights[Index].Diffuse);
                GL.Uniform4(LightSpecularLocation, Lights[Index].Specular);
            }

            GL.UniformMatrix4(ViewMtxLocation, false, ref Transform);

            foreach (Model Mdl in Models)
            {
                Mdl.Render();
            }

            //Reset needed settings back to "default" state
            GL.DepthMask(true);
            GL.ColorMask(true, true, true, true);

            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.StencilTest);
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);

            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.BlendEquation(BlendEquationMode.FuncAdd);

            GL.DepthFunc(DepthFunction.Less);

            GL.UseProgram(GUIShader.Handle);

            foreach (GUIControl Ctrl in Controls)
            {
                Ctrl.Render();
            }

            AfterDraw?.Invoke(this, EventArgs.Empty);
        }

        private bool Disposed;

        protected virtual void Dispose(bool Disposing)
        {
            if (!Disposed)
            {
                MdlShader.Dispose();
                GUIShader.Dispose();

                foreach (Model Mdl in Models)
                {
                    Mdl.Dispose();
                }

                foreach (GUIControl Ctrl in Controls)
                {
                    Ctrl.Dispose();
                }

                Disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
