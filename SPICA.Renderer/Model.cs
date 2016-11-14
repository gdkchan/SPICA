using OpenTK;
using OpenTK.Graphics.ES30;

using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Animation;
using SPICA.Formats.CtrH3D.LUT;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.Formats.CtrH3D.Model.Mesh;
using SPICA.Formats.CtrH3D.Texture;
using SPICA.PICA.Converters;
using SPICA.Renderer.SPICA_GL;

using System;
using System.Collections.Generic;

namespace SPICA.Renderer
{
    public class Model : IDisposable
    {
        private RenderEngine Parent;

        public Matrix4 Transform;

        public List<Mesh> Meshes;

        public PatriciaList<H3DMaterial> Materials;
        private PatriciaList<H3DBone> Skeleton;

        internal Matrix4[] InverseTransform;
        internal Matrix4[] SkeletonTransform;

        private Dictionary<string, int> TextureIds;
        private Dictionary<string, int> LUTHandles;
        private Dictionary<string, bool> IsLUTAbs;

        public Animation SkeletalAnimation;

        public Model(RenderEngine Renderer, H3D Model, int ModelIndex, int ShaderHandle)
        {
            Parent = Renderer;

            UpdateView(Transform = Matrix4.Identity);

            Meshes = new List<Mesh>();

            Materials = Model.Models[ModelIndex].Materials;
            Skeleton = Model.Models[ModelIndex].Skeleton;

            InverseTransform = new Matrix4[Skeleton.Count];
            SkeletonTransform = new Matrix4[Skeleton.Count];

            for (int Index = 0; Index < Skeleton.Count; Index++)
            {
                InverseTransform[Index] = GLConverter.ToMatrix4(Skeleton[Index].InverseTransform);
                SkeletonTransform[Index] = InverseTransform[Index].Inverted();
            }

            foreach (H3DMesh Mesh in Model.Models[ModelIndex].Meshes)
            {
                Meshes.Add(new Mesh(this, Mesh, ShaderHandle));
            }

            TextureIds = new Dictionary<string, int>();

            foreach (H3DTexture Texture in Model.Textures)
            {
                int TextureId = GL.GenTexture();

                if (Texture.IsCubeTexture)
                {
                    GL.BindTexture(TextureTarget.TextureCubeMap, TextureId);
                    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                    for (int Face = 0; Face < 6; Face++)
                    {
                        GL.TexImage2D(TextureTarget2d.TextureCubeMapPositiveX + Face,
                            0,
                            TextureComponentCount.Rgba,
                            (int)Texture.Width,
                            (int)Texture.Height,
                            0,
                            PixelFormat.Rgba,
                            PixelType.UnsignedByte,
                            Texture.ToRGBA(Face));
                    }
                }
                else
                {
                    GL.BindTexture(TextureTarget.Texture2D, TextureId);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                    GL.TexImage2D(TextureTarget2d.Texture2D,
                        0,
                        TextureComponentCount.Rgba,
                        (int)Texture.Width,
                        (int)Texture.Height,
                        0,
                        PixelFormat.Rgba,
                        PixelType.UnsignedByte,
                        Texture.ToRGBA());
                }

                TextureIds.Add(Texture.Name, TextureId);
            }

            LUTHandles = new Dictionary<string, int>();
            IsLUTAbs = new Dictionary<string, bool>();

            foreach (H3DLUT LUT in Model.LUTs)
            {
                foreach (H3DLUTSampler Sampler in LUT.Samplers)
                {
                    string Name = LUT.Name + "/" + Sampler.Name;
                    bool IsAbs = (Sampler.Flags & H3DLUTFlags.IsAbsolute) != 0;

                    int UBOHandle = GL.GenBuffer();

                    GL.BindBuffer(BufferTarget.UniformBuffer, UBOHandle);
                    GL.BufferData(BufferTarget.UniformBuffer, 1024, Sampler.Table, BufferUsageHint.StaticRead);
                    GL.BindBuffer(BufferTarget.UniformBuffer, 0);

                    LUTHandles.Add(Name, UBOHandle);
                    IsLUTAbs.Add(Name, IsAbs);
                }
            }
        }

