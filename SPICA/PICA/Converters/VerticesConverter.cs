using SPICA.Formats.CtrH3D.Model.Mesh;
using SPICA.Math3D;
using SPICA.PICA.Commands;

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SPICA.PICA.Converters
{
    static class VerticesConverter
    {
        public static PICAVertex[] GetVertices(H3DMesh Mesh, bool Transform)
        {
            PICAVertex[] Output = new PICAVertex[Mesh.RawBuffer.Length / Mesh.VertexStride];

            using (MemoryStream MS = new MemoryStream(Mesh.RawBuffer))
            {
                BinaryReader Reader = new BinaryReader(MS);

                for (int Index = 0; Index < Output.Length; Index++)
                {
                    PICAVertex O = new PICAVertex();

                    MS.Seek(Index * Mesh.VertexStride, SeekOrigin.Begin);

                    foreach (PICAAttribute Attrib in Mesh.Attributes)
                    {
                        Vector4D V = new Vector4D();

                        for (int Elem = 0; Elem < Attrib.Elements; Elem++)
                        {
                            switch (Attrib.Format)
                            {
                                case PICAAttributeFormat.Byte: V[Elem] = Reader.ReadSByte(); break;
                                case PICAAttributeFormat.Ubyte: V[Elem] = Reader.ReadByte(); break;
                                case PICAAttributeFormat.Short: V[Elem] = Reader.ReadInt16(); break;
                                case PICAAttributeFormat.Float: V[Elem] = Reader.ReadSingle(); break;
                            }
                        }

                        if (Transform)
                        {
                            V *= Attrib.Scale;

                            if (Attrib.Name == PICAAttributeName.Position) V -= Mesh.PositionOffset;
                        }

                        switch (Attrib.Name)
                        {
                            case PICAAttributeName.Position: O.Position = new Vector3D(V.X, V.Y, V.Z); break;

                            case PICAAttributeName.Normal: O.Normal = new Vector3D(V.X, V.Y, V.Z); break;

                            case PICAAttributeName.Tangent: O.Tangent = new Vector3D(V.X, V.Y, V.Z); break;

                            case PICAAttributeName.Color: O.Color = new RGBAFloat(V.X, V.Y, V.Z, V.W); break;

                            case PICAAttributeName.TexCoord0: O.TexCoord0 = new Vector2D(V.X, V.Y); break;
                            case PICAAttributeName.TexCoord1: O.TexCoord1 = new Vector2D(V.X, V.Y); break;
                            case PICAAttributeName.TexCoord2: O.TexCoord2 = new Vector2D(V.X, V.Y); break;

                            case PICAAttributeName.BoneIndex:
                                O.Indices[0] = (int)V[0];
                                O.Indices[1] = (int)V[1];
                                O.Indices[2] = (int)V[2];
                                O.Indices[3] = (int)V[3];
                                break;
                            case PICAAttributeName.BoneWeight:
                                O.Weights[0] = V[0];
                                O.Weights[1] = V[1];
                                O.Weights[2] = V[2];
                                O.Weights[3] = V[3];
                                break;
                        }
                    }

                    bool HasFixedIndices = Mesh.FixedAttributes.Any(x => x.Name == PICAAttributeName.BoneIndex);
                    bool HasFixedWeights = Mesh.FixedAttributes.Any(x => x.Name == PICAAttributeName.BoneWeight);

                    if (HasFixedIndices || HasFixedWeights)
                    {
                        foreach (PICAFixedAttribute Attr in Mesh.FixedAttributes)
                        {
                            switch (Attr.Name)
                            {
                                case PICAAttributeName.BoneIndex:
                                    O.Indices[0] = (int)Attr.Value[0];
                                    O.Indices[1] = (int)Attr.Value[1];
                                    O.Indices[2] = (int)Attr.Value[2];
                                    break;

                                case PICAAttributeName.BoneWeight:
                                    O.Weights[0] = Attr.Value[0];
                                    O.Weights[1] = Attr.Value[1];
                                    O.Weights[2] = Attr.Value[2];
                                    break;
                            }
                        }
                    }

                    Output[Index] = O;
                }
            }

            return Output;
        }

        public static byte[] GetBuffer(IEnumerable<PICAVertex> Vertices, PICAAttribute[] Attributes)
        {
            using (MemoryStream MS = new MemoryStream())
            {
                BinaryWriter Writer = new BinaryWriter(MS);

                foreach (PICAVertex Vertex in Vertices)
                {
                    foreach (PICAAttribute Attrib in Attributes)
                    {
                        for (int Index = 0; Index < Attrib.Elements; Index++)
                        {
                            switch (Attrib.Name)
                            {
                                case PICAAttributeName.Position: Writer.Write(Quantize(Vertex.Position[Index], Attrib)); break;

                                case PICAAttributeName.Normal: Writer.Write(Quantize(Vertex.Normal[Index], Attrib)); break;

                                case PICAAttributeName.Tangent: Writer.Write(Quantize(Vertex.Tangent[Index], Attrib)); break;

                                case PICAAttributeName.Color: Writer.Write(Quantize(Vertex.Color[Index], Attrib)); break;

                                case PICAAttributeName.TexCoord0: Writer.Write(Quantize(Vertex.TexCoord0[Index], Attrib)); break;
                                case PICAAttributeName.TexCoord1: Writer.Write(Quantize(Vertex.TexCoord1[Index], Attrib)); break;
                                case PICAAttributeName.TexCoord2: Writer.Write(Quantize(Vertex.TexCoord2[Index], Attrib)); break;

                                case PICAAttributeName.BoneIndex: Writer.Write((byte)Vertex.Indices[Index]); break;
                                case PICAAttributeName.BoneWeight: Writer.Write(Quantize(Vertex.Weights[Index], Attrib)); break;
                            }
                        }
                    }
                }

                return MS.ToArray();
            }
        }

        private static dynamic Quantize(float Value, PICAAttribute Attrib)
        {
            switch (Attrib.Format)
            {
                case PICAAttributeFormat.Byte: return (sbyte)(Value / Attrib.Scale);
                case PICAAttributeFormat.Ubyte: return (byte)(Value / Attrib.Scale);
                case PICAAttributeFormat.Short: return (short)(Value / Attrib.Scale);
                case PICAAttributeFormat.Float: return Value / Attrib.Scale;
            }

            return 0;
        }
    }
}
