using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES30;

using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.LUT;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.CtrH3D.Texture;
using SPICA.Renderer.GUI;
using SPICA.Renderer.Shaders;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SPICA.Renderer
{
    public class RenderEngine : TransformableObject, IDisposable
    {
        public const float ClipDistance = 100000f;

        public readonly List<Model>      Models;
        public readonly List<Texture>    Textures;
        public readonly List<LUT>        LUTs;
        public readonly List<Light>      Lights;
        public readonly List<GUIControl> Controls;

        private  Shader  GUI2DShader;
        private  Shader  GUI3DShader;
        internal Matrix4 ProjectionMatrix;
        internal string  FragmentBaseCode;
        internal int     VertexShaderHandle;

        private int Width, Height;

        private Color4 _SceneAmbient;

        public Color4 SceneAmbient
        {
            get
            {
                return _SceneAmbient;
            }
            set
            {
                _SceneAmbient = value;

                UpdateAllUniforms();
            }
        }

        public RenderEngine(int Width, int Height)
        {
            Models   = new List<Model>();
            Textures = new List<Texture>();
            LUTs     = new List<LUT>();
            Lights   = new List<Light>();
            Controls = new List<GUIControl>();

            string VertexShaderCode = GetEmbeddedString("SPICA.Shader.MdlVertexShader");

            FragmentBaseCode = GetEmbeddedString("SPICA.Shader.MdlFragmentShader");

            VertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);

            GL.ShaderSource(VertexShaderHandle, VertexShaderCode);
            GL.CompileShader(VertexShaderHandle);

            Shader.CheckCompilation(VertexShaderHandle);

            SetupGUI2DShader();
            SetupGUI3DShader();

            Resize(Width, Height);

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
        }

        private void SetupGUI2DShader()
        {
            GUI2DShader = new Shader(
                GetEmbeddedString("SPICA.Shader.GUIVertexShader"),
                GetEmbeddedString("SPICA.Shader.GUIFragmentShader"));

            GL.UseProgram(GUI2DShader.Handle);

            GL.Uniform1(GL.GetUniformLocation(GUI2DShader.Handle, "UITexture"), 0);
        }

        private void SetupGUI3DShader()
        {
            string Code = GetEmbeddedString("SPICA.Shader.GUIColFragmentShader");

            GUI3DShader = new Shader();

            GUI3DShader.SetVertexShaderHandle(VertexShaderHandle);
            GUI3DShader.SetFragmentShaderCode(Code);
            GUI3DShader.Link();

            GL.UseProgram(GUI3DShader.Handle);

            int Scales0Location    = GL.GetUniformLocation(GUI3DShader.Handle, "Scales0");
            int Scales1Location    = GL.GetUniformLocation(GUI3DShader.Handle, "Scales1");
            int ColorScaleLocation = GL.GetUniformLocation(GUI3DShader.Handle, "ColorScale");

            GL.Uniform4(Scales0Location, Vector4.One);
            GL.Uniform4(Scales1Location, Vector4.One);
            GL.Uniform1(ColorScaleLocation, 1f);
        }

        public void Resize(int Width, int Height)
        {
            this.Width  = Width;
            this.Height = Height;

            GL.Viewport(0, 0, Width, Height);

            foreach (GUIControl Ctrl in Controls)
            {
                Ctrl.Resize();
            }

            ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
                (float)Math.PI * 0.25f,
                (float)Width / Height,
                0.25f,
                ClipDistance);
        }

        public int Set3DColorShader()
        {
            Matrix4 Identity = Matrix4.Identity;

            GL.UseProgram(GUI3DShader.Handle);

            int TransformLocation = GL.GetUniformLocation(GUI3DShader.Handle, "Transforms[0]");
            int ProjMtxLocation   = GL.GetUniformLocation(GUI3DShader.Handle, "ProjMatrix");
            int ViewMtxLocation   = GL.GetUniformLocation(GUI3DShader.Handle, "ViewMatrix");

            GL.UniformMatrix4(TransformLocation, false, ref Identity);
            GL.UniformMatrix4(ProjMtxLocation,   false, ref ProjectionMatrix);
            GL.UniformMatrix4(ViewMtxLocation,   false, ref Transform);

            return GUI3DShader.Handle;
        }

        public void UpdateAllUniforms()
        {
            foreach (Model Model in Models)
            {
                Model.UpdateUniforms();
            }
        }

        public void SetBackgroundColor(Color Color)
        {
            GL.ClearColor(Color);
        }

        public void Merge(H3D SceneData)
        {
            MergeModels  (SceneData.Models);
            MergeTextures(SceneData.Textures);
            MergeLUTs    (SceneData.LUTs);
        }

        public void MergeModels(PatriciaList<H3DModel> Models)
        {
            foreach (H3DModel Mdl in Models)
            {
                this.Models.Add(new Model(this, Mdl));
            }
        }

        public void MergeTextures(PatriciaList<H3DTexture> Textures)
        {
            foreach (H3DTexture Tex in Textures)
            {
                this.Textures.Add(new Texture(this, Tex));
            }
        }

        public void MergeLUTs(PatriciaList<H3DLUT> LUTs)
        {
            foreach (H3DLUT LUT in LUTs)
            {
                this.LUTs.Add(new LUT(LUT));
            }
        }

        public void DeleteAll()
        {
            DisposeAndClear(Models);
            DisposeAndClear(Textures);
            DisposeAndClear(LUTs);
        }

        public void Clear()
        {
            GL.DepthMask(true);
            GL.ColorMask(true, true, true, true);
            GL.Clear(
                ClearBufferMask.ColorBufferBit |
                ClearBufferMask.StencilBufferBit |
                ClearBufferMask.DepthBufferBit);
        }

        public void Render(int ModelIndex)
        {
            Models[ModelIndex].Render();
        }

        internal void BindTexture(int Unit, string TextureName)
        {
            Textures.FirstOrDefault(x => x.Name == TextureName)?.Bind(Unit);
        }

        internal bool BindLUT(int Unit, string TableName, string SamplerName)
        {
            return LUTs.FirstOrDefault(x => x.Name == TableName)?.BindSampler(Unit, SamplerName) ?? false;
        }

        //Helper methods
        private string GetEmbeddedString(string Name)
        {
            Assembly Asm = Assembly.GetExecutingAssembly();

            using (Stream Text = Asm.GetManifestResourceStream(Name))
            {
                return new StreamReader(Text).ReadToEnd();
            }
        }

        private void DisposeAndClear<T>(List<T> Values) where T : IDisposable
        {
            foreach (T Value in Values)
            {
                Value.Dispose();
            }

            Values.Clear();
        }

        private bool Disposed;

        protected virtual void Dispose(bool Disposing)
        {
            if (!Disposed)
            {
                DeleteAll();

                GUI2DShader.Dispose();
                GUI3DShader.Dispose();

                GL.DeleteShader(VertexShaderHandle);

                Disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
