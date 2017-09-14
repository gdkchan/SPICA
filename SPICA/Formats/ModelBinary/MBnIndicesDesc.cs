using SPICA.Formats.Common;

using System.IO;

namespace SPICA.Formats.ModelBinary
{
    public class MBnIndicesDesc
    {
        public ushort[] BoneIndices;
        public ushort[] Indices;

        public uint PrimitivesCount;

        public MBnIndicesDesc(BinaryReader Reader, bool HasBuffer)
        {
            uint BoneIndicesCount = Reader.ReadUInt32();

            BoneIndices = new ushort[BoneIndicesCount];

            for (int Index = 0; Index < BoneIndicesCount; Index++)
            {
                BoneIndices[Index] = (ushort)Reader.ReadUInt32();
            }

            PrimitivesCount = Reader.ReadUInt32();

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

            Indices = new ushort[PrimitivesCount];

            for (int Index = 0; Index < PrimitivesCount; Index++)
            {
                Indices[Index] = Reader.ReadUInt16();
            }

            Reader.Align(4);
        }
    }
}
