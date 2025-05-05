using System;
using UnityEngine;

namespace Core.MVP.Views
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupView : ViewBase
    {
        [field: SerializeField] public CanvasGroup CanvasGroup { get; private set; }

        private void OnValidate()
        {
            if(CanvasGroup == null)
                CanvasGroup = GetComponent<CanvasGroup>();
        }

        public void Show()
        {
            CanvasGroup.alpha = 1;
            CanvasGroup.interactable = true;
            CanvasGroup.blocksRaycasts = true;
        }

        public void Hide()
        {
            CanvasGroup.alpha = 0;
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
        }
    }
}