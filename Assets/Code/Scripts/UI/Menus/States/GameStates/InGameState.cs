using Code.Scripts.Core.Managers;
using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Patterns.Factory;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.Patterns.State.Interfaces;
using Code.Scripts.UI.Menus.States.GameStates.InGameSubStates;
using Code.Scripts.UI.Windows;

namespace Code.Scripts.UI.Menus.States.GameStates
{
    public class InGameState : AState
    {
        private InGameScreen _uiObject;
        private IStateManager _subStateManager;
        private IGameTime _gameTimeManager;
        
        public InGameState(IStateManager stateManager, BaseUIScreen uiObject, GameStateFactory _subStatesFactory) : base(stateManager)
        {
            _uiObject = (InGameScreen) uiObject;
            _subStateManager = new StateManager(_subStatesFactory);
        }

        public override void Enter(IStateManager gameManager)
        {
            InitializeHUD();
            _uiObject.gameObject.SetActive(true);
            _subStateManager.SetState<DefaultState>();
            _gameTimeManager = ServiceLocator.GetService<IGameTime>();
            _gameTimeManager.StartTimer();
        }

        private void InitializeHUD()
        {
            _uiObject.astrariumBtn.onClick.AddListener(() => _subStateManager.SetState<AstrariumSubState>());
            _uiObject.constructionBtn.onClick.AddListener(() => _subStateManager.SetState<ConstructionSubState>());
            _uiObject.storageBtn.onClick.AddListener(() => _subStateManager.SetState<StorageSubState>());
            _uiObject.returnBtn.onClick.AddListener(() => _stateManager.SetState<MainMenuState>());
        }

        public override void Exit(IStateManager gameManager)
        {
            _uiObject.gameObject.SetActive(false);
            _uiObject.returnBtn.onClick.RemoveAllListeners();
            _uiObject.storageBtn.onClick.RemoveAllListeners();
            _uiObject.constructionBtn.onClick.RemoveAllListeners();
        }

        public override void Update()
        {
        }
        
        
    }
}