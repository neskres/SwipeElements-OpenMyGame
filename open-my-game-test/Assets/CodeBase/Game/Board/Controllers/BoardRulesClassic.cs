using System;
using CodeBase.Game.Board;
using Game.Board.Data;
using Game.Shared.StaticData;

namespace Game.Board.Controllers
{
    public class BoardRulesClassic : IBoardRules
    {
        public int Width { get; } = 6;
        public int Height { get; } = 6;

        private readonly BlockType[] _palette = { BlockType.Fire, BlockType.Water };

        public bool IsSwapValid(BoardCoordinates a,
            BoardCoordinates b,
            IMatchFinder finder,
            IBoardState state)
        {
            if (!InBounds(a, state) || !InBounds(b, state))
                return false;

            if (!AreNeighbours(a, b))
                return false;

            var clone = SwapClone(state, a, b);
            return finder.HasAnyMatch(clone);
        }

        public Block GenerateElement(int x, int y)
        {
            var rnd = UnityEngine.Random.Range(0, _palette.Length);
            return new Block(_palette[rnd]);
        }

        private static bool AreNeighbours(BoardCoordinates a, BoardCoordinates b) =>
            (Math.Abs(a.X - b.X) == 1 && a.Y == b.Y) ||
            (Math.Abs(a.Y - b.Y) == 1 && a.X == b.X);

        private static bool InBounds(BoardCoordinates c, IBoardState s) =>
            c.X >= 0 && c.X < s.Width && c.Y >= 0 && c.Y < s.Height;

        private static BoardState SwapClone(IBoardState src, BoardCoordinates a, BoardCoordinates b)
        {
            var raw = new Block[src.Width, src.Height];

            foreach (var c in src.AllCoords())
            {
                var val = (Block)src.Get(c.X, c.Y);
                raw[c.X, c.Y] = val;
            }

            (raw[a.X, a.Y], raw[b.X, b.Y]) = (raw[b.X, b.Y], raw[a.X, a.Y]);
            return new BoardState(raw);
        }
    }
}