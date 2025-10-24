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
        
        event Action<float> OnTimeAdvanced;
        event Action<int> OnCycleCompleted;
    }
}