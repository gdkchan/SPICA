using System;

namespace SPICA.Serialization.BinaryAttributes
{
    [AttributeUsage(AttributeTargets.Field)]
    class PointerOfAttribute : Attribute
    {
        public string ObjName;

        public PointerOfAttribute(string ObjName)
        {
            this.ObjName = ObjName;
        }
    }
}