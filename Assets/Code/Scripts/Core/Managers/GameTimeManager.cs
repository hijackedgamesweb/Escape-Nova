using System;
using Code.Scripts.Config;
using Code.Scripts.Core.Events;
using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Core.SaveLoad.Interfaces;
using Code.Scripts.Patterns.ServiceLocator;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Scripts.Core.Managers
{
    public class GameTimeManager : MonoBehaviour, IGameTime, ISaveable
    {
        [Header("Configuración de Derrota")]
        [SerializeField] public int maxCycles = 1000; 
        [Header("Configuración de tiempo")]
        [SerializeField] private TimeConfig timeConfig;

        public float GameTime { get; private set; } = 0f;
        public int CurrentCycle { get; private set; } = 0;
        public float TimeScale { get; private set; } = 1f;
        public bool IsPaused => TimeScale == 0f;
        public bool StartPaused = true;

        private float _nextCycleTime;
        
        private static float _lastKnownTimeScale = 1f;

        public event Action<float> OnTimeAdvanced;
        public event Action<int> OnCycleCompleted;
        public event Action OnGameOver; 
        public event Action<float> OnTimeScaleChanged;

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
            
            if (!StartPaused) {
                StartTimer();
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
            OnTimeScaleChanged?.Invoke(TimeScale);
        }

        public void SetSpeed(float timeScale)
        {
            TimeScale = Mathf.Max(0f, timeScale);
            _lastKnownTimeScale = TimeScale;
            OnTimeScaleChanged?.Invoke(TimeScale);
        }

        public void Pause()
        {
            TimeScale = 0f;
            OnTimeScaleChanged?.Invoke(TimeScale);
        }
        
        public void Resume()
        {
            TimeScale = _lastKnownTimeScale;
            OnTimeScaleChanged?.Invoke(TimeScale);
        }
        
        public void SetCurrentCycle(int cycle)
        {
            CurrentCycle += cycle;
        }

        public string GetSaveId()
        {
            return "GameTimeManager";
        }

        public JToken CaptureState()
        {
            JObject obj = new JObject
            {
                ["gameTime"] = GameTime,
                ["currentCycle"] = CurrentCycle,
                ["timeScale"] = TimeScale,
                ["nextCycleTime"] = _nextCycleTime
            };
            return obj;
        }

        public void RestoreState(JToken state)
        {
            GameTime = state["gameTime"].ToObject<float>();
            CurrentCycle = state["currentCycle"].ToObject<int>();
            TimeScale = state["timeScale"].ToObject<float>();
            _nextCycleTime = state["nextCycleTime"].ToObject<float>();
            _lastKnownTimeScale = TimeScale;

            // Forzar notificación para que la UI actualice correctamente
            OnCycleCompleted?.Invoke(CurrentCycle);
        }
    }
}