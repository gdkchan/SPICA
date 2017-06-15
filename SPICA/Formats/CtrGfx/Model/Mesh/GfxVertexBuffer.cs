using SPICA.Formats.Common;

using System.Collections.Generic;

namespace SPICA.Formats.CtrGfx.Model.Mesh
{
    public class GfxVertexBuffer
    {
        private uint unk0;
        private uint unk1;
        private uint unk2;

        public uint BufferObject;
        public uint LocationFlag;

        public byte[] RawBuffer;

        public uint LocationAddress;
        public uint MemoryArea;

        public int VertexStride;

        public readonly List<GfxAttribute> Attributes;

        public GfxVertexBuffer()
        {
            Attributes = new List<GfxAttribute>();
        }
    }
}
