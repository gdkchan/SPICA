using SPICA.Formats.H3D.Model.Mesh;
using SPICA.Math3D;
using SPICA.PICA.Commands;
using SPICA.PICA.Converters;

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace SPICA.Formats.Generic.OBJ
{
    class OBJModel
    {
        List<OBJMesh> Meshes;

        public OBJModel(string FileName)
        {
            using (FileStream FS = new FileStream(FileName, FileMode.Open))
            {
                OBJModelImpl(FS);
            }
        }

        public OBJModel(Stream Stream)
        {
            OBJModelImpl(Stream);
        }

        private void OBJModelImpl(Stream Stream)
        {
            Meshes = new List<OBJMesh>();

            //TODO: Mostly broken (coded in a hurry)
            Queue<Vector3D> Positions = new Queue<Vector3D>();
            Queue<Vector3D> Normals = new Queue<Vector3D>();
            Queue<Vector2D> TexCoords = new Queue<Vector2D>();

            OBJMesh Mesh = new OBJMesh();

            TextReader Reader = new StreamReader(Stream);

            int First = 0;

            string Line;
            while ((Line = Reader.ReadLine()) != null)
            {
                string[] Params = Regex.Split(Line, "\\s+"); //TODO: Don't use RegEx

                switch (Params[0])
                {
                    case "v":
                    case "vn":
                        (Params[0] == "v" ? Positions : Normals).Enqueue(new Vector3D
                        {
                            X = float.Parse(Params[1], CultureInfo.InvariantCulture),
                            Y = float.Parse(Params[2], CultureInfo.InvariantCulture),
                            Z = float.Parse(Params[3], CultureInfo.InvariantCulture)
                        });
                        break;

                    case "vt":
                        TexCoords.Enqueue(new Vector2D
                        {
                            X = float.Parse(Params[1], CultureInfo.InvariantCulture),
                            Y = float.Parse(Params[2], CultureInfo.InvariantCulture)
                        });
                        break;

                    case "f":
                        while (Positions.Count > 0)
                        {
                            Mesh.Vertices.Add(new PICAVertex
                            {
                                Position = Positions.Dequeue(),
                                Normal = Normals.Dequeue(),
                                TextureCoord0 = TexCoords.Dequeue()
                            });
                        }

                        string[][] Idx = new string[Params.Length - 1][];

                        for (int Index = 0; Index < Params.Length - 1; Index++)
                        {
                            Idx[Index] = Params[Index + 1].Split('/');
                        }

                        if (First == 0) First = int.Parse(Idx[0][0]);

                        for (int Index = 0; Index < Params.Length - 1; Index++)
                        {
                            if (Index > 2)
                            {
                                Mesh.Indices.Add(Mesh.Indices[Mesh.Indices.Count - 3]);
                                Mesh.Indices.Add(Mesh.Indices[Mesh.Indices.Count - 2]);
                            }

                            Mesh.Indices.Add((ushort)(int.Parse(Idx[Index][0]) - First));
                        }
                        break;

                    case "g": if (Params.Length > 1) Meshes.Add(Mesh = new OBJMesh()); First = 0; break;
                }
            }
        }

        public List<H3DMesh> ToH3DMeshes()
        {
            List<H3DMesh> Output = new List<H3DMesh>();

            int Index = 0;

            foreach (OBJMesh Mesh in Meshes)
            {
                List<PICAAttribute> Attributes = new List<PICAAttribute>();

                //TODO: Simplify
                Attributes.Add(new PICAAttribute
                {
                    Name = PICAAttributeName.Position,
                    Format = PICAAttributeFormat.Float,
                    Elements = 3,
                    Scale = 1
                });

                Attributes.Add(new PICAAttribute
                {
                    Name = PICAAttributeName.Normal,
                    Format = PICAAttributeFormat.Float,
                    Elements = 3,
                    Scale = 1
                });

                Attributes.Add(new PICAAttribute
                {
                    Name = PICAAttributeName.TextureCoordinate0,
                    Format = PICAAttributeFormat.Float,
                    Elements = 2,
                    Scale = 1
                });

                H3DMesh HM = new H3DMesh(Mesh.Vertices, Attributes.ToArray(), Mesh.Indices.ToArray());

                HM.MaterialIndex = (ushort)Index++;

                Output.Add(HM);
            }

            return Output;
        }
    }
}
