#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Game.Board;
using Cysharp.Threading.Tasks;
using Game.Board.Data;
using Game.Level.Controllers;
using Game.Shared.StaticData;
using UniRx;

namespace Game.Board.Controllers
{
    public class BoardService : IBoardService
    {
        private readonly IBoardRules _rules;
        private readonly IMatchFinder _finder;
        private readonly IInitialBoardProvider _initial;

        private Block?[,] _grid;
        private int Width => _grid.GetLength(0);
        private int Height => _grid.GetLength(1);

        readonly ReactiveProperty<IBoardState> _stateRx;
        public IReadOnlyReactiveProperty<IBoardState> StateRx => _stateRx;

        readonly Subject<IReadOnlyList<Match>> _matchSubject = new();
        public IObservable<IReadOnlyList<Match>> OnMatches => _matchSubject;

        private UniTaskCompletionSource<IReadOnlyList<Match>>? _cascadeResult;

        public BoardService(IBoardRules rules, IMatchFinder finder, IInitialBoardProvider initial)
        {
            _rules = rules;
            _finder = finder;
            _initial = initial;

            var start = _initial.GetInitial();
            _grid = ToNullableGrid(start);
            _stateRx = new ReactiveProperty<IBoardState>(start);
        }

        private void DebugGrid()
        {
            for (int y = Height - 1; y >= 0; y--)
            {
                string line = "";
                for (int x = 0; x < Width; x++)
                {
                    var block = _grid[x, y];
                    line += block != null ? block.Value.Type.ToString()[0] : '.';
                }
            }
        }

        public async UniTask<IReadOnlyList<Match>?> TrySwapAsync(BoardCoordinates a, BoardCoordinates b)
        {
            Swap(a, b);
            DebugGrid();

            ApplyGravity();
            Publish();

            var matches = _finder.DetectMatches(CurrentSnapshot()).ToList();

            if (matches.Count == 0)
                return null;

            _cascadeResult = new UniTaskCompletionSource<IReadOnlyList<Match>>();
            _matchSubject.OnNext(matches);
            return await _cascadeResult.Task;
        }

        public async UniTask<IReadOnlyList<Match>?> ContinueCascadeAsync()
        {
            return await DoCascadeStepAsync();
        }

        private async UniTask<IReadOnlyList<Match>?> DoCascadeStepAsync()
        {
            var matches = _finder.DetectMatches(CurrentSnapshot()).ToList();

            if (matches.Count == 0)
                return null;

            _cascadeResult = new UniTaskCompletionSource<IReadOnlyList<Match>>();
            _matchSubject.OnNext(matches);
            return await _cascadeResult.Task;
        }

        public void NotifyStepComplete()
        {
            var matches = _finder.DetectMatches(CurrentSnapshot()).ToList();
            _cascadeResult?.TrySetResult(matches);
        }

        public UniTask HandleMatchesAsync(IReadOnlyList<Match> matches)
        {
            foreach (var m in matches)
            foreach (var c in m.Cells)
                _grid[c.X, c.Y] = null;

            ApplyGravity();
            Publish();

            DebugGrid();
            return UniTask.CompletedTask;
        }

        public void Reset()
        {
            var fresh = _initial.GetInitial();
            _grid = ToNullableGrid(fresh);

            ApplyGravity();
            Publish();
        }

        private void Swap(BoardCoordinates a, BoardCoordinates b) =>
            (_grid[a.X, a.Y], _grid[b.X, b.Y]) = (_grid[b.X, b.Y], _grid[a.X, a.Y]);

        private void ApplyGravity()
        {
            for (int x = 0; x < Width; x++)
            {
                int writeY = 0;

                for (int y = 0; y < Height; y++)
                {
                    if (_grid[x, y] != null)
                    {
                        if (y != writeY)
                        {
                            _grid[x, writeY] = _grid[x, y];
                            _grid[x, y] = null;
                        }

                        writeY++;
                    }
                }

                while (writeY < Height)
                    _grid[x, writeY++] = null;
            }
        }

        private Block?[,] ToNullableGrid(IBoardState state)
        {
            var arr = new Block?[state.Width, state.Height];
            foreach (var c in state.AllCoords())
            {
                var block = (Block)state.Get(c.X, c.Y);
                arr[c.X, c.Y] = block.Type == BlockType.Empty ? null : block;
            }

            return arr;
        }

        private BoardState CurrentSnapshot()
        {
            var raw = new Block[Width, Height];
            for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                raw[x, y] = _grid[x, y] ?? new Block(BlockType.Empty);
            return new BoardState(raw);
        }

        private void Publish() => _stateRx.Value = CurrentSnapshot();

        public BlockType[][] ExportBoard()
        {
            var result = new BlockType[Width][];
            for (int x = 0; x < Width; x++)
            {
                result[x] = new BlockType[Height];
                for (int y = 0; y < Height; y++)
                {
                    result[x][y] = _grid[x, y]?.Type ?? BlockType.Empty;
                }
            }

            return result;
        }

        public void LoadBoard(BlockType[][] board)
        {
            int width = board.Length;
            int height = board[0].Length;
            _grid = new Block?[width, height];

            for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                var type = board[x][y];
                _grid[x, y] = type == BlockType.Empty ? null : new Block(type);
            }

            Publish();
        }
    }
}