using System;

namespace SPICA.Serialization.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    class TypeChoiceAttribute : Attribute
    {
        public new uint TypeId;
        public     Type Type;

        public TypeChoiceAttribute(uint TypeId, Type Type)
        {
            this.TypeId = TypeId;
            this.Type   = Type;
        }
    }
}
