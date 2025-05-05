using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Gamefield.Data;
using GameUI.Presentation.Views;
using UnityEngine;
using Zenject;

namespace Game.Gamefield.Controllers
{
    [RequireComponent(typeof(BalloonsSpawner))]
    public class BalloonsController : MonoBehaviour
    {
        [SerializeField] private BalloonsConfig _balloonsConfig;
        [SerializeField] private BalloonsSpawner _balloonsSpawner;
        private CancellationTokenSource _cts = new();

        public void StartLaunching()
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            LaunchBalloons();
        }

        private async void LaunchBalloons()
        {
            for (int i = 0; i < _balloonsConfig.MaxCount; i++)
            {
                SpawnBalloon();
                var canceled = await UniTask.Delay(
                        TimeSpan.FromSeconds(_balloonsConfig.Delay),
                        cancellationToken: _cts.Token)
                    .SuppressCancellationThrow();

                if (canceled) return;
            }
        }

        private void SpawnBalloon()
        {
            var balloon = _balloonsSpawner.SpawnRandomBalloon();
            balloon.OutOfScreen += OnBalloonOutOfScreen;
            balloon.SetMovementState(true);
        }

        private void OnBalloonOutOfScreen(BalloonView balloonView)
        {
            balloonView.OutOfScreen -= OnBalloonOutOfScreen;
            SpawnBalloon();
        }

        public void Stop()
        {
            _cts?.Cancel();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _balloonsSpawner ??= GetComponent<BalloonsSpawner>();
        }
#endif
    }
}