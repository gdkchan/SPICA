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
        internal Renderer        Renderer;
        internal H3DModel        BaseModel;
        internal List<Mesh>      Meshes0;
        internal List<Mesh>      Meshes1;
        internal List<Mesh>      Meshes2;
        internal List<Mesh>      Meshes3;
        internal List<Shader>    Shaders;
        internal Matrix4[]       InverseTransforms;
        internal Matrix4[]       SkeletonTransforms;
        internal MaterialState[] MaterialStates;
        internal bool[]          Visibilities;

        public readonly SkeletalAnimation   SkeletalAnim;
        public readonly MaterialAnimation   MaterialAnim;
        public readonly VisibilityAnimation VisibilityAnim;

        private Dictionary<int, int> ShaderHashes;

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

            SkeletalAnim   = new SkeletalAnimation(BaseModel.Skeleton);
            MaterialAnim   = new MaterialAnimation(BaseModel.Materials);
            VisibilityAnim = new VisibilityAnimation(
                BaseModel.MeshNodesTree,
                BaseModel.MeshNodesVisibility);

            Transform = Matrix4.Identity;

            UpdateAnimationTransforms();
        }

        private void AddMeshes(List<Mesh> Dst, List<H3DMesh> Src)
        {
            foreach (H3DMesh Mesh in Src.OrderBy(x => x.Priority))
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

                for (int i = 0; i < 3; i++)
                {
                    int j = i * 2;

                    GL.Uniform1(GL.GetUniformLocation(Shdr.Handle, $"LUTs[{6 + j}]"), 10 + j);
                    GL.Uniform1(GL.GetUniformLocation(Shdr.Handle, $"LUTs[{7 + j}]"), 11 + j);
                }

                //Pokémon uses this
                Vector4 ShaderParam = Vector4.Zero;

                if (Params.MetaData != null)
                {
                    foreach (H3DMetaDataValue MD in Params.MetaData)
                    {
                        if (MD.Type == H3DMetaDataType.Single)
                        {
                            switch (MD.Name)
                            {
                                case "$ShaderParam0": ShaderParam.W = (float)MD.Values[0]; break;
                                case "$ShaderParam1": ShaderParam.Z = (float)MD.Values[0]; break;
                                case "$ShaderParam2": ShaderParam.Y = (float)MD.Values[0]; break;
                                case "$ShaderParam3": ShaderParam.X = (float)MD.Values[0]; break;
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
            FNV1a HashGen = new FNV1a();

            HashGen.Hash(Params.ShaderReference?.GetHashCode() ?? 0);

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

            HashGen.Hash(Params.TexEnvBufferColor.R.GetHashCode());
            HashGen.Hash(Params.TexEnvBufferColor.G.GetHashCode());
            HashGen.Hash(Params.TexEnvBufferColor.B.GetHashCode());
            HashGen.Hash(Params.TexEnvBufferColor.A.GetHashCode());

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

            Equals &= LHS.TexEnvBufferColor.R == RHS.TexEnvBufferColor.R;
            Equals &= LHS.TexEnvBufferColor.G == RHS.TexEnvBufferColor.G;
            Equals &= LHS.TexEnvBufferColor.B == RHS.TexEnvBufferColor.B;
            Equals &= LHS.TexEnvBufferColor.A == RHS.TexEnvBufferColor.A;

            return Equals;
        }

        public void UpdateUniforms()
        {
            foreach (Shader Shader in Shaders)
            {
                GL.UseProgram(Shader.Handle);

                int fi = 0;

                for (int i = 0; i < Renderer.Lights.Count; i++)
                {
                    if (!Renderer.Lights[i].Enabled) continue;

                    switch (Renderer.Lights[i].Type)
                    {
                        case LightType.PerFragment:
                            if (fi < 3)
                            {
                                SetFragmentLight(Shader, Renderer.Lights[i], fi++);
                            }
                            break;
                    }
                }

                int LightsCountLocation  = GL.GetUniformLocation(Shader.Handle, "LightsCount");

                GL.Uniform1(LightsCountLocation, fi);
            }
        }

        private void SetFragmentLight(Shader Shader, Light Light, int fi)
        {
            int PositionLocation     = GL.GetUniformLocation(Shader.Handle, $"Lights[{fi}].Position");
            int DirectionLocation    = GL.GetUniformLocation(Shader.Handle, $"Lights[{fi}].Direction");
            int AmbientLocation      = GL.GetUniformLocation(Shader.Handle, $"Lights[{fi}].Ambient");
            int DiffuseLocation      = GL.GetUniformLocation(Shader.Handle, $"Lights[{fi}].Diffuse");
            int Specular0Location    = GL.GetUniformLocation(Shader.Handle, $"Lights[{fi}].Specular0");
            int Specular1Location    = GL.GetUniformLocation(Shader.Handle, $"Lights[{fi}].Specular1");
            int LUTInputLocation     = GL.GetUniformLocation(Shader.Handle, $"Lights[{fi}].AngleLUTInput");
            int LUTScaleLocation     = GL.GetUniformLocation(Shader.Handle, $"Lights[{fi}].AngleLUTScale");
            int AttScaleLocation     = GL.GetUniformLocation(Shader.Handle, $"Lights[{fi}].AttScale");
            int AttBiasLocation      = GL.GetUniformLocation(Shader.Handle, $"Lights[{fi}].AttBias");
            int DistAttEnbLocation   = GL.GetUniformLocation(Shader.Handle, $"Lights[{fi}].DistAttEnb");
            int TwoSidedDiffLocation = GL.GetUniformLocation(Shader.Handle, $"Lights[{fi}].TwoSidedDiff");
            int DirectionalLocation  = GL.GetUniformLocation(Shader.Handle, $"Lights[{fi}].Directional");

            GL.Uniform3(PositionLocation,     Light.Position);
            GL.Uniform3(DirectionLocation,    Light.Direction);
            GL.Uniform4(AmbientLocation,      Light.Ambient);
            GL.Uniform4(DiffuseLocation,      Light.Diffuse);
            GL.Uniform4(Specular0Location,    Light.Specular0);
            GL.Uniform4(Specular1Location,    Light.Specular1);
            GL.Uniform1(LUTInputLocation,     Light.AngleLUTInput);
            GL.Uniform1(LUTScaleLocation,     Light.AngleLUTScale);
            GL.Uniform1(AttScaleLocation,     Light.AttenuationScale);
            GL.Uniform1(AttBiasLocation,      Light.AttenuationBias);
            GL.Uniform1(DistAttEnbLocation,   Light.DistAttEnabled  ? 1 : 0);
            GL.Uniform1(TwoSidedDiffLocation, Light.TwoSidedDiffuse ? 1 : 0);
            GL.Uniform1(DirectionalLocation,  Light.Directional     ? 1 : 0);

            Renderer.TryBindLUT(10 + fi * 2,
                Light.AngleLUTTableName,
                Light.AngleLUTSamplerName);

            Renderer.TryBindLUT(11 + fi * 2,
                Light.DistanceLUTTableName,
                Light.DistanceLUTSamplerName);
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
                SkeletonTransforms = SkeletalAnim.GetSkeletonTransforms();

                MaterialStates = MaterialAnim.GetMaterialStates();

                Visibilities = VisibilityAnim.GetMeshVisibilities();
            }
        }

        public void Render()
        {
            RenderMeshes(Meshes0);
            RenderMeshes(Meshes1);
            RenderMeshes(Meshes2);
            RenderMeshes(Meshes3);
        }

        private void RenderMeshes(IEnumerable<Mesh> Meshes)
        {
            foreach (Mesh Mesh in Meshes)
            {
                int n = Mesh.BaseMesh.NodeIndex;

                if (n < Visibilities.Length && !Visibilities[n])
                {
                    continue;
                }

                Shader Shader = Shaders[Mesh.BaseMesh.MaterialIndex];

                GL.UseProgram(Shader.Handle);

                Shader.SetVtx4x4Array(DefaultShaderIds.ProjMtx, Renderer.Camera.ProjectionMatrix);
                Shader.SetVtx3x4Array(DefaultShaderIds.ViewMtx, Renderer.Camera.ViewMatrix * Transform);
                Shader.SetVtx3x4Array(DefaultShaderIds.NormMtx, Transform.ClearScale());
                Shader.SetVtx3x4Array(DefaultShaderIds.WrldMtx, Matrix4.Identity);

                int MaterialIndex = Mesh.BaseMesh.MaterialIndex;

                H3DMaterialParams MP = BaseModel.Materials[MaterialIndex].MaterialParams;

                MaterialState MS = MaterialStates[Mesh.BaseMesh.MaterialIndex];

                Vector4 MatAmbient = new Vector4(
                    MS.Ambient.R,
                    MS.Ambient.G,
                    MS.Ambient.B,
                    MP.ColorScale);

                Vector4 MatDiffuse = new Vector4(
                    MS.Diffuse.R,
                    MS.Diffuse.G,
                    MS.Diffuse.B,
                    MS.Diffuse.A);

                Shader.SetVtxVector4(DefaultShaderIds.MatAmbi, MatAmbient);
                Shader.SetVtxVector4(DefaultShaderIds.MatDiff, MatDiffuse);
                Shader.SetVtx3x4Array(DefaultShaderIds.TexMtx0, MS.Transforms[0]);
                Shader.SetVtx3x4Array(DefaultShaderIds.TexMtx1, MS.Transforms[1]);
                Shader.SetVtx2x4Array(DefaultShaderIds.TexMtx2, MS.Transforms[2]);

                Shader.SetVtxVector4(DefaultShaderIds.TexTran, new Vector4(
                    MS.Transforms[0].Row3.X,
                    MS.Transforms[0].Row3.Y,
                    MS.Transforms[1].Row3.X,
                    MS.Transforms[1].Row3.Y));

                GL.Uniform4(GL.GetUniformLocation(Shader.Handle, FragmentShaderGenerator.EmissionUniform),  MS.Emission);
                GL.Uniform4(GL.GetUniformLocation(Shader.Handle, FragmentShaderGenerator.AmbientUniform),   MS.Ambient);
                GL.Uniform4(GL.GetUniformLocation(Shader.Handle, FragmentShaderGenerator.DiffuseUniform),   MS.Diffuse);
                GL.Uniform4(GL.GetUniformLocation(Shader.Handle, FragmentShaderGenerator.Specular0Uniform), MS.Specular0);
                GL.Uniform4(GL.GetUniformLocation(Shader.Handle, FragmentShaderGenerator.Specular1Uniform), MS.Specular1);
                GL.Uniform4(GL.GetUniformLocation(Shader.Handle, FragmentShaderGenerator.Constant0Uniform), MS.Constant0);
                GL.Uniform4(GL.GetUniformLocation(Shader.Handle, FragmentShaderGenerator.Constant1Uniform), MS.Constant1);
                GL.Uniform4(GL.GetUniformLocation(Shader.Handle, FragmentShaderGenerator.Constant2Uniform), MS.Constant2);
                GL.Uniform4(GL.GetUniformLocation(Shader.Handle, FragmentShaderGenerator.Constant3Uniform), MS.Constant3);
                GL.Uniform4(GL.GetUniformLocation(Shader.Handle, FragmentShaderGenerator.Constant4Uniform), MS.Constant4);
                GL.Uniform4(GL.GetUniformLocation(Shader.Handle, FragmentShaderGenerator.Constant5Uniform), MS.Constant5);

                Mesh.Texture0Name = MS.Texture0Name;
                Mesh.Texture1Name = MS.Texture1Name;
                Mesh.Texture2Name = MS.Texture2Name;

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
