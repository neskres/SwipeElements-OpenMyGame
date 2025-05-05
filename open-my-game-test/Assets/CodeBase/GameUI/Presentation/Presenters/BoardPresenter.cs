using System;
using System.Collections.Generic;
using CodeBase.Game.Board;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;
using DG.Tweening;
using Game.Board.Controllers;
using Game.Board.Data;
using Game.Domain.Services.Save;
using Game.Level.Controllers;
using Game.Services.Input;
using Game.Shared.StaticData;
using GameUI.Presentation.Views;

namespace GameUI.Presentation.Presenters
{
    public class BoardPresenter : IInitializable, IDisposable
    {
        private readonly IBoardService _svc;
        private readonly IBoardRules _rules;
        private readonly IInputService _input;
        private readonly IBoardView _view;
        private readonly BlockVisualCache _visualCache;
        private readonly SaveManager _saveManager;
        private readonly Level _levelManager;

        private readonly Dictionary<BoardCoordinates, BlockPresenter> _views = new();
        private readonly CompositeDisposable _d = new();

        private bool _cascadeRunning;

        private BlockPool _blockPool;

        public BoardPresenter(
            IBoardService svc,
            IBoardRules rules,
            IInputService input,
            IBoardView view,
            BlockVisualCache visualCache,
            SaveManager saveManager,
            Level levelManager,
            BlockPool blockPool)
        {
            _svc = svc;
            _rules = rules;
            _input = input;
            _view = view;
            _visualCache = visualCache;
            _saveManager = saveManager;
            _levelManager = levelManager;
            _blockPool = blockPool;
        }

        public void Initialize()
        {
            _input.OnSwipe += HandleSwipe;

            _view.OnSwapRequest
                .Where(_ => !_cascadeRunning)
                .Subscribe(p => HandleCascade(p.Item1, p.Item2).Forget())
                .AddTo(_d);

            _svc.OnMatches
                .Subscribe(async matches =>
                {
                    await AnimateDestroyAsync(matches);
                    _svc.NotifyStepComplete();
                })
                .AddTo(_d);

            Render(_svc.StateRx.Value);

            _svc.StateRx.Skip(1)
                .Subscribe(Render)
                .AddTo(_d);
        }

        public void Dispose()
        {
            _input.OnSwipe -= HandleSwipe;
            _d.Dispose();
        }

        private async UniTaskVoid HandleCascade(BoardCoordinates a, BoardCoordinates b)
        {
            _cascadeRunning = true;

            await AnimateSwap(a, b);

            var matches = await _svc.TrySwapAsync(a, b);
            if (matches == null)
            {
                _cascadeRunning = false;
                return;
            }

            await AnimateDestroyAsync(matches);
            await _svc.HandleMatchesAsync(matches);
            _svc.NotifyStepComplete();

            while (true)
            {
                var nextMatches = await _svc.ContinueCascadeAsync();
                if (nextMatches == null || nextMatches.Count == 0)
                    break;

                await AnimateDestroyAsync(nextMatches);
                await _svc.HandleMatchesAsync(nextMatches);
                _svc.NotifyStepComplete();
            }

            _saveManager.Save(new SaveData
            {
                CurrentLevelIndex = _levelManager.Index,
                Board = _svc.ExportBoard()
            });

            _cascadeRunning = false;
        }

        private void HandleSwipe(SwipeData data)
        {
            if (data.Direction == SwipeDirection.None) return;

            var fromOpt = _view.ScreenToBoard(data.StartPosition);
            if (!fromOpt.HasValue) return;

            var from = fromOpt.Value;
            var delta = Dir2Delta(data.Direction);
            var to = new BoardCoordinates(from.X + delta.X, from.Y + delta.Y);

            if (!IsInsideBoard(to)) return;

            if (_views.TryGetValue(from, out var v1) && v1.IsBusy) return;
            if (_views.TryGetValue(to, out var v2) && v2.IsBusy) return;

            bool isHorizontal = Math.Abs(from.X - to.X) == 1 && from.Y == to.Y;
            bool oneEmpty = !_views.ContainsKey(from) || !_views.ContainsKey(to);

            if (oneEmpty && !isHorizontal)
                return;

            _view.OnSwapRequestSubject.OnNext((from, to));
        }

