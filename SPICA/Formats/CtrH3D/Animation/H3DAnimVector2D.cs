using SPICA.Serialization;
using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrH3D.Animation
{
    [Inline]
    public class H3DAnimVector2D : ICustomSerialization
    {
        [Ignore] private H3DFloatKeyFrameGroup[] Vector;

        public H3DFloatKeyFrameGroup X { get { return Vector[0]; } }
        public H3DFloatKeyFrameGroup Y { get { return Vector[1]; } }

        public H3DAnimVector2D()
        {
            Vector = new H3DFloatKeyFrameGroup[]
            {
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
