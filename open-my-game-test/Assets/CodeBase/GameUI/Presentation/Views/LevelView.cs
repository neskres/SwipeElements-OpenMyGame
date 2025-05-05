using System.Collections;
using System.Collections.Generic;
using Core.MVP.Views;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI.Presentation.Views
{
    public class LevelView : ViewBase
    {
        [field: SerializeField] public Button RestartButton { get; private set; }
        [field: SerializeField] public Button NextButton { get; private set; }
    }
}