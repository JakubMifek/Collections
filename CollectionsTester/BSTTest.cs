using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CollectionsLibrary.Trees;
using CollectionsLibrary.Trees.BinaryTrees;
using CollectionsLibrary.Trees.MiniMax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CollectionsTester
{
    /// <summary>
    /// Summary description for MiniMaxTest
    /// </summary>
    [TestClass]
    public class BSTTest
    {
        public BSTTest()
        {
            _sw = new Stopwatch();
        }

        private readonly Stopwatch _sw;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestMethod1()
        {
            var tree = new BinarySearchTree<int>();
            Assert.IsNotNull(tree);

            var rand = new Random(161803398);
            bool op;
            int added;
            TreeNode<int> node;
            BinaryNode<int> binary;

            op = tree.Add(added = 10);
            Assert.IsTrue(op);
            Assert.IsNotNull(tree.Root);
            Assert.IsNotNull(tree.BinaryRoot);
            Assert.AreEqual(tree.BinaryRoot, tree.Root);

            node = tree.Find(added);
            Assert.IsNotNull(node);
            Assert.AreEqual(node.Item, added);
            Assert.AreEqual(node.Degree, 0);

            op = tree.Add(added = 100);
            Assert.IsTrue(op);
            Assert.AreEqual(node.Degree, 1);

            binary = tree.FindBinary(added);
            Assert.IsNotNull(binary);
            Assert.IsNull(binary.Left);
            Assert.IsNull(binary.Right);
            Assert.IsNotNull(binary.Parent);
            Assert.AreEqual(binary.BinaryParent, binary.Parent);

            op = tree.Remove(10);
            Assert.IsTrue(op);
            Assert.IsNotNull(tree.Root);
            Assert.AreEqual(tree.BinaryRoot.Item, added);
            Assert.AreEqual(tree.BinaryRoot.Degree, 0);

            tree.Add(50);
            tree.Add(200);
            op = tree.Remove(added);
            Assert.IsTrue(op);
            Assert.IsNotNull(tree.Root);
            Assert.AreEqual(tree.BinaryRoot.Item, 200);
            Assert.AreEqual(tree.BinaryRoot.Degree, 1);

            var list = new List<int>();
            for (var i = 0; i < 100000; i++)
            {
                var x = rand.Next();
                list.Add(x);
                tree.Add(x);
            }

            foreach (var item in list)
                Assert.IsNotNull(tree.Find(item));

            var r = list[rand.Next(list.Count)];
            tree.Remove(r);
            list.Remove(r);

            foreach (var item in list)
                Assert.IsNotNull(tree.Find(item));
            Assert.IsNull(tree.Find(r));

            var list2 = new List<int>();
            var a = new BinaryNode<int>(rand.Next());
            list2.Add(a.Item);
            a.Left = new BinaryNode<int>(rand.Next());
            list2.Add(a.Left.Item);
            a.Right = new BinaryNode<int>(rand.Next());
            list2.Add(a.Right.Item);

            foreach (var item in list2)
                Assert.IsTrue(a.Contains(item));

            Assert.IsTrue(tree.Graft(a));
            foreach (var item in list2)
                Assert.IsNotNull(tree.FindBinary(item));

            list.AddRange(list2);

            var z = tree.FindBinary(a.Item);
            tree.Prun(z.Item);
            foreach (var item in z)
                Assert.IsNull(tree.Find(item));

            foreach (var item in z)
                list.Remove(item);

            if (list.Count < 5)
            {
                Console.WriteLine("Tree too small");
                for (var i = 0; i < 50; i++)
                {
                    var y = rand.Next();
                    tree.Add(y);
                    list.Add(y);
                }
            }

            var root = tree.Root.Item;

            tree.Remove(root);
            list.Remove(root);
            Assert.IsNotNull(tree.Root);

            foreach (var item in list)
                Assert.IsNotNull(tree.Find(item));

            Assert.IsNull(tree.Find(root));

            list.Add(50);
            RemoveDuplicates(list);

            while (list.Count > 0)
            {
                var h = list[rand.Next(list.Count)];
                list.Remove(h);
                op = tree.Remove(h);
                Assert.IsTrue(op);
            }
            
            Assert.IsNull(tree.Root);
            Assert.IsNull(tree.Find(root));
            Assert.IsFalse(tree.Prun(root));
            Assert.IsFalse(tree.Any(x => true));
            Assert.IsTrue(tree.All(x => false));
        }

        private void RemoveDuplicates<T>(IList<T> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i] == null)
                    list.RemoveAt(i--);
                else
                    for (var y = i + 1; y < list.Count; y++)
                        if (list[i].Equals(list[y]))
                            list.RemoveAt(y--);
            }
        }

        private void CheckIntegrity<T>(BinarySearchTree<T> tree) where T : IComparable<T>
        {
            var stack = new Stack<BinaryNode<T>>();
            stack.Push(tree.BinaryRoot);
            while (stack.Count != 0)
            {
                var node = stack.Pop();

                if (node.Left != null)
                {
                    Assert.AreEqual(node.Left.Parent, node);
                    stack.Push(node.Left);
                }

                if (node.Right != null)
                {
                    Assert.AreEqual(node.Right.Parent, node);
                    stack.Push(node.Right);
                }
            }
        }
    }
}
