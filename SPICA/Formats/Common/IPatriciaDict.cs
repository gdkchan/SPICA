using System.Collections.Generic;
using System.Collections.Specialized;

namespace SPICA.Formats.Common
{
    public interface IPatriciaDict<T> : INotifyCollectionChanged, IEnumerable<T>
    {
        int Count { get; }

        T this[int Index]   { get; set; }
        T this[string Name] { get; set; }

        bool Contains(string Name);
        
        void Add(T Value);
        void Insert(int Index, T Value);
        void Remove(T Value);
        void Clear();
    }
}
