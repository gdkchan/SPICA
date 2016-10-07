using SPICA.Serialization.Attributes;

using System.Collections.Generic;

namespace SPICA.Formats.H3D
{
    [Inline]
    class PatriciaList<T>
    {
        public List<T> Contents;
        public PatriciaTree Tree;

        public T this[int Index]
        {
            get { return Contents[Index]; }
            set { Contents[Index] = value; }
        }
    }
}
