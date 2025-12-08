using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Patterns.ServiceLocator;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code.Scripts.UI.HUD
{
    public class TimerControlUI : MonoBehaviour
    {
        private IGameTime _gameTime;
        
        [SerializeField] private Image _pauseButton;
        [SerializeField] private Sprite _basePauseButtonSprite;
        [SerializeField] private Sprite _highlightedPauseButtonSprite;
        
        [SerializeField] private Image _baseSpeedButton;
        [SerializeField] private Sprite _baseBaseSpeedButtonSprite;
        [SerializeField] private Sprite _hightlightedBaseButtonSprite;
        
        [SerializeField] private Image _doubleSpeedButton;
        [SerializeField] private Sprite _baseDoubleSpeedButtonSprite;
        [SerializeField] private Sprite _highlightedDoubleSpeedButtonSprite;
        
        [SerializeField] private Image _fourthSpeedButton;
        [SerializeField] private Sprite _baseFourthSpeedButtonSprite;
        [SerializeField] private Sprite _highlightedFourthSpeedButtonSprite;
        
        void Start()
        {
            _gameTime = ServiceLocator.GetService<IGameTime>();
            
            if (_gameTime != null)
            {
                _gameTime.OnTimeScaleChanged += UpdateVisuals;
                if (SceneManager.GetActiveScene().name == "InGame") { UpdateVisuals(_gameTime.TimeScale); }
            }
        }

        private void OnDestroy()
        {
            if (_gameTime != null)
            {
                _gameTime.OnTimeScaleChanged -= UpdateVisuals;
            }
        }

        public void Pause()
        {
            _gameTime?.Pause();
        }

        public void PlayX1()
        {
            _gameTime?.SetSpeed(1f);
        }
        
        public void PlayX2()
        {
            _gameTime?.SetSpeed(2f);
        }
        
        public void PlayX4()
        {
            _gameTime?.SetSpeed(4f);
        }

        private void UpdateVisuals(float currentScale)
        {
            _pauseButton.sprite = _basePauseButtonSprite;
            _baseSpeedButton.sprite = _baseBaseSpeedButtonSprite;
            _doubleSpeedButton.sprite = _baseDoubleSpeedButtonSprite;
            _fourthSpeedButton.sprite = _baseFourthSpeedButtonSprite;
            
            AudioManager.Instance.PlaySFX("GameSpeedChanged");
            
            if (Mathf.Approximately(currentScale, 0f))
            {
                _pauseButton.sprite = _highlightedPauseButtonSprite;
            }
            else if (Mathf.Approximately(currentScale, 1f))
            {
                _baseSpeedButton.sprite = _hightlightedBaseButtonSprite;
            }
            else if (Mathf.Approximately(currentScale, 2f))
            {
                _doubleSpeedButton.sprite = _highlightedDoubleSpeedButtonSprite;
            }
            else if (Mathf.Approximately(currentScale, 4f))
            {
                _fourthSpeedButton.sprite = _highlightedFourthSpeedButtonSprite;
            }
        }
    }
}