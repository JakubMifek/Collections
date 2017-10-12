using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CollectionsLibrary.Lists;

namespace CollectionsLibrary.Trees.Heaps
{
    public class BinaryHeap<T> : IHeap<T> where T : IComparable<T>
    {
        /// <summary>
        /// Returns the top element of the heap.
        /// 
        /// O(1)
        /// </summary>
        public T Significant => _heap[0];

        private readonly T[] _heap;

        /// <summary>
        /// Returns the maximum number of elements in the heap.
        /// 
        /// O(1)
        /// </summary>
        public int Size => _heap.Length;

        private int _count = 0;
        /// <summary>
        /// Returns the actual number of elements in the heap.
        /// </summary>
        public int Count => _count;

        private IComparer<T> _comparer;

        /// <summary>
        /// Gets [O(1)] or Sets [O(n)] heap item's comparer.
        /// 
        /// 
        /// </summary>
        public IComparer<T> Comparer
        {
            get { return _comparer; }
            set
            {
                _comparer = value;
                for (int i = Size / 2; i >= 0; i--)
                    RepairAt(i);
            }
        }

        private bool _mod;
        private readonly T _init;

        public BinaryHeap(int size, T init = default(T), IComparer<T> comparer = null)
        {
            if (size < 1)
                throw new InvalidOperationException("Heap cannot be empty. At least one element is always required.");

            _init = init;

            int x = 1;
            while (x < size) x *= 2;
            _heap = new LazyFunctionEnumerator<T>(() => init).TakeWhile((val, i) => i < x).ToArray();

            if (comparer == null)
                comparer = Comparer<T>.Default;

            Comparer = comparer;
        }

        /// <summary>
        /// Returns an item on given index in an array representation of binary heap with top item on index 0.
        /// 
        /// O(1)
        /// </summary>
        /// <param name="index">Index of wanted item</param>
        /// <returns>Item on given index</returns>
        public T this[int index] => _heap[index];

        /// <summary>
        /// Inserts item into the heap.
        /// 
        /// O(log n)
        /// </summary>
        /// <param name="item">Item to insert</param>
        public void Insert(T item)
        {
            int index = Count;
            _mod = true;
            _count++;

            _heap[index] = item;
            while (index > 0)
            {
                if (Comparer.Compare(_heap[index], _heap[index - 1]) < 0)
                    Swap(index, index - 1);
                else return;

                index--;
            }
        }

        /// <summary>
        /// Finds an index of given item.
        /// 
        /// O(n)
        /// </summary>
        /// <param name="item">Item to find</param>
        /// <returns>Index of item</returns>
        public int IndexOf(T item)
        {
            for (int i = 0; i < Count; i++)
                if (Comparer.Compare(_heap[i], item) == 0)
                    return i;

            return -1;
        }

        /// <summary>
        /// Deletes top item.
        /// 
        /// O(log n)
        /// </summary>
        /// <returns>Top item</returns>
        public T DeleteSignificant()
        {
            _count--;
            _mod = true;
            var tmp = Significant;
            _heap[0] = _init;

            Swap(0, Count);

            RepairAt(0);

            return tmp;
        }

        /// <summary>
        /// Deletes given item.
        /// 
        /// O(n)
        /// </summary>
        /// <param name="item">Item to delete</param>
        public void Delete(T item)
        {
            var io = IndexOf(item);

            if (io == -1)
                throw new InvalidOperationException("Given item was not present in the heap.");

            DeleteAt(io);
        }

        /// <summary>
        /// Deletes an item on given index.
        /// 
        /// O(log n)
        /// </summary>
        /// <param name="index">Index of item to delete</param>
        public void DeleteAt(int index)
        {
            _count--;
            _mod = true;

            _heap[index] = _init;
            Swap(index, Count);

            RepairAt(index);
        }

        /// <summary>
        /// Decreases value of an item about given value. To use this method, make sure that type T is able to perform subtraction.
        /// 
        /// O(n)
        /// </summary>
        /// <param name="item">Item to decrease</param>
        /// <param name="about">Koeficient</param>
        public void DecreaseKey(T item, T about)
        {
            _mod = true;
            Delete(item);

            Insert(OperationExtensions.Sub(item, about));
        }

        /// <summary>
        /// Increases value of an item about given value. To use this method, make sure that type T is able to perform addition.
        /// 
        /// O(n)
        /// </summary>
        /// <param name="item">Item to increase</param>
        /// <param name="about">Koeficient</param>
        public void IncreaseKey(T item, T about)
        {
            _mod = true;
            Delete(item);

            Insert(OperationExtensions.Add(item, about));
        }

