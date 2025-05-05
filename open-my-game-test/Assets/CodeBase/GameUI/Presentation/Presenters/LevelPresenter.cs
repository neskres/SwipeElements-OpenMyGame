using Core.MVP.Presenters;
using Game.Board.Controllers;
using Game.Domain.Services.Save;
using Game.Level.Controllers;
using GameUI.Presentation.Views;
using UnityEngine;
using Zenject;

namespace GameUI.Presentation.Presenters
{
    public class LevelPresenter : IPresenter, IInitializable
    {
        private LevelView _view;
        private Level _levelManager;
        private IBoardService _boardService;
        private BoardPresenter _boardPresenter;
        private SaveManager _save;
        [Inject] private BalloonsPresenter _balloonsPresenter;
        [Inject] private CameraBoardScaler _cameraScaler;

        [Inject]
        public void Construct(
            LevelView view,
            Level levelManager,
            IBoardService boardService,
            BoardPresenter boardPresenter,
            SaveManager save)
        {
            _view = view;
            _levelManager = levelManager;
            _boardService = boardService;
            _boardPresenter = boardPresenter;
            _save = save;
        }

        public void Initialize()
        {
            _view.Construct(this);
            TryLoad();
            _balloonsPresenter.OnLevelStarted();
        }

        public void Enable()
        {
            _view.RestartButton.onClick.AddListener(RestartLevel);
            _view.NextButton.onClick.AddListener(NextLevel);
        }

        public void Disable()
        {
            _view.RestartButton.onClick.RemoveListener(RestartLevel);
            _view.NextButton.onClick.RemoveListener(NextLevel);

            _balloonsPresenter.OnLevelEnded();
        }

        private void RestartLevel()
        {
            _levelManager.Restart();
            _boardPresenter.ResetView();
            _boardService.Reset();
            Save();
        }

        private void NextLevel()
        {
            _levelManager.Next();
            _boardPresenter.ResetView();
            _boardService.Reset();
            Save();
            TryLoad();
        }

        private void Save()
        {
            var saveData = new SaveData
            {
                CurrentLevelIndex = _levelManager.Index,
                Board = _boardService.ExportBoard()
            };
            _save.Save(saveData);
        }

        private void TryLoad()
        {
            var data = _save.Load();
            if (data != null)
            {
                _levelManager.Load(data.CurrentLevelIndex);
                _boardPresenter.ResetView();
                _boardService.LoadBoard(data.Board);

                _cameraScaler.AdjustCameraToBoard(_levelManager.Current);
            }
        }
    }
}