using System;
using System.Linq;
using Code.Scripts.Core.Systems.Astrarium;
using Code.Scripts.Core.Systems.Construction;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Patterns.Factory;
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
        private GameStateFactory _stateFactory;
        
        private void Awake()
        {
            InitializeStates();
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
    }
}