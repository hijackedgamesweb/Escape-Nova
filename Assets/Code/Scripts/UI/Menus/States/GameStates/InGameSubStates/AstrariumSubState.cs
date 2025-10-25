using Code.Scripts.Core.Systems.Astrarium;
using Code.Scripts.Patterns.State.Interfaces;
using Code.Scripts.UI.Windows;

namespace Code.Scripts.UI.Menus.States.GameStates.InGameSubStates
{
    public class AstrariumSubState : AState
    {
        private AstrariumUI _objectUI;
        
        public AstrariumSubState(IStateManager stateManager, BaseUIScreen uiObject) : base(stateManager)
        {
            _objectUI = uiObject as AstrariumUI;
            
        }

        public override void Enter(IStateManager gameManager)
        {
            _objectUI.exitButton.onClick.AddListener(() => gameManager.SetState<DefaultState>());
            _objectUI.Show();
        }

        public override void Exit(IStateManager gameManager)
        {
            _objectUI.exitButton.onClick.RemoveAllListeners();
            _objectUI.Hide();
        }

        public override void Update()
        {
            
        }
    }
}