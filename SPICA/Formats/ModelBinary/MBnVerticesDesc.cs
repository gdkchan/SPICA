using SPICA.Formats.Common;
using SPICA.PICA.Commands;

using System.Collections.Generic;
using System.IO;

namespace SPICA.Formats.ModelBinary
{
    public class MBnVerticesDesc
    {
        public readonly List<PICAAttribute> Attributes;

        public byte[] RawBuffer;

        public int BufferLength;
        public int SubMeshesCount;
        public int VertexStride;

        public MBnVerticesDesc(BinaryReader Reader, int SubMeshesCount, bool HasBuffer)
        {
            this.SubMeshesCount = SubMeshesCount;

            uint AttributesCount = Reader.ReadUInt32();

            Attributes = new List<PICAAttribute>();

            for (int Index = 0; Index < AttributesCount; Index++)
            {
                MBnAttributeName   AttrName   = (MBnAttributeName)Reader.ReadUInt32();
                MBnAttributeFormat AttrFormat = (MBnAttributeFormat)Reader.ReadUInt32();

                float Scale = Reader.ReadSingle();

                int Elements = 4;

                switch (AttrName)
                {
                    case MBnAttributeName.Position:  
                    case MBnAttributeName.Normal:
                        Elements = 3; break;
                    case MBnAttributeName.TexCoord0: 
                    case MBnAttributeName.TexCoord1:
                    case MBnAttributeName.BoneIndex:
                    case MBnAttributeName.BoneWeight:
                        Elements = 2; break;
                }

                /*
                 * Byte attributes that are not aligned on a 2 bytes boundary (for example, Byte Vector3)
                 * needs to be aligned to a 2 byte boundary, so we insert a 1 byte dummy element to force alignment.
                 * Attributes of the same type doesn't need to be aligned however.
                 * For example:
                 * A Byte Vector3 Normal followed by a Byte Vector4 Color, followed by a Short Vector2 TexCoord is
                 * stored like this: NX NY NZ CR CG CB CA <Padding0> TX TX TY TY
                 */
                if (AttrFormat != MBnAttributeFormat.Ubyte &&
                    AttrFormat != MBnAttributeFormat.Byte)
                {
                    VertexStride += VertexStride & 1;
                }

                int Size = Elements;

                switch (AttrFormat)
                {
                    case MBnAttributeFormat.Short: Size <<= 1; break;
                    case MBnAttributeFormat.Float: Size <<= 2; break;
                }

                VertexStride += Size;

                Attributes.Add(new PICAAttribute()
                {
                    Name     = AttrName.ToPICAAttributeName(),
                    Format   = AttrFormat.ToPICAAttributeFormat(),
                    Elements = Elements,
                    Scale    = Scale
                });
            }

            VertexStride += VertexStride & 1;

            BufferLength = Reader.ReadInt32();

            if (HasBuffer)
            {
                ReadBuffer(Reader, false);
            }
        }

        public void ReadBuffer(BinaryReader Reader, bool NeedsAlign)
        {
            if (NeedsAlign)
            {
                Reader.Align(0x20);
            }

            RawBuffer = Reader.ReadBytes(BufferLength);

            Reader.Align(4);
        }
    }
}
