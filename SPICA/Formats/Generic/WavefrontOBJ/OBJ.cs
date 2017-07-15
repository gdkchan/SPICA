using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.Formats.CtrH3D.Model.Mesh;
using SPICA.Formats.CtrH3D.Texture;
using SPICA.PICA.Commands;
using SPICA.PICA.Converters;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;

namespace SPICA.Formats.Generic.WavefrontOBJ
{
    public class OBJ
    {
        private string MtlFile;

        List<OBJMesh> Meshes;

        public OBJ(string FileName)
        {
            using (FileStream FS = new FileStream(FileName, FileMode.Open))
            {
                OBJModelImpl(FS);
            }
        }

        public OBJ(Stream Stream)
        {
            OBJModelImpl(Stream);
        }

        private void OBJModelImpl(Stream Stream)
        {
            Meshes = new List<OBJMesh>();

            List<Vector4> Positions = new List<Vector4>();
            List<Vector4> Normals   = new List<Vector4>();
            List<Vector4> TexCoords = new List<Vector4>();

            OBJMesh Mesh = new OBJMesh();

            TextReader Reader = new StreamReader(Stream);

            for (string Line; (Line = Reader.ReadLine()) != null;)
            {
                string[] Params = Line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                if (Params.Length == 0) continue;

                switch (Params[0])
                {
                    case "v":
                    case "vn":
                        if (Params.Length >= 4)
                        {
                            (Params[0] == "v"
                                ? Positions
                                : Normals)
                                .Add(new Vector4()
                                {
                                    X = float.Parse(Params[1], CultureInfo.InvariantCulture),
                                    Y = float.Parse(Params[2], CultureInfo.InvariantCulture),
                                    Z = float.Parse(Params[3], CultureInfo.InvariantCulture)
                                });
                        }
                        break;

                    case "vt":
                        if (Params.Length >= 3)
                        {
                            TexCoords.Add(new Vector4()
                            {
                                X = float.Parse(Params[1], CultureInfo.InvariantCulture),
                                Y = float.Parse(Params[2], CultureInfo.InvariantCulture)
                            });
                        }
                        break;

                    case "f":
                        string[][] Indices = new string[Params.Length - 1][];

                        for (int Index = 0; Index < Params.Length - 1; Index++)
                        {
                            Indices[Index] = Params[Index + 1].Split('/');
                        }

                        for (int Index = 0; Index < Indices.Length; Index++)
                        {
                            if (Index > 2)
                            {
                                Mesh.Vertices.Add(Mesh.Vertices[Mesh.Vertices.Count - 3]);
                                Mesh.Vertices.Add(Mesh.Vertices[Mesh.Vertices.Count - 2]);
                            }

                            PICAVertex Vertex = new PICAVertex();

                            if (Indices[Index].Length > 0 && Indices[Index][0] != string.Empty)
                            {
                                Mesh.HasPosition = true;

                                Vertex.Position = Positions[GetIndex(Indices[Index][0], Positions.Count)];
                            }

                            if (Indices[Index].Length > 1 && Indices[Index][1] != string.Empty)
                            {
                                Mesh.HasTexCoord = true;

                                Vertex.TexCoord0 = TexCoords[GetIndex(Indices[Index][1], Normals.Count)];
                            }

                            if (Indices[Index].Length > 2 && Indices[Index][2] != string.Empty)
                            {
                                Mesh.HasNormal = true;

                                Vertex.Normal = Normals[GetIndex(Indices[Index][2], TexCoords.Count)];
                            }
                            
                            Vertex.Weights[0] = 1;

                            Vertex.Color = Vector4.One;

                            Mesh.Vertices.Add(Vertex);
                        }
                        break;

                    case "usemtl":
                        if (Params.Length > 1)
                        {
                            string MaterialName = Line.Substring(Line.IndexOf(" ")).Trim();

                            if (Mesh.Vertices.Count > 0)
                            {
                                Meshes.Add(Mesh);

                                Mesh = new OBJMesh(MaterialName);
                            }
                            else
                            {
                                Mesh.MaterialName = MaterialName;
                            }
                        }
                        break;

                    case "mtllib":
                        string MtlLibName = Line.Substring(Line.IndexOf(" ")).Trim();

                        if (Params.Length > 1)
                        {
                            MtlFile = MtlLibName;
                        }
                        break;
                }
            }

            if (Mesh.Vertices.Count > 0) Meshes.Add(Mesh);
        }

