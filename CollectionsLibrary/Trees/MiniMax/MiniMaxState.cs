using System;
using System.Collections.Generic;

namespace CollectionsLibrary.Trees.MiniMax
{
    public abstract class MiniMaxState<TState, TAction> : IComparable<MiniMaxState<TState, TAction>> where TState : IComparable<TState>
    {
        public abstract TState State { get; }
        public abstract TAction Action { get; }
        public abstract int Value { get; set; }
        public abstract IEnumerable<MiniMaxState<TState, TAction>> NextStates { get; }
        public abstract int Wins { get; set; }
        public abstract int Loses { get; set; }

        public int CompareTo(MiniMaxState<TState, TAction> other)
        {
            return other == null ? -1 : (Value * 1000000 + Wins).CompareTo(other.Value * 1000000 + other.Wins);
        }
    }
}
