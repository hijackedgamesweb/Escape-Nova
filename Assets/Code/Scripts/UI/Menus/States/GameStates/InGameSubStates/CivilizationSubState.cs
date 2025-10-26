using Code.Scripts.Core.Systems.Astrarium;
using Code.Scripts.Patterns.State.Interfaces;
using Code.Scripts.UI.Windows;

namespace Code.Scripts.UI.Menus.States.GameStates.InGameSubStates
{
    public class CivilizationSubState : AState
    {
        private CivilizationScreen _objectUI;
        
        public CivilizationSubState(IStateManager stateManager, BaseUIScreen uiObject) : base(stateManager)
        {
            _objectUI = uiObject as CivilizationScreen;
            
        }

        public override void Enter(IStateManager gameManager)
        {
            _objectUI.Show();
        }

        public override void Exit(IStateManager gameManager)
        {
            _objectUI.Hide();
        }

        public override void Update()
        {
            
        }
    }
}