using SPICA.Serialization.Attributes;

using System.Collections.Generic;

namespace SPICA.Formats.H3D
{
    [Inline]
    class RangeList<T>
    {
        [Range]
        public List<T> Elems;

        public T this[int Index]
        {
            get { return Elems[Index]; }
            set { Elems[Index] = value; }
        }
    }
}
