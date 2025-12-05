using Code.Scripts.Core.Managers;
using Code.Scripts.Core.World;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.Patterns.State.Interfaces;
using Code.Scripts.UI.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.UI.Menus.States.GameStates
{
    public class MainMenuState : AState
    {
        MainMenuScreen _mainMenuScreen;
        OptionsScreen _optionsScreen;
        private IStateManager _stateManager;
        public MainMenuState(IStateManager stateManager) : base(stateManager)
        {
            _mainMenuScreen = UIManager.Instance.ShowScreen<MainMenuScreen>();
            _stateManager = stateManager;
        }

        public override void Enter(IStateManager stateManager)
        {
            _mainMenuScreen.PlayButton.onClick.AddListener(() =>
            {

                _stateManager.SetState(new InGameState(_stateManager));
                AudioManager.Instance.StopMusic(); //Llama al AudioMannager y detienne la musica del menu principal

            });
            
            _mainMenuScreen.SettingsButton.onClick.AddListener(() =>
            {
                AudioManager.Instance.PlaySFX("MainMenuButtonPressed");
                _stateManager.SetState(new OptionsState(_stateManager));
            });
        }



        public override void Exit(IStateManager gameManager)
        {
            
        }

        public override void Update()
        {
        }
        
        
    }
}