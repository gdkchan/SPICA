using System;

namespace SPICA.Serialization.BinaryAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    class CountOfAttribute : Attribute
    {
        public string ArrName;
        public int Increment;

        public CountOfAttribute(string ArrName, int Increment = 0)
        {
            this.ArrName = ArrName;
            this.Increment = Increment;
        }
    }
}
