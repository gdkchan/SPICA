using SPICA.Math3D;
using SPICA.PICA.Commands;

using System.Collections.Generic;
using System.IO;

namespace SPICA.PICA.Converters
{
    class VerticesConverter
    {
        public static PICAVertex[] GetVertices(byte[] RawBuffer, int VertexStride, PICAAttribute[] Attributes)
        {
            PICAVertex[] Output = new PICAVertex[RawBuffer.Length / VertexStride];

            using (MemoryStream MS = new MemoryStream(RawBuffer))
            {
                BinaryReader Reader = new BinaryReader(MS);

                for (int Index = 0; Index < Output.Length; Index++)
                {
                    PICAVertex O = new PICAVertex();

                    MS.Seek(Index * VertexStride, SeekOrigin.Begin);

                    foreach (PICAAttribute Attrib in Attributes)
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
                                for (int Node = 0; Node < Attrib.Elements; Node++)
                                {
                                    O.Indices[Node] = (int)V[Node];
                                }
                                break;
                            case PICAAttributeName.BoneWeight:
                                for (int Node = 0; Node < Attrib.Elements; Node++)
                                {
                                    O.Weights[Node] = V[Node];
                                }
                                break;
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
