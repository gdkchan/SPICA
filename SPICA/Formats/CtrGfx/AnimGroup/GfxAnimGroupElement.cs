using SPICA.Formats.Common;
using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrGfx.AnimGroup
{
    [TypeChoice(0x00080000u, typeof(GfxAnimGroupMeshNodeVis))]
    [TypeChoice(0x01000000u, typeof(GfxAnimGroupMesh))]
    [TypeChoice(0x02000000u, typeof(GfxAnimGroupTexSampler))]
    [TypeChoice(0x04000000u, typeof(GfxAnimGroupBlendOp))]
    [TypeChoice(0x08000000u, typeof(GfxAnimGroupMaterialColor))]
    [TypeChoice(0x10000000u, typeof(GfxAnimGroupModel))]
    [TypeChoice(0x20000000u, typeof(GfxAnimGroupTexMapper))]
    [TypeChoice(0x40000000u, typeof(GfxAnimGroupBone))]
    [TypeChoice(0x80000000u, typeof(GfxAnimGroupTexCoord))]
    public class GfxAnimGroupElement : INamed
    {
        private string _Name;

        public string Name
        {
            get => _Name;
            set => _Name = value ?? throw Exceptions.GetNullException("Name");
        }

        public int MemberOffset;

        public int BlendOpIndex;

        protected GfxAnimGroupObjType ObjType;

        public uint MemberType;

        private uint MaterialPtr;
    }
}
