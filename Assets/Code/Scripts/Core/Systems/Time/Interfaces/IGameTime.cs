using System;

namespace Code.Scripts.Core.Managers.Interfaces
{
    public interface IGameTime
    {
        float GameTime { get; }
        float TimeScale { get; }
        bool IsPaused { get; }
        
        void SetSpeed(float timeScale);
        void Pause();
        void Resume();

        void StartTimer();
        
        event Action<float> OnTimeAdvanced;
    }
}