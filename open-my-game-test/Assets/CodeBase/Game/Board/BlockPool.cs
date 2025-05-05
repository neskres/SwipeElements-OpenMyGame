using GameUI.Presentation.Views;
using UnityEngine;
using Zenject;


namespace CodeBase.Game.Board
{
    public class BlockPool : MonoMemoryPool<Transform, BlockView>
    {
        protected override void Reinitialize(Transform parent, BlockView item)
        {
            item.transform.SetParent(parent);
            item.ResetState();
            item.gameObject.SetActive(true);
        }

        protected override void OnDespawned(BlockView item)
        {
            item.gameObject.SetActive(false);
        }
    }
}