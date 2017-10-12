using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CollectionsLibrary.Trees.BinaryTrees
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    public class BinarySearchTree<T> : ITree<T> where T : IComparable<T>
    {
        protected bool Mod;
        protected EventHandler<ModifyArgs> ModifyHandler;

        public BinarySearchTree()
        {
            ModifyHandler = (sender, args) => Mod = true;
        }

        public BinarySearchTree(IEnumerable<T> items) : this()
        {
            foreach (var item in items)
                Add(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            Mod = false;
            if (Root == null) yield break;

            foreach (var item in Root)
            {
                if (Mod)
                    throw new InvalidOperationException("The tree was modified!");
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds an item into the BST.
        /// 
        /// O(N) in worst case
        /// O(log N) in case of randomly distributed data
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <returns>True if the addition was successful</returns>
        public virtual bool Add(T item)
        {
            if (Root == null)
            {
                Mod = true;
                BinaryRoot = new BinaryNode<T>(item);
                return true;
            }

            BinaryNode<T> n = BinaryRoot;
            while (n != null)
            {
                if (n.Item.CompareTo(item) > 0)
                    if (n.Left == null)
                    {
                        Mod = true;
                        n.Left = new BinaryNode<T>(item) { Parent = n };
                        n.Left.OnModify += ModifyHandler;
                        return true;
                    }
                    else
                    {
                        n = n.Left;
                    }
                else if (n.Item.CompareTo(item) < 0)
                    if (n.Right == null)
                    {
                        Mod = true;
                        n.Right = new BinaryNode<T>(item) { Parent = n };
                        n.Right.OnModify += ModifyHandler;
                        return true;
                    }
                    else
                    {
                        n = n.Right;
                    }
                else
                    return false; // The item was already present in the tree
            }

            return false;
        }

        /// <summary>
        /// Removes an item from BST.
        /// 
        /// O(N) - worst case scenario
        /// O(log N) in randomly distributed data
        /// </summary>
        /// <param name="item">Item to remove</param>
        /// <returns>If the operation was successful</returns>
        public virtual bool Remove(T item)
        {
            if (Root == null)
                return false; // There is no tree

            var node = BinaryRoot;
            while (node != null)
            {
                var comp = item.CompareTo(node.Item);

                if (comp < 0) // Search left subtree
                {
                    if (node.Left != null)
                        node = node.Left;
                    else return false;
                }
                else if (comp > 0) // Search right subtree
                {
                    if (node.Right != null)
                        node = node.Right;
                    else return false;
                }
                else // We found our match!
                {
                    node.OnModify -= ModifyHandler;

                    if (node.Left != null && node.Right != null)
                    { // We have both sons - rather complicated
                        Mod = true;
                        var rep = node.Right.Minimum(); // Find minimum node of right subtree

                        if (rep.Right != null)
                            rep.Right.BinaryParent = rep.BinaryParent;

                        if (rep.BinaryParent.Left == rep) // If the minimum is on the left of his parent
                            rep.BinaryParent.Left = rep.Right; // then replace it in its parent for its right subtree 
                        else // otherwise
                            rep.BinaryParent.Right = rep.Right; // replace it in its parent for its right subtree

                        var par = node.BinaryParent;

                        rep.BinaryParent = par; // set the parent of replacement node
                        if (par != null) // if node is not root
                        {
                            if (par.Left == node) // set the original node parent up
                                par.Left = rep;
                            else
                                par.Right = rep;
                        }

                        rep.Left = node.Left; // set up the sons of origin node as sons of replacement node
                        if (rep.Left != null)
                            rep.Left.BinaryParent = rep;

                        rep.Right = node.Right;
                        if (rep.Right != null)
                            rep.Right.BinaryParent = rep;

                        if (par == null)
                            BinaryRoot = rep;

                        return true; // removal is done
                    }

                    if (node.Leaf) // If the node doesn't have any sons
                    {
                        Mod = true;

                        if (node.BinaryParent != null) // If the node is not root
                        {
                            if (node.BinaryParent.Left == node) // the parent should have one son less.
                                node.BinaryParent.Left = null;
                            else
                                node.BinaryParent.Right = null;
                        }
                        else BinaryRoot = null; // node is root - just forget about the root

                        return true; // removal is done
                    }

                    // node has one son

                    Mod = true;

                    var son = node.Left ?? node.Right; // is it left or right son?
                    son.BinaryParent = node.BinaryParent; // set the nodes parent as the sons parent

                    if (son.BinaryParent == null) // if the node is root
                    {
                        BinaryRoot = son; // set the son as root
                        return true; // removal is done
                    }

                    // we have one son and we're not root
                    if (node.BinaryParent.Left == node) // set up the parent
                        node.BinaryParent.Left = son;
                    else
                        node.BinaryParent.Right = son;

                    return true; // removal is done
                }
            }

            return false; // there were no more nodes -> removal failed
        }

        /// <summary>
        /// Removes node and its subtree from BST.
        /// 
        /// This method uses recursion - should not be used for large number of N.
        /// 
        /// O(N) - worst case scenario
        /// O(log N) - with randomly distributed data
        /// </summary>
        /// <param name="item">Item to remove</param>
        /// <returns>If the operation was successful</returns>
        public virtual bool Prun(T item)
        {
            var node = FindBinary(item);
            if (node == null)
                return false;

            Mod = true;
            if (node.BinaryParent == null)
            {
                BinaryRoot = null;
            }
            else
            {
                if (node.BinaryParent.Left == node)
                    node.BinaryParent.Left = null;
                else if (node.BinaryParent.Right == node)
                    node.BinaryParent.Right = null;
            }

            RemoveHandlers(node);

            return true;
        }

        /// <summary>
        /// Removes handlers from node and its children.
        /// </summary>
        /// <param name="node">Root</param>
        protected void RemoveHandlers(BinaryNode<T> node)
        {
            if (node == null)
                return;

            node.OnModify -= ModifyHandler;
            RemoveHandlers(node.Left);
            RemoveHandlers(node.Right);
        }

        /// <summary>
        /// Adds a tree node to the BST.
        /// 
        /// Calls GraftBinary with casted node.
        /// 
        /// O(M * N)  - worst case scenario, where M is the number of nodes in subtree of added node and N is the number of nodes in BST.
        ///           - This will happen in the data in BST are not distributed randomly (there is not a tree but a line) and input node is
        ///           - not in the right order for input to the tree (there will need to be done some corrections)
        /// O(log N) - in randomly distributed data with correct input node.
        /// </summary>
        /// <param name="node">Input node</param>
        /// <returns>If the addition was successful - always true</returns>
        public virtual bool Graft(TreeNode<T> node)
        {
            var bnode = node as BinaryNode<T>;
            if (bnode == null)
                throw new ArgumentException("Grafting node must be a BinaryNode of the same type.");

            return GraftBinary(bnode);
        }

        /// <summary>
        /// Adds input node to the BST.
        /// 
        /// This method is recursive and should not be used for large N.
        /// 
        /// O(M * N) / O(log N)
        /// </summary>
        /// <param name="grafted">Input node</param>
        /// <returns>If the addition was successful</returns>
        public virtual bool GraftBinary(BinaryNode<T> grafted)
        {
            var node = BinaryRoot;
            var last = node;
            while (node != null)
            {
                var comp = grafted.Item.CompareTo(node.Item);
                if (comp == 0)
                    return GraftBinary(grafted.Left) && GraftBinary(grafted.Right);

                last = node;
                node = comp < 0 ? node.Left : node.Right;
            }

            Mod = true;
            if (last == null)
            {
                BinaryRoot = grafted;
            }
            else
            {
                if (grafted.Item.CompareTo(last.Item) < 0)
                    last.Left = grafted;
                else
                    last.Right = grafted;
                grafted.Parent = last;
            }

            if (CheckIntegrity())
            {
                AddHandlers(grafted);
                return true;
            }
            grafted.OnModify += ModifyHandler;

            var left = grafted.Left;
            var right = grafted.Right;
            grafted.Left = null;
            grafted.Right = null;
            left.BinaryParent = null;
            right.BinaryParent = null;

            return GraftBinary(left) && GraftBinary(right);
        }

        /// <summary>
        /// Adds a modify handler to node and its subnodes.
        /// </summary>
        /// <param name="node">Root</param>
        protected void AddHandlers(BinaryNode<T> node)
        {
            if (node == null)
                return;

            node.OnModify += ModifyHandler;
            AddHandlers(node.Left);
            AddHandlers(node.Right);
        }

        /// <summary>
        /// Finds a node in the BST.
        /// 
        /// O(N) vs. O(log N)
        /// </summary>
        /// <param name="item">Item to find</param>
        /// <returns>Node containing the item or null</returns>
        public virtual TreeNode<T> Find(T item)
        {
            return FindBinary(item);
        }

        /// <summary>
        /// Finds a binary ndoe in the BST.
        /// 
        /// O(N) vs. O(log N)
        /// </summary>
        /// <param name="item">Item to find</param>
        /// <returns>Binary node containing the item or null</returns>
        public virtual BinaryNode<T> FindBinary(T item)
        {
            var node = BinaryRoot;
            while (node != null)
            {
                var comp = item.CompareTo(node.Item);
                if (comp == 0)
                    return node;

                node = comp < 0 ? node.Left : node.Right;
            }

            return null;
        }

        /// <summary>
        /// Root of the BST
        /// 
        /// Is in fact binary.
        /// </summary>
        public virtual TreeNode<T> Root => BinaryRoot;

        /// <summary>
        /// Binary Root of the BST
        /// </summary>
        public virtual BinaryNode<T> BinaryRoot { get; protected set; }

        /// <summary>
        /// Checks if the BST has the right pattern - all the data are in the right places.
        /// </summary>
        /// <returns>If everything is as it should be.</returns>
        private bool CheckIntegrity()
        {
            return this.All(item => FindBinary(item) != null);
        }
    }
}
