using SPICA.Math3D;
using SPICA.PICA.Commands;

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

                            case PICAAttributeName.TextureCoordinate0: O.TextureCoord0 = new Vector2D(V.X, V.Y); break;
                            case PICAAttributeName.TextureCoordinate1: O.TextureCoord1 = new Vector2D(V.X, V.Y); break;
                            case PICAAttributeName.TextureCoordinate2: O.TextureCoord2 = new Vector2D(V.X, V.Y); break;

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
    }
}
