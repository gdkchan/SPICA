using OpenTK;
using OpenTK.Graphics.OpenGL;
using SPICA.Formats.Common;
using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.Formats.CtrH3D.Model.Mesh;
using SPICA.PICA.Commands;
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

        private Dictionary<int, int> ShaderHashes;

        public Model(Renderer Renderer, H3DModel BaseModel)
        {
            this.Renderer  = Renderer;
            this.BaseModel = BaseModel;

            Meshes0 = new List<Mesh>();
            Meshes1 = new List<Mesh>();
            Meshes2 = new List<Mesh>();
            Meshes3 = new List<Mesh>();
            Shaders = new List<Shader>();

            ShaderHashes = new Dictionary<int, int>();

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

                int Hash = GetMaterialShaderHash(Params);

                bool HasHash = false;

                if (ShaderHashes.TryGetValue(Hash, out int ShaderIndex))
                {
                    HasHash = true;

                    H3DMaterial m = BaseModel.Materials[ShaderIndex];

                    if (CompareMaterials(m.MaterialParams, Params))
                    {
                        Shaders.Add(Shaders[ShaderIndex]);

                        continue;
                    }
                }

                if (!HasHash)
                {
                    ShaderHashes.Add(Hash, Shaders.Count);
                }

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

        private static int GetMaterialShaderHash(H3DMaterialParams Params)
        {
            FNVHash HashGen = new FNVHash();

            //TODO: Remove this once colors are passed by uniforms instead of being
            //hardcoded into fragment shader code.
            HashGen.Hash(Params.EmissionColor.GetHashCode());
            HashGen.Hash(Params.AmbientColor.GetHashCode());
            HashGen.Hash(Params.DiffuseColor.GetHashCode());
            HashGen.Hash(Params.Specular0Color.GetHashCode());
            HashGen.Hash(Params.Specular1Color.GetHashCode());
            HashGen.Hash(Params.Constant0Color.GetHashCode());
            HashGen.Hash(Params.Constant1Color.GetHashCode());
            HashGen.Hash(Params.Constant2Color.GetHashCode());
            HashGen.Hash(Params.Constant3Color.GetHashCode());
            HashGen.Hash(Params.Constant4Color.GetHashCode());
            HashGen.Hash(Params.Constant5Color.GetHashCode());

            HashGen.Hash(Params?.ShaderReference.GetHashCode() ?? 0);

            HashGen.Hash(Params.TranslucencyKind.GetHashCode());

            HashGen.Hash(Params.TexCoordConfig.GetHashCode());

            HashGen.Hash(Params.FresnelSelector.GetHashCode());

            HashGen.Hash(Params.BumpMode.GetHashCode());
            HashGen.Hash(Params.BumpTexture.GetHashCode());

            HashGen.Hash(Params.Constant0Assignment.GetHashCode());
            HashGen.Hash(Params.Constant1Assignment.GetHashCode());
            HashGen.Hash(Params.Constant2Assignment.GetHashCode());
            HashGen.Hash(Params.Constant3Assignment.GetHashCode());
            HashGen.Hash(Params.Constant4Assignment.GetHashCode());
            HashGen.Hash(Params.Constant5Assignment.GetHashCode());

            HashGen.Hash(Params.LUTInputAbsolute.Dist0.GetHashCode());
            HashGen.Hash(Params.LUTInputAbsolute.Dist1.GetHashCode());
            HashGen.Hash(Params.LUTInputAbsolute.Fresnel.GetHashCode());
            HashGen.Hash(Params.LUTInputAbsolute.ReflecR.GetHashCode());
            HashGen.Hash(Params.LUTInputAbsolute.ReflecG.GetHashCode());
            HashGen.Hash(Params.LUTInputAbsolute.ReflecB.GetHashCode());

            HashGen.Hash(Params.LUTInputSelection.Dist0.GetHashCode());
            HashGen.Hash(Params.LUTInputSelection.Dist1.GetHashCode());
            HashGen.Hash(Params.LUTInputSelection.Fresnel.GetHashCode());
            HashGen.Hash(Params.LUTInputSelection.ReflecR.GetHashCode());
            HashGen.Hash(Params.LUTInputSelection.ReflecG.GetHashCode());
            HashGen.Hash(Params.LUTInputSelection.ReflecB.GetHashCode());

            HashGen.Hash(Params.LUTInputScale.Dist0.GetHashCode());
            HashGen.Hash(Params.LUTInputScale.Dist1.GetHashCode());
            HashGen.Hash(Params.LUTInputScale.Fresnel.GetHashCode());
            HashGen.Hash(Params.LUTInputScale.ReflecR.GetHashCode());
            HashGen.Hash(Params.LUTInputScale.ReflecG.GetHashCode());
            HashGen.Hash(Params.LUTInputScale.ReflecB.GetHashCode());

            HashGen.Hash(Params.LUTDist0TableName?.GetHashCode() ?? 0);
            HashGen.Hash(Params.LUTDist1TableName?.GetHashCode() ?? 0);
            HashGen.Hash(Params.LUTFresnelTableName?.GetHashCode() ?? 0);
            HashGen.Hash(Params.LUTReflecRTableName?.GetHashCode() ?? 0);
            HashGen.Hash(Params.LUTReflecGTableName?.GetHashCode() ?? 0);
            HashGen.Hash(Params.LUTReflecBTableName?.GetHashCode() ?? 0);

            HashGen.Hash(Params.LUTDist0SamplerName?.GetHashCode() ?? 0);
            HashGen.Hash(Params.LUTDist1SamplerName?.GetHashCode() ?? 0);
            HashGen.Hash(Params.LUTFresnelSamplerName?.GetHashCode() ?? 0);
            HashGen.Hash(Params.LUTReflecRSamplerName?.GetHashCode() ?? 0);
            HashGen.Hash(Params.LUTReflecGSamplerName?.GetHashCode() ?? 0);
            HashGen.Hash(Params.LUTReflecBSamplerName?.GetHashCode() ?? 0);

            foreach (PICATexEnvStage Stage in Params.TexEnvStages)
            {
                HashGen.Hash(Stage.Source.Color[0].GetHashCode());
                HashGen.Hash(Stage.Source.Color[1].GetHashCode());
                HashGen.Hash(Stage.Source.Color[2].GetHashCode());
                HashGen.Hash(Stage.Source.Alpha[0].GetHashCode());
                HashGen.Hash(Stage.Source.Alpha[1].GetHashCode());
                HashGen.Hash(Stage.Source.Alpha[2].GetHashCode());

                HashGen.Hash(Stage.Operand.Color[0].GetHashCode());
                HashGen.Hash(Stage.Operand.Color[1].GetHashCode());
                HashGen.Hash(Stage.Operand.Color[2].GetHashCode());
                HashGen.Hash(Stage.Operand.Alpha[0].GetHashCode());
                HashGen.Hash(Stage.Operand.Alpha[1].GetHashCode());
                HashGen.Hash(Stage.Operand.Alpha[2].GetHashCode());

                HashGen.Hash(Stage.Combiner.Color.GetHashCode());
                HashGen.Hash(Stage.Combiner.Alpha.GetHashCode());

                HashGen.Hash(Stage.Scale.Color.GetHashCode());
                HashGen.Hash(Stage.Scale.Alpha.GetHashCode());

                HashGen.Hash(Stage.UpdateColorBuffer.GetHashCode());
                HashGen.Hash(Stage.UpdateAlphaBuffer.GetHashCode());
            }

            return (int)HashGen.HashCode;
        }

        private static bool CompareMaterials(H3DMaterialParams LHS, H3DMaterialParams RHS)
        {
            bool Equals = true;

            Equals &= LHS.ShaderReference == RHS.ShaderReference;

            Equals &= LHS.TranslucencyKind == RHS.TranslucencyKind;

            Equals &= LHS.TexCoordConfig == RHS.TexCoordConfig;

            Equals &= LHS.FresnelSelector == RHS.FresnelSelector;

            Equals &= LHS.BumpMode    == RHS.BumpMode;
            Equals &= LHS.BumpTexture == RHS.BumpTexture;

            Equals &= LHS.Constant0Assignment == RHS.Constant0Assignment;
            Equals &= LHS.Constant1Assignment == RHS.Constant1Assignment;
            Equals &= LHS.Constant2Assignment == RHS.Constant2Assignment;
            Equals &= LHS.Constant3Assignment == RHS.Constant3Assignment;
            Equals &= LHS.Constant4Assignment == RHS.Constant4Assignment;
            Equals &= LHS.Constant5Assignment == RHS.Constant5Assignment;

            Equals &= LHS.LUTInputAbsolute.Dist0   == RHS.LUTInputAbsolute.Dist0;
            Equals &= LHS.LUTInputAbsolute.Dist1   == RHS.LUTInputAbsolute.Dist1;
            Equals &= LHS.LUTInputAbsolute.Fresnel == RHS.LUTInputAbsolute.Fresnel;
            Equals &= LHS.LUTInputAbsolute.ReflecR == RHS.LUTInputAbsolute.ReflecR;
            Equals &= LHS.LUTInputAbsolute.ReflecG == RHS.LUTInputAbsolute.ReflecG;
            Equals &= LHS.LUTInputAbsolute.ReflecB == RHS.LUTInputAbsolute.ReflecB;

            Equals &= LHS.LUTInputSelection.Dist0   == RHS.LUTInputSelection.Dist0;
            Equals &= LHS.LUTInputSelection.Dist1   == RHS.LUTInputSelection.Dist1;
            Equals &= LHS.LUTInputSelection.Fresnel == RHS.LUTInputSelection.Fresnel;
            Equals &= LHS.LUTInputSelection.ReflecR == RHS.LUTInputSelection.ReflecR;
            Equals &= LHS.LUTInputSelection.ReflecG == RHS.LUTInputSelection.ReflecG;
            Equals &= LHS.LUTInputSelection.ReflecB == RHS.LUTInputSelection.ReflecB;

            Equals &= LHS.LUTInputScale.Dist0   == RHS.LUTInputScale.Dist0;
            Equals &= LHS.LUTInputScale.Dist1   == RHS.LUTInputScale.Dist1;
            Equals &= LHS.LUTInputScale.Fresnel == RHS.LUTInputScale.Fresnel;
            Equals &= LHS.LUTInputScale.ReflecR == RHS.LUTInputScale.ReflecR;
            Equals &= LHS.LUTInputScale.ReflecG == RHS.LUTInputScale.ReflecG;
            Equals &= LHS.LUTInputScale.ReflecB == RHS.LUTInputScale.ReflecB;

            Equals &= LHS.LUTDist0TableName   == RHS.LUTDist0TableName;
            Equals &= LHS.LUTDist1TableName   == RHS.LUTDist1TableName;
            Equals &= LHS.LUTFresnelTableName == RHS.LUTFresnelTableName;
            Equals &= LHS.LUTReflecRTableName == RHS.LUTReflecRTableName;
            Equals &= LHS.LUTReflecGTableName == RHS.LUTReflecGTableName;
            Equals &= LHS.LUTReflecBTableName == RHS.LUTReflecBTableName;

            Equals &= LHS.LUTDist0SamplerName   == RHS.LUTDist0SamplerName;
            Equals &= LHS.LUTDist1SamplerName   == RHS.LUTDist1SamplerName;
            Equals &= LHS.LUTFresnelSamplerName == RHS.LUTFresnelSamplerName;
            Equals &= LHS.LUTReflecRSamplerName == RHS.LUTReflecRSamplerName;
            Equals &= LHS.LUTReflecGSamplerName == RHS.LUTReflecGSamplerName;
            Equals &= LHS.LUTReflecBSamplerName == RHS.LUTReflecBSamplerName;

            for (int i = 0; i < 6; i++)
            {
                Equals &= LHS.TexEnvStages[i].Source.Color[0] == RHS.TexEnvStages[i].Source.Color[0];
                Equals &= LHS.TexEnvStages[i].Source.Color[1] == RHS.TexEnvStages[i].Source.Color[1];
                Equals &= LHS.TexEnvStages[i].Source.Color[2] == RHS.TexEnvStages[i].Source.Color[2];
                Equals &= LHS.TexEnvStages[i].Source.Alpha[0] == RHS.TexEnvStages[i].Source.Alpha[0];
                Equals &= LHS.TexEnvStages[i].Source.Alpha[1] == RHS.TexEnvStages[i].Source.Alpha[1];
                Equals &= LHS.TexEnvStages[i].Source.Alpha[2] == RHS.TexEnvStages[i].Source.Alpha[2];

                Equals &= LHS.TexEnvStages[i].Operand.Color[0] == RHS.TexEnvStages[i].Operand.Color[0];
                Equals &= LHS.TexEnvStages[i].Operand.Color[1] == RHS.TexEnvStages[i].Operand.Color[1];
                Equals &= LHS.TexEnvStages[i].Operand.Color[2] == RHS.TexEnvStages[i].Operand.Color[2];
                Equals &= LHS.TexEnvStages[i].Operand.Alpha[0] == RHS.TexEnvStages[i].Operand.Alpha[0];
                Equals &= LHS.TexEnvStages[i].Operand.Alpha[1] == RHS.TexEnvStages[i].Operand.Alpha[1];
                Equals &= LHS.TexEnvStages[i].Operand.Alpha[2] == RHS.TexEnvStages[i].Operand.Alpha[2];

                Equals &= LHS.TexEnvStages[i].Combiner.Color == RHS.TexEnvStages[i].Combiner.Color;
                Equals &= LHS.TexEnvStages[i].Combiner.Alpha == RHS.TexEnvStages[i].Combiner.Alpha;

                Equals &= LHS.TexEnvStages[i].Scale.Color == RHS.TexEnvStages[i].Scale.Color;
                Equals &= LHS.TexEnvStages[i].Scale.Alpha == RHS.TexEnvStages[i].Scale.Alpha;

                Equals &= LHS.TexEnvStages[i].UpdateColorBuffer == RHS.TexEnvStages[i].UpdateColorBuffer;
                Equals &= LHS.TexEnvStages[i].UpdateAlphaBuffer == RHS.TexEnvStages[i].UpdateAlphaBuffer;
            }

            return Equals;
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

            ShaderHashes.Clear();
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
