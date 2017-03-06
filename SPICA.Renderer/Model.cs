using OpenTK;
using OpenTK.Graphics.ES30;

using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.Formats.CtrH3D.Model.Mesh;
using SPICA.PICA.Commands;
using SPICA.PICA.Converters;
using SPICA.Renderer.Animation;
using SPICA.Renderer.Shaders;
using SPICA.Renderer.SPICA_GL;

using System;
using System.Collections.Generic;
using System.Linq;

namespace SPICA.Renderer
{
    public class Model : TransformableObject, IDisposable
    {
        internal RenderEngine Renderer;
        internal H3DModel     BaseModel;
        internal List<Mesh>   Meshes;
        internal List<Shader> Shaders;
        internal Matrix4[]    InverseTransform;
        internal Matrix4[]    SkeletonTransform;
        internal Matrix3[][]  MaterialTransform;

        public SkeletalAnimation SkeletalAnim;
        public MaterialAnimation MaterialAnim;

        public Model(RenderEngine Renderer, H3DModel BaseModel)
        {
            this.Renderer  = Renderer;
            this.BaseModel = BaseModel;

            Meshes  = new List<Mesh>();
            Shaders = new List<Shader>();

            InverseTransform = new Matrix4[BaseModel.Skeleton.Count];

            for (int Bone = 0; Bone < BaseModel.Skeleton.Count; Bone++)
            {
                InverseTransform[Bone] = BaseModel.Skeleton[Bone].InverseTransform.ToMatrix4();
            }

            foreach (H3DMaterial Material in BaseModel.Materials)
            {
                Shader Shader = new Shader();

                H3DMaterialParams Params = Material.MaterialParams;

                string ShaderCode = ShaderGenerator.GenFragShader(Params, Renderer.FragmentBaseCode);

                Shader.SetVertexShaderHandle(Renderer.VertexShaderHandle);
                Shader.SetFragmentShaderCode(ShaderCode);
                Shader.Link();

                Shaders.Add(Shader);

                GL.UseProgram(Shader.Handle);

                GL.Uniform1(GL.GetUniformLocation(Shader.Handle, "Textures[0]"), 0);
                GL.Uniform1(GL.GetUniformLocation(Shader.Handle, "Textures[1]"), 1);
                GL.Uniform1(GL.GetUniformLocation(Shader.Handle, "Textures[2]"), 2);
                GL.Uniform1(GL.GetUniformLocation(Shader.Handle, "TextureCube"), 3);
                GL.Uniform1(GL.GetUniformLocation(Shader.Handle, "LUTs[0]"),     4);
                GL.Uniform1(GL.GetUniformLocation(Shader.Handle, "LUTs[1]"),     5);
                GL.Uniform1(GL.GetUniformLocation(Shader.Handle, "LUTs[2]"),     6);
                GL.Uniform1(GL.GetUniformLocation(Shader.Handle, "LUTs[3]"),     7);
                GL.Uniform1(GL.GetUniformLocation(Shader.Handle, "LUTs[4]"),     8);
                GL.Uniform1(GL.GetUniformLocation(Shader.Handle, "LUTs[5]"),     9);

                Vector4 PowerScale = Vector4.Zero;

                if (Params.MetaData != null)
                {
                    //Only Pokémon uses this (for custom Rim lighting and Phong shading on the shaders)
                    foreach (H3DMetaDataValue MetaData in Params.MetaData.Values)
                    {
                        if (MetaData.Type == H3DMetaDataType.Single && MetaData.Values.Count > 0)
                        {
                            float Value = (float)MetaData.Values[0];

                            switch (MetaData.Name)
                            {
                                case "$RimPow":     PowerScale[0] = Value; break;
                                case "$RimScale":   PowerScale[1] = Value; break;
                                case "$PhongPow":   PowerScale[2] = Value; break;
                                case "$PhongScale": PowerScale[3] = Value; break;
                            }
                        }
                    }
                }

                int MDiffuseLocation   = GL.GetUniformLocation(Shader.Handle, "MDiffuse");
                int PowerScaleLocation = GL.GetUniformLocation(Shader.Handle, "PowerScale");
                int ColorScaleLocation = GL.GetUniformLocation(Shader.Handle, "ColorScale");
                int BumpModeLocation   = GL.GetUniformLocation(Shader.Handle, "BumpMode");

                GL.Uniform4(MDiffuseLocation,   Params.DiffuseColor.ToColor4());
                GL.Uniform4(PowerScaleLocation, PowerScale);
                GL.Uniform1(ColorScaleLocation, Params.ColorScale);
                GL.Uniform1(BumpModeLocation,   (int)Params.BumpMode);
            }

            foreach (H3DMesh Mesh in BaseModel.Meshes)
            {
                int ShaderHandle = Shaders[Mesh.MaterialIndex].Handle;

                Meshes.Add(new Mesh(this, Mesh, ShaderHandle));
            }

            SkeletalAnim = new SkeletalAnimation();
            MaterialAnim = new MaterialAnimation();

            ResetTransform();
            UpdateAnimationTransforms();
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

                int ObjNormalMapLocation = GL.GetUniformLocation(Shader.Handle, "ObjNormalMap");
                int LightsCountLocation  = GL.GetUniformLocation(Shader.Handle, "LightsCount");

                GL.Uniform1(ObjNormalMapLocation, Renderer.ObjectSpaceNormalMap ? 1 : 0);
                GL.Uniform1(LightsCountLocation, Index);
            }
        }

