using System.Collections.Generic;
using CodeBase.Game.Board;
using Game.Shared.StaticData;

namespace Game.Board.Controllers
{
    public interface IMatchFinder
    {
        public IEnumerable<Match> DetectMatches(IBoardState state);
        public bool HasAnyMatch(IBoardState state);
    }
}