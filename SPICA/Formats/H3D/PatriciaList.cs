using System.Collections.Generic;
using System.Collections;

namespace SPICA.Formats.H3D
{
    struct PatriciaList<T> : IEnumerable<T> where T : INamed
    {
        public List<T> Contents;
        public PatriciaTree Tree;

        public T this[int Index]
        {
            get { return Contents[Index]; }
            set { Contents[Index] = value; }
        }

        public T this[string Key]
        {
            get { return Contents[Tree.Find(Key)]; }
            set { Contents[Tree.Find(Key)] = value; }
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

        public void Add(T Data)
        {
            (Contents ?? (Contents = new List<T>())).Add(Data); Tree.Add(((INamed)Data).ObjectName);
        }

        public void Remove(T Data)
        {
            (Contents ?? (Contents = new List<T>())).Remove(Data); Tree.Add(((INamed)Data).ObjectName);
        }

        public void Clear()
        {
            (Contents ?? (Contents = new List<T>())).Clear(); Tree.Clear();
        }
    }
}
