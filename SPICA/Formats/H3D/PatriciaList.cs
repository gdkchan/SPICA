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

        //List management methods
        public void Add(T Data)
        {
            if (Data != null)
            {
                (Contents ?? (Contents = new List<T>())).Add(Data);

                Tree.Add(((INamed)Data).ObjectName);
            }
        }

        public void Insert(int Index, T Data)
        {
            if (Data != null)
            {
                (Contents ?? (Contents = new List<T>())).Insert(Index, Data);

                Tree.Insert(Index, ((INamed)Data).ObjectName);
            }
        }

        public void Remove(T Data)
        {
            if (Contents != null && Data != null)
            {
                Contents.Remove(Data);

                Tree.Remove(((INamed)Data).ObjectName);
            }
        }

        public void Clear()
        {
            if (Contents != null)
            {
                Contents.Clear();

                Tree.Clear();
            }
        }

        //Extended methods
        public void Remove(int Index)
        {
            Remove(this[Index]);
        }

        public void Remove(string Name)
        {
            Remove(this[Name]);
        }
    }
}
