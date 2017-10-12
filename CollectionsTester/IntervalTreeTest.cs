using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CollectionsLibrary.Trees;
using CollectionsLibrary.Trees.IntervalTrees;

namespace CollectionsTester
{
    [TestClass]
    public class IntervalTreeTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            //var tree = new SumIntervalTree<int>(new[] { 1, 2, 3, 4, 5, 6, 7 }, 7);
            //Assert.IsTrue(tree.Count == 15);
            //Assert.IsTrue(tree.LeavesStartIndex == 8);
            //Assert.IsTrue(tree.Sum == 28);
            //Console.WriteLine(tree.IntervalSum(2, 5));
            //Assert.IsTrue(tree.IntervalSum(2,5) == 14);

            //var tree = new MaxAddIntervalTree<long>(new long[1024], 1024);
            //Assert.IsTrue(tree.Max == 0L);
            //Console.WriteLine(tree.LeavesStartIndex);
            //tree.AddToInterval(0, 1025, 0L);
            //Assert.IsTrue(tree.Max == 0L);

            var ints = Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
            var longs = new long[ints[0] + 1];
            long max = 0L, tmp = 0L;

            for (int i = 0; i < ints[1]; i++)
            {
                var parts = Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
                longs[parts[0]] += parts[2];
                if (parts[1] + 1 < longs.Length) longs[parts[1] + 1] -= parts[2];
            }
            
            max = longs.Max(x => tmp += x);
            
            Console.WriteLine(max);
        }
    }
}
