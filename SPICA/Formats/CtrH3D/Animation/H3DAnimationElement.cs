using SPICA.Formats.Common;
using SPICA.Serialization.Attributes;

using System;

namespace SPICA.Formats.CtrH3D.Animation
{
    public class H3DAnimationElement
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

        public H3DTargetType TargetType;

        public H3DPrimitiveType PrimitiveType;

        [TypeChoiceName("PrimitiveType")]
        [TypeChoice((uint)H3DPrimitiveType.Float,         typeof(H3DAnimFloat))]
        [TypeChoice((uint)H3DPrimitiveType.Vector2D,      typeof(H3DAnimVector2D))]
        [TypeChoice((uint)H3DPrimitiveType.Transform,     typeof(H3DAnimTransform))]
        [TypeChoice((uint)H3DPrimitiveType.QuatTransform, typeof(H3DAnimQuatTransform))]
        [TypeChoice((uint)H3DPrimitiveType.Boolean,       typeof(H3DAnimBoolean))]
        [TypeChoice((uint)H3DPrimitiveType.MtxTransform,  typeof(H3DAnimMtxTransform))]
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
    }
}
