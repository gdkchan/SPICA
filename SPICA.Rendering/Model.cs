using OpenTK;
using OpenTK.Graphics.OpenGL;

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
        internal RenderEngine Renderer;
        internal H3DModel     BaseModel;
        internal List<Mesh>   Meshes0;
        internal List<Mesh>   Meshes1;
        internal List<Mesh>   Meshes2;
        internal List<Mesh>   Meshes3;
        internal List<ShaderManager> ShaderMgrs;
        internal List<Shader> Shaders;
        internal Matrix4[]    InverseTransform;
        internal Matrix4[]    SkeletonTransform;
        internal Matrix4[][]  MaterialTransform;

        public SkeletalAnimation SkeletalAnim;
        public MaterialAnimation MaterialAnim;

        public Matrix4 Transform;

        public Model(RenderEngine Renderer, H3DModel BaseModel)
        {
            this.Renderer  = Renderer;
            this.BaseModel = BaseModel;

            Meshes0 = new List<Mesh>();
            Meshes1 = new List<Mesh>();
            Meshes2 = new List<Mesh>();
            Meshes3 = new List<Mesh>();
            ShaderMgrs = new List<ShaderManager>();
            Shaders = new List<Shader>();

            InverseTransform = new Matrix4[BaseModel.Skeleton.Count];

            for (int Bone = 0; Bone < BaseModel.Skeleton.Count; Bone++)
            {
                InverseTransform[Bone] = BaseModel.Skeleton[Bone].InverseTransform.ToMatrix4();
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

                string ShaderCode = FragmentShaderGenerator.GenFragShader(Params);

                int FragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);

                ShaderManager.CompileAndCheck(FragmentShaderHandle, ShaderCode);

                Shader Shader = Renderer.GetShader(Params.ShaderReference);

                ShaderManager ShaderMgr = new ShaderManager(
                    FragmentShaderHandle,
                    Shader?.VertexShaderHandle   ?? Renderer.VertexShaderHandle,
                    Shader?.GeometryShaderHandle ?? 0);

                ShaderMgrs.Add(ShaderMgr);
                Shaders.Add(Shader);

                GL.UseProgram(ShaderMgr.Handle);

                GL.Uniform1(GL.GetUniformLocation(ShaderMgr.Handle, "Textures[0]"), 0);
                GL.Uniform1(GL.GetUniformLocation(ShaderMgr.Handle, "Textures[1]"), 1);
                GL.Uniform1(GL.GetUniformLocation(ShaderMgr.Handle, "Textures[2]"), 2);
                GL.Uniform1(GL.GetUniformLocation(ShaderMgr.Handle, "TextureCube"), 3);
                GL.Uniform1(GL.GetUniformLocation(ShaderMgr.Handle, "LUTs[0]"),     4);
                GL.Uniform1(GL.GetUniformLocation(ShaderMgr.Handle, "LUTs[1]"),     5);
                GL.Uniform1(GL.GetUniformLocation(ShaderMgr.Handle, "LUTs[2]"),     6);
                GL.Uniform1(GL.GetUniformLocation(ShaderMgr.Handle, "LUTs[3]"),     7);
                GL.Uniform1(GL.GetUniformLocation(ShaderMgr.Handle, "LUTs[4]"),     8);
                GL.Uniform1(GL.GetUniformLocation(ShaderMgr.Handle, "LUTs[5]"),     9);

                if (Shader != null)
                {
                    //Reset ti default value
                    foreach (string Name in Shader.VertexShaderUniforms.Vec4s.Where(x => x != null))
                    {
                        ShaderMgr.SetVector4(Name, Vector4.UnitW);
                    }

                    //Send values from material matching register ids to names.
                    foreach (KeyValuePair<uint, System.Numerics.Vector4> KV in Params.VertexShaderUniforms)
                    {
                        string Name = Shader.VertexShaderUniforms.Vec4s[KV.Key];

                        if (Name != null) ShaderMgr.SetVector4(Name, KV.Value);
                    }

                    foreach (KeyValuePair<uint, System.Numerics.Vector4> KV in Params.GeometryShaderUniforms)
                    {
                        string Name = Shader.GeometryShaderUniforms.Vec4s[KV.Key];

                        if (Name != null) ShaderMgr.SetVector4(Name, KV.Value);
                    }
                }

                Vector4 MatAmbient = new Vector4(
                    Params.AmbientColor.R / 255f,
                    Params.AmbientColor.G / 255f,
                    Params.AmbientColor.B / 255F,
                    Params.ColorScale);

                Vector4 TexCoordMap = new Vector4(
                    Params.TextureSources[0],
                    Params.TextureSources[1],
                    Params.TextureSources[2],
                    Params.TextureSources[3]);

                ShaderMgr.SetVector4("is_left_render_flag", Vector4.One); //SSB needs this
                ShaderMgr.SetVector4("HslGCol", Vector4.UnitW);
                ShaderMgr.SetVector4("HslSCol", Vector4.One);
                ShaderMgr.SetVector4("HslSDir", new Vector4(0, 1, 0, 0.5f));
                ShaderMgr.SetVector4("MatAmbi", MatAmbient);
                ShaderMgr.SetVector4("MatDiff", Params.DiffuseColor.ToVector4());
                ShaderMgr.SetVector4("TexcMap", TexCoordMap);
            }
        }

        public void UpdateUniforms()
        {
            foreach (ShaderManager Shader in ShaderMgrs)
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

        public Tuple<Vector3, Vector3> GetCenterDim()
        {
            bool IsFirst = true;

            Vector3 Min = Vector3.Zero;
            Vector3 Max = Vector3.Zero;

            foreach (H3DMesh Mesh in BaseModel.Meshes)
            {
                PICAVertex[] Vertices = Mesh.ToVertices();

                if (Vertices.Length == 0) continue;

                if (IsFirst)
                {
                    Min = Max = Vertices[0].Position.ToVector4().Xyz;

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

        public void UpdateAnimationTransforms()
        {
            if (BaseModel.Meshes.Count > 0)
            {
                SkeletonTransform = SkeletalAnim.GetSkeletonTransforms(BaseModel.Skeleton);
                MaterialTransform = MaterialAnim.GetUVTransforms(BaseModel.Materials);
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
                ShaderManager ShaderMgr = ShaderMgrs[Mesh.BaseMesh.MaterialIndex];

                GL.UseProgram(ShaderMgr.Handle);

                Matrix4 NormalMatrix = Transform.ClearScale();

                Matrix4 ModelViewMatrix = Renderer.ViewMatrix * Transform;

                Matrix4 Identity = Matrix4.Identity;

                ShaderMgr.Set4x4Array("ProjMtx", ref Renderer.ProjectionMatrix);
                ShaderMgr.Set3x4Array("ViewMtx", ref ModelViewMatrix);
                ShaderMgr.Set3x4Array("NormMtx", ref NormalMatrix);
                ShaderMgr.Set3x4Array("WrldMtx", ref Identity);

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
            foreach (ShaderManager Shader in ShaderMgrs)
            {
                Shader.DetachAllShaders();
                Shader.DeleteFragmentShader();
            }

            ShaderMgrs.Clear();
            Shaders.Clear();
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
