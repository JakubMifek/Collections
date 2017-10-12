using System;
using System.Collections.Generic;
using System.Diagnostics;
using CollectionsLibrary.Trees.MiniMax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CollectionsTester
{
    class State : IComparable<State>, IEquatable<State>
    {
        public State(int[,] table)
        {
            Table = table;
        }

        public int[,] Table { get; }

        public bool IsAtEnd
        {
            get
            {
                for (int p = 0; p < 2; p++)
                    if (Table[0, 0] == p && Table[1, 1] == p && Table[2, 2] == p ||
                        Table[2, 0] == p && Table[1, 1] == p && Table[0, 2] == p)
                    {
                        _winner = p;
                        return true;
                    }

                for (int x = 0; x < 3; x++)
                    for (int p = 0; p < 2; p++)
                        if (Table[x, 0] == p && Table[x, 1] == p && Table[x, 2] == p ||
                            Table[0, x] == p && Table[1, x] == p && Table[2, x] == p)

                        {
                            _winner = p;
                            return true;
                        }

                for (int i = 0; i < 9; i++)
                    if (Table[i / 3, i % 3] == -1)
                        return false;

                return true;
            }
        }

        private int _winner = -1;
        public int Winner
        {
            get
            {
                if (IsAtEnd) { }
                return _winner;
            }
        }

        public int CompareTo(State other)
        {
            for (int x = 0; x < Table.GetLength(0); x++)
                for (int y = 0; y < Table.GetLength(1); y++)
                    if (Table[x, y] != other.Table[x, y])
                        return -1;

            return 0;
        }

        public override int GetHashCode()
        {
            return
                (((((((Table[0, 0] * 10 + Table[0, 1]) * 10 + Table[0, 2])
                * 10 + Table[1, 0]) * 10 + Table[1, 1]) * 10 + Table[1, 2])
                * 10 + Table[2, 0]) * 10 + Table[2, 1]) * 10 + Table[2, 2];
        }

        public bool Equals(State other)
        {
            return CompareTo(other) == 0;
        }
    }

    class MiniMaxState : MiniMaxState<State, int[]>
    {
        private readonly int _depth;

        public MiniMaxState(int[] action, State state, int player, int depth)
        {
            Action = action;
            State = state;

            Player = player;
            _depth = depth;

            if (state.IsAtEnd)
            {
                Value = state.Winner == -1 ? 0 : 10 - depth;
                Wins = state.Winner == -1 ? 0 : 1;
                Loses = 0;
            }
        }

        public int Player { get; }
        public sealed override int Wins { get; set; }
        public sealed override int Loses { get; set; }
        public override State State { get; }
        public override int[] Action { get; }
        public sealed override int Value { get; set; }

        public override IEnumerable<MiniMaxState<State, int[]>> NextStates
        {
            get
            {
                if (!State.IsAtEnd)
                {
                    for (int x = 0; x < State.Table.GetLength(0); x++)
                        for (int y = 0; y < State.Table.GetLength(1); y++)
                            if (State.Table[x, y] == -1)
                            {
                                var table = new int[State.Table.GetLength(0), State.Table.GetLength(1)];
                                for (int xx = 0; xx < State.Table.GetLength(0); xx++)
                                    for (int yy = 0; yy < State.Table.GetLength(1); yy++)
                                        table[xx, yy] = State.Table[xx, yy];

                                table[x, y] = Player;
                                yield return new MiniMaxState(new[] { x, y }, new State(table), (Player + 1) % 2, _depth + 1);
                            }
                }
            }
        }

        public override int GetHashCode()
        {
            return State.GetHashCode();
        }
    }

    /// <summary>
    /// Summary description for MiniMaxTest
    /// </summary>
    [TestClass]
    public class MiniMaxTest
    {
        public MiniMaxTest()
        {
            _sw = new Stopwatch();
            _sw.Start();
            _root = MiniMaxTree<State, int[]>.GenerateMiniMax(new MiniMaxState(null, new State(new[,] { { -1, -1, -1 }, { -1, -1, -1 }, { -1, -1, -1 } }), 0, 0));
            _sw.Stop();
        }

        private Stopwatch _sw;
        private MiniMaxTree<State, int[]> _root;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestMethod1()
        {
            Console.WriteLine($"Tree built in: {_sw.ElapsedMilliseconds}ms");
            Assert.IsTrue(_root.Root);
            while (_root.BestAction != null)
            {
                Console.WriteLine($"[{_root.BestAction.Action[0]}, {_root.BestAction.Action[1]}](V:{_root.BestAction.Value}; W: {_root.BestAction.Wins}; L: {_root.BestAction.Loses}) = {(_root.BestAction as MiniMaxState)?.Player}");
                _root = _root.Top;
            }
        }
    }
}
