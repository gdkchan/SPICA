using System.Collections.Generic;

namespace SPICA.Formats.CtrGfx.Model.Mesh
{
    public struct GfxAttrNormal
    {
        public uint BufferObject;
        public uint LocationFlag;

        public byte[] RawBuffer;

        public uint LocationAddress;
        public uint MemoryArea;

        public int VertexStride;

        public readonly List<GfxAttribute> Attributes;
    }
}
