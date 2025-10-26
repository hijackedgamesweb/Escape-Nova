using Code.Scripts.Core.Managers;
using Code.Scripts.Patterns.State.Interfaces;
using Code.Scripts.UI.Windows;
using UnityEngine.UI;

namespace Code.Scripts.UI.Menus.States.GameStates
{
    public class MainMenuState : AState
    {
        MainMenuScreen _mainMenuScreen;
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