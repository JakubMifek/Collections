using System;
using System.Collections.Generic;

namespace CollectionsLibrary.Trees
{
    public interface ITree<T> : IEnumerable<T>
    {
        /// <summary>
        /// Every common tree should be able to add new item.
        /// 
        /// O(??)
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <returns>True if the addition was successful</returns>
        bool Add(T item);

        /// <summary>
        /// Every tree should be able to remove items.
        /// 
        /// O(??)
        /// </summary>
        /// <param name="item">Item to remove</param>
        /// <returns>True if the removal was successful</returns>
        bool Remove(T item);

        /// <summary>
        /// Every tree should be able to remove subsection defined by an item.
        /// 
        /// O(??)
        /// </summary>
        /// <param name="item">Item to remove with its sons</param>
        /// <returns>True if the removal was successful</returns>
        bool Prun(T item);

        /// <summary>
        /// Every tree should be able to add a tree node.
        /// 
        /// O(??)
        /// </summary>
        /// <param name="node">Node to add</param>
        /// <returns>If the addition was successful</returns>
        bool Graft(TreeNode<T> node);

        /// <summary>
        /// Every tree should be able to find a node defined by an item.
        /// 
        /// O(??)
        /// </summary>
        /// <param name="item">Item defining node to find</param>
        /// <returns>Node containing the item or null if unavalible</returns>
        TreeNode<T> Find(T item);

        /// <summary>
        /// Every tree has to have a root.
        /// 
        /// O(??)
        /// </summary>
        TreeNode<T> Root { get; }
    }
}
