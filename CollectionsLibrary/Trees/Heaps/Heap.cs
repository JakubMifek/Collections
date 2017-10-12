using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionsLibrary.Trees.Heaps
{
    public interface IHeap<T> : IEnumerable<T>
    {
        T Significant { get; }
        int Size { get; }
        int Count { get; }
        IComparer<T> Comparer { get; set; }

        void Insert(T item);
        T DeleteSignificant();
        void Delete(T item);
        void DecreaseKey(T item, T about);
        void IncreaseKey(T item, T about);
        IHeap<T> Merge(IHeap<T> heap);
        IHeap<T> Make(IComparer<T> comp = null, T init = default(T), params T[] array);
    }
}
