using System.Collections.Generic;
using CodeBase.Game.Board;

namespace Game.Board.Data
{
    public class BoardState : IBoardState
    {
        readonly Block[,] _grid;

        public int Width => _grid.GetLength(0);
        public int Height => _grid.GetLength(1);

        public BoardState(Block[,] grid) => _grid = grid;

        public IBlock Get(int x, int y) => _grid[x, y];

        public IEnumerable<BoardCoordinates> AllCoords()
        {
            for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                yield return new BoardCoordinates(x, y);
        }
    }
}