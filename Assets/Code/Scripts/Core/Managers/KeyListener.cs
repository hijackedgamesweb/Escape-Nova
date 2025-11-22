using System.Collections;
using Code.Scripts.UI.Windows;
using UnityEngine;
using UnityEngine.InputSystem;
using Code.Scripts.Core.Events;
using Code.Scripts.Camera;
using Code.Scripts.Core.Systems.Research;
using Code.Scripts.Patterns.ServiceLocator;
using UnityEngine.SceneManagement;

namespace Code.Scripts.Core.Managers
{
    public class EscapeKeyListener : MonoBehaviour
    {
        [SerializeField] private GameObject _gameTimeManager;
        
        private GameTimeManager _timeManagerComponent;

        private void Start()
        {
            if (_gameTimeManager != null)
            {
                _timeManagerComponent = _gameTimeManager.GetComponent<GameTimeManager>();
            }
        }

        private void Update()
        {
            if (Keyboard.current == null) return;
            if (Keyboard.current.tabKey.wasPressedThisFrame)
            {
                HandleTabPress();
            }
            
            //mientras pulsas el shift (mantienes pulsado)
            if (Keyboard.current.shiftKey.isPressed)
            {
                //al pulsar la tecla 0, 1, 2 o 3 (pausa, veloicdad 1, 2 o 4)
                if (Keyboard.current.digit0Key.wasPressedThisFrame) ChangeGameSpeed(0f);
                if (Keyboard.current.digit1Key.wasPressedThisFrame) ChangeGameSpeed(1f);
                if (Keyboard.current.digit2Key.wasPressedThisFrame) ChangeGameSpeed(2f);
                if (Keyboard.current.digit4Key.wasPressedThisFrame) ChangeGameSpeed(4f);
            }
            
            if (Keyboard.current.f1Key.wasPressedThisFrame)
            {
                StartCoroutine(SecuenciaVictoria());
            }
            
            if (Keyboard.current.f2Key.wasPressedThisFrame)
            {
                if (_timeManagerComponent != null)
                {
                    _timeManagerComponent.SetCurrentCycle(1000);
                }
            }
        }
        
        private void ChangeGameSpeed(float newSpeed)
        {
            if (_timeManagerComponent != null)
            {
                _timeManagerComponent.SetSpeed(newSpeed);
            }
        }
        
        private IEnumerator SecuenciaVictoria()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.StopMusic();
            }
            SceneManager.LoadScene("CreditsScene");
            yield return null;
        }
        
        private void HandleTabPress()
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