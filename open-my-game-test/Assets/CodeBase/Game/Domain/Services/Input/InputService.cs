using System;
using UnityEngine;
using Zenject;

namespace Game.Services.Input
{
    public class InputService : IInputService, ITickable, IInitializable, IDisposable
    {
        private readonly SwipeConfig _cfg;

        private Vector2 _startPos;
        private float _startTime;
        private bool _isSwiping;

        public event Action<SwipeData> OnSwipe = delegate { };
        public SwipeData LastSwipe { get; private set; }

        [Inject]
        public InputService(SwipeConfig cfg) => _cfg = cfg;

        public void Initialize()
        {
        }

        public void Dispose() => OnSwipe = null;

        public void Tick()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            HandleMouse();
#else
            HandleTouch();
#endif
        }

        #region Platform‑specific input

        private void HandleMouse()
        {
            if (UnityEngine.Input.GetMouseButtonDown(0))
                BeginSwipe(UnityEngine.Input.mousePosition);

            if (UnityEngine.Input.GetMouseButtonUp(0))
                EndSwipe(UnityEngine.Input.mousePosition);
        }

        private void HandleTouch()
        {
            if (UnityEngine.Input.touchCount == 0) return;

            var touch = UnityEngine.Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    BeginSwipe(touch.position);
                    break;
                case TouchPhase.Ended:
                    EndSwipe(touch.position);
                    break;
            }
        }

        #endregion

        #region Swipe logic

        private void BeginSwipe(Vector2 position)
        {
            _isSwiping = true;
            _startPos = position;
            _startTime = Time.time;
        }

        private void EndSwipe(Vector2 endPos)
        {
            if (!_isSwiping) return;
            _isSwiping = false;

            float duration = Time.time - _startTime;
            float distance = (endPos - _startPos).magnitude;

            if (distance < _cfg.MinimumDistance || duration > _cfg.MaximumTime)
            {
                DispatchSwipe(SwipeDirection.None, endPos, duration);
                return;
            }

            var delta = endPos - _startPos;
            var normalized = delta.normalized;

            var dir = Mathf.Abs(normalized.x) > Mathf.Abs(normalized.y)
                ? (normalized.x > 0 ? SwipeDirection.Right : SwipeDirection.Left)
                : (normalized.y > 0 ? SwipeDirection.Up : SwipeDirection.Down);

            DispatchSwipe(dir, endPos, duration);
        }

        private void DispatchSwipe(SwipeDirection dir, Vector2 endPos, float duration)
        {
            LastSwipe = new SwipeData
            {
                Direction = dir,
                StartPosition = _startPos,
                EndPosition = endPos,
                Duration = duration
            };

            OnSwipe.Invoke(LastSwipe);
        }

        #endregion
    }
}