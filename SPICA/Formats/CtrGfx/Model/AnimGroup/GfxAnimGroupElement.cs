using SPICA.Formats.Common;
using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrGfx.Model.AnimGroup
{
    //TODO: Fill correct type values for all inherited types
    [TypeChoice(0x40000000u, typeof(GfxAnimGroupBone))]
    public class GfxAnimGroupElement : INamed
    {
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

        public int MemberOffset;

        public int BlendOpIndex;

        public uint ObjType;

        public uint MemberType;

        private uint MaterialPtr;
    }
}
