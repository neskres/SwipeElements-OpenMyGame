using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace GameUI.Presentation.Views
{
    public interface IBlockView
    {
        public bool IsBusy { get; }
        public bool IsDestroying { get; }
        public Transform transform { get; }
        public GameObject gameObject { get; }

        public void Setup(RuntimeAnimatorController controller, float normalizedTime, int sortingOrder);
        public Tween MoveTo(Vector3 target);
        public UniTask PlayDestroyAsync();
        public void ResetState();
    }
}