using Code.Scripts.UI.Windows;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Scripts.Core.Managers
{
    public class EscapeKeyListener : MonoBehaviour
    {
        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                HandleEscapePress();
            }
        }

        private void HandleEscapePress()
        {
            var currentScreen = UIManager.Instance.GetCurrentScreen();
            if (currentScreen is ActionPanelScreen)
            {
                UIManager.Instance.ShowScreen<InGameScreen>();
            }
            else if (currentScreen is PerfectViewScreen)
            {
                UIManager.Instance.ShowScreen<InGameScreen>();
            }
            else if (currentScreen is ActionPanelScreen || currentScreen is PerfectViewScreen)
            {
                UIManager.Instance.ShowScreen<InGameScreen>();
            }
        }
    }
}