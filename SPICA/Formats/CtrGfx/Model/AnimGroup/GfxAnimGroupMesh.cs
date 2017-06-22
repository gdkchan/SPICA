namespace SPICA.Formats.CtrGfx.Model.AnimGroup
{
    class GfxAnimGroupMesh : GfxAnimGroupElement
    {
        public int MeshIndex;

        private GfxAnimGroupObjType ObjType2;

        public GfxAnimGroupMesh()
        {
            ObjType = ObjType2 = GfxAnimGroupObjType.Mesh;
        }
    }
}
