using SPICA.Serialization.Attributes;

using System.Collections.Generic;
using System.Collections;

namespace SPICA.Formats.H3D
{
    [Inline]
    class PatriciaList<T> : IEnumerable<T> where T : INamed
    {
        private List<T> Contents;
        public PatriciaTree NameTree;

        public T this[int Index]
        {
            get { return Contents[Index]; }
            set { Contents[Index] = value; }
        }

        public T this[string Name]
        {
            get { return Contents[FindIndex(Name)]; }
            set { Contents[FindIndex(Name)] = value; }
        }

        public int Count { get { return Contents.Count; } }

        public PatriciaList()
        {
            Contents = new List<T>();
            NameTree = new PatriciaTree();
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
        public void Add(T Value)
        {
            Contents.Add(Value);
            NameTree.Add(((INamed)Value).ObjectName);
        }

        public void Insert(int Index, T Value)
        {
            Contents.Insert(Index, Value);
            NameTree.Insert(Index, ((INamed)Value).ObjectName);
        }

        public void Remove(T Value)
        {
            Contents.Remove(Value);
            NameTree.Remove(((INamed)Value).ObjectName);
        }

        public void Clear()
        {
            Contents.Clear();
            NameTree.Clear();
        }

        public int FindIndex(string Name)
        {
            return NameTree.Find(Name);
        }

        public string FindName(int Index)
        {
            return NameTree[Index + 1].Name;
        }

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
