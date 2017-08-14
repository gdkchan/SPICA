using SPICA.Formats.Common;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace SPICA.Formats.CtrGfx
{
    [Inline]
    public class GfxDict<T> : ICustomSerialization, IPatriciaDict<T> where T : INamed
    {
        private int _Count;

        private GfxDictionary<T> Contents;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public bool IsReadOnly => false;

        public int Count => Contents.Count;

        public T this[int Index]
        {
            get => Contents[Index];
            set => Contents[Index] = value;
        }

        public T this[string Name]
        {
            get => Contents[Contents.Find(Name)];
            set => Contents[Contents.Find(Name)] = value;
        }

        public GfxDict()
        {
            Contents = new GfxDictionary<T>();
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer) { }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            _Count = Count;

            if (_Count == 0) Serializer.BaseStream.Seek(8, SeekOrigin.Current);

            return _Count == 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Contents.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction Action, T NewItem, int Index = -1)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(Action, NewItem, Index));
        }

        //Implementation
        public bool Contains(string Name)
        {
            return Contents.Contains(Name);
        }

        public bool Contains(T Value)
        {
            return Contents.Contains(Value);
        }

        public int Find(string Name)
        {
            return Contents.Find(Name);
        }

        public void CopyTo(T[] Array, int Index)
        {
            Contents.CopyTo(Array, Index);
        }

        public void Add(T Value)
        {
            Contents.Add(Value);

            OnCollectionChanged(NotifyCollectionChangedAction.Add, Value);
        }

        public void Insert(int Index, T Value)
        {
            Contents.Insert(Index, Value);

            OnCollectionChanged(NotifyCollectionChangedAction.Replace, Value, Index);
        }

        public bool Remove(T Value)
        {
            bool Removed = Contents.Remove(Value);

            OnCollectionChanged(NotifyCollectionChangedAction.Remove, Value);

            return Removed;
        }

        public void Clear()
        {
            Contents.Clear();

            OnCollectionChanged(NotifyCollectionChangedAction.Reset, default(T));
        }
    }
}