        private int GetIndex(string Value, int Count)
        {
            int Index = int.Parse(Value);

            if (Index < 0)
                return Count + Index;
            else
                return Index - 1;
        }

        private struct OBJMaterial
        {
            public Vector4 Ambient;
            public Vector4 Diffuse;
            public Vector4 Specular;

            public string DiffuseTexture; 
        }

        public H3D ToH3D(string TextureAndMtlSearchPath = null)
        {
            H3D Output = new H3D();

            Dictionary<string, OBJMaterial> Materials = new Dictionary<string, OBJMaterial>();

            if (TextureAndMtlSearchPath != null)
            {
                string MaterialFile = Path.Combine(TextureAndMtlSearchPath, MtlFile);

                if (File.Exists(MaterialFile))
                {
                    string MaterialName = null;

                    OBJMaterial Material = default(OBJMaterial);

                    TextReader Reader = new StreamReader(MaterialFile);

                    for (string Line; (Line = Reader.ReadLine()) != null;)
                    {
                        string[] Params = Line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                        if (Params.Length == 0) continue;

                        switch (Params[0])
                        {
                            case "newmtl":
                                if (Params.Length > 1)
                                {
                                    if (MaterialName != null && Material.DiffuseTexture != null)
                                    {
                                        Materials.Add(MaterialName, Material);
                                    }

                                    Material = new OBJMaterial();

                                    MaterialName = Line.Substring(Line.IndexOf(" ")).Trim();
                                }
                                break;

                            case "map_Kd":
                                if (Params.Length > 1)
                                {
                                    string Name = Line.Substring(Line.IndexOf(Params[1]));

                                    string TextureFile = Path.Combine(TextureAndMtlSearchPath, Name);
                                    string TextureName = Path.GetFileNameWithoutExtension(TextureFile);

                                    if (File.Exists(TextureFile) && !Output.Textures.Contains(TextureName))
                                    {
                                        Output.Textures.Add(new H3DTexture(TextureFile));
                                    }

                                    Material.DiffuseTexture = Path.GetFileNameWithoutExtension(TextureName);
                                }
                                break;

                            case "Ka":
                            case "Kd":
                            case "Ks":
                                if (Params.Length >= 4)
                                {
                                    Vector4 Color = new Vector4(
                                        float.Parse(Params[1], CultureInfo.InvariantCulture),
                                        float.Parse(Params[2], CultureInfo.InvariantCulture),
                                        float.Parse(Params[3], CultureInfo.InvariantCulture), 1);

                                    switch (Params[0])
                                    {
                                        case "Ka": Material.Ambient  = Color; break;
                                        case "Kd": Material.Diffuse  = Color; break;
                                        case "Ks": Material.Specular = Color; break;
                                    }
                                }
                                break;
                        }
                    }

                    Reader.Dispose();

                    if (MaterialName != null && Material.DiffuseTexture != null)
                    {
                        Materials.Add(MaterialName, Material);
                    }
                }
            }

            H3DModel Model = new H3DModel();

            Model.Name = "Model";

            ushort MaterialIndex = 0;

            Model.Flags       = H3DModelFlags.HasSkeleton;
            Model.BoneScaling = H3DBoneScaling.Maya;
            Model.MeshNodesVisibility.Add(true);

            float Height = 0;

            foreach (OBJMesh Mesh in Meshes)
            {
                Vector3 MinVector = new Vector3();
                Vector3 MaxVector = new Vector3();

                Dictionary<PICAVertex, int> Vertices = new Dictionary<PICAVertex, int>();

                List<H3DSubMesh> SubMeshes = new List<H3DSubMesh>();

                Queue<PICAVertex> VerticesQueue = new Queue<PICAVertex>();

                foreach (PICAVertex Vertex in Mesh.Vertices)
                {
                    VerticesQueue.Enqueue(Vertex);
                }

                while (VerticesQueue.Count > 2)
                {
                    List<ushort> Indices = new List<ushort>();

                    while (VerticesQueue.Count > 0)
                    {
                        for (int Tri = 0; Tri < 3; Tri++)
                        {
                            PICAVertex Vertex = VerticesQueue.Dequeue();

                            if (Vertices.ContainsKey(Vertex))
                            {
                                Indices.Add((ushort)Vertices[Vertex]);
                            }
                            else
                            {
                                Indices.Add((ushort)Vertices.Count);

                                if (Vertex.Position.X < MinVector.X) MinVector.X = Vertex.Position.X;
                                if (Vertex.Position.Y < MinVector.Y) MinVector.Y = Vertex.Position.Y;
                                if (Vertex.Position.Z < MinVector.Z) MinVector.Z = Vertex.Position.Z;

                                if (Vertex.Position.X > MaxVector.X) MaxVector.X = Vertex.Position.X;
                                if (Vertex.Position.Y > MaxVector.Y) MaxVector.Y = Vertex.Position.Y;
                                if (Vertex.Position.Z > MaxVector.Z) MaxVector.Z = Vertex.Position.Z;

                                Vertices.Add(Vertex, Vertices.Count);
                            }
                        }
                    }

                    H3DSubMesh SM = new H3DSubMesh();

                    SM.BoneIndices = new ushort[] { 0 };
                    SM.Skinning    = H3DSubMeshSkinning.Smooth;
                    SM.Indices     = Indices.ToArray();

                    SubMeshes.Add(SM);
                }

                //Mesh
                List<PICAAttribute> Attributes = PICAAttribute.GetAttributes(
                    PICAAttributeName.Position,
                    PICAAttributeName.Normal,
                    PICAAttributeName.TexCoord0,
                    PICAAttributeName.Color,
                    PICAAttributeName.BoneIndex,
                    PICAAttributeName.BoneWeight);

                H3DMesh M = new H3DMesh(Vertices.Keys, Attributes, SubMeshes)
                {
                    Skinning      = H3DMeshSkinning.Smooth,
                    MeshCenter    = (MinVector + MaxVector) * 0.5f,
                    MaterialIndex = MaterialIndex
                };

                if (Height < MaxVector.Y)
                    Height = MaxVector.Y;

                //Material
                string MatName = $"Mat{MaterialIndex++.ToString("D5")}_{Mesh.MaterialName}";

                H3DMaterial Material = H3DMaterial.GetSimpleMaterial(Model.Name, MatName, null);

                if (Materials.ContainsKey(Mesh.MaterialName))
                    Material.Texture0Name = Materials[Mesh.MaterialName].DiffuseTexture;
                else
                    Material.Texture0Name = "NoTexture";

                Model.Materials.Add(Material);

                M.UpdateBoolUniforms(Material);

                Model.AddMesh(M);
            }

            /*
             * On Pokémon, the root bone (on the animaiton file) is used by the game to move
             * characters around, and all rigged bones are parented to this bone.
             * It's usually the Waist bone, that points upward and is half the character height.
             */
            Model.Skeleton.Add(new H3DBone(
                new Vector3(0, Height * 0.5f, 0),
                new Vector3(0, 0, (float)(Math.PI * 0.5)),
                Vector3.One,
                "Waist",
                -1));

            Model.Skeleton[0].CalculateTransform(Model.Skeleton);

            Output.Models.Add(Model);

            Output.CopyMaterials();

            return Output;
        }
    }
}