        public Tuple<Vector3, Vector3> GetCenterDim()
        {
            bool IsFirst = true;

            Vector3 Min = Vector3.Zero;
            Vector3 Max = Vector3.Zero;

            foreach (Mesh Mesh in Meshes)
            {
                PICAVertex[] Vertices = Mesh.BaseMesh.ToVertices(true);

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

            SkeletalAnim.AdvanceFrame();
            MaterialAnim.AdvanceFrame();
        }

        public void UpdateAnimationTransforms()
        {
            if (Meshes.Count > 0)
            {
                SkeletonTransform = SkeletalAnim.GetSkeletonTransforms(BaseModel.Skeleton);
                MaterialTransform = MaterialAnim.GetUVTransforms(BaseModel.Materials);
            }
        }

        public void Render()
        {
            List<Mesh> RenderLater = new List<Mesh>(Meshes.Count);

            foreach (Mesh Mesh in Meshes)
            {
                GL.UseProgram(Mesh.ShaderHandle);

                int ProjMtxLocation = GL.GetUniformLocation(Mesh.ShaderHandle, "ProjMatrix");
                int ViewMtxLocation = GL.GetUniformLocation(Mesh.ShaderHandle, "ViewMatrix");
                int MdlMtxLocation  = GL.GetUniformLocation(Mesh.ShaderHandle, "ModelMatrix");

                GL.UniformMatrix4(ProjMtxLocation, false, ref Renderer.ProjectionMatrix);
                GL.UniformMatrix4(ViewMtxLocation, false, ref Renderer.Transform);
                GL.UniformMatrix4(MdlMtxLocation,  false, ref Transform);

                Mesh.Render();

                if (Mesh.BaseMesh.MaterialIndex < BaseModel.Materials.Count)
                {
                    H3DMaterial Material = BaseModel.Materials[Mesh.BaseMesh.MaterialIndex];

                    if (Material.MaterialParams.StencilTest.Enabled &&
                        Material.MaterialParams.StencilTest.Function != PICATestFunc.Always)
                    {
                        RenderLater.Add(Mesh);
                    }
                }
            }

            /*
             * Objects that have the Stencil Test enabled may need to be rendered twice.
             * This ensures that the Stencil buffer have the required values when a object.
             * that relies on said values to "cut off" an region will render properly,
             * independent of the order the meshes are organized.
             * This may have some impact on performance through.
             */
            foreach (Mesh Mesh in RenderLater)
            {
                Mesh.Render();
            }
        }

        private bool Disposed;

        protected virtual void Dispose(bool Disposing)
        {
            if (!Disposed)
            {
                foreach (Mesh Mesh in Meshes)
                {
                    Mesh.Dispose();
                }

                foreach (Shader Shader in Shaders)
                {
                    Shader.Dispose();
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
