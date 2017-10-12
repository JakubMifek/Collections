using System;
using System.Collections.Generic;

namespace CollectionsLibrary.Trees.IntervalTrees
{
    public class IntervalNode<T>
    {
        public T Value { get; internal set; }
        public int StartIndex { get; }
        public int EndIndex { get; }

        public IntervalNode(T value, int startIndex, int endIndex)
        {
            Value = value;
            StartIndex = startIndex;
            EndIndex = endIndex;
        }
    }
}
