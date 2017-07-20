using OpenTK;
using OpenTK.Graphics.OpenGL;

using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.Formats.CtrH3D.Model.Mesh;
using SPICA.PICA.Converters;
using SPICA.Rendering.Animation;
using SPICA.Rendering.Shaders;
using SPICA.Rendering.SPICA_GL;

using System;
using System.Collections.Generic;
using System.Linq;

namespace SPICA.Rendering
{
    public class Model : IDisposable
    {
        internal Renderer     Renderer;
        internal H3DModel     BaseModel;
        internal List<Mesh>   Meshes0;
        internal List<Mesh>   Meshes1;
        internal List<Mesh>   Meshes2;
        internal List<Mesh>   Meshes3;
        internal List<Shader> Shaders;
        internal Matrix4[]    InverseTransforms;
        internal Matrix4[]    SkeletonTransforms;
        internal Matrix4[][]  MaterialTransforms;

        public SkeletalAnimation SkeletalAnim;
        public MaterialAnimation MaterialAnim;

        public Matrix4 Transform;

        public Model(Renderer Renderer, H3DModel BaseModel)
        {
            this.Renderer  = Renderer;
            this.BaseModel = BaseModel;

            Meshes0 = new List<Mesh>();
            Meshes1 = new List<Mesh>();
            Meshes2 = new List<Mesh>();
            Meshes3 = new List<Mesh>();
            Shaders = new List<Shader>();

            InverseTransforms = new Matrix4[BaseModel.Skeleton.Count];

            for (int Bone = 0; Bone < BaseModel.Skeleton.Count; Bone++)
            {
                InverseTransforms[Bone] = BaseModel.Skeleton[Bone].InverseTransform.ToMatrix4();
            }

            UpdateShaders();

            AddMeshes(Meshes0, BaseModel.MeshesLayer0);
            AddMeshes(Meshes1, BaseModel.MeshesLayer1);
            AddMeshes(Meshes2, BaseModel.MeshesLayer2);
            AddMeshes(Meshes3, BaseModel.MeshesLayer3);

            SkeletalAnim = new SkeletalAnimation();
            MaterialAnim = new MaterialAnimation();

            Transform = Matrix4.Identity;

            UpdateAnimationTransforms();
        }

        private void AddMeshes(List<Mesh> Dst, List<H3DMesh> Src)
        {
            foreach (H3DMesh Mesh in Src)
            {
                Dst.Add(new Mesh(this, Mesh));
            }
        }

        private void DisposeMeshes(List<Mesh> Meshes)
        {
            foreach (Mesh Mesh in Meshes)
            {
                Mesh.Dispose();
            }
        }

        public void UpdateShaders()
        {
            DisposeShaders();

            foreach (H3DMaterial Material in BaseModel.Materials)
            {
                H3DMaterialParams Params = Material.MaterialParams;

                FragmentShaderGenerator FragShaderGen = new FragmentShaderGenerator(Params);

                int FragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);

                Shader.CompileAndCheck(FragmentShaderHandle, FragShaderGen.GetFragShader());

                VertexShader VtxShader = Renderer.GetShader(Params.ShaderReference);

                Shader Shdr = new Shader(FragmentShaderHandle, VtxShader);

                Shaders.Add(Shdr);

                GL.UseProgram(Shdr.Handle);

                GL.Uniform1(GL.GetUniformLocation(Shdr.Handle, "Textures[0]"), 0);
                GL.Uniform1(GL.GetUniformLocation(Shdr.Handle, "Textures[1]"), 1);
                GL.Uniform1(GL.GetUniformLocation(Shdr.Handle, "Textures[2]"), 2);
                GL.Uniform1(GL.GetUniformLocation(Shdr.Handle, "TextureCube"), 3);
                GL.Uniform1(GL.GetUniformLocation(Shdr.Handle, "LUTs[0]"),     4);
                GL.Uniform1(GL.GetUniformLocation(Shdr.Handle, "LUTs[1]"),     5);
                GL.Uniform1(GL.GetUniformLocation(Shdr.Handle, "LUTs[2]"),     6);
                GL.Uniform1(GL.GetUniformLocation(Shdr.Handle, "LUTs[3]"),     7);
                GL.Uniform1(GL.GetUniformLocation(Shdr.Handle, "LUTs[4]"),     8);
                GL.Uniform1(GL.GetUniformLocation(Shdr.Handle, "LUTs[5]"),     9);

                //Pokémon uses this
                Vector4 ShaderParam = Vector4.Zero;

                if (Params.MetaData != null)
                {
                    foreach (H3DMetaDataValue Value in Params.MetaData.Values)
                    {
                        if (Value.Type == H3DMetaDataType.Single)
                        {
                            switch (Value.Name)
                            {
                                case "$ShaderParam0": ShaderParam.W = (float)Value[0]; break;
                                case "$ShaderParam1": ShaderParam.Z = (float)Value[0]; break;
                                case "$ShaderParam2": ShaderParam.Y = (float)Value[0]; break;
                                case "$ShaderParam3": ShaderParam.X = (float)Value[0]; break;
                            }
                        }
                    }
                }

                Shdr.SetVtxVector4(85, ShaderParam);

                //Send values from material matching register ids to names.
                foreach (KeyValuePair<uint, System.Numerics.Vector4> KV in Params.VtxShaderUniforms)
                {
                    Shdr.SetVtxVector4((int)KV.Key, KV.Value.ToVector4());
                }

                foreach (KeyValuePair<uint, System.Numerics.Vector4> KV in Params.GeoShaderUniforms)
                {
                    Shdr.SetGeoVector4((int)KV.Key, KV.Value.ToVector4());
                }

                Vector4 MatAmbient = new Vector4(
                    Params.AmbientColor.R / 255f,
                    Params.AmbientColor.G / 255f,
                    Params.AmbientColor.B / 255F,
                    Params.ColorScale);

                Vector4 MatDiffuse = new Vector4(
                    Params.DiffuseColor.R / 255f,
                    Params.DiffuseColor.G / 255f,
                    Params.DiffuseColor.B / 255f,
                    1f);

                Vector4 TexCoordMap = new Vector4(
                    Params.TextureSources[0],
                    Params.TextureSources[1],
                    Params.TextureSources[2],
                    Params.TextureSources[3]);

                Shdr.SetVtxVector4(DefaultShaderIds.MatAmbi, MatAmbient);
                Shdr.SetVtxVector4(DefaultShaderIds.MatDiff, MatDiffuse);
                Shdr.SetVtxVector4(DefaultShaderIds.TexcMap, TexCoordMap);
            }

