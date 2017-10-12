using System;
using System.Collections.Generic;
using System.Linq;

namespace CollectionsLibrary.Trees
{
    /// <summary>
    /// Basic TreeNode representation.
    /// </summary>
    /// <typeparam name="T">Custom type T</typeparam>
    public class BasicNode<T> : CollectableTreeNode<T>
    {
        /// <summary>
        /// Children are hold in form of a list.
        /// </summary>
        private readonly List<TreeNode<T>> _children;

        /// <summary>
        /// Count starts with itself.
        /// </summary>
        private int _count = 1;

        /// <summary>
        /// Denotes whether the tree was edited.
        /// </summary>
        private bool _edit;

        /// <summary>
        /// Constructor only creates new list instance for the children and passes item to the base.
        /// </summary>
        /// <param name="item">T to pass to the base</param>
        public BasicNode(T item) : base(item)
        {
            _children = new List<TreeNode<T>>();
        }

        /// <summary>
        /// Children are only pointing at list _children.
        /// 
        /// O(1)
        /// </summary>
        public override IEnumerable<TreeNode<T>> Children => _children;

        public override int Degree => _children.Count;

        /// <summary>
        /// Count is only pointing at int _count.
        /// 
        /// O(1)
        /// </summary>
        public override int Count => _count;

        /// <summary>
        /// The basic TreeNode is not read-only.
        /// 
        /// O(1)
        /// </summary>
        public override bool IsReadOnly { get; } = false;

        /// <summary>
        /// Enumerator first returns our own Item and then goes through items in all children.
        /// 
        /// O(N) where N is number of items in the subtree.
        /// </summary>
        /// <returns>T Enumerator</returns>
        public override IEnumerator<T> GetEnumerator()
        {
            _edit = false;
            yield return Item;

            foreach (var child in Children)
            foreach (var item in child)
            {
                if (_edit)
                    throw new InvalidOperationException("Cannot enumerate eddited object.");

                yield return item;
            }
        }

        /// <summary>
        /// Adds given node to the list of children nodes.
        /// 
        /// O(node.Parent.Remove + node.Count)
        /// </summary>
        /// <param name="node">Node to add to Children</param>
        public virtual void AddNode(BasicNode<T> node)
        {
            _edit = true;

            (node.Parent as BasicNode<T>)?.Remove(node.Item);
            node.Parent = this;
            _count += node.Count;

            _children.Add(node);
        }

        /// <summary>
        /// Adds the given item to the Children as new BasicNode.
        /// 
        /// O(1)
        /// </summary>
        /// <param name="item">Item to add</param>
        public override void Add(T item)
        {
            _edit = true;
            _children.Add(new BasicNode<T>(item) { Parent = this });
            _count++;
        }

        /// <summary>
        /// Removes all Children.
        /// 
        /// O(N) where N is number of children nodes.
        /// </summary>
        public override void Clear()
        {
            _edit = true;
            _children.Clear();
            _count = 1;
        }

        /// <summary>
        /// Decides whether the given item is in the subtree of this node counting itself.
        /// 
        /// O(N) where N is number of subnodes.
        /// </summary>
        /// <param name="item">Item to find</param>
        /// <returns></returns>
        public override bool Contains(T item)
        {
            if (Equals(Item, item))
                return true;

            foreach (var child in Children)
                if (child.Contains(item))
                    return true;

            return false;
        }

        /// <summary>
        /// Copies subtree of this node counting itself to the given array starting from arrayIndex.
        /// The items are copied in enumerating order.
        /// 
        /// The subtree is copied whole. If there is not enough space, the method will throw an exception.
        /// 
        /// O(N) where N is number of subnodes.
        /// </summary>
        /// <param name="array">Array to copy to</param>
        /// <param name="arrayIndex">Index to start from</param>
        public override void CopyTo(T[] array, int arrayIndex)
        {
            foreach (var item in this)
                array[arrayIndex++] = item;
        }

        /// <summary>
        /// Finds and removes given item.
        /// 
        /// If the given item could not be found or removed, returns false.
        /// 
        /// If the given item is the Item of this node returns false.
        /// 
        /// All children of the removed node will be appended to the children of its parent node.
        /// 
        /// O(N) where N is number of subnodes.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override bool Remove(T item)
        {
            if (Equals(Item, item)) return false;

            foreach (var child in Children)
                if (Equals(child.Item, item))
                {
                    _edit = true;

                    foreach (var c in child.Children)
                    {
                        c.Parent = this;
                        _children.Add(c);
                    }

                    _children.Remove(child);
                    _count--;
                    return true;
                }

            return false;
        }

        /// <summary>
        /// Finds and removes given node with all its children.
        /// 
        /// Returns false if given node is not between Children.
        /// 
        /// O(N)
        /// </summary>
        /// <param name="node">Node to remove</param>
        public virtual bool Remove(TreeNode<T> node)
        {
            if (_children.Contains(node))
                return _children.Remove(node);

            return false;
        }

        /// <summary>
        /// Checks the given node for equality with current node.
        /// </summary>
        /// <param name="other">Node to check</param>
        /// <returns></returns>
        public override bool Equals(TreeNode<T> other)
        {
            var n = other as BasicNode<T>;
            if (n == null)
                return false;

            return Equals(Item, n.Item);
        }
    }
}