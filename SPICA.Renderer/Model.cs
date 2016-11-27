using OpenTK;
using OpenTK.Graphics.ES30;

using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.LUT;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.CtrH3D.Model.Mesh;
using SPICA.Formats.CtrH3D.Texture;
using SPICA.PICA.Converters;
using SPICA.Renderer.Animation;
using SPICA.Renderer.SPICA_GL;

using System;
using System.Collections.Generic;

namespace SPICA.Renderer
{
    public class Model : IDisposable
    {
        private int ShaderHandle;

        public Matrix4 Transform;

        public List<Mesh> Meshes;

        private PatriciaList<H3DBone>[] Skeletons;

        private Matrix4[][] InverseTransform;
        private Matrix4[][] SkeletonTransform;

        private Dictionary<string, int> TextureIds;
        private Dictionary<string, int> LUTHandles;
        private Dictionary<string, bool> IsLUTAbs;

        public SkeletalAnim SkeletalAnimation;
        public MaterialAnim MaterialAnimation;

        //This is used to know the range of each Model
        //Each model is basically a range of meshes
        private Range[] MeshRanges;

        private int CurrModel;

        public int CurrentModelIndex
        {
            get
            {
                return CurrModel;
            }
            set
            {
                if (value < 0 || value >= MeshRanges.Length)
                {
                    throw new ArgumentOutOfRangeException(string.Format(InvalidModelIndexEx, MeshRanges.Length));
                }

                CurrModel = value;
            }
        }

        private const string InvalidModelIndexEx = "Expected a value >= 0 and < {0}!";

        public Model(H3D SceneData, int ShaderHandle)
        {
            this.ShaderHandle = ShaderHandle;

            ResetTransform();

            Meshes = new List<Mesh>();

            Skeletons = new PatriciaList<H3DBone>[SceneData.Models.Count];

            InverseTransform = new Matrix4[SceneData.Models.Count][];
            SkeletonTransform = new Matrix4[SceneData.Models.Count][];

            MeshRanges = new Range[SceneData.Models.Count];

            int MeshStart = 0;

            for (int Mdl = 0; Mdl < SceneData.Models.Count; Mdl++)
            {
                H3DModel Model = SceneData.Models[Mdl];

                Skeletons[Mdl] = Model.Skeleton;

                PatriciaList<H3DBone> Skeleton = Skeletons[Mdl];

                InverseTransform[Mdl] = new Matrix4[Skeleton.Count];
                SkeletonTransform[Mdl] = new Matrix4[Skeleton.Count];

                for (int Bone = 0; Bone < Skeleton.Count; Bone++)
                {
                    InverseTransform[Mdl][Bone] = Skeleton[Bone].InverseTransform.ToMatrix4();
                    SkeletonTransform[Mdl][Bone] = InverseTransform[Mdl][Bone].Inverted();
                }

                foreach (H3DMesh Mesh in Model.Meshes)
                {
                    Meshes.Add(new Mesh(this, Mesh, Model.Materials[Mesh.MaterialIndex], ShaderHandle));
                }

                MeshRanges[Mdl] = new Range(MeshStart, MeshStart += Model.Meshes.Count);
            }

            TextureIds = new Dictionary<string, int>();

            foreach (H3DTexture Texture in SceneData.Textures)
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

            foreach (H3DLUT LUT in SceneData.LUTs)
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

            SkeletalAnimation = new SkeletalAnim();
            MaterialAnimation = new MaterialAnim();
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
                    Min = Vertices[0].Position.ToVector3();
                    Max = Vertices[0].Position.ToVector3();

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
            UpdateSkeletonTransform();

            SkeletalAnimation.AdvanceFrame();
            MaterialAnimation.AdvanceFrame();
        }

        public void UpdateSkeletonTransform()
        {
            SkeletonTransform[CurrModel] = SkeletalAnimation.GetSkeletonTransform(Skeletons[CurrModel]);
        }

        public void ResetTransform()
        {
            Transform = Matrix4.Identity; UpdateTransform();
        }

        public void Scale(Vector3 Scale)
        {
            Transform *= Matrix4.CreateScale(Scale); UpdateTransform();
        }

        public void Rotate(Vector3 Rotation)
        {
            Transform *= Utils.EulerRotate(Rotation); UpdateTransform();
        }

        public void Translate(Vector3 Translation)
        {
            Transform *= Matrix4.CreateTranslation(Translation); UpdateTransform();
        }

        public void ScaleAbs(Vector3 Scale)
        {
            Transform = Transform.ClearScale() * Matrix4.CreateScale(Scale); UpdateTransform();
        }

        public void RotateAbs(Vector3 Rotation)
        {
            Transform = Transform.ClearRotation() * Utils.EulerRotate(Rotation); UpdateTransform();
        }

        public void TranslateAbs(Vector3 Translation)
        {
            Transform = Transform.ClearTranslation() * Matrix4.CreateTranslation(Translation); UpdateTransform();
        }

        private void UpdateTransform()
        {
            GL.UseProgram(ShaderHandle);

            GL.UniformMatrix4(GL.GetUniformLocation(ShaderHandle, "ModelMatrix"), false, ref Transform);
        }

        public void Render()
        {
            int Index;

            for (
                Index = MeshRanges[CurrModel].Start;
                Index < MeshRanges[CurrModel].End; 
                Index++)
                Meshes[Index].Render();
        }

        internal Matrix4 GetSkeletonTransform(int MeshIndex)
        {
            return SkeletonTransform[CurrModel][MeshIndex];
        }

        internal Matrix4 GetInverseTransform(int MeshIndex)
        {
            return InverseTransform[CurrModel][MeshIndex];
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