        private void Render(IBoardState state)
        {
            var fresh = new Dictionary<BoardCoordinates, BlockPresenter>();

            foreach (var c in state.AllCoords())
            {
                var block = (Block)state.Get(c.X, c.Y);
                if (block.Type == BlockType.Empty) continue;

                if (_views.TryGetValue(c, out var existing) && !existing.IsDestroying)
                {
                    existing.MoveTo(_view.CoordToPos(c));
                    fresh[c] = existing;
                    continue;
                }

                var view = _blockPool.Spawn(_view.Root);
                var presenter = new BlockPresenter(view, block.Type);
                presenter.Initialize(
                    _visualCache.GetOverride(block.Type),
                    UnityEngine.Random.value,
                    c.Y
                );
                presenter.SetLocalPosition(_view.CoordToPos(c));
                presenter.MoveTo(_view.CoordToPos(c));
                fresh[c] = presenter;
            }

            foreach (var kv in _views)
                if (!kv.Value.IsDestroying && !fresh.ContainsValue(kv.Value))
                    GameObject.Destroy(kv.Value.Transform.gameObject);

            _views.Clear();
            foreach (var kv in fresh)
                _views[kv.Key] = kv.Value;
        }

        private UniTask AnimateSwap(BoardCoordinates a, BoardCoordinates b)
        {
            var aHas = _views.TryGetValue(a, out var va);
            var bHas = _views.TryGetValue(b, out var vb);

            if (!aHas && !bHas)
                return UniTask.CompletedTask;

            var ta = aHas ? va.MoveTo(_view.CoordToPos(b)).AsyncWaitForCompletion().AsUniTask() : UniTask.CompletedTask;
            var tb = bHas ? vb.MoveTo(_view.CoordToPos(a)).AsyncWaitForCompletion().AsUniTask() : UniTask.CompletedTask;

            if (aHas) _views[b] = va;
            else _views.Remove(b);
            if (bHas) _views[a] = vb;
            else _views.Remove(a);

            return UniTask.WhenAll(ta, tb);
        }

        private readonly HashSet<BoardCoordinates> _alreadyRemoved = new();

        private async UniTask AnimateDestroyAsync(IReadOnlyList<Match> matches)
        {
            var tasks = new List<UniTask>();
            var toRemove = new List<BoardCoordinates>();

            foreach (var m in matches)
            foreach (var c in m.Cells)
            {
                if (_alreadyRemoved.Contains(c))
                {
                }
                else
                {
                    _alreadyRemoved.Add(c);
                }

                if (_views.TryGetValue(c, out var p))
                {
                    tasks.Add(p.PlayDestroyAsync());
                    toRemove.Add(c);
                }
            }

            if (tasks.Count > 0)
                await UniTask.WhenAll(tasks);

            foreach (var c in toRemove)
            {
                if (_views.TryGetValue(c, out var p))
                    _blockPool.Despawn(p.View);
                _views.Remove(c);
            }
        }

        private bool IsInsideBoard(BoardCoordinates c) =>
            c.X >= 0 && c.X < _rules.Width && c.Y >= 0 && c.Y < _rules.Height;

        private static BoardCoordinates Dir2Delta(SwipeDirection dir) => dir switch
        {
            SwipeDirection.Left => new(-1, 0),
            SwipeDirection.Right => new(1, 0),
            SwipeDirection.Up => new(0, 1),
            SwipeDirection.Down => new(0, -1),
            _ => new(0, 0)
        };

        public void ResetView()
        {
            foreach (var presenter in _views.Values)
            {
                DOTween.Kill(presenter.Transform);
                GameObject.Destroy(presenter.Transform.gameObject);
            }

            _views.Clear();
        }
    }
}