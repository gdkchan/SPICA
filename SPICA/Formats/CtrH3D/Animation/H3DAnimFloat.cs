using SPICA.Serialization;
using SPICA.Serialization.Attributes;

using System;

namespace SPICA.Formats.CtrH3D.Animation
{
    public class H3DAnimFloat : ICustomSerialization
    {
        private uint Flags;

        [Ignore] public H3DFloatKeyFrameGroup Value;

        public H3DAnimFloat()
        {
            Value = new H3DFloatKeyFrameGroup();
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            bool Constant = (Flags & 0x1) != 0;
            bool Exists = (Flags & 0x100) == 0;

            if (Exists)
            {
                Value = H3DFloatKeyFrameGroup.ReadGroup(Deserializer, Constant);
            }
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            throw new NotImplementedException();
        }
    }
}
