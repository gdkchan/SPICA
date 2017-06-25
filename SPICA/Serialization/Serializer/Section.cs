using System;
using System.Collections.Generic;

namespace SPICA.Serialization.Serializer
{
    class Section
    {
        public readonly List<RefValue> Values;

        public Comparison<RefValue> Comparer;

        public object Header;

        public int Position;
        public int Length;
        public int LengthWithHeader;
        public int HeaderLength;
        public int Padding;

        public Section(int Padding = 1)
        {
            Values = new List<RefValue>();

            this.Padding = Padding;
        }

        public Section(int Padding, Comparison<RefValue> Comparer) : this(Padding)
        {
            this.Comparer = Comparer;
        }
    }
}
