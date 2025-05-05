using System.Collections.Generic;
using CodeBase.Game.Board;
using Game.Board.Data;
using Game.Shared.StaticData;

namespace Game.Board.Controllers
{
    public class MatchFinderBasic : IMatchFinder
    {
        public IEnumerable<Match> DetectMatches(IBoardState state)
        {
            for (int y = 0; y < state.Height; y++)
            {
                int run = 1;
                BlockType prev = state.Get(0, y).Type;

                for (int x = 1; x < state.Width; x++)
                {
                    BlockType cur = state.Get(x, y).Type;

                    if (cur == prev && cur != BlockType.Empty)
                    {
                        run++;
                    }
                    else
                    {
                        if (run >= 3 && prev != BlockType.Empty)
                        {
                            var match = BuildHorizontalMatch(x - 1, y, run);
                            if (match.Cells.Count > 0)
                                yield return match;
                        }

                        run = 1;
                        prev = cur;
                    }
                }

                if (run >= 3 && prev != BlockType.Empty)
                {
                    var match = BuildHorizontalMatch(state.Width - 1, y, run);
                    if (match.Cells.Count > 0)
                        yield return match;
                }
            }

            for (int x = 0; x < state.Width; x++)
            {
                int run = 1;
                BlockType prev = state.Get(x, 0).Type;

                for (int y = 1; y < state.Height; y++)
                {
                    BlockType cur = state.Get(x, y).Type;

                    if (cur == prev && cur != BlockType.Empty)
                    {
                        run++;
                    }
                    else
                    {
                        if (run >= 3 && prev != BlockType.Empty)
                        {
                            var match = BuildVerticalMatch(x, y - 1, run);
                            if (match.Cells.Count > 0)
                                yield return match;
                        }

                        run = 1;
                        prev = cur;
                    }
                }

                if (run >= 3 && prev != BlockType.Empty)
                {
                    var match = BuildVerticalMatch(x, state.Height - 1, run);
                    if (match.Cells.Count > 0)
                        yield return match;
                }
            }
        }

        public bool HasAnyMatch(IBoardState state)
        {
            foreach (var _ in DetectMatches(state))
                return true;
            return false;
        }

        private static Match BuildHorizontalMatch(int rightX, int y, int length)
        {
            if (length < 3) return new Match(System.Array.Empty<BoardCoordinates>());

            var cells = new List<BoardCoordinates>(length);
            for (int i = 0; i < length; i++)
                cells.Add(new BoardCoordinates(rightX - i, y));
            return new Match(cells);
        }

        private static Match BuildVerticalMatch(int x, int topY, int length)
        {
            if (length < 3) return new Match(System.Array.Empty<BoardCoordinates>());

            var cells = new List<BoardCoordinates>(length);
            for (int i = 0; i < length; i++)
                cells.Add(new BoardCoordinates(x, topY - i));
            return new Match(cells);
        }
    }
}