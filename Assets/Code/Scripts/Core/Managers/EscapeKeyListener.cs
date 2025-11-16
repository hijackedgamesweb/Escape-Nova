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
        
        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                HandleEscapePress();
            }
            
            //ESTAS LINEAS DE AQUI SON PARA DEJARNOS HERRAMIENTAS QUE PODAMOS USAR PARA DEBUGGEAR EL JUEGO
            
            //F1 PARA GANAR LA PARTIDA INMEDIATAMENTE
            if (Keyboard.current != null && Keyboard.current.f1Key.wasPressedThisFrame)
            {
                StartCoroutine(SecuenciaVictoria());
            }
            
            //F2 PARA AUMENTAR LOS CICLOS
            if (Keyboard.current != null && Keyboard.current.f2Key.wasPressedThisFrame)
            {
                _gameTimeManager.GetComponent<GameTimeManager>().SetCurrentCycle(1000);
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