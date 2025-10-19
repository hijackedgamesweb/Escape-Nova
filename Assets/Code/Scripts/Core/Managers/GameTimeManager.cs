using System;
using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Patterns.ServiceLocator;
using UnityEngine;

namespace Code.Scripts.Core.Managers
{
    public class GameTimeManager : MonoBehaviour, IGameTime
    {
        public float GameTime { get; private set; } = 0f;
        public float TimeScale { get; private set; } = 1f;
        public bool IsPaused => TimeScale == 0f;
        
        public event Action<float> OnTimeAdvanced;

        public void Awake()
        {
            TimeScale = 0f;
            ServiceLocator.RegisterService<IGameTime>(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.UnregisterService<IGameTime>();
        }


        public void Update()
        {
            if (IsPaused) return;

            float deltaTime = Time.deltaTime * TimeScale;
            GameTime += deltaTime;
            
            OnTimeAdvanced?.Invoke(deltaTime);
        }

        public void StartTimer()
        {
            GameTime = 0f;
            TimeScale = 1f;
        }

        public void SetSpeed(float timeScale) => TimeScale = Mathf.Max(0f, timeScale);
        public void Pause() => TimeScale = 0f;
        public void Resume() => TimeScale = 1f;

    }
}