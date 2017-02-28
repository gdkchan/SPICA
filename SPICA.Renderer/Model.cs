using OpenTK;
using OpenTK.Graphics.ES30;

using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.LUT;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.Formats.CtrH3D.Model.Mesh;
using SPICA.Formats.CtrH3D.Texture;
using SPICA.PICA.Commands;
using SPICA.PICA.Converters;
using SPICA.Renderer.Animation;
using SPICA.Renderer.SPICA_GL;

using System;
using System.Collections.Generic;

namespace SPICA.Renderer
{
    public class Model : TransformableObject, IDisposable
    {
        public RenderEngine Parent;

        //Per model
        public List<Mesh> Meshes;
        public PatriciaList<H3DBone>[] Skeletons;
        public PatriciaList<H3DMaterial>[] Materials;

        //Per model mesh
        private Matrix4[][] InverseTransform;
        private Matrix4[] SkeletonTransform;
        private UVTransform[][] MaterialTransform;

        //Shared
        private Dictionary<string, int> TextureIds;
        private Dictionary<string, int> LUTHandles;
        private Dictionary<string, bool> IsLUTAbs;

        //Animation related
        public SkeletalAnim SkeletalAnimation;
        public MaterialAnim MaterialAnimation;

        private struct Range
        {
            public int Start;
            public int End;

            public Range(int Start, int End)
            {
                this.Start = Start;
                this.End   = End;
            }
        }

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

                UpdateAnimationTransforms();
            }
        }

        private const string InvalidModelIndexEx = "Expected a value >= 0 and < {0}!";

        public Model(RenderEngine Parent, H3D SceneData)
        {
            this.Parent = Parent;

            ResetTransform();

            Meshes = new List<Mesh>();

            Skeletons = new PatriciaList<H3DBone>[SceneData.Models.Count];
            Materials = new PatriciaList<H3DMaterial>[SceneData.Models.Count];

            InverseTransform = new Matrix4[SceneData.Models.Count][];

            MeshRanges = new Range[SceneData.Models.Count];

            int MeshStart = 0;

            for (int Mdl = 0; Mdl < SceneData.Models.Count; Mdl++)
            {
                H3DModel Model = SceneData.Models[Mdl];

                Skeletons[Mdl] = Model.Skeleton;
                Materials[Mdl] = Model.Materials;

                PatriciaList<H3DBone> Skeleton = Skeletons[Mdl];

                InverseTransform[Mdl] = new Matrix4[Skeleton.Count];

                for (int Bone = 0; Bone < Skeleton.Count; Bone++)
                {
                    InverseTransform[Mdl][Bone] = Skeleton[Bone].InverseTransform.ToMatrix4();
                }

                foreach (H3DMesh Mesh in Model.Meshes)
                {
                    Meshes.Add(new Mesh(this, Mesh, Model.Materials[Mesh.MaterialIndex], Parent.MdlShader.Handle));
                }

                //Mesh ranges are used to separate different models
                //Each model have a mesh start and end index
                MeshRanges[Mdl] = new Range(MeshStart, MeshStart += Model.Meshes.Count);
            }

            TextureIds = new Dictionary<string, int>();

            foreach (H3DTexture Texture in SceneData.Textures)
            {
                int TextureId = GL.GenTexture();

                if (Texture.IsCubeTexture)
                {
                    GL.BindTexture(TextureTarget.TextureCubeMap, TextureId);

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
            {
                foreach (H3DLUTSampler Sampler in LUT.Samplers)
                {
                    string Name = $"{LUT.Name}/{Sampler.Name}";
                    bool IsAbs = (Sampler.Flags & H3DLUTFlags.IsAbsolute) != 0;

                    int UBOHandle = GL.GenBuffer();

                    GL.BindBuffer(BufferTarget.UniformBuffer, UBOHandle);
                    GL.BufferData(BufferTarget.UniformBuffer, 1024, Sampler.Table, BufferUsageHint.StaticRead);
                    GL.BindBuffer(BufferTarget.UniformBuffer, 0);

                    LUTHandles.Add(Name, UBOHandle);
                    IsLUTAbs.Add(Name, IsAbs);
                }
            }

            SkeletalAnimation = new SkeletalAnim();
            MaterialAnimation = new MaterialAnim();

            UpdateAnimationTransforms();
        }

        public Tuple<Vector3, Vector3> GetCenterDim(int MdlIndex)
        {
            if (MdlIndex == -1) return Tuple.Create(Vector3.Zero, Vector3.Zero);

            bool IsFirst = true;

            Vector3 Min = Vector3.Zero;
            Vector3 Max = Vector3.Zero;

            for (int
            Index = MeshRanges[CurrModel].Start;
            Index < MeshRanges[CurrModel].End;
            Index++)
            {
                PICAVertex[] Vertices = Meshes[Index].BaseMesh.ToVertices(true);

                if (Vertices.Length == 0) continue;

                if (IsFirst)
                {
                    Min = Max = Vertices[0].Position.ToVector3();

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

            return Tuple.Create((Min + Max) * 0.5f, Max - Min);
        }

        public void Animate()
        {
            UpdateAnimationTransforms();

            SkeletalAnimation.AdvanceFrame();
            MaterialAnimation.AdvanceFrame();
        }

        public void UpdateAnimationTransforms()
        {
            if (Meshes.Count > 0)
            {
                SkeletonTransform = SkeletalAnimation.GetSkeletonTransforms(Skeletons[CurrModel]);
                MaterialTransform = MaterialAnimation.GetUVTransforms(Materials[CurrModel]);
            }
        }

        public void Render()
        {
            int MdlMtxLocation = GL.GetUniformLocation(Parent.MdlShader.Handle, "ModelMatrix");

            GL.UniformMatrix4(MdlMtxLocation, false, ref Transform);

            if (MeshRanges.Length > 0)
            {
                List<Mesh> RenderLater = new List<Mesh>(Meshes.Count);

                for (int
                Index = MeshRanges[CurrModel].Start;
                Index < MeshRanges[CurrModel].End;
                Index++)
                {
                    Meshes[Index].Render();

                    if (Meshes[Index].Material.MaterialParams.StencilTest.Enabled &&
                        Meshes[Index].Material.MaterialParams.StencilTest.Function != PICATestFunc.Always)
                    {
                        RenderLater.Add(Meshes[Index]);
                    }
                }

                /*
                 * Objects that have the Stencil Test enabled may need to be rendered twice
                 * This ensures that the Stencil buffer have the required values when a object
                 * that relies on said values to "cut off" an region will render properly,
                 * independent of the order the meshes are organized.
                 * This may have some impact on performance through.
                 */
                foreach (Mesh Mesh in RenderLater) Mesh.Render();
            }
        }

        internal Matrix4 GetInverseTransform(int MeshIndex)
        {
            return MeshIndex < InverseTransform[CurrModel].Length ? InverseTransform[CurrModel][MeshIndex] : Matrix4.Identity;
        }

        internal Matrix4 GetSkeletonTransform(int MeshIndex)
        {
            return MeshIndex < SkeletonTransform.Length ? SkeletonTransform[MeshIndex] : Matrix4.Identity;
        }

        internal UVTransform[] GetMaterialTransform(int MatIndex)
        {
            return MaterialTransform[MatIndex];
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
