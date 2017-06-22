using System;

namespace SPICA.Serialization.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    class IfVersionAttribute : Attribute
    {
        public CmpOp Comparer;
        public int   Version;

        public IfVersionAttribute(CmpOp Comparer, int Version)
        {
            this.Comparer = Comparer;
            this.Version  = Version;
        }

        public bool Compare(int Version)
        {
            switch (Comparer)
            {
                case CmpOp.Equal:   return Version == this.Version;
                case CmpOp.Notqual: return Version != this.Version;
                case CmpOp.Greater: return Version >  this.Version;
                case CmpOp.Gequal:  return Version >= this.Version;
                case CmpOp.Less:    return Version <  this.Version;
                case CmpOp.Lequal:  return Version <= this.Version;

                default: throw new InvalidOperationException($"Invalid comparison operator {Comparer}!");
            }
        }
    }
}