        public Vector3 GetMassCenter()
        {
            Vector3 Center = Vector3.Zero;

            foreach (Mesh Mesh in Meshes)
            {
                Center += Mesh.MeshCenter;
            }

            Center /= Meshes.Count;

            return Center;
        }

        public Tuple<Vector3, float> GetCenterMaxXY()
        {
            bool IsFirst = true;

            Vector3 Min = Vector3.Zero;
            Vector3 Max = Vector3.Zero;

            foreach (Mesh Mesh in Meshes)
            {
                PICAVertex[] Vertices = Mesh.BaseMesh.ToVertices();

                if (Vertices.Length == 0) continue;

                if (IsFirst)
                {
                    Min = GLConverter.ToVector3(Vertices[0].Position);
                    Max = GLConverter.ToVector3(Vertices[0].Position);

                    IsFirst = false;
                }

                foreach (PICAVertex Vertex in Vertices)
                {
                    Min.X = Math.Min(Min.X, Vertex.Position.X);
                    Min.Y = Math.Min(Min.Y, Vertex.Position.Y);
                    Min.Z = Math.Min(Min.Z, Vertex.Position.Z);

                    Max.X = Math.Max(Max.X, Vertex.Position.X);
                    Max.Y = Math.Max(Max.Y, Vertex.Position.Y);
                    Max.Z = Math.Max(Max.Z, Vertex.Position.Z);
                }
            }

            float MaxX = Math.Max(Math.Abs(Min.X), Math.Abs(Max.X));
            float MaxY = Math.Max(Math.Abs(Min.Y), Math.Abs(Max.Y));

            return Tuple.Create((Min + Max) * 0.5f, Math.Max(MaxX, MaxY));
        }

        public void Animate()
        {
            SkeletonTransform = SkeletalAnimation.GetSkeletonTransform(Skeleton);
        }

        public void SetSkeletalAnimation(H3DAnimation BaseAnimation, float Step = 1)
        {
            SkeletalAnimation = new Animation(BaseAnimation, 0, Step);
        }

        public void Scale(Vector3 Scale)
        {
            UpdateView(Transform *= Matrix4.CreateScale(Scale));
        }

        public void Rotate(Vector3 Rotation)
        {
            UpdateView(Transform *= Utils.EulerRotate(Rotation));
        }

        public void Translate(Vector3 Translation)
        {
            UpdateView(Transform *= Matrix4.CreateTranslation(Translation));
        }

        public void ScaleAbs(Vector3 Scale)
        {
            UpdateView(Transform = (Transform.ClearScale() * Matrix4.CreateScale(Scale)));
        }

        public void RotateAbs(Vector3 Rotation)
        {
            UpdateView(Transform = (Transform.ClearRotation() * Utils.EulerRotate(Rotation)));
        }

        public void TranslateAbs(Vector3 Translation)
        {
            UpdateView(Transform = (Transform.ClearTranslation() * Matrix4.CreateTranslation(Translation)));
        }

        private void UpdateView(Matrix4 Mtx)
        {
            GL.UniformMatrix4(Parent.ModelMtxLocation, false, ref Transform);
        }

        public void Render()
        {
            foreach (Mesh Mesh in Meshes) Mesh.Render();
        }

        internal int GetTextureId(string Name)
        {
            return TextureIds.ContainsKey(Name) ? TextureIds[Name] : -1;
        }

        internal int GetLUTHandle(string Name)
        {
            return LUTHandles.ContainsKey(Name) ? LUTHandles[Name] : -1;
        }

        internal bool GetIsLUTAbs(string Name)
        {
            return IsLUTAbs.ContainsKey(Name) ? IsLUTAbs[Name] : false;
        }

        private bool Disposed;

        protected virtual void Dispose(bool Disposing)
        {
            if (!Disposed)
            {
                foreach (int Handle in TextureIds.Values) GL.DeleteTexture(Handle);
                foreach (int Handle in LUTHandles.Values) GL.DeleteBuffer(Handle);

                foreach (Mesh Mesh in Meshes) Mesh.Dispose();

                Disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