        /// <summary>
        /// Decreases value of an item on given index  about given value. To use this method, make sure that type T is able to perform subtraction.
        /// 
        /// O(log n)
        /// </summary>
        /// <param name="index">Index of item to decrease</param>
        /// <param name="about">Koeficient</param>
        public void DecreaseKey(int index, T about)
        {
            _mod = true;

            if (index < 0 || index >= Count)
                throw new InvalidOperationException("Given item was not present in the heap.");

            var tmp = _heap[index];
            DeleteAt(index);
            Insert(OperationExtensions.Sub(tmp, about));
        }

        /// <summary>
        /// Increases value of an item on given index about given value. To use this method, make sure that type T is able to perform addition.
        /// 
        /// O(log n)
        /// </summary>
        /// <param name="index">Index of item to increase</param>
        /// <param name="about">Koeficient</param>
        public void IncreaseKey(int index, T about)
        {
            _mod = true;

            if (index < 0 || index >= Count)
                throw new InvalidOperationException("Given item was not present in the heap.");

            var tmp = _heap[index];
            DeleteAt(index);
            Insert(OperationExtensions.Add(tmp, about));
        }

        /// <summary>
        /// Merges together with another heap.
        /// 
        /// O(n + m)
        /// </summary>
        /// <param name="heap">Heap to merge with.</param>
        /// <returns>New heap of size n + m.</returns>
        public IHeap<T> Merge(IHeap<T> heap)
        {
            return Merge((IEnumerable<T>)heap);
        }

        /// <summary>
        /// Merges together with an enumerable into new heap.
        /// 
        /// O(n + m)
        /// </summary>
        /// <param name="other">Enumerable to add.</param>
        /// <returns>New heap of size n + m.</returns>
        public BinaryHeap<T> Merge(IEnumerable<T> other)
        {
            return Heapify(Comparer, _init, new TwoEnumeratorEnumerator<T>(this, other).ToArray());
        }

        /// <summary>
        /// Creates a heap from an array.
        /// 
        /// O(n)
        /// </summary>
        /// <param name="comp">Comparer of Ts.</param>
        /// <param name="array">Array to heapify</param>
        /// <returns>Heap</returns>
        public IHeap<T> Make(IComparer<T> comp = null, T init = default(T), params T[] array)
        {
            return Heapify(comp, init, array);
        }

        /// <summary>
        /// Creates a heap from an array.
        /// 
        /// O(n)
        /// </summary>
        /// <param name="comp">Comparer of Ts.</param>
        /// <param name="init">Initial value of Ts.</param>
        /// <param name="array">Array to heapify.</param>
        /// <returns>Heap</returns>
        public static BinaryHeap<T> Heapify(IComparer<T> comp = null, T init = default(T), params T[] array)
        {
            BinaryHeap<T> heap = new BinaryHeap<T>(array.Length, init, comp);

            Array.Copy(array, heap._heap, array.Length);

            heap._count = array.Length;

            for (int i = heap.Size / 2; i >= 0; i--)
                heap.RepairAt(i);

            return heap;
        }

        /// <summary>
        /// Swaps two elements of the heap.
        /// </summary>
        /// <param name="index1">Index of first element.</param>
        /// <param name="index2">Index of second element.</param>
        private void Swap(int index1, int index2)
        {
            var tmp = _heap[index1];
            _heap[index1] = _heap[index2];
            _heap[index2] = tmp;
        }

        /// <summary>
        /// Repairs binary heap from given index.
        /// 
        /// O(log n)
        /// </summary>
        /// <param name="index">Index to start at.</param>
        private void RepairAt(int index)
        {
            while (Size > 2 * index)
            {
                int x = index;

                if (Comparer.Compare(_heap[2 * index], _heap[index]) < 0)
                    x = Size > 2 * index + 1 && Comparer.Compare(_heap[2 * index + 1], _heap[2 * index]) < 0 ? 2 * index + 1 : 2 * index;
                else if (Size > 2 * index + 1 && Comparer.Compare(_heap[2 * index + 1], _heap[2 * index]) < 0)
                    x = 2 * index + 1;

                if (x == index) return;

                Swap(index, x);

                index = x;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            _mod = false;
            for (int i = 0; i < Count; i++)
                if (_mod)
                    throw new InvalidOperationException("You cannot iterate modified enumerable.");
                else
                    yield return _heap[i];
        }
    }
}
