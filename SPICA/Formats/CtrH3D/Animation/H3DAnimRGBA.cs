using SPICA.Serialization;
using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrH3D.Animation
{
    public class H3DAnimRGBA : ICustomSerialization
    {
        [Ignore] private H3DFloatKeyFrameGroup[] Vector;

        public H3DFloatKeyFrameGroup R => Vector[0];
        public H3DFloatKeyFrameGroup G => Vector[1];
        public H3DFloatKeyFrameGroup B => Vector[2];
        public H3DFloatKeyFrameGroup A => Vector[3];

        public H3DAnimRGBA()
        {
            Vector = new H3DFloatKeyFrameGroup[]
            {
                new H3DFloatKeyFrameGroup(),
                new H3DFloatKeyFrameGroup(),
                new H3DFloatKeyFrameGroup(),
                new H3DFloatKeyFrameGroup()
            };
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            H3DAnimVector.SetVector(Deserializer, Vector);
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            H3DAnimVector.WriteVector(Serializer, Vector);

            return true;
        }
    }
}
