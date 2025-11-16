using Code.Scripts.Core.Managers;
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
                AudioManager.Instance.PlayMusic("GameMusic"); //Llama al Audiomanager y comienza la musica del juego
                AudioManager.Instance.PlaySFX("MainMenuButtonPressed");
                GameInfoPanel.Instance.ShowPanel("Welcome", "Welcome to NOVA. We'll guide you through the basics of the game." +
                                                            "First, take a look at the 'Missions' menu, on the lower side fo your screen.");
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