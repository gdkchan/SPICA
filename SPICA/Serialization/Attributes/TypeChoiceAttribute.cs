using System;

namespace SPICA.Serialization.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    class TypeChoiceAttribute : Attribute
    {
        public uint TypeVal;
        public Type Type;

        public TypeChoiceAttribute(uint TypeVal, Type Type)
        {
            this.TypeVal = TypeVal;
            this.Type    = Type;
        }
    }
}
