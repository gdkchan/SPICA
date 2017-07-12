using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.LUT;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.CtrH3D.Shader;
using SPICA.Formats.CtrH3D.Texture;
using SPICA.Rendering.Shaders;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SPICA.Rendering
{
    public class RenderEngine : IDisposable
    {
        public const float ClipDistance = 100000f;

        public readonly List<Model>   Models;
        public readonly List<Texture> Textures;
        public readonly List<LUT>     LUTs;
        public readonly List<Shader>  Shaders;
        public readonly List<Light>   Lights;

        internal Matrix4 ProjectionMatrix;
        internal Matrix4 ViewMatrix;

        internal int VertexShaderHandle;

        private int Width, Height;

        private Color4 _SceneAmbient;

        public Color4 SceneAmbient
        {
            get => _SceneAmbient;
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
            Shaders  = new List<Shader>();
            Lights   = new List<Light>();

            VertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);

            GL.ShaderSource(VertexShaderHandle, BuiltInShaders.DefaultVertexShader);
            GL.CompileShader(VertexShaderHandle);

            ShaderManager.CheckCompilation(VertexShaderHandle);

            ViewMatrix = Matrix4.Identity;

            Resize(Width, Height);

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
        }

        public void Resize(int Width, int Height)
        {
            this.Width  = Width;
            this.Height = Height;

            GL.Viewport(0, 0, Width, Height);

            ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
                (float)Math.PI * 0.25f,
                (float)Width / Height,
                0.25f,
                ClipDistance);
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
            Merge(Scene.Shaders);
        }

        public void Merge(H3DDict<H3DModel> Models)
        {
            foreach (H3DModel Mdl in Models)
            {
                this.Models.Add(new Model(this, Mdl));
            }
        }

        public void Merge(H3DDict<H3DTexture> Textures)
        {
            foreach (H3DTexture Tex in Textures)
            {
                this.Textures.Add(new Texture(Tex));
            }
        }

        public void Merge(H3DDict<H3DLUT> LUTs)
        {
            foreach (H3DLUT LUT in LUTs)
            {
                this.LUTs.Add(new LUT(LUT));
            }
        }

        public void Merge(H3DDict<H3DShader> Shaders)
        {
            foreach (H3DShader Shdr in Shaders)
            {
                this.Shaders.Add(new Shader(Shdr));
            }

            UpdateAllShaders();
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

        public void SetAllTransforms(Matrix4 Transform)
        {
            foreach (Model Model in Models)
            {
                Model.Transform = Transform;
            }
        }

        public void AnimateAll()
        {
            foreach (Model Model in Models)
            {
                Model.SkeletalAnim.AdvanceFrame();
                Model.MaterialAnim.AdvanceFrame();
                Model.UpdateAnimationTransforms();
            }
        }

        public void UpdateAllUniforms()
        {
            foreach (Model Model in Models)
            {
                Model.UpdateUniforms();
            }
        }

        public void UpdateAllShaders()
        {
            foreach (Model Model in Models)
            {
                Model.UpdateShaders();
            }
        }

        internal void BindTexture(int Unit, string TextureName)
        {
            Textures.FirstOrDefault(x => x.Name == TextureName)?.Bind(Unit);
        }

        internal bool BindLUT(int Unit, string TableName, string SamplerName)
        {
            return LUTs.FirstOrDefault(x => x.Name == TableName)?.BindSampler(Unit, SamplerName) ?? false;
        }

        internal Shader GetShader(string ShaderName)
        {
            return Shaders.FirstOrDefault(x => x.Name == ShaderName);
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
