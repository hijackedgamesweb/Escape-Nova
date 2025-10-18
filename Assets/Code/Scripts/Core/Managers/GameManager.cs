using System;
using System.Linq;
using Code.Scripts.Patterns.Factory;
using Code.Scripts.Patterns.State.Interfaces;
using Code.Scripts.Patterns.State.States.GameStates;
using Code.Scripts.UI.Common;
using Code.Scripts.UI.Menus.States.GameStates;
using Code.Scripts.UI.Windows;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Scripts.Core.Managers
{
    public class GameManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private UIRepository _uiRepository;
        
        private IStateManager _stateManager;
        private GameStateFactory _stateFactory;
        
        private void Awake()
        {
            _stateFactory = new GameStateFactory();
            
            _stateFactory.RegisterUI<MainMenuState>(_uiRepository.GetUI<MainMenuScreen>());
            _stateFactory.RegisterUI<InGameState>(_uiRepository.GetUI<InGameScreen>());
            _stateFactory.RegisterUI<OptionsState>(_uiRepository.GetUI<OptionsScreen>());
            
            _stateManager = new StateManager(_stateFactory);
        }
        
        private void Start()
        {
            _stateManager.SetState<MainMenuState>();
        }
        
        private void Update()
        {
            _stateManager.GetCurrentState()?.Update();
        }
    }
}