            UpdateUniforms();
        }

        public void UpdateUniforms()
        {
            foreach (Shader Shader in Shaders)
            {
                GL.UseProgram(Shader.Handle);

                int Index = 0;

                foreach (Light Light in Renderer.Lights.Where(x => x.Enabled))
                {
                    int LightPositionLocation = GL.GetUniformLocation(Shader.Handle, $"Lights[{Index}].Position");
                    int LightAmbientLocation  = GL.GetUniformLocation(Shader.Handle, $"Lights[{Index}].Ambient");
                    int LightDiffuseLocation  = GL.GetUniformLocation(Shader.Handle, $"Lights[{Index}].Diffuse");
                    int LightSpecularLocation = GL.GetUniformLocation(Shader.Handle, $"Lights[{Index}].Specular");

                    GL.Uniform3(LightPositionLocation, Renderer.Lights[Index].Position);
                    GL.Uniform4(LightAmbientLocation,  Renderer.Lights[Index].Ambient);
                    GL.Uniform4(LightDiffuseLocation,  Renderer.Lights[Index].Diffuse);
                    GL.Uniform4(LightSpecularLocation, Renderer.Lights[Index].Specular);

                    if (++Index == 8) break;
                }

                int LightsCountLocation  = GL.GetUniformLocation(Shader.Handle, "LightsCount");

                GL.Uniform1(LightsCountLocation, Index);
            }
        }

        public BoundingBox GetModelAABB()
        {
            bool IsFirst = true;

            Vector4 Min = Vector4.Zero;
            Vector4 Max = Vector4.Zero;

            foreach (H3DMesh Mesh in BaseModel.Meshes)
            {
                PICAVertex[] Vertices = Mesh.GetVertices();

                if (Vertices.Length == 0) continue;

                if (IsFirst)
                {
                    Min = Max = Vertices[0].Position.ToVector4();

                    IsFirst = false;
                }

                foreach (PICAVertex Vertex in Vertices)
                {
                    Vector4 P = Vertex.Position.ToVector4();

                    Min = Vector4.Min(Min, P);
                    Max = Vector4.Max(Max, P);
                }
            }

            return new BoundingBox(
                ((Max + Min) * 0.5f).Xyz,
                 (Max - Min).Xyz);
        }

        public void UpdateAnimationTransforms()
        {
            if (BaseModel.Meshes.Count > 0)
            {
                SkeletonTransforms = SkeletalAnim.GetSkeletonTransforms(BaseModel.Skeleton);
                MaterialTransforms = MaterialAnim.GetUVTransforms(BaseModel.Materials);
            }
        }

        public void Render()
        {
            RenderMeshes(Meshes0);
            RenderMeshes(Meshes1);
            RenderMeshes(Meshes2);
            RenderMeshes(Meshes3);
        }

        private void RenderMeshes(List<Mesh> Meshes)
        {
            foreach (Mesh Mesh in Meshes.OrderBy(x => x.BaseMesh.Priority))
            {
                Shader Shader = Shaders[Mesh.BaseMesh.MaterialIndex];

                GL.UseProgram(Shader.Handle);

                Shader.SetVtx4x4Array(DefaultShaderIds.ProjMtx, Renderer.ProjectionMatrix);
                Shader.SetVtx3x4Array(DefaultShaderIds.ViewMtx, Renderer.ViewMatrix * Transform);
                Shader.SetVtx3x4Array(DefaultShaderIds.NormMtx, Transform.ClearScale());
                Shader.SetVtx3x4Array(DefaultShaderIds.WrldMtx, Matrix4.Identity);

                Mesh.Render();
            }
        }

        private bool Disposed;

        protected virtual void Dispose(bool Disposing)
        {
            if (!Disposed)
            {
                DisposeMeshes(Meshes0);
                DisposeMeshes(Meshes1);
                DisposeMeshes(Meshes2);
                DisposeMeshes(Meshes3);

                DisposeShaders();

                Disposed = true;
            }
        }

        private void DisposeShaders()
        {
            foreach (Shader Shader in Shaders)
            {
                Shader.DetachAllShaders();
                Shader.DeleteFragmentShader();
                Shader.DeleteProgram();
            }

            Shaders.Clear();
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
