using UnityEngine;

namespace Utilities
{
    public sealed class Timer
    {
        readonly float duration;
        float elapsed;
        public bool IsRunning { get; private set; }
        public float RemainingTime => Mathf.Max(0f, duration - elapsed);
        public int RemainingSeconds => Mathf.CeilToInt(RemainingTime);
        public bool IsFinished => elapsed >= duration;
        public Timer(float duration) => this.duration = duration;

        public void Start()
        {
            IsRunning = true;
            elapsed = 0f;
        }

        public void Tick(float deltaTime)
        {
            if (!IsRunning) return;
            elapsed += deltaTime;
            if (elapsed >= duration)
            {
                elapsed = duration;
                IsRunning = false;
            }

        }

    }

}


