using System.Collections.Generic;

namespace SPICA.Formats.CtrGfx.Model.Mesh
{
    public class GfxSubMesh
    {
        public readonly List<int> BoneIndices;

        public GfxSubMeshSkinning Skinning;

        public readonly List<GfxFace> Faces;

        public GfxSubMesh()
        {
            BoneIndices = new List<int>();

            Faces = new List<GfxFace>();
        }
    }
}
