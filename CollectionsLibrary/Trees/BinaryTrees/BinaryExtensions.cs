using System;
using System.Diagnostics.CodeAnalysis;

namespace CollectionsLibrary.Trees.BinaryTrees
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class BinaryExtensions
    {
        public static BinaryNode<T> Splay<T>(this BinaryNode<T> node) where T : IComparable<T>
        {
            while (node.BinaryParent?.BinaryParent != null)
            {
                if (node.BinaryParent.Item.CompareTo(node.Item) > 0)
                {
                    // n is on the left
                    node = node.BinaryParent.BinaryParent.Item.CompareTo(node.BinaryParent.Item) > 0
                        ? RotateLL(node.BinaryParent.BinaryParent)
                        : RotateRL(node.BinaryParent.BinaryParent);
                }
                else
                { // n is on the right
                    node = node.BinaryParent.BinaryParent.Item.CompareTo(node.BinaryParent.Item) > 0
                        ? RotateLR(node.BinaryParent.BinaryParent)
                        : RotateRR(node.BinaryParent.BinaryParent);
                }
            }

            while (node.BinaryParent != null)
            {
                // Do left/right rotation
                node = node.BinaryParent.Item.CompareTo(node.Item) > 0
                    ? RotateL(node.BinaryParent)
                    : RotateR(node.BinaryParent);
            }

            return node;
        }

        //      r              r
        //      |              |
        //      a              b
        //    /   \          /   \
        //   b     c   ->   d     a
        //  / \                  / \
        // d   e                e   c
        public static BinaryNode<T> RotateL<T>(this BinaryNode<T> node) where T : IComparable<T>
        {
            var b = node;

            var a = node.BinaryParent;
            if (a == null)
                throw new ArgumentNullException($"Parent of a node must not be null.");

            var r = node.BinaryParent.BinaryParent.BinaryParent;

            a.Left = b.Right;
            if (a.Left != null)
                a.Left.BinaryParent = a;

            b.Right = a;
            a.BinaryParent = b;

            b.BinaryParent = r;

            if (r != null)
            {
                if (r.Left == a)
                    r.Left = b;
                else
                    r.Right = b;
            }

            return b;
        }

        //      r              r
        //      |              |
        //      a              c
        //    /   \          /   \
        //   b     c   ->   a     e
        //        / \      / \
        //       d   e    b   d
        public static BinaryNode<T> RotateR<T>(this BinaryNode<T> node) where T : IComparable<T>
        {
            var c = node;

            var a = node.BinaryParent;
            if (a == null)
                throw new ArgumentNullException($"Parent of a node must not be null.");

            var r = node.BinaryParent.BinaryParent.BinaryParent;

            a.Right = c.Left;
            if (a.Right != null)
                a.Right.BinaryParent = a;

            c.Left = a;
            a.BinaryParent = c;

            c.BinaryParent = r;

            if (r != null)
            {
                if (r.Left == a)
                    r.Left = c;
                else
                    r.Right = c;
            }

            return c;
        }

        //         r              r
        //         |              |
        //         a              d
        //       /   \          /   \
        //      b     c   ->   f     b
        //     / \                  / \
        //    d   e                g   a
        //   /\                        /\
        //  f  g                      e  c
        public static BinaryNode<T> RotateLL<T>(this BinaryNode<T> node) where T : IComparable<T>
        {
            var d = node;

            var b = node.BinaryParent;
            if (b == null)
                throw new ArgumentNullException($"Parent of the node must not be null.");

            var a = node.BinaryParent.BinaryParent;
            if (a == null)
                throw new ArgumentNullException($"Parent of a parent of the node must not be null.");

            var r = node.BinaryParent.BinaryParent.BinaryParent;

            a.Left = b.Right;
            if (a.Left != null)
                a.Left.BinaryParent = a;
            a.BinaryParent = b;

            b.Left = d.Right;
            if (b.Left != null)
                b.Left.BinaryParent = b;
            b.Right = a;
            b.BinaryParent = d;

            d.Right = b;
            d.BinaryParent = r;

            if (r != null)
            {
                if (r.Left == a)
                    r.Left = d;
                else
                    r.Right = d;
            }

            return d;
        }

        //         r                  r
        //         |                  |
        //         a                  e
        //       /   \              /   \
        //      b     c     ->     c     g
        //           / \          / \
        //          d   e        a   f
        //              /\      /\
        //             f  g    b  d
        public static BinaryNode<T> RotateRR<T>(this BinaryNode<T> node) where T : IComparable<T>
        {
            var e = node;

            var c = node.BinaryParent;
            if (c == null)
                throw new ArgumentNullException($"Parent of the node must not be null and must be a BinaryNode of the same types.");

            var a = node.BinaryParent.BinaryParent;
            if (a == null)
                throw new ArgumentNullException($"Parent of a parent of the node must not be null.");

            var r = node.BinaryParent.BinaryParent.BinaryParent;

            a.Right = c.Left;
            if (a.Right != null)
                a.Right.BinaryParent = a;
            a.BinaryParent = c;

            c.Right = e.Left;
            if (c.Right != null)
                c.Right.BinaryParent = c;
            c.Left = a;
            c.BinaryParent = e;

            e.Left = c;
            e.BinaryParent = r;

            if (r != null)
            {
                if (r.Left == a)
                    r.Left = e;
                else
                    r.Right = e;
            }

            return e;
        }

        //         r                  r
        //         |                  |
        //         a                  d
        //       /   \              /   \
        //      b     c     ->     a     c
        //           / \          / \   / \
        //          d   e        b   f g   e
        //         /\      
        //        f  g    
        public static BinaryNode<T> RotateRL<T>(this BinaryNode<T> node) where T : IComparable<T>
        {
            var d = node;

            var c = node.BinaryParent;
            if (c == null)
                throw new ArgumentNullException($"Parent of the node must not be null.");

            var a = node.BinaryParent.BinaryParent;
            if (a == null)
                throw new ArgumentNullException($"Parent of a parent of the node must not be null.");

            var r = node.BinaryParent.BinaryParent.BinaryParent;

            a.Right = d.Left;
            if (a.Right != null)
                a.Right.BinaryParent = a;
            a.BinaryParent = d;

            c.Left = d.Right;
            if (c.Left != null)
                c.Left.BinaryParent = c;
            c.BinaryParent = d;

            d.Left = a;
            d.Right = c;
            d.BinaryParent = r;

            if (r != null)
            {
                if (r.Left == a)
                    r.Left = d;
                else
                    r.Right = d;
            }

            return d;
        }

        //         r                  r
        //         |                  |
        //         a                  e
        //       /   \              /   \
        //      b     c     ->     b     a
        //     / \                / \   / \
        //    d   e              b   f g   c
        //        /\      
        //       f  g    
        public static BinaryNode<T> RotateLR<T>(this BinaryNode<T> node) where T : IComparable<T>
        {
            var e = node;

            var b = node.BinaryParent;
            if (b == null)
                throw new ArgumentNullException($"Parent of the node must not be null.");

            var a = node.BinaryParent.BinaryParent;
            if (a == null)
                throw new ArgumentNullException($"Parent of a parent of the node must not be null.");

            var r = node.BinaryParent.BinaryParent.BinaryParent;

            a.Left = e.Right;
            if (a.Left != null)
                a.Left.BinaryParent = a;
            a.BinaryParent = e;

            b.Right = e.Left;
            if (b.Right != null)
                b.Right.BinaryParent = b;
            b.BinaryParent = e;

            e.Left = b;
            e.Right = a;
            e.BinaryParent = r;

            if (r != null)
            {
                if (r.Left == a)
                    r.Left = e;
                else
                    r.Right = e;
            }

            return e;
        }

        public static BinaryNode<T> Minimum<T>(this BinaryNode<T> node) where T : IComparable<T>
        {
            while (node.Left != null)
                node = node.Left;

            return node;
        }

        public static BinaryNode<T> Maximum<T>(this BinaryNode<T> node) where T : IComparable<T>
        {
            while (node.Right != null)
                node = node.Right;

            return node;
        }
    }
}
