using SPICA.Formats.CtrH3D.Model.Mesh;
using SPICA.PICA.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace SPICA.PICA.Converters
{
    public static class VerticesConverter
    {
        public static PICAVertex[] GetVertices(H3DMesh Mesh)
        {
            if (Mesh.RawBuffer.Length == 0) return new PICAVertex[0];

            float[] Elems = new float[4];

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
                        AlignStream(MS, Attrib.Format);

                        for (int Elem = 0; Elem < Attrib.Elements; Elem++)
                        {
                            switch (Attrib.Format)
                            {
                                case PICAAttributeFormat.Byte:  Elems[Elem] = Reader.ReadSByte();  break;
                                case PICAAttributeFormat.Ubyte: Elems[Elem] = Reader.ReadByte();   break;
                                case PICAAttributeFormat.Short: Elems[Elem] = Reader.ReadInt16();  break;
                                case PICAAttributeFormat.Float: Elems[Elem] = Reader.ReadSingle(); break;
                            }
                        }

                        Vector4 v = new Vector4(Elems[0], Elems[1], Elems[2], Elems[3]);

                        v *= Attrib.Scale;

                        if (Attrib.Name == PICAAttributeName.Position)
                        {
                            v += Mesh.PositionOffset;
                        }

                        switch (Attrib.Name)
                        {
                            case PICAAttributeName.Position:  Out.Position  = v; break;
                            case PICAAttributeName.Normal:    Out.Normal    = v; break;
                            case PICAAttributeName.Tangent:   Out.Tangent   = v; break;
                            case PICAAttributeName.Color:     Out.Color     = v; break;
                            case PICAAttributeName.TexCoord0: Out.TexCoord0 = v; break;
                            case PICAAttributeName.TexCoord1: Out.TexCoord1 = v; break;
                            case PICAAttributeName.TexCoord2: Out.TexCoord2 = v; break;

                            case PICAAttributeName.BoneIndex:
                                Out.Indices[bi++] = (int)v.X; if (Attrib.Elements == 1) break;
                                Out.Indices[bi++] = (int)v.Y; if (Attrib.Elements == 2) break;
                                Out.Indices[bi++] = (int)v.Z; if (Attrib.Elements == 3) break;
                                Out.Indices[bi++] = (int)v.W;                           break;

                            case PICAAttributeName.BoneWeight:
                                Out.Weights[wi++] =      v.X; if (Attrib.Elements == 1) break;
                                Out.Weights[wi++] =      v.Y; if (Attrib.Elements == 2) break;
                                Out.Weights[wi++] =      v.Z; if (Attrib.Elements == 3) break;
                                Out.Weights[wi++] =      v.W;                           break;
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
                                        Out.Indices[0] = (int)Attr.Value.X;
                                        Out.Indices[1] = (int)Attr.Value.Y;
                                        Out.Indices[2] = (int)Attr.Value.Z;
                                        break;

                                    case PICAAttributeName.BoneWeight:
                                        Out.Weights[0] =      Attr.Value.X;
                                        Out.Weights[1] =      Attr.Value.Y;
                                        Out.Weights[2] =      Attr.Value.Z;
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

        public static byte[] GetBuffer(IEnumerable<PICAVertex> Vertices, IEnumerable<PICAAttribute> Attributes)
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
                        AlignStream(MS, Attrib.Format);

                        for (int i = 0; i < Attrib.Elements; i++)
                        {
                            switch (Attrib.Name)
                            {
                                case PICAAttributeName.Position:   Write(Writer, Attrib, Vertex.Position,  i);  break;
                                case PICAAttributeName.Normal:     Write(Writer, Attrib, Vertex.Normal,    i);  break;
                                case PICAAttributeName.Tangent:    Write(Writer, Attrib, Vertex.Tangent,   i);  break;
                                case PICAAttributeName.Color:      Write(Writer, Attrib, Vertex.Color,     i);  break;
                                case PICAAttributeName.TexCoord0:  Write(Writer, Attrib, Vertex.TexCoord0, i);  break;
                                case PICAAttributeName.TexCoord1:  Write(Writer, Attrib, Vertex.TexCoord1, i);  break;
                                case PICAAttributeName.TexCoord2:  Write(Writer, Attrib, Vertex.TexCoord2, i);  break;
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

        private static void Write(BinaryWriter Writer, PICAAttribute Attrib, Vector4 v, int i)
        {
            switch (i)
            {
                case 0: Write(Writer, Attrib, v.X); break;
                case 1: Write(Writer, Attrib, v.Y); break;
                case 2: Write(Writer, Attrib, v.Z); break;
                case 3: Write(Writer, Attrib, v.W); break;
            }
        }

        private static void Write(BinaryWriter Writer, PICAAttribute Attrib, float Value)
        {
            Value /= Attrib.Scale;

            if (Attrib.Format != PICAAttributeFormat.Float)
            {
                //Due to float lack of precision it's better to round the number,
                //because directly casting it will always use the lowest number that
                //may cause issues for values that float can't represent (like 0.1).
                Value = (float)Math.Round(Value);
            }

            switch (Attrib.Format)
            {
                case PICAAttributeFormat.Byte:  Writer.Write((sbyte)Value); break;
                case PICAAttributeFormat.Ubyte: Writer.Write((byte)Value);  break;
                case PICAAttributeFormat.Short: Writer.Write((short)Value); break;
                case PICAAttributeFormat.Float: Writer.Write(Value);        break;
            }
        }

        private static void AlignStream(Stream Strm, PICAAttributeFormat Fmt)
        {
            //Short and Float types needs to be aligned into 2 bytes boundaries.
            //TODO: Float may actually need a 4 bytes alignment, need to test later.
            if (Fmt != PICAAttributeFormat.Byte &&
                Fmt != PICAAttributeFormat.Ubyte)
            {
                Strm.Position += Strm.Position & 1;
            }
        }
    }
}
