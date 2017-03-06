using SPICA.Formats.Utils;

using System.Collections.Generic;
using System.IO;

namespace SPICA.Formats.MTFramework
{
    class MTModel
    {
        private List<MTMesh> Meshes;

        public MTModel()
        {
            Meshes = new List<MTMesh>();
        }

        public MTModel(BinaryReader Reader) : this()
        {
            string Magic = StringUtils.ReadPaddedString(Reader, 4);

            ushort Version        = Reader.ReadUInt16();
            ushort BonesCount     = Reader.ReadUInt16();
            ushort MeshesCount    = Reader.ReadUInt16();
            ushort MaterialsCount = Reader.ReadUInt16();

            uint VerticesBufferCount = Reader.ReadUInt32();
            uint IndicesBufferCount = Reader.ReadUInt32();
            Reader.ReadUInt32();
            uint VerticesBufferLength = Reader.ReadUInt32();
            Reader.ReadUInt32();
            Reader.ReadUInt32();
            Reader.ReadUInt32();

            uint SkeletonTransAddress = Reader.ReadUInt32();
            uint SkeletonBonesAddress = Reader.ReadUInt32();
            uint MaterialsAddress = Reader.ReadUInt32();
            uint MeshesAddress = Reader.ReadUInt32();
            uint VerticesBufferAddress = Reader.ReadUInt32();
            uint IndicesBufferAddress = Reader.ReadUInt32();

            for (int Index = 0; Index < MeshesCount; Index++)
            {

            }
        }
    }
}
