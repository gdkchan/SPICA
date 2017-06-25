using SPICA.PICA.Commands;

using System.IO;
using System.Numerics;

namespace SPICA.Formats.CtrGfx.Model.Mesh
{
    public class GfxAttribute : GfxVertexBuffer
    {
        private uint BufferObj;
        private uint LocationFlag;

        public byte[] RawBuffer;

        private uint LocationPtr;
        private uint MemoryArea;

        public GfxGLDataType Format;

        public int Elements;

        public float Scale;

        public int Offset;

        public Vector4[] GetVectors()
        {
            if (RawBuffer == null) return null;

            int Length = RawBuffer.Length / Elements;

            switch (Format)
            {
                case GfxGLDataType.GL_SHORT: Length >>= 1; break;
                case GfxGLDataType.GL_FLOAT: Length >>= 2; break;
            }

            Vector4[] Output = new Vector4[Length];

            using (MemoryStream MS = new MemoryStream(RawBuffer))
            {
                BinaryReader Reader = new BinaryReader(MS);

                for (int i = 0; i < Output.Length; i++)
                {
                    for (int j = 0; j < Elements; j++)
                    {
                        float Value = 0;

                        switch (Format)
                        {
                            case GfxGLDataType.GL_BYTE:          Value = Reader.ReadSByte();  break;
                            case GfxGLDataType.GL_UNSIGNED_BYTE: Value = Reader.ReadByte();   break;
                            case GfxGLDataType.GL_SHORT:         Value = Reader.ReadInt16();  break;
                            case GfxGLDataType.GL_FLOAT:         Value = Reader.ReadSingle(); break;
                        }

                        Value *= Scale;

                        switch (j)
                        {
                            case 0: Output[i].X = Value; break;
                            case 1: Output[i].Y = Value; break;
                            case 2: Output[i].Z = Value; break;
                            case 3: Output[i].W = Value; break;
                        }
                    }
                }
            }

            return Output;
        }

        public PICAAttribute ToPICAAttribute()
        {
            return new PICAAttribute()
            {
                Name     = AttrName,
                Format   = Format.ToPICAAttributeFormat(),
                Elements = Elements,
                Scale    = Scale
            };
        }
    }
}
