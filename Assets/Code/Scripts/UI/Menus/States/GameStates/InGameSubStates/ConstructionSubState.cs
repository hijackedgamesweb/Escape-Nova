using Code.Scripts.Core.Systems.Astrarium;
using Code.Scripts.Core.Systems.Construction;
using Code.Scripts.Patterns.State.Interfaces;
using Code.Scripts.UI.Windows;

namespace Code.Scripts.UI.Menus.States.GameStates.InGameSubStates
{
    public class ConstructionSubState : AState
    {
        private ConstructionUI _objectUI;
        
        public ConstructionSubState(IStateManager stateManager, BaseUIScreen uiObject) : base(stateManager)
        {
            _objectUI = uiObject as ConstructionUI;
        }

        public override void Enter(IStateManager gameManager)
        {
            _objectUI.exitBtn.onClick.AddListener(() => gameManager.SetState<DefaultState>());
            _objectUI.Show();
        }

        public override void Exit(IStateManager gameManager)
        {
            _objectUI.Hide();
            _objectUI.exitBtn.onClick.RemoveAllListeners();
        }

        public override void Update()
        {
            
        }
    }
}