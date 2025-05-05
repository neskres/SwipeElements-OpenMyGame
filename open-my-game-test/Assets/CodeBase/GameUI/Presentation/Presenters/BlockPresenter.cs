using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Board.Data;
using Game.Shared.StaticData;
using GameUI.Presentation.Views;
using UnityEngine;

namespace GameUI.Presentation.Presenters
{
    public class BlockPresenter
    {
        private readonly IBlockView _view;
        
        private readonly BlockType _type;
        public BlockType Type => _type;

        public bool IsBusy => _view.IsBusy;
        public bool IsDestroying => _view.IsDestroying;
        public Transform Transform => _view.transform;
        public GameObject GameObject => _view.gameObject;
        public BlockView View => (BlockView)_view;

        public BlockPresenter(IBlockView view, BlockType type)
        {
            _view = view;
            _type = type;
        }

        public void Initialize(RuntimeAnimatorController controller, float normalizedTime, int sortingOrder)
        {
            _view.Setup(controller, normalizedTime, sortingOrder);
        }

        public void SetLocalPosition(Vector3 pos)
        {
            _view.transform.localPosition = pos;
        }

        public Tween MoveTo(Vector3 target)
        {
            return _view.MoveTo(target);
        }

        public UniTask PlayDestroyAsync()
        {
            return _view.PlayDestroyAsync();
        }
         
        public void UpdateSortingOrder(BoardCoordinates coord, int boardWidth)
        {
            int sortingOrder = coord.Y * boardWidth + coord.X;
            var sr = View.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.sortingOrder = sortingOrder;
        }

        public void Reset()
        {
            _view.ResetState();
        }

        public void Enable()
        {
            _view.gameObject.SetActive(true);
        }

        public void Disable()
        {
            _view.gameObject.SetActive(false);
        }
    }
}