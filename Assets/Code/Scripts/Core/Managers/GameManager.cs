using System;
using System.Linq;
using Code.Scripts.Core.Events;
using Code.Scripts.Core.Systems.Astrarium;
using Code.Scripts.Core.Systems.Construction;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Patterns.Factory;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.Patterns.State.Interfaces;
using Code.Scripts.UI.Menus.States.GameStates;
using Code.Scripts.UI.Windows;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Scripts.Core.Managers
{
    public class GameManager : MonoBehaviour
    {
        private IStateManager _stateManager;
        
        private void Awake()
        {
            InitializeStates();
            ServiceLocator.RegisterService(this);
            SystemEvents.OnRequestMainMenu += HandleRequestMainMenu;
        }
        
        private void OnDestroy()
        {
            SystemEvents.OnRequestMainMenu -= HandleRequestMainMenu;
            SystemEvents.OnGameOver -= HandleGameOver;
        }
        
        public IStateManager GetStateManager()
        {
            return _stateManager;
        }
        
        private void InitializeStates()
        {
            _stateManager = new StateManager();
        }

        private void Start()
        {
            _stateManager.SetState(new MainMenuState(_stateManager));
        }
        
        private void Update()
        {
            _stateManager.GetCurrentState()?.Update();
        }
        
        private void HandleRequestMainMenu()
        {
            _stateManager.SetState(new MainMenuState(_stateManager));
        }
        
        private void HandleGameOver()
        {
            _stateManager.SetState(new GameOverState(_stateManager));
        }
    }
}