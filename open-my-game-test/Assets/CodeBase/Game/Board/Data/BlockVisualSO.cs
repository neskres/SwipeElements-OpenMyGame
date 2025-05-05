using Game.Shared.StaticData;
using UnityEngine;

namespace Game.Board.Data
{
    [CreateAssetMenu(menuName = "Match-3/Block Visual (Override)")]
    public class BlockVisualSO : ScriptableObject
    {
        public BlockType Type;
        public AnimationClip IdleClip;
        public AnimationClip DestroyClip;
    }
}