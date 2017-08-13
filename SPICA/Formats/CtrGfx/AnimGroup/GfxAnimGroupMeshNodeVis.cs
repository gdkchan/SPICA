using SPICA.Formats.Common;

namespace SPICA.Formats.CtrGfx.AnimGroup
{
    class GfxAnimGroupMeshNodeVis : GfxAnimGroupElement
    {
        private string _NodeName;

        public string NodeName
        {
            get => _NodeName;
            set => _NodeName = value ?? throw Exceptions.GetNullException("NodeName");
        }

        private GfxAnimGroupObjType ObjType2;

        public GfxAnimGroupMeshNodeVis()
        {
            ObjType = ObjType2 = GfxAnimGroupObjType.MeshNodeVisibility;
        }
    }
}
