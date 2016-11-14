using SPICA.Serialization;

using System;

namespace SPICA.Formats.CtrH3D.Animation
{
    public class H3DAnimationElement : ICustomSerialization
    {
        public string Name;

        public H3DAnimTargetType TargetType;
        public H3DAnimPrimitiveType PrimitiveType;

        [NonSerialized] public object Content;

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            switch (PrimitiveType)
            {
                case H3DAnimPrimitiveType.Transform: Content = Deserializer.Deserialize<H3DAnimTransform>(); break;
                case H3DAnimPrimitiveType.QuatTransform: Content = Deserializer.Deserialize<H3DAnimQuatTransform>(); break;
            }
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            throw new NotImplementedException();
        }
    }
}
