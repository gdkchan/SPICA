using SPICA.Formats.Common;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

using System.Collections;
using System.Collections.Generic;

namespace SPICA.Formats.CtrGfx
{
    [Inline]
    public class GfxDict<T> : ICustomSerialization, IEnumerable<T> where T : INamed
    {
        private int _Count;

        private GfxDictionary<T> Contents;

        public int Count { get { return Contents.Count; } }

        public T this[int Index]
        {
            get
            {
                return Contents[Index];
            }
            set
            {
                Contents[Index] = value;
            }
        }

        public T this[string Name]
        {
            get
            {
                return Contents[Contents.Find(Name)];
            }
            set
            {
                Contents[Contents.Find(Name)] = value;
            }
        }

        public GfxDict()
        {
            Contents = new GfxDictionary<T>();
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer) { }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            _Count = Count;

            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Contents.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        //Implementation
        public bool Contains(string Name)
        {
            return Contents.Contains(Name);
        }

        public T Find(string Name)
        {
            return Contents[Contents.Find(Name)];
        }

        public void Add(T Value)
        {
            Contents.Add(Value);
        }

        public void Insert(int Index, T Value)
        {
            Contents.Insert(Index, Value);
        }

        public void Remove(T Value)
        {
            Contents.Remove(Value);
        }

        public void Clear()
        {
            Contents.Clear();
        }
    }
}
