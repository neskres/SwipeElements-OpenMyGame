using System;

namespace Game.Services.Input
{
    public interface IInputService
    {
        public event Action<SwipeData> OnSwipe;
        public SwipeData LastSwipe { get; }
    }
}