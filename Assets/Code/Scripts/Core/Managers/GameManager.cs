using System;
using System.Linq;
using Code.Scripts.Core.Systems.Astrarium;
using Code.Scripts.Core.Systems.Construction;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Patterns.Factory;
using Code.Scripts.Patterns.State.Interfaces;
using Code.Scripts.UI.Common;
using Code.Scripts.UI.Menus.States.GameStates;
using Code.Scripts.UI.Menus.States.GameStates.InGameSubStates;
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
            InitializeStates();
        }

        private void InitializeStates()
        {
            
            // Factory para los subestados de InGame
            var actionPanelStateFactory = new GameStateFactory();
            
            actionPanelStateFactory.Register<DefaultState>(m => new DefaultState(m));
            actionPanelStateFactory.RegisterUI<AstrariumSubState, AstrariumUI>(_uiRepository.GetUI<AstrariumUI>(), (manager, ui) => new AstrariumSubState(manager, ui));
            actionPanelStateFactory.RegisterUI<ConstructionSubState, ConstructionUI>(_uiRepository.GetUI<ConstructionUI>(), (manager, ui) => new ConstructionSubState(manager, ui));
            actionPanelStateFactory.RegisterUI<StorageSubState, StorageScreen>(_uiRepository.GetUI<StorageScreen>(), (manager, ui) => new StorageSubState(manager, ui));
            actionPanelStateFactory.RegisterUI<CivilizationSubState, CivilizationScreen>(_uiRepository.GetUI<CivilizationScreen>(), (manager, ui) => new CivilizationSubState(manager, ui));
            
            _stateFactory = new GameStateFactory();
            
            _stateFactory.RegisterUI<MainMenuState, MainMenuScreen>(_uiRepository.GetUI<MainMenuScreen>(), (manager, ui) => new MainMenuState(manager, ui));
            _stateFactory.RegisterUI<OptionsState, OptionsScreen>(_uiRepository.GetUI<OptionsScreen>(), (manager, screen) => new OptionsState(manager, screen));
            _stateFactory.RegisterUI<InGameState, InGameScreen>(_uiRepository.GetUI<InGameScreen>(), (manager, screen) => new InGameState(manager, screen));
            _stateFactory.RegisterUI<ActionPanelState, ActionPanelScreen>(_uiRepository.GetUI<ActionPanelScreen>(), (manager, screen) => new ActionPanelState(manager, screen, actionPanelStateFactory));
            
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