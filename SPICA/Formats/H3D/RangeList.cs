using SPICA.Serialization.Attributes;

using System.Collections;
using System.Collections.Generic;

namespace SPICA.Formats.H3D
{
    [Inline]
    class RangeList<T> : IEnumerable<T>
    {
        [Range]
        public List<T> Elems;

        public T this[int Index]
        {
            get { return Elems[Index]; }
            set { Elems[Index] = value; }
        }

        public RangeList()
        {
            Elems = new List<T>();
        }

        public int Count
        {
            get { return Elems.Count; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Elems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
