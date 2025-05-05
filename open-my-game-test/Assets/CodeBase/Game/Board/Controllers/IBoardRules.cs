using CodeBase.Game.Board;
using Game.Board.Data;

namespace Game.Board.Controllers
{
    public interface IBoardRules
    {
        public int Width { get; }
        public int Height { get; }
        public Block GenerateElement(int x, int y);
        public bool IsSwapValid(BoardCoordinates a, BoardCoordinates b, IMatchFinder finder, IBoardState state);
    }
}