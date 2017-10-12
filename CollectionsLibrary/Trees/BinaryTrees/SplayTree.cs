using System;
using System.Diagnostics;

namespace CollectionsLibrary.Trees.BinaryTrees
{
    public class SplayTree<T> : BinarySearchTree<T> where T : IComparable<T>
    {
        public override bool Add(T item)
        {
            if (BinaryRoot == null)
            {
                BinaryRoot = new BinaryNode<T>(item);
                return true;
            }

            var node = BinaryRoot;
            var r = false;
            int comp = 0;
            while (node != null)
            {
                comp = item.CompareTo(node.Item);
                if (comp == 0 ||
                    comp < 0 && node.Left == null ||
                    comp > 0 && node.Right == null)
                    break;

                node = comp < 0 ? node.Left : node.Right;
            }

            Debug.Assert(node != null, "node != null");

            Mod = true;
            if (comp < 0)
            {
                node.Left = new BinaryNode<T>(item);
                node = node.Left;
                r = true;
            }
            else if (comp > 0)
            {
                node.Right = new BinaryNode<T>(item);
                node = node.Right;
                r = true;
            }

            node.OnModify += ModifyHandler;
            BinaryRoot = node.Splay();
            return r;
        }

        public override TreeNode<T> Find(T item)
        {
            return FindBinary(item);
        }

        public override BinaryNode<T> FindBinary(T item)
        {
            var node = base.FindBinary(item);
            BinaryRoot = node.Splay();
            return node;
        }

        public override bool Remove(T item)
        {
            var node = FindBinary(item);
            if (node == null)
                return false;

            Mod = true;
            node.OnModify -= ModifyHandler;
            var left = node.Left;
            if (left != null)
                left.BinaryParent = null;

            var right = node.Right;
            if (right != null)
                right.BinaryParent = null;

            if (left == null)
            {
                BinaryRoot = right;
                return true;
            }

            if (right == null)
            {
                BinaryRoot = left;
                return true;
            }

            BinaryRoot = Join(left, right);
            return true;
        }

        public override bool Graft(TreeNode<T> node)
        {
            var binary = node as BinaryNode<T>;
            if (node == null)
                throw new InvalidCastException("Cannot graft other than non-null BinaryNode<T>.");

            return GraftBinary(binary);
        }

        public override bool GraftBinary(BinaryNode<T> grafted)
        {
            if (grafted == null)
                throw new InvalidOperationException("Cannot graft null BinaryNode<T>.");

            var r = base.GraftBinary(grafted);
            if (r)
                grafted.Splay();

            return r;
        }

        public override bool Prun(T item)
        {
            var node = base.FindBinary(item);
            var par = node?.BinaryParent;
            var r = base.Prun(item);
            if (r)
            {
                RemoveHandlers(node);
                par?.Splay();
            }
            else
                node?.Splay();

            return r;
        }

        private static BinaryNode<T> Join(BinaryNode<T> left, BinaryNode<T> right)
        {
            if (left == null || right == null)
                throw new ArgumentNullException();

            var max = left.Maximum();
            if (max.Item.CompareTo(right.Minimum().Item) > 0)
                throw new InvalidOperationException($"All elements in '{nameof(right)}' must be greater than all elements in '{nameof(left)}.'");

            max.Splay();

            Debug.Assert(max.Right == null, "max.Right == null");

            max.Right = right;
            right.BinaryParent = max;
            return max;
        }
    }
}
