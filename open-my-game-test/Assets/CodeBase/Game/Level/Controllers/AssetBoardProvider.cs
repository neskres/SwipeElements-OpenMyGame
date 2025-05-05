using Game.Board.Data;

namespace Game.Level.Controllers
{
    public class AssetBoardProvider : IInitialBoardProvider
    {
        private readonly Level _manager;

        public AssetBoardProvider(Level manager) => _manager = manager;

        public BoardState GetInitial() => _manager.Current.ToBoardState();
    }
}