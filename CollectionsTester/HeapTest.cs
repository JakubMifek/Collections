using System;
using System.Linq;
using CollectionsLibrary.Trees.Heaps;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CollectionsTester
{
    [TestClass]
    public class HeapTest
    {
        [TestMethod]
        public void BinaryHeapInitTest()
        {
            var heap = BinaryHeap<double>.Heapify(null, double.MaxValue, 1, 5, 2, 4, 8);
            Assert.IsTrue(heap.Count == 5);
            Assert.IsTrue(heap.Significant == 1.0);
            Assert.IsTrue(heap.Size == 8);

            heap.DeleteSignificant();
            Assert.IsTrue(heap.Count == 4);
            Assert.IsTrue(heap.Significant == 2);

            heap.Insert(-1);
            Assert.IsTrue(heap.Count == 5);
            Assert.IsTrue(heap.Significant == -1);

            heap.Delete(2);
            Assert.IsTrue(heap.Count == 4);
            Assert.IsFalse(heap.Contains(2));
            Assert.IsTrue(heap.Significant == -1);

            Assert.IsTrue(2 * heap.IndexOf(8) > heap.Count - 1);

            Assert.IsTrue(heap.Contains(8));
            heap.DecreaseKey(8.0, 10);
            Assert.IsTrue(heap.Count == 4);
            
            Assert.IsTrue(heap.Significant == -2);

            heap.IncreaseKey((int)0, 12.0);
            Assert.IsTrue(heap.Count == 4);
            Assert.IsTrue(heap.Significant == -1);
            Assert.IsTrue(heap.Contains(10));

            var heap2 = heap.Make(null, double.MaxValue, -8, -5, -12, -3);
            Assert.IsTrue(heap2.Count == 4);
            Assert.IsTrue(heap2.Significant == -12);

            var merged = heap.Merge(heap2);
            Assert.IsTrue(merged.Count == 8);
            Assert.IsTrue(merged.Size == 8);
            Assert.IsTrue(merged.Significant == -12);
            Assert.IsTrue(merged.Contains(10));
            foreach (var item in merged)
                Console.WriteLine(item);
        }
    }
}
