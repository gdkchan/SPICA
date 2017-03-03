using OpenTK;
using OpenTK.Graphics.ES30;

using SPICA.Formats.CtrH3D;
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

        public List<Model>   Models;
        public List<Texture> Textures;
        public List<LUT>     LUTs;

        public List<Light> Lights;

        public ShaderManager GUIShader;

        public List<GUIControl> Controls;

        public Vector4 SceneAmbient;

        public Matrix4 ProjectionMatrix;

        public bool ObjectSpaceNormalMap;

        internal int VertexShaderHandle;

        internal string FragmentBaseCode;

        private int Width;
        private int Height;

        public RenderEngine(int Width, int Height)
        {
            //Set initial and default values
            Models   = new List<Model>();
            Textures = new List<Texture>();
            LUTs     = new List<LUT>();

            Lights = new List<Light>();

            Controls = new List<GUIControl>();

            SceneAmbient = new Vector4(0.1f);

            //Create Shaders
            GUIShader = new ShaderManager(
                GetEmbeddedString("SPICA.Shader.GUIVertexShader"),
                GetEmbeddedString("SPICA.Shader.GUIFragmentShader"));

            VertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);

            FragmentBaseCode = GetEmbeddedString("SPICA.Shader.MdlFragmentShader");

            GL.ShaderSource(VertexShaderHandle, GetEmbeddedString("SPICA.Shader.MdlVertexShader"));

            GL.CompileShader(VertexShaderHandle);

            //Configure GUI Shader
            GL.UseProgram(GUIShader.Handle);

            GL.Uniform1(GL.GetUniformLocation(GUIShader.Handle, "UITexture"), 0);

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            UpdateResolution(Width, Height);
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
        }

        public void SetBackgroundColor(Color Color)
        {
            GL.ClearColor(Color);
        }

        public void Merge(H3D SceneData)
        {
            Merge(Models,   SceneData.Models);
            Merge(Textures, SceneData.Textures);
            Merge(LUTs,     SceneData.LUTs);
        }

        public void DeleteAll()
        {
            DisposeAndClear(Models);
            DisposeAndClear(Textures);
            DisposeAndClear(LUTs);
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

        private void Merge<T, T2>(List<T2> Dst, PatriciaList<T> Src) where T : INamed
        {
            foreach (T Value in Src)
            {
                T2 NewVal = (T2)Activator.CreateInstance(typeof(T2), this, Value);

                Dst.Add(NewVal);
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

        internal int GetTextureId(string TextureName)
        {
            return Textures.FirstOrDefault(x => x.Name == TextureName)?.Id ?? -1;
        }

        internal bool BindLUT(int Unit, string TableName, string SamplerName)
        {
            return LUTs.FirstOrDefault(x => x.Name == TableName)?.BindSampler(Unit, SamplerName) ?? false;
        }

        private bool Disposed;

        protected virtual void Dispose(bool Disposing)
        {
            if (!Disposed)
            {
                DeleteAll();

                GUIShader.Dispose();

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
