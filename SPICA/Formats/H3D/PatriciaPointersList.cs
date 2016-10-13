using SPICA.Serialization.Attributes;

using System.Collections.Generic;
using System.Collections;

namespace SPICA.Formats.H3D
{
    struct PatriciaPointersList<T> : IEnumerable<T>
    {
        [Pointers]
        public List<T> Contents;
        public PatriciaTree Tree;

        public T this[int Index]
        {
            get { return Contents[Index]; }
            set { Contents[Index] = value; }
        }

        public int Count
        {
            get { return Contents.Count; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Contents.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
