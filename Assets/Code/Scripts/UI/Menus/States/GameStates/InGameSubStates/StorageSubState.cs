using Code.Scripts.Core.Systems.Construction;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Patterns.State.Interfaces;
using Code.Scripts.UI.Windows;

namespace Code.Scripts.UI.Menus.States.GameStates.InGameSubStates
{
    public class StorageSubState : AState
    {
        private StorageSystemTester _objectUI;
        
        public StorageSubState(IStateManager stateManager, BaseUIScreen uiObject) : base(stateManager)
        {
            _objectUI = uiObject as StorageSystemTester;
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