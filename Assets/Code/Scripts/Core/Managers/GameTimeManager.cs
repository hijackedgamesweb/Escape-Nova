using System;
using Code.Scripts.Config;
using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Patterns.ServiceLocator;
using UnityEngine;

namespace Code.Scripts.Core.Managers
{
    public class GameTimeManager : MonoBehaviour, IGameTime
    {
        [Header("Configuración de tiempo")]
        [SerializeField] private TimeConfig timeConfig;
        
        public float GameTime { get; private set; } = 0f;
        public int CurrentCycle { get; private set ; } = 0;
        public float TimeScale { get; private set; } = 1f;
        public bool IsPaused => TimeScale == 0f;
        
        private float _nextCycleTime;
        public event Action<float> OnTimeAdvanced;
        public event Action<int> OnCycleCompleted;

        public void Awake()
        {
            TimeScale = 0f;
            ServiceLocator.RegisterService<IGameTime>(this);
            
            if (timeConfig == null)
            {
                Debug.LogWarning("TimeConfig no asignado, usando valores por defecto");
                timeConfig = ScriptableObject.CreateInstance<TimeConfig>();
            }

            _nextCycleTime = timeConfig.secondsPerCycle;
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
            
            while (GameTime >= _nextCycleTime)
            {
                CurrentCycle += 1;
                OnCycleCompleted?.Invoke(CurrentCycle);
                _nextCycleTime += timeConfig.secondsPerCycle;
            }
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