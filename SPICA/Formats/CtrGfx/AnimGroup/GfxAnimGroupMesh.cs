namespace SPICA.Formats.CtrGfx.AnimGroup
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
