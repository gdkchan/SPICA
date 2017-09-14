using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Light;
using SPICA.Formats.CtrH3D.LUT;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.CtrH3D.Shader;
using SPICA.Formats.CtrH3D.Texture;
using SPICA.Rendering.Properties;
using SPICA.Rendering.Shaders;

using System;
using System.Collections.Generic;
using System.Drawing;

namespace SPICA.Rendering
{
    public class Renderer : IDisposable
    {
        public readonly List<Model> Models;
        public readonly List<Light> Lights;

        public readonly Dictionary<string, Texture>      Textures;
        public readonly Dictionary<string, LUT>          LUTs;
        public readonly Dictionary<string, VertexShader> Shaders;

        public Color4 SceneAmbient;

        private VertexShader DefaultShader;

        public readonly Camera Camera;

        internal int Width, Height;

        public Renderer(int Width, int Height)
        {
            Models = new List<Model>();
            Lights = new List<Light>();

            Textures = new Dictionary<string, Texture>();
            LUTs     = new Dictionary<string, LUT>();
            Shaders  = new Dictionary<string, VertexShader>();

            int VertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);

            GL.ShaderSource(VertexShaderHandle, Resources.DefaultVertexShader);
            GL.CompileShader(VertexShaderHandle);

            Shader.CheckCompilation(VertexShaderHandle);

            DefaultShader = new VertexShader(VertexShaderHandle);

            Camera = new Camera(this);

            Resize(Width, Height);

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
        }

        public void Resize(int Width, int Height)
        {
            this.Width  = Width;
            this.Height = Height;

            GL.Viewport(0, 0, Width, Height);

            Camera.RecalculateMatrices();
        }

        public void SetBackgroundColor(Color Color)
        {
            GL.ClearColor(Color);
        }

        public void Merge(H3D Scene)
        {
            Merge(Scene.Models);
            Merge(Scene.Textures);
            Merge(Scene.LUTs);
            Merge(Scene.Lights);
            Merge(Scene.Shaders);
        }

        public void Merge(H3DDict<H3DModel> Models)
        {
            foreach (H3DModel Model in Models)
            {
                this.Models.Add(new Model(this, Model));
            }
        }

        public void Merge(H3DDict<H3DTexture> Textures)
        {
            foreach (H3DTexture Texture in Textures)
            {
                this.Textures.Add(Texture.Name, new Texture(Texture));
            }
        }

        public void Merge(H3DDict<H3DLUT> LUTs)
        {
            foreach (H3DLUT LUT in LUTs)
            {
                this.LUTs.Add(LUT.Name, new LUT(LUT));
            }
        }

        public void Merge(H3DDict<H3DLight> Lights)
        {
            foreach (H3DLight Light in Lights)
            {
                this.Lights.Add(new Light(Light));
            }
        }

        public void Merge(H3DDict<H3DShader> Shaders)
        {
            if (Shaders.Count > 0)
            {
                foreach (H3DShader Shader in Shaders)
                {
                    this.Shaders.Add(Shader.Name, new VertexShader(Shader));
                }

                UpdateAllShaders();
            }
        }

        public void DeleteAll()
        {
            DisposeAndClear(Models);
            DisposeAndClear(Textures);
            DisposeAndClear(LUTs);
            DisposeAndClear(Shaders);

            Lights.Clear();
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

        public void Render()
        {
            foreach (Model Model in Models)
            {
                Model.Render();
            }
        }

        public void UpdateAllShaders()
        {
            foreach (Model Model in Models)
            {
                Model.UpdateShaders();
            }
        }

        public void UpdateAllUniforms()
        {
            foreach (Model Model in Models)
            {
                Model.UpdateUniforms();
            }
        }

        internal bool TryBindTexture(int Unit, string TextureName)
        {
            if (TextureName != null && Textures.TryGetValue(TextureName, out Texture Texture))
            {
                Texture.Bind(Unit);

                return true;
            }

            return false;
        }

        internal bool TryBindLUT(int Unit, string TableName, string SamplerName)
        {
            if (TableName != null && LUTs.TryGetValue(TableName, out LUT LUT))
            {
                return LUT.BindSampler(Unit, SamplerName);
            }

            return false;
        }

        internal VertexShader GetShader(string ShaderName)
        {
            if (ShaderName == null || !Shaders.TryGetValue(ShaderName, out VertexShader Output))
            {
                Output = DefaultShader;
            }

            return Output;
        }

        private void DisposeAndClear<T>(List<T> Values) where T : IDisposable
        {
            foreach (T Value in Values)
            {
                Value.Dispose();
            }

            Values.Clear();
        }

        private void DisposeAndClear<T>(Dictionary<string, T> Dict) where T : IDisposable
        {
            foreach (T Value in Dict.Values)
            {
                Value.Dispose();
            }

            Dict.Clear();
        }

        private bool Disposed;

        protected virtual void Dispose(bool Disposing)
        {
            if (!Disposed)
            {
                DeleteAll();

                DefaultShader.Dispose();

                Disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
