using System;
using System.Linq;
using Code.Scripts.Patterns.Factory;
using Code.Scripts.Patterns.State.Interfaces;
using Code.Scripts.Patterns.State.States.GameStates;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Scripts.Core.Managers
{
    public class GameManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject mainMenuUI;
        [SerializeField] private GameObject inGameUI;
        
        private IStateManager _stateManager;
        private GameStateFactory _stateFactory;
        
        private void Awake()
        {
            _stateFactory = new GameStateFactory();
            
            _stateFactory.RegisterUI<MainMenuState>(mainMenuUI);
            _stateFactory.RegisterUI<InGameState>(inGameUI);
            
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