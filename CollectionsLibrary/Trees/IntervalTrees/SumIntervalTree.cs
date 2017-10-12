using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionsLibrary.Trees.IntervalTrees
{
    /// <summary>
    /// Representation of sum interval tree.
    /// 
    /// Nodes are indexed from 1 to 2*N - 1.
    /// Leaves are on indices from N to 2*N -1.
    /// 
    /// Root of tree is on index 1.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SumIntervalTree<T>
    {
        public class SumIntervalNode : IntervalNode<T>
        {
            private readonly SumIntervalTree<T> _tree;
            private readonly int _index;

            public SumIntervalNode(T value, int startIndex, int endIndex, SumIntervalTree<T> tree, int index) : base(value, startIndex, endIndex)
            {
                _tree = tree;
                _index = index;
            }

            public SumIntervalNode Parent => _tree._array[_index / 2];
            public SumIntervalNode LeftSon => _index * 2 < _tree._array.Length ? _tree._array[_index * 2] : null;
            public SumIntervalNode RightSon => _index * 2 + 1 < _tree._array.Length ? _tree._array[_index * 2 + 1] : null;
            public bool Leaf => _index >= _tree.LeavesStartIndex;

            public T GetIntervalSum(int startLeaf, int endLeaf)
            {
                if (StartIndex >= startLeaf && EndIndex <= endLeaf)
                    return Value;

                var sum = default(T);
                if (LeftSon.EndIndex >= startLeaf)
                    sum = OperationExtensions.Add(sum, LeftSon.GetIntervalSum(startLeaf, endLeaf));

                if (RightSon.StartIndex <= endLeaf)
                    sum = OperationExtensions.Add(sum, RightSon.GetIntervalSum(startLeaf, endLeaf));

                return sum;
            }
        }

        private readonly SumIntervalNode[] _array;

        public SumIntervalTree(IEnumerable<T> filling, int count = 0)
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

            _array = new SumIntervalNode[2 * LeavesStartIndex];
            int index = 1;
            foreach (var item in filling)
            {
                _array[index + LeavesStartIndex - 1] = new SumIntervalNode(item, index, index, this, index + LeavesStartIndex - 1);
                index++;
            }
            for (; index <= LeavesStartIndex; index++)
                _array[index + LeavesStartIndex - 1] = new SumIntervalNode(default(T), index, index, this, index + LeavesStartIndex - 1);

            for (index = LeavesStartIndex - 1; index > 0; index--)
                _array[index] = new SumIntervalNode(OperationExtensions.Add(_array[2 * index].Value, _array[2 * index + 1].Value), _array[2 * index].StartIndex, _array[2 * index + 1].EndIndex, this, index);
        }

        public T Sum => _array[1].Value;
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
                    _array[index].Value = OperationExtensions.Add(_array[index].LeftSon.Value, _array[index].RightSon.Value);
            }
        }

        public SumIntervalNode GetNodeAt(int index)
        {
            if (index < 0 || index >= _array.Length)
                throw new ArgumentOutOfRangeException($"Getting index must be between 0 and {_array.Length}");

            return _array[index];
        }

        public T IntervalSum(int startLeaf, int endLeaf)
        {
            return _array[1].GetIntervalSum(startLeaf, endLeaf);
        }
    }
}
