using UnityEngine;

namespace Game.Services.Input
{
    [CreateAssetMenu(menuName = "Config/Swipe Config")]
    public class SwipeConfig : ScriptableObject
    {
        [Min(1f), Tooltip("Min swipe length, px")]
        public float MinimumDistance = 100f;

        [Min(0f), Tooltip("Max swipe time, сек")]
        public float MaximumTime = 0.5f;
    }
}