using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace GameUI.Presentation.Views
{
    [RequireComponent(typeof(Animator))]
    public class BlockView : MonoBehaviour, IBlockView
    {
        [SerializeField] private Animator _animator;

        private const float MoveDuration = 0.25f;

        public bool IsDestroying { get; private set; }
        public bool IsBusy { get; private set; }

        public void Setup(RuntimeAnimatorController controller, float normalizedTime, int sortingOrder)
        {
            _animator.runtimeAnimatorController = controller;
            _animator.Play("Idle", 0, normalizedTime);

            var sr = _animator.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.sortingOrder = sortingOrder;
        }

        public void ResetState()
        {
            IsBusy = false;
            IsDestroying = false;
        }

        public Tween MoveTo(Vector3 localTarget)
        {
            return transform.DOLocalMove(localTarget, MoveDuration)
                .SetEase(Ease.InOutQuad)
                .OnPlay(() => IsBusy = true)
                .OnComplete(() => IsBusy = false);
        }

        public UniTask PlayDestroyAsync() => WaitDestroyFinished();

        private async UniTask WaitDestroyFinished()
        {
            if (IsDestroying)
            {
                return;
            }

            IsDestroying = true;
            IsBusy = true;

            _animator.Play("Destroy", 0, 0);
            _animator.Update(0);

            var len = _animator.GetCurrentAnimatorStateInfo(0).length;

            if (len <= 0.01f)
            {
                return;
            }

            await UniTask.Delay(TimeSpan.FromSeconds(len));
        }
    }
}