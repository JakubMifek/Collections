using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionsLibrary.Trees.IntervalTrees
{

    /// <summary>
    /// Representation of max add interval tree.
    /// 
    /// Is able to add value to an interval and find maximum value in the tree.
    /// 
    /// Nodes are indexed from 1 to 2*N - 1.
    /// Leaves are on indices from N to 2*N -1.
    /// 
    /// Root of tree is on index 1.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaxAddIntervalTree<T> where T : IComparable<T>
    {
        public class MaxAddIntervalNode : IntervalNode<T>
        {
            private readonly MaxAddIntervalTree<T> _tree;
            private readonly int _index;

            public MaxAddIntervalNode(T value, int startIndex, int endIndex, MaxAddIntervalTree<T> tree, int index) : base(value, startIndex, endIndex)
            {
                _tree = tree;
                _index = index;
            }

            public MaxAddIntervalNode Parent => _tree._array[_index / 2];
            public MaxAddIntervalNode LeftSon => _index * 2 < _tree._array.Length ? _tree._array[_index * 2] : null;
            public MaxAddIntervalNode RightSon => _index * 2 + 1 < _tree._array.Length ? _tree._array[_index * 2 + 1] : null;
            public bool Leaf => _index >= _tree.LeavesStartIndex;

            public T ToAdd { get; set; }
            public T Max => OperationExtensions.Add(Value, ToAdd);

            public void AddToInterval(int startIndex, int endIndex, T value)
            {
                if (StartIndex >= startIndex && EndIndex <= endIndex)
                {
                    ToAdd = OperationExtensions.Add(ToAdd, value);
                    return;
                }

                if (LeftSon.EndIndex >= startIndex)
                    LeftSon.AddToInterval(startIndex, endIndex, value);

                if (RightSon.StartIndex <= endIndex)
                    RightSon.AddToInterval(startIndex, endIndex, value);

                Value = OperationExtensions.Max(RightSon.Max, LeftSon.Max);
            }
        }

        private readonly MaxAddIntervalNode[] _array;

        public MaxAddIntervalTree(IEnumerable<T> filling, int count = 0)
        {
            if (count == 0)
            {
                var l = new List<T>(filling);
                count = l.Count;
                filling = l;
            }

            int num = 1;
            while (num < count) num *= 2;
            LeavesStartIndex = num;

            _array = new MaxAddIntervalNode[2 * LeavesStartIndex];
            int index = 1;
            foreach (var item in filling)
            {
                _array[index + LeavesStartIndex - 1] = new MaxAddIntervalNode(item, index, index, this, index + LeavesStartIndex - 1);
                index++;
            }
            for (; index <= LeavesStartIndex; index++)
                _array[index + LeavesStartIndex - 1] = new MaxAddIntervalNode(default(T), index, index, this, index + LeavesStartIndex - 1);

            for (index = LeavesStartIndex - 1; index > 0; index--)
                _array[index] = new MaxAddIntervalNode(OperationExtensions.Max(_array[2 * index].Value, _array[2 * index + 1].Value), _array[2 * index].StartIndex, _array[2 * index + 1].EndIndex, this, index);
        }

        public T Max => _array[1].Max;
        public int LeavesStartIndex { get; }
        public int Count => LeavesStartIndex * 2 - 1;

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _array.Length)
                    throw new ArgumentOutOfRangeException($"Getting index must be between 0 and {_array.Length}");

                return _array[index].Value;
            }

            set
            {
                if (index < LeavesStartIndex || index >= _array.Length)
                    throw new ArgumentOutOfRangeException($"Setting index must be between {LeavesStartIndex} and {_array.Length}");

                _array[index].Value = value;
                while ((index /= 2) > 0)
                    _array[index].Value = OperationExtensions.Max(_array[index].LeftSon.Max, _array[index].RightSon.Max);
            }
        }

        public MaxAddIntervalNode GetNodeAt(int index)
        {
            if (index < 0 || index >= _array.Length)
                throw new ArgumentOutOfRangeException($"Getting index must be between 0 and {_array.Length}");

            return _array[index];
        }

        public void AddToInterval(int startIndex, int endIndex, T value)
        {
            _array[1].AddToInterval(startIndex, endIndex, value);
        }
    }
}
