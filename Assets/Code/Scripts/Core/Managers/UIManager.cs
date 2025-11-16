using System;
using System.Collections.Generic;
using Code.Scripts.Patterns.Singleton;
using Code.Scripts.UI.Windows;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.Scripts.Core.Managers
{
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField] private List<BaseUIScreen> screens;
        private BaseUIScreen _currentScreen;
        
        public static event Action OnScreenChanged;
        
        public void Awake()
        {
            base.Awake();
            screens = new List<BaseUIScreen>(GetComponentsInChildren<BaseUIScreen>(true));
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
    }
}