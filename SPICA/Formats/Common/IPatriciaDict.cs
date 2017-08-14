using System.Collections.Generic;
using System.Collections.Specialized;

namespace SPICA.Formats.Common
{
    public interface IPatriciaDict<T> : INotifyCollectionChanged, ICollection<T>, INameIndexed
    {
        T this[int    Index]   { get; set; }
        T this[string Name]    { get; set; }

        bool Contains(string Name);
        
        void Insert(int Index, T Value);
    }
}
