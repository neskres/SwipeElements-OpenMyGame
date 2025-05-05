#nullable enable
using System;
using System.Collections.Generic;
using CodeBase.Game.Board;
using Cysharp.Threading.Tasks;
using Game.Board.Data;
using Game.Shared.StaticData;
using UniRx;

namespace Game.Board.Controllers
{
    public interface IBoardService
    {
        public IReadOnlyReactiveProperty<IBoardState> StateRx { get; }
        public IObservable<IReadOnlyList<Match>> OnMatches { get; }

        public UniTask<IReadOnlyList<Match>?> TrySwapAsync(BoardCoordinates a, BoardCoordinates b);
        public UniTask<IReadOnlyList<Match>?> ContinueCascadeAsync();
        public UniTask HandleMatchesAsync(IReadOnlyList<Match> matches);
        public void NotifyStepComplete();

        public void Reset();
        void LoadBoard(BlockType[][] dataBoard);
        BlockType[][] ExportBoard();
    }
}