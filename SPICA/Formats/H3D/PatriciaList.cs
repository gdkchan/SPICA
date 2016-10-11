using SPICA.Serialization.Attributes;

using System.Collections.Generic;
using System.Collections;

namespace SPICA.Formats.H3D
{
    [Inline]
    class PatriciaList<T> : IEnumerable<T>
    {
        public List<T> Contents;
        public PatriciaTree Tree;

        public T this[int Index]
        {
            get { return Contents[Index]; }
            set { Contents[Index] = value; }
        }

        public PatriciaList()
        {
            Contents = new List<T>();
            Tree = new PatriciaTree();
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
