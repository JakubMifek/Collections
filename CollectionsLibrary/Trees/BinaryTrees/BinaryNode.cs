using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionsLibrary.Trees.BinaryTrees
{
    public class BinaryNode<T> : TreeNode<T>, IModifiable where T : IComparable<T>
    {
        private BinaryNode<T> _left;
        public BinaryNode<T> Left
        {
            get { return _left; }
            set
            {
                _left = null;
                OnModify?.Invoke(this, new ModifyArgs(Modification.Abandon));

                _left = value;
                OnModify?.Invoke(this, new ModifyArgs(Modification.Adopt));
            }
        }

        private BinaryNode<T> _right;
        public BinaryNode<T> Right
        {
            get { return _right; }
            set
            {
                _right = null;
                OnModify?.Invoke(this, new ModifyArgs(Modification.Abandon));

                _right = value;
                OnModify?.Invoke(this, new ModifyArgs(Modification.Adopt));
            }
        }

        private BinaryNode<T> _parent;

        public BinaryNode<T> BinaryParent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                OnModify?.Invoke(this, new ModifyArgs(Modification.Move));
            }
        }

        public override TreeNode<T> Parent
        {
            get { return _parent; }
            set
            {
                var p = value as BinaryNode<T>;
                if (p == null && value != null)
                    throw new InvalidCastException("Parent must be of the same type.");

                _parent = p;
                OnModify?.Invoke(this, new ModifyArgs(Modification.Move));
            }
        }

        private bool _mod;

        public BinaryNode(T item) : base(item)
        {
        }

        public override IEnumerable<TreeNode<T>> Children
        {
            get
            {
                _mod = false;
                yield return Left;
                if (_mod)
                    throw new InvalidOperationException("Enumeration while editing is forbiden.");
                yield return Right;
            }
        }

        public override int Degree =>
            Left == null ?
                Right == null ? 0 : 1
            :
                Right == null ? 1 : 2;

        public override IEnumerator<T> GetEnumerator()
        {
            _mod = false;
            if (Left != null)
                foreach (var item in Left)
                {
                    if (_mod)
                        throw new InvalidOperationException("Enumeration while editing is forbiden.");
                    yield return item;
                }

            if (_mod)
                throw new InvalidOperationException("Enumeration while editing is forbiden.");
            yield return Item;

            if (Right != null)
                foreach (var item in Right)
                {
                    if (_mod)
                        throw new InvalidOperationException("Enumeration while editing is forbiden.");
                    yield return item;
                }
        }

        public override bool Equals(TreeNode<T> other)
        {
            var node = other as BinaryNode<T>;

            return node?.Item.CompareTo(Item) == 0 && node.Parent == Parent && node.Left == Left && node.Right == Right;
        }

        public event EventHandler<ModifyArgs> OnModify;
    }
}
