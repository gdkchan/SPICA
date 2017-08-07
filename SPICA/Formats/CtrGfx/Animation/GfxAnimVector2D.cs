using SPICA.Serialization;
using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrGfx.Animation
{
    public class GfxAnimVector2D : ICustomSerialization
    {
        [Ignore] private GfxFloatKeyFrameGroup[] Vector;

        public GfxFloatKeyFrameGroup X => Vector[0];
        public GfxFloatKeyFrameGroup Y => Vector[1];

        public GfxAnimVector2D()
        {
            Vector = new GfxFloatKeyFrameGroup[]
            {
                new GfxFloatKeyFrameGroup(),
                new GfxFloatKeyFrameGroup()
            };
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            GfxAnimVector.SetVector(Deserializer, Vector);
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            GfxAnimVector.WriteVector(Serializer, Vector);

            return true;
        }
    }
}
