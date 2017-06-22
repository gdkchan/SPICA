using SPICA.Serialization.Attributes;

using System.Collections.Generic;

namespace SPICA.Formats.CtrGfx.Model.Mesh
{
    class GfxVertexBufferInterleaved : GfxVertexBuffer
    {
        private uint BufferObj;
        private uint LocationFlag;

        [Section((uint)GfxSectionId.Image)] public byte[] RawBuffer;

        private uint LocationPtr;
        private uint MemoryArea;

        public int VertexStride;

        public readonly List<GfxAttribute> Attributes;
    }
}
