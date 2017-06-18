using SPICA.Formats.Common;
using SPICA.Serialization.Attributes;

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace SPICA.Formats.CtrH3D
{
    [Inline]
    public class H3DDict<T> : IPatriciaDict<T> where T : INamed
    {
        private List<T>         Values;
        private H3DPatriciaTree NameTree;

        public T this[int Index]
        {
            get
            {
                return Values[Index];
            }
            set
            {
                Values[Index] = value;
            }
        }

        public T this[string Name]
        {
            get
            {
                return Values[NameTree.Find(Name)];
            }
            set
            {
                Values[NameTree.Find(Name)] = value;
            }
        }

        public int Count { get { return Values.Count; } }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public H3DDict()
        {
            Values   = new List<T>();
            NameTree = new H3DPatriciaTree();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction Action, T NewItem, int Index = -1)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(Action, NewItem, Index));
        }

        //List management methods
        public void Add(T Value)
        {
            Values.Add(Value);

            NameTree.Add(((INamed)Value).Name);

            OnCollectionChanged(NotifyCollectionChangedAction.Add, Value);
        }

        public void Insert(int Index, T Value)
        {
            Values.Insert(Index, Value);

            NameTree.Insert(Index, ((INamed)Value).Name);

            OnCollectionChanged(NotifyCollectionChangedAction.Replace, Value, Index);
        }

        public void Remove(T Value)
        {
            Values.Remove(Value);

            NameTree.Remove(((INamed)Value).Name);

            OnCollectionChanged(NotifyCollectionChangedAction.Remove, Value);
        }

        public void Clear()
        {
            Values.Clear();

            NameTree.Clear();

            OnCollectionChanged(NotifyCollectionChangedAction.Reset, default(T));
        }

        public int FindIndex(string Name)
        {
            return NameTree.Find(Name);
        }

        public bool Contains(string Name)
        {
            return NameTree.Contains(Name);
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
