using SPICA.Formats.Common;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;
using SPICA.Serialization.Serializer;

using System;

namespace SPICA.Formats.CtrH3D.Animation
{
    public class H3DAnimationElement : ICustomSerialization
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

        public H3DAnimTargetType    TargetType;
        public H3DAnimPrimitiveType PrimitiveType;

        [Ignore]
        private object _Content;

        public object Content
        {
            get
            {
                return _Content;
            }
            set
            {
                Type ValueType = value.GetType();

                if (ValueType != typeof(H3DAnimVector2D)      &&
                    ValueType != typeof(H3DAnimTransform)     &&
                    ValueType != typeof(H3DAnimQuatTransform) &&
                    ValueType != typeof(H3DAnimBoolean)       &&
                    ValueType != typeof(H3DAnimMtxTransform))
                {
                    throw Exceptions.GetTypeException("Content", ValueType.ToString());
                }

                _Content = value ?? throw Exceptions.GetNullException("Content");
            }
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            switch (PrimitiveType)
            {
                case H3DAnimPrimitiveType.Vector2D:      Content = Deserializer.Deserialize<H3DAnimVector2D>();      break;
                case H3DAnimPrimitiveType.Transform:     Content = Deserializer.Deserialize<H3DAnimTransform>();     break;
                case H3DAnimPrimitiveType.QuatTransform: Content = Deserializer.Deserialize<H3DAnimQuatTransform>(); break;
                case H3DAnimPrimitiveType.Boolean:       Content = Deserializer.Deserialize<H3DAnimBoolean>();       break;
                case H3DAnimPrimitiveType.MtxTransform:  Content = Deserializer.Deserialize<H3DAnimMtxTransform>();  break;
            }
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            Serializer.Strings.Values.Add(new RefValue
            {
                Position = Serializer.BaseStream.Position,
                Value    = _Name
            });

            Serializer.Writer.Write(0u);
            Serializer.Writer.Write((ushort)TargetType);
            Serializer.Writer.Write((ushort)PrimitiveType);
            Serializer.WriteValue(Content);

            return true;
        }
    }
}
