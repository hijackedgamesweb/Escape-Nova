using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.InputSystem;

namespace Code.Scripts.UI.Windows
{
    public class MainMenuScreen : BaseUIScreen
    {
        [SerializeField] public Button PlayButton;
        [SerializeField] public Button SettingsButton;
        [SerializeField] public Button ExitButton;

        private GameObject _lastSelectedButton;

        private void OnEnable()
        {
            StartCoroutine(SetInitialFocus());
        }
        private IEnumerator SetInitialFocus()
        {
            yield return null; 
            
            EventSystem.current.SetSelectedGameObject(null);
            
            if (PlayButton != null)
            {
                _lastSelectedButton = PlayButton.gameObject;
                EventSystem.current.SetSelectedGameObject(_lastSelectedButton);
            }
        }
        public void SetLastSelectedButton(GameObject button)
        {
            _lastSelectedButton = button;
        }

        private void Update()
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                if (WasNavigationKeyPressed())
                {
                    EventSystem.current.SetSelectedGameObject(_lastSelectedButton);
                }
            }
        }
        private bool WasNavigationKeyPressed()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return false;

            return keyboard.upArrowKey.wasPressedThisFrame ||
                   keyboard.downArrowKey.wasPressedThisFrame ||
                   keyboard.leftArrowKey.wasPressedThisFrame ||
                   keyboard.rightArrowKey.wasPressedThisFrame;
        }
    }
}