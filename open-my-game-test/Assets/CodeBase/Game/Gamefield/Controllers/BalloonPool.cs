using GameUI.Presentation.Views;
using UnityEngine;
using Zenject;

namespace Game.Gamefield.Controllers
{
    public class BalloonPool : MonoMemoryPool<BalloonView>
    {
        protected override void Reinitialize(BalloonView balloonView)
        {
            balloonView.gameObject.SetActive(true);
            balloonView.ResetPoolState();
        }

        protected override void OnDespawned(BalloonView balloonView)
        {
            balloonView.gameObject.SetActive(false);
        }
    }
}