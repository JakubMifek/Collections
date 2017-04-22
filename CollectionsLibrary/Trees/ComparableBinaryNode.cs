using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionsLibrary.Trees
{
    // TODO: Fix Removals
    public class ComparableBinaryNode<T> : TreeNode<T>, IList<T> where T : IComparable<T>
    {
        public override IEnumerable<TreeNode<T>> Children => new[] { Left, Right };

        private ComparableBinaryNode<T> _left, _right;
        public virtual ComparableBinaryNode<T> Left
        {
            get { return _left; }

            set
            {
                _edit = true;
                _left = value;

                _count = _left.Count + 1;
                if (Right != null)
                    _count += Right.Count;
            }
        }

        public virtual ComparableBinaryNode<T> Right
        {
            get { return _right; }
            set
            {
                _edit = true;
                _right = value;

                _count = _right.Count + 1;
                if (Left != null)
                    _count += Left.Count;
            }
        }

        private bool _edit;

        public ComparableBinaryNode(T item) : base(item)
        {
        }

        public override void Add(T item)
        {
            _edit = true;

            var comparsion = Item.CompareTo(item);

            if (comparsion <= 0)
            {
                if (Right == null) Right = new ComparableBinaryNode<T>(item) { ParentNode = this };
                else Right.Add(item);
            }
            else
            {
                if (Left == null) Left = new ComparableBinaryNode<T>(item) { ParentNode = this };
                else Left.Add(item);
            }

            _count++;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override IEnumerator<T> GetEnumerator()
        {
            _edit = false;

            if (Left != null)
                foreach (var node in Left)
                {
                    if (_edit)
                        throw new InvalidOperationException("You cannot enumerate while editing.");

                    yield return node;
                }

            if (_edit)
                throw new InvalidOperationException("You cannot enumerate while editing.");

            yield return Item;

            if (Right != null)
                foreach (var node in Right)
                {
                    if (_edit)
                        throw new InvalidOperationException("You cannot enumerate while editing.");

                    yield return node;
                }
        }

        public override void Clear()
        {
            _edit = true;

            Left = null;
            Right = null;
            _count = 1;
        }

        public override bool Contains(T item)
        {
            var comparsion = Item.CompareTo(item);

            if (comparsion < 0)
                return Right != null && Right.Contains(item);

            if (comparsion > 0)
                return Left != null && Left.Contains(item);

            return true;
        }

        public override void CopyTo(T[] array, int arrayIndex)
        {
            if (array.Length < arrayIndex)
                throw new IndexOutOfRangeException($"Could not copy tree to the array. Needed index: {arrayIndex}");

            array[arrayIndex] = Item;

            Left?.CopyTo(array, arrayIndex * 2 + 1);
            Right?.CopyTo(array, arrayIndex * 2 + 2);
        }

        public override bool Remove(T item)
        {
            var comparsion = Item.CompareTo(item);
            if (comparsion == 0) return false;

            if (Left != null && Left.Item.CompareTo(item) == 0)
            {
                Left = null;

                _edit = true;
                _count--;

                return true;
            }

            if (Right != null && Right.Item.CompareTo(item) == 0)
            {
                Right = null;

                _edit = true;
                _count--;

                return true;
            }

            bool ret;

            if (comparsion < 0)
            {
                ret = Right != null && Right.Remove(item);
                if (ret)
                {
                    _edit = true;
                    _count--;
                }

                return ret;
            }

            ret = Left != null && Left.Remove(item);
            if (ret)
            {
                _edit = true;
                _count--;
            }

            return ret;
        }

        private int _count;
        public override int Count => _count;

        public override bool IsReadOnly { get; } = false;

        public virtual int IndexOf(T item)
        {
            int i = 0;
            foreach (var node in this)
            {
                if (node.CompareTo(item) == 0)
                    return i;

                i++;
            }

            return -1;
        }

        public void Insert(int index, T item)
        {
            _edit = true;

            if (Left != null)
            {
                if (index <= Left.Count)
                    Left.Insert(index, item);

                else if (Right != null)
                    Right.Insert(index, item);

                else Right = new ComparableBinaryNode<T>(item);
            }
            else if (index == 0)
                Left = new ComparableBinaryNode<T>(item);
            else
            {
                if (Right != null)
                    Right.Insert(index, item);

                else Right = new ComparableBinaryNode<T>(item);
            }
        }

        private bool IsOnIndex(int index)
        {
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException("Index out of range of the tree.");

            if (Left?.Count == index)
                return true;

            if (index == 0)
                return true;

            return false;

        }

        public void RemoveAt(int index)
        {
            _edit = true;

            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException("Index out of range of the tree.");

            if (Left != null)
            {
                if (Left.Count > index)
                {
                    if (Left.IsOnIndex(index))
                    {
                        _count -= Left.Count;
                        Left = null;
                    }
                    else Left.RemoveAt(index);
                }
                else
                {
                    if (Right.IsOnIndex(index))
                    {
                        _count -= Right.Count;
                        Right = null;
                    }
                    else Right.RemoveAt(index);
                }
            }

            if (Right != null && Right.IsOnIndex(index))
            {
                _count -= Right.Count;
                Right = null;
            }
            else Right?.RemoveAt(index);

            Remove(this[index]);
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException("Index out of range of the tree.");

                if (Left != null)
                {
                    if (Left.Count > index)
                        return Left[index];

                    if (Left.Count == index)
                        return Item;

                    return Right[index];
                }

                if (index == 0)
                    return Item;

                return Right[index];
            }

            set
            {
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException("Index out of range of the tree.");

                if (Left != null)
                {
                    if (Left.Count > index)
                        Left[index] = value;

                    else if (Left.Count == index)
                        Item = value;

                    else
                        Right[index] = value;
                }
                else if (index == 0)
                    Item = value;

                else
                    Right[index] = value;
            }
        }

        public override bool Equals(TreeNode<T> other)
        {
            var n = other as ComparableBinaryNode<T>;

            return n?.Item.CompareTo(Item) == 0;
        }
    }
}
