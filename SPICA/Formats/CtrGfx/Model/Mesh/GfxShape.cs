using SPICA.Formats.Common;
using SPICA.Serialization.Attributes;

using System.Collections.Generic;
using System.Numerics;

namespace SPICA.Formats.CtrGfx.Model.Mesh
{
    [TypeChoice(0x10000001u, typeof(GfxShape))]
    public class GfxShape : GfxObject
    {
        private uint Flags;

        public readonly GfxBoundingBox BoundingBox;

        public Vector3 PositionOffset;

        public readonly List<GfxSubMesh> SubMeshes;

        private uint BaseAddress;

        public readonly List<GfxVertexBuffer> VertexBuffers;

        public GfxBlendShape BlendShape;

        public GfxShape()
        {
            BoundingBox = new GfxBoundingBox();

            SubMeshes = new List<GfxSubMesh>();

            VertexBuffers = new List<GfxVertexBuffer>();
        }
    }
}
