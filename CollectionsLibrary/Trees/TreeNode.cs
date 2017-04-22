using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CollectionsLibrary.Trees
{
    /// <summary>
    /// Abstract class representing nodes of a tree.
    /// </summary>
    /// <typeparam name="T">Custom type T</typeparam>
    public abstract class TreeNode<T> : ICollection<T>, IEquatable<TreeNode<T>>
    {
        /// <summary>
        /// Protected constructor. The only thing we need from a programmer is the item to represent.
        /// 
        /// O(1)
        /// </summary>
        /// <param name="item"></param>
        protected TreeNode(T item)
        {
            Item = item;
        }

        /// <summary>
        /// Every node can hold information about its parent.
        /// </summary>
        public TreeNode<T> ParentNode { get; set; }

        /// <summary>
        /// Denotes whether the node is a root.
        /// 
        /// O(1)
        /// </summary>
        public bool Root => ParentNode == null;

        /// <summary>
        /// Denotes wheter the node is a leaf (Doesn't have children nodes or all of them are null).
        /// 
        /// O(1)
        /// </summary>
        public bool Leaf => Children.All(x => x == null);

        /// <summary>
        /// Holds the item.
        /// 
        /// O(1)
        /// </summary>
        public T Item { get; protected set; }

        /// <summary>
        /// Enumerable representation of children nodes.
        /// 
        /// O(??)
        /// </summary>
        public abstract IEnumerable<TreeNode<T>> Children { get; }

        /// <summary>
        /// Returns enumerator of all T items in the subtree including itself.
        /// 
        /// Returns generic enumerator.
        /// 
        /// O(??)
        /// </summary>
        /// <returns>Enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns generic enumerator of all T items in the subtree including itself.
        /// 
        /// O(??)
        /// </summary>
        /// <returns>T Enumerator</returns>
        public abstract IEnumerator<T> GetEnumerator();

        /// <summary>
        /// Adds a T item to the subtree.
        /// 
        /// O(??)
        /// </summary>
        /// <param name="item">Item to add</param>
        public abstract void Add(T item);

        /// <summary>
        /// Clears all the elements in the subtree.
        /// 
        /// O(??)
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// Decides whether given item is in the subtree or self.
        /// 
        /// O(??)
        /// </summary>
        /// <param name="item">Seeked item</param>
        /// <returns>Whether given item is in the subtree or self</returns>
        public abstract bool Contains(T item);

        /// <summary>
        /// Copies whole subtree including itself to the given array starting at the arrayIndex.
        /// 
        /// Beware not sufficient size of the array.
        /// 
        /// O(??)
        /// </summary>
        /// <param name="array">Array to copy to</param>
        /// <param name="arrayIndex">Where to begin</param>
        public abstract void CopyTo(T[] array, int arrayIndex);

        /// <summary>
        /// Finds given item in the subtree and removes its node. Following behavior depends on tree implementation.
        /// 
        /// O(??)
        /// </summary>
        /// <param name="item">Item to remove</param>
        /// <returns>Whether the removal was successful</returns>
        public abstract bool Remove(T item);

        /// <summary>
        /// Denotes the count of elements within the subtree counting itself.
        /// 
        /// O(??)
        /// </summary>
        public abstract int Count { get; }

        /// <summary>
        /// Denotes whether the subtree is for read-only.
        /// </summary>
        public abstract bool IsReadOnly { get; }

        /// <summary>
        /// Casts the given node to T item.
        /// 
        /// O(1)
        /// </summary>
        /// <param name="node">Node to cast</param>
        public static explicit operator T(TreeNode<T> node)
        {
            return node.Item;
        }

        /// <summary>
        /// Returns string representation of format:
        /// 
        /// [Item: {Children1, Children2, ...}]
        /// </summary>
        /// <returns>[Item: {Children1, Children2, ...}]</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"[{Item}");

            if (!Leaf)
            {
                sb.Append(": {");
                foreach (var child in Children)
                    sb.Append(child).Append(", ");
                sb.Replace(", ", "}", sb.Length - 2, 2);
            }

            return sb.Append("]").ToString();
        }

        public abstract bool Equals(TreeNode<T> other);
    }

    /// <summary>
    /// Basic TreeNode representation.
    /// </summary>
    /// <typeparam name="T">Custom type T</typeparam>
    public class BasicNode<T> : TreeNode<T>
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
        public void AddNode(TreeNode<T> node)
        {
            _edit = true;

            node.ParentNode?.Remove(node.Item);
            node.ParentNode = this;
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
            _children.Add(new BasicNode<T>(item) { ParentNode = this });
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
                        c.ParentNode = this;
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
        public bool Remove(TreeNode<T> node)
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