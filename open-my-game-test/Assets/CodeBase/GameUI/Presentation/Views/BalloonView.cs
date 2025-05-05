using System;
using Game.Gamefield.Data;
using UnityEngine;

namespace GameUI.Presentation.Views
{
    public class BalloonView : MonoBehaviour
    {
        public event Action<BalloonView> OutOfScreen;

        [SerializeField] private SpriteRenderer _spriteRenderer;
        private BalloonData _balloonData;
        private bool _canMove;
        private bool _isPooled;

        public Action<BalloonView> ReturnToPool;

        public void SetData(Sprite sprite, BalloonData balloonData)
        {
            _spriteRenderer.sprite = sprite;
            _balloonData = balloonData;
        }

        public void SetMovementState(bool value)
        {
            _canMove = value;
        }

        private void Update()
        {
            if (!_canMove) return;

            if (Mathf.Abs(_balloonData.Destination.x - transform.position.x) <= 0.02f)
            {
                SetMovementState(false);
                OutOfScreen?.Invoke(this);

                if (_balloonData.AutoDespawn && !_isPooled && ReturnToPool != null)
                {
                    _isPooled = true;
                    ReturnToPool(this);
                }
            }

            var direction = (_balloonData.Destination - transform.position);
            direction.z = 0;
            transform.position += direction.normalized * _balloonData.Speed * Time.deltaTime;

            var posY = Mathf.Sin(Time.time * _balloonData.PathSineFrequency)
                       * _balloonData.PathSineAmplitude * Time.deltaTime;
            transform.position += Vector3.up * posY;
        }

        private void OnDisable()
        {
            OutOfScreen = null;
        }

        public void ResetPoolState()
        {
            _isPooled = false;
            SetMovementState(false);
            ReturnToPool = null;
        }
    }
}