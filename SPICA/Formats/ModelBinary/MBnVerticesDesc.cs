using SPICA.Formats.Common;
using SPICA.PICA.Commands;

using System.IO;

namespace SPICA.Formats.ModelBinary
{
    class MBnVerticesDesc
    {
        public PICAAttribute[] Attributes;

        public byte[] RawBuffer;

        public int BufferLength;
        public int SubMeshesCount;

        public MBnVerticesDesc(BinaryReader Reader, int SubMeshesCount, bool HasBuffer)
        {
            this.SubMeshesCount = SubMeshesCount;

            uint AttributesCount = Reader.ReadUInt32();

            Attributes = new PICAAttribute[AttributesCount];

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

                if ((
                    AttrFormat == MBnAttributeFormat.Ubyte ||
                    AttrFormat == MBnAttributeFormat.Byte) &&
                    Elements == 3)
                    Elements++;

                Attributes[Index] = new PICAAttribute
                {
                    Name     = AttrName.ToPICAAttributeName(),
                    Format   = AttrFormat.ToPICAAttributeFormat(),
                    Elements = Elements,
                    Scale    = Scale
                };
            }

            BufferLength = Reader.ReadInt32();

            if (HasBuffer) ReadBuffer(Reader, false);
        }

        public void ReadBuffer(BinaryReader Reader, bool NeedsAlign)
        {
            if (NeedsAlign) Reader.Align(0x20);

            RawBuffer = Reader.ReadBytes(BufferLength);

            Reader.Align(4);
        }
    }
}
