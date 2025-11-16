using Code.Scripts.UI.Windows;
using UnityEngine;
using UnityEngine.InputSystem;
using Code.Scripts.Core.Events;
using Code.Scripts.Camera;

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
            if (currentScreen is ActionPanelScreen || currentScreen is PerfectViewScreen)
            {
                UIManager.Instance.ShowScreen<InGameScreen>();
                var mainCamera = UnityEngine.Camera.main;
                if (mainCamera != null && mainCamera.TryGetComponent<CameraController2D>(out var cameraController))
                {
                    cameraController.ClearTarget(); 
                    cameraController.ResetDragState(); 
                }
            }
            else if (currentScreen is InGameScreen)
            {
                SystemEvents.RequestMainMenu();
            }
        }
    }
}