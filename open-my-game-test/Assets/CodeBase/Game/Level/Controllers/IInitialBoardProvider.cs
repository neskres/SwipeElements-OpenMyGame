using CodeBase.Game.Board;
using Game.Board.Data;

namespace Game.Level.Controllers
{
    public interface IInitialBoardProvider
    {
        public BoardState GetInitial();
    }
}