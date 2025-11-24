using System.Collections;
using Code.Scripts.Camera;
using Code.Scripts.Core.Events;
using Code.Scripts.UI.Windows;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Code.Scripts.Core.Managers
{
    public class KeyListener : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private GameObject _gameTimeManager;
        
        [Header("Input Configuration")]
        [SerializeField] private InputActionAsset inputActionAsset;
        [SerializeField] private string actionMapName = "Shortcuts";

        // Actions
        private InputAction _toggleMenuAction;
        private InputAction _pauseTimeAction;
        private InputAction _speed1Action;
        private InputAction _speed2Action;
        private InputAction _speed3Action;
        
        // Debug Actions
        private InputAction _debugWinAction;
        private InputAction _debugCycleAction;

        private GameTimeManager _timeManagerComponent;

        private void Awake()
        {
            // 1. Inicializar las acciones
            var map = inputActionAsset.FindActionMap(actionMapName);

            if (map == null)
            {
                Debug.LogError($"[EscapeKeyListener] No se encontró el Action Map: {actionMapName}");
                return;
            }

            _toggleMenuAction = map.FindAction("ToggleMenu");
            _pauseTimeAction = map.FindAction("TimePause");
            _speed1Action = map.FindAction("TimeSpeed1");
            _speed2Action = map.FindAction("TimeSpeed2");
            _speed3Action = map.FindAction("TimeSpeed3");
            
            _debugWinAction = map.FindAction("DebugWin");
            _debugCycleAction = map.FindAction("DebugSkip");
        }

        private void OnEnable()
        {
            _toggleMenuAction?.Enable();
            _pauseTimeAction?.Enable();
            _speed1Action?.Enable();
            _speed2Action?.Enable();
            _speed3Action?.Enable();
            _debugWinAction?.Enable();
            _debugCycleAction?.Enable();
        }

        private void OnDisable()
        {
            _toggleMenuAction?.Disable();
            _pauseTimeAction?.Disable();
            _speed1Action?.Disable();
            _speed2Action?.Disable();
            _speed3Action?.Disable();
            _debugWinAction?.Disable();
            _debugCycleAction?.Disable();
        }

        private void Start()
        {
            if (_gameTimeManager != null)
            {
                _timeManagerComponent = _gameTimeManager.GetComponent<GameTimeManager>();
            }
        }

        private void Update()
        {
            
            if (_toggleMenuAction != null && _toggleMenuAction.WasPerformedThisFrame())
            {
                HandleTabPress();
            }

            if (_pauseTimeAction != null && _pauseTimeAction.WasPerformedThisFrame()) ChangeGameSpeed(0f);
            if (_speed1Action != null && _speed1Action.WasPerformedThisFrame()) ChangeGameSpeed(1f);
            if (_speed2Action != null && _speed2Action.WasPerformedThisFrame()) ChangeGameSpeed(2f);
            if (_speed3Action != null && _speed3Action.WasPerformedThisFrame()) ChangeGameSpeed(4f);
            
            if (_debugWinAction != null && _debugWinAction.WasPerformedThisFrame())
            {
                StartCoroutine(SecuenciaVictoria());
            }
            if (_debugCycleAction != null && _debugCycleAction.WasPerformedThisFrame())
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