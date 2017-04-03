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
            if (Mesh.RawBuffer.Length == 0) return new PICAVertex[0];

            PICAVertex[] Output = new PICAVertex[Mesh.RawBuffer.Length / Mesh.VertexStride];

            using (MemoryStream MS = new MemoryStream(Mesh.RawBuffer))
            {
                BinaryReader Reader = new BinaryReader(MS);

                for (int Index = 0; Index < Output.Length; Index++)
                {
                    PICAVertex Out = new PICAVertex();

                    MS.Seek(Index * Mesh.VertexStride, SeekOrigin.Begin);

                    int bi = 0;
                    int wi = 0;

                    foreach (PICAAttribute Attrib in Mesh.Attributes)
                    {
                        Vector4D Vec = new Vector4D();

                        for (int Elem = 0; Elem < Attrib.Elements; Elem++)
                        {
                            switch (Attrib.Format)
                            {
                                case PICAAttributeFormat.Byte:  Vec[Elem] = Reader.ReadSByte();  break;
                                case PICAAttributeFormat.Ubyte: Vec[Elem] = Reader.ReadByte();   break;
                                case PICAAttributeFormat.Short: Vec[Elem] = Reader.ReadInt16();  break;
                                case PICAAttributeFormat.Float: Vec[Elem] = Reader.ReadSingle(); break;
                            }
                        }

                        if (Transform)
                        {
                            Vec *= Attrib.Scale;

                            if (Attrib.Name == PICAAttributeName.Position) Vec += Mesh.PositionOffset;
                        }

                        switch (Attrib.Name)
                        {
                            case PICAAttributeName.Position:  Out.Position  = new Vector3D(Vec);  break;
                            case PICAAttributeName.Normal:    Out.Normal    = new Vector3D(Vec);  break;
                            case PICAAttributeName.Tangent:   Out.Tangent   = new Vector3D(Vec);  break;
                            case PICAAttributeName.Color:     Out.Color     = new RGBAFloat(Vec); break;
                            case PICAAttributeName.TexCoord0: Out.TexCoord0 = new Vector2D(Vec);  break;
                            case PICAAttributeName.TexCoord1: Out.TexCoord1 = new Vector2D(Vec);  break;
                            case PICAAttributeName.TexCoord2: Out.TexCoord2 = new Vector2D(Vec);  break;
                            case PICAAttributeName.BoneIndex:
                                for (int i = 0; i < Attrib.Elements && bi < 4; i++)
                                {
                                    Out.Indices[bi++] = (int)Vec[i];
                                }
                                break;
                            case PICAAttributeName.BoneWeight:
                                for (int i = 0; i < Attrib.Elements && wi < 4; i++)
                                {
                                    Out.Weights[wi++] = Vec[i];
                                }
                                break;
                        }
                    }

                    if (Mesh.FixedAttributes != null)
                    {
                        bool HasFixedIndices = Mesh.FixedAttributes.Any(x => x.Name == PICAAttributeName.BoneIndex);
                        bool HasFixedWeights = Mesh.FixedAttributes.Any(x => x.Name == PICAAttributeName.BoneWeight);

                        if (HasFixedIndices || HasFixedWeights)
                        {
                            foreach (PICAFixedAttribute Attr in Mesh.FixedAttributes)
                            {
                                switch (Attr.Name)
                                {
                                    case PICAAttributeName.BoneIndex:
                                        Out.Indices[0] = (int)Attr.Value[0];
                                        Out.Indices[1] = (int)Attr.Value[1];
                                        Out.Indices[2] = (int)Attr.Value[2];
                                        break;
                                    case PICAAttributeName.BoneWeight:
                                        Out.Weights[0] = Attr.Value[0];
                                        Out.Weights[1] = Attr.Value[1];
                                        Out.Weights[2] = Attr.Value[2];
                                        break;
                                }
                            }
                        }
                    }

                    Output[Index] = Out;
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
                    int bi = 0;
                    int wi = 0;

                    foreach (PICAAttribute Attrib in Attributes)
                    {
                        for (int i = 0; i < Attrib.Elements; i++)
                        {
                            switch (Attrib.Name)
                            {
                                case PICAAttributeName.Position:   Write(Writer, Attrib, Vertex.Position[i]);   break;
                                case PICAAttributeName.Normal:     Write(Writer, Attrib, Vertex.Normal[i]);     break;
                                case PICAAttributeName.Tangent:    Write(Writer, Attrib, Vertex.Tangent[i]);    break;
                                case PICAAttributeName.Color:      Write(Writer, Attrib, Vertex.Color[i]);      break;
                                case PICAAttributeName.TexCoord0:  Write(Writer, Attrib, Vertex.TexCoord0[i]);  break;
                                case PICAAttributeName.TexCoord1:  Write(Writer, Attrib, Vertex.TexCoord1[i]);  break;
                                case PICAAttributeName.TexCoord2:  Write(Writer, Attrib, Vertex.TexCoord2[i]);  break;
                                case PICAAttributeName.BoneIndex:  Write(Writer, Attrib, Vertex.Indices[bi++]); break;
                                case PICAAttributeName.BoneWeight: Write(Writer, Attrib, Vertex.Weights[wi++]); break;

                                default: Write(Writer, Attrib, 0); break;
                            }
                        }
                    }
                }

                return MS.ToArray();
            }
        }

        private static void Write(BinaryWriter Writer, PICAAttribute Attrib, float Value)
        {
            switch (Attrib.Format)
            {
                case PICAAttributeFormat.Byte: Writer.Write((sbyte)(Value / Attrib.Scale)); break;
                case PICAAttributeFormat.Ubyte: Writer.Write((byte)(Value / Attrib.Scale)); break;
                case PICAAttributeFormat.Short: Writer.Write((short)(Value / Attrib.Scale)); break;
                case PICAAttributeFormat.Float: Writer.Write(Value / Attrib.Scale); break;
            }
        }
    }
}
