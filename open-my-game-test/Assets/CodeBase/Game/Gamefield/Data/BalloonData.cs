using UnityEngine;

namespace Game.Gamefield.Data
{
    public struct BalloonData
    {
        public float Speed;
        public Vector3 Destination;
        public bool AutoDespawn;
        public float PathSineAmplitude;
        public float PathSineFrequency;
    }
}