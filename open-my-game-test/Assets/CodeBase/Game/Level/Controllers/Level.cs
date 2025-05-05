using Game.Level.Data;
using UnityEngine;

namespace Game.Level.Controllers
{
    public class Level
    {
        private readonly LevelAsset[] _levels;
        private int _currentIndex;

        public LevelAsset Current => _levels[_currentIndex];
        public int Index => _currentIndex;

        public Level(LevelAsset[] levels)
        {
            _levels = levels;
            _currentIndex = 0;
        }

        private bool CanGoNext => _currentIndex < _levels.Length - 1;

        public void Restart()
        {
            
        }

        public void Next()
        {
            if (CanGoNext)
                _currentIndex++;
            else
                ResetProgress();
        }

        public void Load(int index)
        {
            if (index >= 0 && index < _levels.Length)
                _currentIndex = index;
            else
                Debug.LogWarning($"[Level] Invalid index: {index}");
        }

        private void ResetProgress() => _currentIndex = 0;
    }
}