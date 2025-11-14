using Code.Scripts.Core.Managers;
using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Patterns.Factory;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.Patterns.State.Interfaces;
using Code.Scripts.UI.Windows;

namespace Code.Scripts.UI.Menus.States.GameStates
{
    public class InGameState : AState
    {
        private InGameScreen _uiObject;
        private IGameTime _gameTimeManager;
        
        public InGameState(IStateManager stateManager) : base(stateManager)
        {
            _uiObject = UIManager.Instance.ShowScreen<InGameScreen>();
        }

        public override void Enter(IStateManager gameManager)
        {
            InitializeHUD();
            _uiObject.gameObject.SetActive(true);
            _gameTimeManager = ServiceLocator.GetService<IGameTime>();
            _gameTimeManager.StartTimer();
        }

        private void InitializeHUD()
        {
            _uiObject.returnBtn.onClick.AddListener(() => _stateManager.SetState(new MainMenuState(_stateManager)));
        }

        public override void Exit(IStateManager gameManager)
        {
            _uiObject.gameObject.SetActive(false);
            _uiObject.returnBtn.onClick.RemoveAllListeners();
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlayMusic("MainMenuMusic");
        }

        public override void Update()
        {
        }
        
        
    }
}