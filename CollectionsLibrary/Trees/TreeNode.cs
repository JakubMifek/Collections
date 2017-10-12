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
    public abstract class TreeNode<T> : IEquatable<TreeNode<T>>, IEnumerable<T>
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
        public virtual TreeNode<T> Parent { get; set; }

        /// <summary>
        /// Denotes whether the node is a root.
        /// 
        /// O(1)
        /// </summary>
        public bool Root => Parent == null;

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
        /// Degree of a node.
        /// 
        /// O(??)
        /// </summary>
        public abstract int Degree { get; }

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

        ///// <summary>
        ///// Adds a T item to the subtree.
        ///// 
        ///// O(??)
        ///// </summary>
        ///// <param name="item">Item to add</param>
        //public abstract void Add(T item);

        ///// <summary>
        ///// Clears all the elements in the subtrees.
        ///// 
        ///// O(??)
        ///// </summary>
        //public abstract void Clear();

        ///// <summary>
        ///// Decides whether given item is in the subtree or self.
        ///// 
        ///// O(??)
        ///// </summary>
        ///// <param name="item">Seeked item</param>
        ///// <returns>Whether given item is in the subtree or self</returns>
        //public abstract bool Contains(T item);

        ///// <summary>
        ///// Copies whole subtree including itself to the given array starting at the arrayIndex.
        ///// 
        ///// Beware not sufficient size of the array.
        ///// 
        ///// O(??)
        ///// </summary>
        ///// <param name="array">Array to copy to</param>
        ///// <param name="arrayIndex">Where to begin</param>
        //public abstract void CopyTo(T[] array, int arrayIndex);

        ///// <summary>
        ///// Finds given item in the subtree and removes its node. Following behavior depends on tree implementation.
        ///// 
        ///// O(??)
        ///// </summary>
        ///// <param name="item">Item to remove</param>
        ///// <returns>Whether the removal was successful</returns>
        //public abstract bool Remove(T item);

        ///// <summary>
        ///// Denotes the count of elements within the subtree counting itself.
        ///// 
        ///// O(??)
        ///// </summary>
        //public abstract int Count { get; }

        ///// <summary>
        ///// Denotes whether the subtree is for read-only.
        ///// </summary>
        //public abstract bool IsReadOnly { get; }

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

    public abstract class CollectableTreeNode<T> : TreeNode<T>, ICollection<T>
    {
        protected CollectableTreeNode(T item) : base(item)
        {
        }

        public abstract void Add(T item);
        public abstract void Clear();
        public abstract bool Contains(T item);
        public abstract void CopyTo(T[] array, int arrayIndex);
        public abstract bool Remove(T item);
        public abstract int Count { get; }
        public abstract bool IsReadOnly { get; }
    }
}