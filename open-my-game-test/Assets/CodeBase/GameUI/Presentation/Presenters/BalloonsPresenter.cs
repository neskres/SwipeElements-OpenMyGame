using Core.MVP.Presenters;
using Game.Gamefield.Controllers;
using Zenject;

namespace GameUI.Presentation.Presenters
{
    public class BalloonsPresenter : IPresenter, IInitializable
    {
        private readonly BalloonsController _controller;

        [Inject]
        public BalloonsPresenter(BalloonsController controller)
        {
            _controller = controller;
        }

        public void Initialize()
        {
        }

        public void OnLevelStarted()
        {
            _controller.StartLaunching();
        }

        public void OnLevelEnded()
        {
            _controller.Stop();
        }

        public void Enable()
        {
            throw new System.NotImplementedException();
        }

        public void Disable()
        {
            throw new System.NotImplementedException();
        }
    }
}