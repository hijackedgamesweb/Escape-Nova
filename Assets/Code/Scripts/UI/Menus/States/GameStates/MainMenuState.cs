using Code.Scripts.Patterns.State.Interfaces;
using Code.Scripts.Patterns.State.States.GameStates;
using Code.Scripts.UI.Windows;
using UnityEngine.UI;

namespace Code.Scripts.UI.Menus.States.GameStates
{
    public class MainMenuState : AState
    {

        private readonly MainMenuScreen _uiObject;
        
        public MainMenuState(IStateManager stateManager, BaseUIScreen uiObject) : base(stateManager)
        {
            _uiObject = (MainMenuScreen) uiObject;
        }

        public override void Enter(IStateManager gameManager)
        {
            _uiObject.gameObject.SetActive(true);
            _uiObject.playButton.onClick.AddListener(OnPlayClicked);
            _uiObject.settingsButton.onClick.AddListener(OnSettingsClicked);
            
        }



        public override void Exit(IStateManager gameManager)
        {
            _uiObject.gameObject.SetActive(false);
            _uiObject.playButton.onClick.RemoveListener(OnPlayClicked);
            _uiObject.settingsButton.onClick.RemoveListener(OnSettingsClicked);
            
        }

        public override void Update()
        {
        }
        
        
        private void OnPlayClicked()
        {
            _stateManager.SetState<InGameState>();
        }
        
        private void OnSettingsClicked()
        {
            _stateManager.SetState<OptionsState>();
        }
        
    }
}