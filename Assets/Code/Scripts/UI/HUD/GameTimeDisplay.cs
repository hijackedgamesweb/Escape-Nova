using Code.Scripts.Config;
using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Patterns.ServiceLocator;
using TMPro;
using UnityEngine;

namespace Code.Scripts.UI.HUD
{
    public class GameTimeDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timeText;
        [SerializeField] private TimeConfig _timeConfig;
        
        private IGameTime _gameTime;
        private int _lastCycle = 0;
        void Start()
        {
            if (_timeConfig == null)
            {
                _timeConfig = Resources.Load<TimeConfig>("Configs/TimeConfig");
            }
            _gameTime = ServiceLocator.GetService<IGameTime>();
            if (_gameTime == null)
            {
                Debug.LogError("GameTime service not found!");
                return;
            }
            _gameTime.OnCycleCompleted += OnTimeAdvanced;

            // Forzar actualización al inicio, en caso de que ya haya un valor guardado
            OnTimeAdvanced(_gameTime.CurrentCycle);
        }
        
        private void OnDestroy()
        {
            if (_gameTime != null)
            {
                _gameTime.OnCycleCompleted -= OnTimeAdvanced;
            }
        }
        
        private void OnTimeAdvanced(int currentCycle)
        {
            if (currentCycle != _lastCycle)
            {
                _lastCycle = currentCycle;
                UpdateLabel();
            }
        }
        
        private void UpdateLabel()
        {
            _timeText.text = $"{_lastCycle}";
        }
    }
}
