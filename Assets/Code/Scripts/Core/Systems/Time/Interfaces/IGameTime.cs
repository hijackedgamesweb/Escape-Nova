using System;

namespace Code.Scripts.Core.Managers.Interfaces
{
    public interface IGameTime
    {
        float GameTime { get; }
        int CurrentCycle { get; }
        float TimeScale { get; }
        bool IsPaused { get; }
        
        void SetSpeed(float timeScale);
        void Pause();
        void Resume();

        void StartTimer();
        
        event Action<float> OnTimeAdvanced;
        event Action<int> OnCycleCompleted;
        event Action<float> OnTimeScaleChanged;
    }
}