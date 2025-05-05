using CodeBase.Game.Board;
using Game.Board.Controllers;
using Game.Board.Data;
using Game.Domain.Services.Save;
using Game.Gamefield.Controllers;
using Game.Level.Controllers;
using Game.Level.Data;
using Game.Services.Input;
using GameUI.Presentation.Presenters;
using GameUI.Presentation.Views;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Game.Infrastructure.Installers
{
    public class GameplayInstaller : MonoInstaller
    {
        [Header("Configs")] [SerializeField] private SwipeConfig _swipeConfig;
        [SerializeField] private BalloonsController _balloonsController;

        [Header("Levels")] [SerializeField] private LevelAsset[] _levels;
        [SerializeField] private LevelView _levelView;

        [Header("Block Data")] [SerializeField]
        private BlockView blockViewPrefab;

        [SerializeField] private Transform blockParent;
        [SerializeField] private RuntimeAnimatorController baseBlock;

        [SerializeField] private BalloonView balloonViewPrefab;

        [SerializeField] private Transform _balloonsParent;

        public override void InstallBindings()
        {
            Container.Bind<BalloonsController>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container.BindInterfacesAndSelfTo<BalloonsPresenter>()
                .AsSingle();

            Container.BindMemoryPool<BalloonView, BalloonPool>()
                .WithInitialSize(10)
                .FromComponentInNewPrefab(balloonViewPrefab)
                .UnderTransform(_balloonsParent);

            // Configs & Assets
            Container.BindInstance(_swipeConfig);
            Container.BindInstance(_levels).AsSingle();
            Container.Bind<RuntimeAnimatorController>().WithId("BaseBlock").FromInstance(baseBlock);

            // SaveService
            Container.Bind<SaveManager>().AsSingle();
            Container.Bind<Level.Controllers.Level>().AsSingle();

            // Blocks Visuals
            Container.BindInterfacesAndSelfTo<BlockVisualSO>()
                .FromResources("BlockVisuals")
                .AsCached();
            Container.BindInterfacesAndSelfTo<BlockVisualCache>().AsSingle().NonLazy();

            // Input
            Container.BindInterfacesTo<InputService>().AsSingle();

            // Gameplay
            Container.Bind<IInitialBoardProvider>().To<AssetBoardProvider>().AsSingle();
            Container.Bind<IBoardRules>().To<BoardRulesClassic>().AsSingle();
            Container.Bind<IMatchFinder>().To<MatchFinderBasic>().AsSingle();
            Container.Bind<IBoardService>().To<BoardService>().AsSingle();

            // Board
            Container.Bind<IBoardView>()
                .To<BoardView>()
                .FromComponentInHierarchy()
                .AsSingle()
                .NonLazy();

            Container.BindInterfacesAndSelfTo<BoardPresenter>().AsSingle().NonLazy();

            // UI
            Container.Bind<LevelView>().FromInstance(_levelView).AsSingle();

            Container.Bind<CameraBoardScaler>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container.BindInterfacesAndSelfTo<LevelPresenter>().AsSingle();

            // Pool
            Container.BindMemoryPool<BlockView, BlockPool>()
                .WithInitialSize(64)
                .FromComponentInNewPrefab(blockViewPrefab)
                .UnderTransform(blockParent);
        }
    }
}