using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.LUT;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.Formats.CtrH3D.Model.Mesh;
using SPICA.Formats.CtrH3D.Texture;
using SPICA.Math3D;
using SPICA.PICA.Commands;
using SPICA.PICA.Converters;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SPICA.Formats.Generic.WavefrontOBJ
{
    class OBJ
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

            List<Vector3D> Positions = new List<Vector3D>();
            List<Vector3D> Normals   = new List<Vector3D>();
            List<Vector2D> TexCoords = new List<Vector2D>();

            OBJMesh Mesh = new OBJMesh();

            TextReader Reader = new StreamReader(Stream);

            string Line;
            while ((Line = Reader.ReadLine()) != null)
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
                                .Add(new Vector3D
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
                            TexCoords.Add(new Vector2D
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

                            if (Indices[Index][0] != string.Empty)
                            {
                                Mesh.HasPosition = true;

                                int i = int.Parse(Indices[Index][0]);

                                Vertex.Position = Positions[GetIndex(Indices[Index][0], Positions.Count)];
                            }

                            if (Indices[Index][1] != string.Empty)
                            {
                                Mesh.HasTexCoord = true;

                                Vertex.TexCoord0 = TexCoords[GetIndex(Indices[Index][1], Normals.Count)];
                            }

                            if (Indices[Index][2] != string.Empty)
                            {
                                Mesh.HasNormal = true;

                                Vertex.Normal = Normals[GetIndex(Indices[Index][2], TexCoords.Count)];
                            }

                            Mesh.Vertices.Add(Vertex);
                        }
                        break;

                    case "usemtl":
                        if (Params.Length > 1)
                        {
                            if (Mesh.Vertices.Count > 0)
                            {
                                Meshes.Add(Mesh);

                                Mesh = new OBJMesh(Params[1]);
                            }
                            else
                            {
                                Mesh.MaterialName = Params[1];
                            }
                        }
                        break;

                    case "mtllib":
                        if (Params.Length > 1)
                        {
                            MtlFile = Params[1];
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
            public RGBAFloat Ambient;
            public RGBAFloat Diffuse;
            public RGBAFloat Specular;

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

                    string Line;
                    while ((Line = Reader.ReadLine()) != null)
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

                                    MaterialName = Params[1];
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

                                    Material.DiffuseTexture = TextureName;
                                }
                                break;

                            case "Ka":
                            case "Kd":
                            case "Ks":
                                if (Params.Length >= 4)
                                {
                                    RGBAFloat Color = new RGBAFloat
                                    {
                                        R = float.Parse(Params[1], CultureInfo.InvariantCulture),
                                        G = float.Parse(Params[2], CultureInfo.InvariantCulture),
                                        B = float.Parse(Params[3], CultureInfo.InvariantCulture),
                                        A = 1
                                    };

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

            Model.BoneScaling = H3DBoneScaling.Maya;
            Model.MeshNodesVisibility.Add(true);

            foreach (OBJMesh Mesh in Meshes)
            {
                Vector3D MinVector = new Vector3D();
                Vector3D MaxVector = new Vector3D();

                Dictionary<PICAVertex, int> Vertices = new Dictionary<PICAVertex, int>();

                List<H3DSubMesh> SubMeshes = new List<H3DSubMesh>();

                Queue<PICAVertex> VerticesQueue = new Queue<PICAVertex>();

                foreach (PICAVertex Vertex in Mesh.Vertices)
                {
                    VerticesQueue.Enqueue(Vertex.Clone());
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

                    SM.Indices = Indices.ToArray();

                    SubMeshes.Add(SM);
                }

                //Mesh
                List<PICAAttribute> Attributes = new List<PICAAttribute>();

                if (Mesh.HasPosition)
                {
                    Attributes.Add(new PICAAttribute
                    {
                        Name     = PICAAttributeName.Position,
                        Format   = PICAAttributeFormat.Float,
                        Elements = 3,
                        Scale    = 1
                    });
                }

                if (Mesh.HasNormal)
                {
                    Attributes.Add(new PICAAttribute
                    {
                        Name     = PICAAttributeName.Normal,
                        Format   = PICAAttributeFormat.Float,
                        Elements = 3,
                        Scale    = 1
                    });
                }

                if (Mesh.HasTexCoord)
                {
                    Attributes.Add(new PICAAttribute
                    {
                        Name     = PICAAttributeName.TexCoord0,
                        Format   = PICAAttributeFormat.Float,
                        Elements = 2,
                        Scale    = 1
                    });
                }

                H3DMesh M = new H3DMesh(Vertices.Keys, Attributes.ToArray(), SubMeshes);

                M.Skinning = H3DMeshSkinning.Smooth;
                M.MeshCenter = (MinVector + MaxVector) * 0.5f;
                M.MaterialIndex = MaterialIndex;

                M.UpdateBoolUniforms();

                Model.AddMesh(M);

                //Material
                H3DMaterial Material = H3DMaterial.Default;

                Material.Name = $"Mat{MaterialIndex++.ToString("D5")}_{Mesh.MaterialName}";
                Material.MaterialParams.ShaderReference = "0@DefaultShader";
                Material.MaterialParams.ModelReference = $"{Material.Name}@{Model.Name}";
                Material.MaterialParams.LUTDist0TableName = "SpecTable";
                Material.MaterialParams.LUTDist0SamplerName = "SpecSampler";

                if (Materials.ContainsKey(Mesh.MaterialName))
                    Material.Texture0Name = Materials[Mesh.MaterialName].DiffuseTexture;
                else
                    Material.Texture0Name = "NoTexture";

                Model.Materials.Add(Material);
            }

            Output.LUTs.Add(H3DLUT.CelShading);

            Output.Models.Add(Model);

            Output.CopyMaterials();

            return Output;
        }
    }
}
