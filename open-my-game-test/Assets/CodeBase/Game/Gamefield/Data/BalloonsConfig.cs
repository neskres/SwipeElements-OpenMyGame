using System.Collections.Generic;
using GameUI.Presentation.Views;
using UnityEngine;

namespace Game.Gamefield.Data
{
    [CreateAssetMenu(fileName = "BalloonsConfig", menuName = "Gameplay/Balloons/BalloonsConfig")]
    public class BalloonsConfig : ScriptableObject
    {
        [field: SerializeField] public BalloonView Prefab { get; private set; }
        [field: SerializeField] public List<Sprite> BalloonsSprites { get; private set; }


        [field: SerializeField] public int MaxCount { get; private set; }
        [field: SerializeField] public int Delay { get; private set; }

        [field: SerializeField] public bool IsStraightDirection { get; private set; } = true;
        [field: SerializeField] public float MinSpeed { get; private set; }
        [field: SerializeField] public float MaxSpeed { get; private set; }
        [field: SerializeField] public float PathSineAmplitude { get; private set; }
        [field: SerializeField] public float PathSineFrequency { get; private set; }
    }
}