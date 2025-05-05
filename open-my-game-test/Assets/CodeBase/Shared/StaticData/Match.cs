using System.Collections.Generic;
using CodeBase.Game.Board;
using Game.Board.Data;

namespace Game.Shared.StaticData
{
    public readonly struct Match
    {
        public IReadOnlyList<BoardCoordinates> Cells { get; }

        public Match(IReadOnlyList<BoardCoordinates> cells) => Cells = cells;
    }
}