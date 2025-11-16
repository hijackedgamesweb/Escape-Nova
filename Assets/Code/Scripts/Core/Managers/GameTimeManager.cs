using System;
using Code.Scripts.Config;
using Code.Scripts.Core.Events;
using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Patterns.ServiceLocator;
using UnityEngine;

namespace Code.Scripts.Core.Managers
{
    public class GameTimeManager : MonoBehaviour, IGameTime
    {
        [Header("Configuración de Derrota")]
        [SerializeField] public int maxCycles = 1000; 
        [Header("Configuración de tiempo")]
        [SerializeField] private TimeConfig timeConfig;

        public float GameTime { get; private set; } = 0f;
        public int CurrentCycle { get; private set; } = 0;
        public float TimeScale { get; private set; } = 1f;
        public bool IsPaused => TimeScale == 0f;

        private float _nextCycleTime;
        
        private static float _lastKnownTimeScale = 1f;

        public event Action<float> OnTimeAdvanced;
        public event Action<int> OnCycleCompleted;
        public event Action OnGameOver; 

        public void Awake()
        {
            TimeScale = 0f;

            ServiceLocator.RegisterService<IGameTime>(this);
            ServiceLocator.RegisterService<GameTimeManager>(this); 
            ServiceLocator.RegisterService<TimeConfig>(timeConfig);

            if (timeConfig == null)
            {
                timeConfig = ScriptableObject.CreateInstance<TimeConfig>();
                ServiceLocator.RegisterService<TimeConfig>(timeConfig);
            }

            _nextCycleTime = timeConfig.secondsPerCycle;
        }

        private void OnDestroy()
        {
            ServiceLocator.UnregisterService<IGameTime>();
            ServiceLocator.UnregisterService<GameTimeManager>(); 
            ServiceLocator.UnregisterService<TimeConfig>();
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

                if (CurrentCycle >= maxCycles) 
                {
                    Pause(); 
                    SystemEvents.TriggerGameOver();
                    enabled = false;
                    
                    return;
                }
            }
        }

        public void StartTimer()
        {
            TimeScale = _lastKnownTimeScale;
        }

        public void SetSpeed(float timeScale)
        {
            TimeScale = Mathf.Max(0f, timeScale);
            _lastKnownTimeScale = TimeScale;
        }

        public void Pause()
        {
            TimeScale = 0f;
        }
        
        public void Resume()
        {
            TimeScale = _lastKnownTimeScale;
        }
        
        //ESTE METODO EXISTE SOLO PARA DEBUGGEAR (LO USA EL SCRIPT "EscapeKeyListener.cs" HAY QUE BORRAR PARA EL RELESE
        public void SetCurrentCycle(int cycle)
        {
            CurrentCycle += cycle;
        }
    }
}