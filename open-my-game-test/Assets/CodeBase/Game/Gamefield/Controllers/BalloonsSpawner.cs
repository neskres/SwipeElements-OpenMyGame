using Game.Gamefield.Data;
using GameUI.Presentation.Views;
using UnityEngine;
using Zenject;

namespace Game.Gamefield.Controllers
{
    public class BalloonsSpawner : MonoBehaviour
    {
        [SerializeField] private BalloonsConfig _balloonsConfig;
        [SerializeField] private Transform _balloonsParent;

        private Camera _cam;
        private float _screenBoundsOffset = 0.2f;

        [Inject] private BalloonPool _balloonPool;

        private void Awake()
        {
            _cam = Camera.main;
            transform.position = _cam.transform.position;
        }

        public BalloonView SpawnRandomBalloon()
        {
            var sprite = _balloonsConfig.BalloonsSprites[
                Random.Range(0, _balloonsConfig.BalloonsSprites.Count)];

            var spriteWidth = sprite.bounds.size.x;
            var spriteHeight = sprite.bounds.size.y;

            var bounds = GetBoundsForTarget(spriteWidth, spriteHeight);
            var randomY = Random.Range(bounds.min.y, bounds.max.y);

            var minX = bounds.min.x + spriteWidth / 2;
            var maxX = bounds.max.x - spriteWidth / 2;

            var side = Random.Range(0, 2);
            var fromX = side == 0 ? minX : maxX;
            var toX = side == 0 ? maxX : minX;

            var fromPos = new Vector3(fromX, randomY, 0f);
            randomY = _balloonsConfig.IsStraightDirection ? randomY : Random.Range(bounds.min.y, bounds.max.y);
            var toPos = new Vector3(toX, randomY, 0f);

            var speed = Random.Range(_balloonsConfig.MinSpeed, _balloonsConfig.MaxSpeed);

            var balloon = _balloonPool.Spawn();
            balloon.transform.SetParent(_balloonsParent, false);
            balloon.transform.position = fromPos;

            balloon.SetData(sprite, new BalloonData
            {
                AutoDespawn = true,
                Destination = toPos,
                Speed = speed,
                PathSineAmplitude = _balloonsConfig.PathSineAmplitude,
                PathSineFrequency = _balloonsConfig.PathSineFrequency
            });

            balloon.ReturnToPool = _balloonPool.Despawn;
            return balloon;
        }

        private Bounds GetBoundsForTarget(float spriteWidth, float spriteHeight)
        {
            var doubleWidth = spriteWidth * 2f;
            var doubleHeight = spriteHeight * 2f;

            var height = _cam.orthographicSize * 2f;
            var width = height * _cam.aspect;

            var finalHeight = height - doubleHeight - _screenBoundsOffset;
            var finalWidth = width + doubleWidth + _screenBoundsOffset;

            return new Bounds(_cam.transform.position, new Vector3(finalWidth, finalHeight));
        }
    }
}