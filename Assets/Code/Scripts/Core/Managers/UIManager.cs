using System;
using System.Collections.Generic;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.Patterns.Singleton;
using Code.Scripts.UI;
using Code.Scripts.UI.Windows;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.Scripts.Core.Managers
{
    public class UIManager : Singleton<UIManager>
    {
        [Header("Referencias de Pantallas")]
        [SerializeField] private GameOverAnimationController gameOverScreen;
        [SerializeField] private List<BaseUIScreen> screens;
        private BaseUIScreen _currentScreen;
        
        public static event Action OnScreenChanged;
        
        public void Awake()
        {
            ServiceLocator.RegisterService<UIManager>(this);
            base.Awake();
            screens = new List<BaseUIScreen>(GetComponentsInChildren<BaseUIScreen>(true));
        }
        
        private void OnDestroy()
        {
            ServiceLocator.UnregisterService<UIManager>();
        }
        
        public T ShowScreen<T>(object parameter = null) where T : BaseUIScreen
        {
            if (_currentScreen != null)
            {
                _currentScreen.Hide();
            }
            ClearUIFocus();
            
            _currentScreen = screens.Find(screen => screen is T);
            _currentScreen.Show(parameter);
            
            OnScreenChanged?.Invoke();
            
            return _currentScreen as T;
        }
        
        public T ShowOverlay<T>(object parameter = null) where T : BaseUIScreen
        {
            ClearUIFocus();
            
            BaseUIScreen overlay = screens.Find(screen => screen is T);
            if (overlay != null)
            {
                overlay.Show(parameter);
            }
            return overlay as T;
        }
        
        public BaseUIScreen GetCurrentScreen()
        {
            return _currentScreen;
        }
        
        public void ClearUIFocus()
        {
            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
        
        public void ShowGameOverScreen()
        {
            if (gameOverScreen != null)
            {
                gameOverScreen.gameObject.SetActive(true);
                gameOverScreen.StartGameOverSequence();
            }
            else
            {
            }
        }
        public void HideGameOverScreen()
        {
            if (gameOverScreen != null)
            {
                gameOverScreen.gameObject.SetActive(false);
            }
        }
    }
}