using System.Collections.Generic;
using Game.Board.Data;

namespace CodeBase.Game.Board
{
    public interface IBoardState
    {
        public int Width { get; }
        public int Height { get; }
        public IBlock Get(int x, int y);
        public IEnumerable<BoardCoordinates> AllCoords();
    }
}