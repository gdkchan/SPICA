using SPICA.Formats.Common;

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace SPICA.Formats.CtrH3D
{
    public class H3DMetaData : IPatriciaDict<H3DMetaDataValue>
    {
        private H3DDict<H3DMetaDataValue> Values;

        public H3DMetaData()
        {
            Values = new H3DDict<H3DMetaDataValue>();
        }

        public H3DMetaDataValue this[int Index]
        {
            get => Values[Index];
            set => Values[Index] = value;
        }

        public H3DMetaDataValue this[string Name]
        {
            get => Values[Name];
            set => Values[Name] = value;
        }

        public int Count => Values.Count;

        public bool IsReadOnly => Values.IsReadOnly;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public IEnumerator<H3DMetaDataValue> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        public void Add(H3DMetaDataValue item)
        {
            Values.Add(item);
        }

        public void Insert(int Index, H3DMetaDataValue Value)
        {
            Values.Insert(Index, Value);
        }

        public bool Remove(H3DMetaDataValue item)
        {
            return Values.Remove(item);
        }

        public void Clear()
        {
            Values.Clear();
        }

        public int Find(string Name)
        {
            return Values.Find(Name);
        }

        public bool Contains(string Name)
        {
            return Values.Contains(Name);
        }

        public bool Contains(H3DMetaDataValue item)
        {
            return Values.Contains(item);
        }

        public void CopyTo(H3DMetaDataValue[] array, int arrayIndex)
        {
            Values.CopyTo(array, arrayIndex);
        }
    }
}
