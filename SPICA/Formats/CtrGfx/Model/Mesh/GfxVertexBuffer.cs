using SPICA.PICA.Commands;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrGfx.Model.Mesh
{
    public class GfxVertexBuffer : ICustomSerialization
    {
        //FIXME: This is kinda hack, the serializers need to be modified to work with inheritance properly.
        public uint unk0; //Inheritance

        public PICAAttributeName AttrName;

        public uint unk2; //Type (Normal or Fixed)

        [Ignore] public GfxAttrNormal Attrib;
        [Ignore] public GfxAttrFixed AttribFixed;

        public GfxVertexBuffer()
        {

        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            if (unk2 == 2)
            {
                Attrib = Deserializer.Deserialize<GfxAttrNormal>();
            }
            else
            {
                AttribFixed = Deserializer.Deserialize<GfxAttrFixed>();
            }
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            //TODO

            return false;
        }
    }
}
