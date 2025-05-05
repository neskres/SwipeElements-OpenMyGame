using UnityEngine;

namespace Game.Services.Input
{
    public struct SwipeData
    {
        public SwipeDirection Direction;
        public Vector2 StartPosition;
        public Vector2 EndPosition;
        public float Duration;
    }
}