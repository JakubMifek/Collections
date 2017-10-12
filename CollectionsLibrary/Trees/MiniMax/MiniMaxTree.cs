using System;
using System.Collections.Generic;
using System.Linq;

namespace CollectionsLibrary.Trees.MiniMax
{
    public class MiniMaxTree<TState, TAction> : CollectableTreeNode<MiniMaxState<TState, TAction>>
        where TState : IComparable<TState>, IEquatable<TState>
    {
        private MiniMaxTree(MiniMaxState<TState, TAction> item) : base(item)
        {
            _children = new List<MiniMaxTree<TState, TAction>>();
            _rand = new Random();
        }

        private readonly List<MiniMaxTree<TState, TAction>> _children;
        private readonly Random _rand;
        public override IEnumerable<TreeNode<MiniMaxState<TState, TAction>>> Children => _children;
        public override int Degree => _children.Count;

        public MiniMaxState<TState, TAction> BestAction
        {
            get
            {
                var r = Children.Where(
                    y => y.Item.Value * 1000000 + y.Item.Wins
                    == Children.Max(x => x.Item.Value * 1000000 + x.Item.Wins)).ToList();

                return (r[_rand.Next(r.Count)] as MiniMaxTree<TState, TAction>)?.Item;
            }
        }

        public override IEnumerator<MiniMaxState<TState, TAction>> GetEnumerator()
        {
            yield return Item;

            foreach (var child in Children)
                foreach (var item in child)
                    yield return item;
        }

        public override void Add(MiniMaxState<TState, TAction> item)
        {
            throw new InvalidOperationException("Tree is delete-only.");
        }

        public override void Clear()
        {
            _children.Clear();
        }

        public override bool Contains(MiniMaxState<TState, TAction> item)
        {
            return Equals(Item, item) || Children.Any(x => x.Contains(item));
        }

        public override void CopyTo(MiniMaxState<TState, TAction>[] array, int arrayIndex)
        {
            foreach (var item in this)
                if (arrayIndex < array.Length)
                    array[arrayIndex++] = item;
                else break;
        }

        public override bool Remove(MiniMaxState<TState, TAction> item)
        {
            if (Equals(Item, item))
                return false;

            foreach (var child in Children)
            {
                if (Equals(child.Item, item))
                    return _children.Remove(child as MiniMaxTree<TState, TAction>);
            }

            return _children.Any(x => x.Remove(item));
        }

        public MiniMaxTree<TState, TAction> ReturnSelected(TState state)
        {
            if (state == null) return null;

            foreach (var child in Children)
            {
                if (state.Equals(child.Item.State))
                    return child as MiniMaxTree<TState, TAction>;
            }

            foreach (var child in Children)
            {
                MiniMaxTree<TState, TAction> x;
                if ((x = (child as MiniMaxTree<TState, TAction>)?.ReturnSelected(state)) != null)
                    return x;
            }

            return null;
        }

        public override int Count => _children.Count;
        public override bool IsReadOnly => true;
        public override bool Equals(TreeNode<MiniMaxState<TState, TAction>> other)
        {
            var t = other as MiniMaxTree<TState, TAction>;

            if (t?._children.Count != _children.Count)
                return false;

            return Equals(t.Item, Item);
        }

        public static MiniMaxTree<TI, TA1> GenerateMiniMax<TI, TA1>(MiniMaxState<TI, TA1> state) where TI : IComparable<TI>, IEquatable<TI>
        {
            var root = new MiniMaxTree<TI, TA1>(state);

            var dict = new Dictionary<int, MiniMaxTree<TI, TA1>>();

            AddStates(root, dict);

            return root;
        }

        private static void AddStates<TI, TA1>(MiniMaxTree<TI, TA1> root, IDictionary<int, MiniMaxTree<TI, TA1>> dict) where TI : IComparable<TI>, IEquatable<TI>
        {
            foreach (var state in root.Item.NextStates)
            {
                if (dict.ContainsKey(state.GetHashCode()))
                {
                    root._children.Add(dict[state.GetHashCode()]);
                }
                else
                {
                    var y = new MiniMaxTree<TI, TA1>(state);
                    AddStates(y, dict);

                    dict.Add(state.GetHashCode(), y);
                    root._children.Add(y);
                }
            }

            int max = int.MinValue;
            foreach (var item in root._children)
            {
                root.Item.Wins += item.Item.Loses;
                root.Item.Loses += item.Item.Wins;

                if (max < item.Item.Value)
                    max = item.Item.Value;
            }

            if (root._children.Count != 0) root.Item.Value = -max;
        }

        public MiniMaxTree<TState, TAction> Top
        {
            get
            {
                var r = (from x in _children
                         where x.Item.CompareTo(BestAction) == 0
                         select x).ToList();
                return r[_rand.Next(r.Count)];
            }
        }
    }
}
