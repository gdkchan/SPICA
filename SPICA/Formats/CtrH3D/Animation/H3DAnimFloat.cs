using SPICA.Serialization;
using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrH3D.Animation
{
    [Inline]
    public class H3DAnimFloat : ICustomSerialization
    {
        [Ignore] private H3DFloatKeyFrameGroup _Value;

        public H3DFloatKeyFrameGroup Value { get { return _Value; } }

        public H3DAnimFloat()
        {
            _Value = new H3DFloatKeyFrameGroup();
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            H3DAnimVector.SetVector(Deserializer, _Value);
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            H3DAnimVector.WriteVector(Serializer, _Value);

            return true;
        }
    }
}
