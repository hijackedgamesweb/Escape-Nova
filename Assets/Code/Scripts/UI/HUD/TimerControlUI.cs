using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Patterns.ServiceLocator;
using UnityEngine;

namespace Code.Scripts.UI.HUD
{
    public class TimerControlUI : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private IGameTime _gameTime;
        void Start()
        {
            _gameTime = ServiceLocator.GetService<IGameTime>();
            if (_gameTime == null)
            {
                Debug.LogError("GameTime service not found!");
            }
        }

        public void Pause() => _gameTime?.Pause();
        public void PlayX1() => _gameTime?.SetSpeed(1f);
        public void PlayX2() => _gameTime?.SetSpeed(2f);
        public void PlayX4() => _gameTime?.SetSpeed(4f);
    }
}
