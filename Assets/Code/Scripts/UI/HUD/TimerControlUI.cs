using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Patterns.ServiceLocator;
using UnityEngine;
using UnityEngine.UIElements;

namespace Code.Scripts.UI.HUD
{
    public class TimerControlUI : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private IGameTime _gameTime;
        
        [SerializeField] private UnityEngine.UI.Image _pauseButton;
        [SerializeField] private Sprite _basePauseButtonSprite;
        [SerializeField] private Sprite _highlightedPauseButtonSprite;
        
        [SerializeField] private UnityEngine.UI.Image _baseSpeedButton;
        [SerializeField] private Sprite _baseBaseSpeedButtonSprite;
        [SerializeField] private Sprite _hightlightedBaseButtonSprite;
        
        [SerializeField] private UnityEngine.UI.Image _doubleSpeedButton;
        [SerializeField] private Sprite _baseDoubleSpeedButtonSprite;
        [SerializeField] private Sprite _highlightedDoubleSpeedButtonSprite;
        
        [SerializeField] private UnityEngine.UI.Image _fourthSpeedButton;
        [SerializeField] private Sprite _baseFourthSpeedButtonSprite;
        [SerializeField] private Sprite _highlightedFourthSpeedButtonSprite;
        
        void Start()
        {
            _gameTime = ServiceLocator.GetService<IGameTime>();
            if (_gameTime == null)
            {
                Debug.LogError("GameTime service not found!");
            }
        }

        public void Pause()
        {
            _gameTime?.Pause();
            _pauseButton.sprite = _highlightedPauseButtonSprite;
            
            _baseSpeedButton.sprite = _baseBaseSpeedButtonSprite;
            _doubleSpeedButton.sprite = _baseDoubleSpeedButtonSprite;
            _fourthSpeedButton.sprite = _baseFourthSpeedButtonSprite;
        }
            
        
        public void PlayX1()
        {
            _gameTime?.SetSpeed(1f);
            _baseSpeedButton.sprite = _hightlightedBaseButtonSprite;
            
            _pauseButton.sprite = _basePauseButtonSprite;
            _doubleSpeedButton.sprite = _baseDoubleSpeedButtonSprite;
            _fourthSpeedButton.sprite = _baseFourthSpeedButtonSprite;
        }
        
        
        public void PlayX2()
        {
            _gameTime?.SetSpeed(2f);
            _doubleSpeedButton.sprite = _highlightedDoubleSpeedButtonSprite;
            
            _pauseButton.sprite = _basePauseButtonSprite;
            _baseSpeedButton.sprite = _baseBaseSpeedButtonSprite;
            _fourthSpeedButton.sprite = _baseFourthSpeedButtonSprite;
            
        }
        
        
        public void PlayX4()
        {
            _gameTime?.SetSpeed(4f);
            _fourthSpeedButton.sprite = _highlightedFourthSpeedButtonSprite;
            
            _pauseButton.sprite = _basePauseButtonSprite;
            _baseSpeedButton.sprite = _baseBaseSpeedButtonSprite;
            _doubleSpeedButton.sprite = _baseDoubleSpeedButtonSprite;
        }
    }
}