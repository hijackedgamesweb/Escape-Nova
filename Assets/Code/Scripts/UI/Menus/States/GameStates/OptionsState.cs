using Code.Scripts.Core.Managers;
using Code.Scripts.Patterns.State.Interfaces;
using Code.Scripts.UI.Windows;

namespace Code.Scripts.UI.Menus.States.GameStates
{
    public class OptionsState : AState
    {
        OptionsScreen _uiObject;
        private IStateManager _stateManager;
        
        public OptionsState(IStateManager stateManager) : base(stateManager)
        {
            _uiObject = UIManager.Instance.ShowScreen<OptionsScreen>();
            _stateManager = stateManager;
        }

        public override void Enter(IStateManager gameManager)
        {
            _uiObject.gameObject.SetActive(true);
            _uiObject.returnBtn.onClick.AddListener(OnReturnButtonClicked);
        }

        public override void Exit(IStateManager gameManager)
        {
            _uiObject.gameObject.SetActive(false);
            _uiObject.returnBtn.onClick.RemoveListener(OnReturnButtonClicked);
        }

        private void OnReturnButtonClicked()
        {
            _stateManager.SetState(new MainMenuState(_stateManager));
        }

        public override void Update()
        {
        }
    }
}