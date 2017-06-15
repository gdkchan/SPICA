using SPICA.Formats.Common;
using SPICA.Serialization.Attributes;

using System.Collections.Generic;
using System.Numerics;

namespace SPICA.Formats.CtrGfx.Model.Mesh
{
    public class GfxShape : INamed
    {
        public uint Unk;

        private uint MagicNumber;
        private uint Revision;

        private string _Name;

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value ?? throw Exceptions.GetNullException("Name");
            }
        }

        public readonly GfxDict<GfxMetaData> MetaData;

        private uint Flags;

        public readonly GfxBoundingBox BoundingBox;

        public Vector3 PositionOffset;

        public readonly List<GfxSubMesh> SubMeshes;

        private uint BaseAddress;

        public int AttributesCount;

        [FixedLength(1)] public readonly GfxVertexBuffer[] VertexBuffers;

        public GfxBlendShape BlendShape;

        public GfxShape()
        {
            MetaData = new GfxDict<GfxMetaData>();

            BoundingBox = new GfxBoundingBox();

            SubMeshes = new List<GfxSubMesh>();

            VertexBuffers = new GfxVertexBuffer[1];
        }
    }
}
