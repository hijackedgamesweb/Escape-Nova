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
        private IGameTime _gameTimeManager;
        
        public InGameState(IStateManager stateManager, BaseUIScreen uiObject) : base(stateManager)
        {
            _uiObject = (InGameScreen) uiObject;
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
            _uiObject.astrariumBtn.onClick.AddListener(() => _stateManager.SetState<ActionPanelState>(typeof(AstrariumSubState)));
            _uiObject.constructionBtn.onClick.AddListener(() => _stateManager.SetState<ActionPanelState>(typeof(ConstructionSubState)));
            _uiObject.storageBtn.onClick.AddListener(() => _stateManager.SetState<ActionPanelState>(typeof(StorageSubState)));
            _uiObject.diplomacyBtn.onClick.AddListener(() => _stateManager.SetState<ActionPanelState>(typeof(CivilizationSubState)));
            _uiObject.returnBtn.onClick.AddListener(() => _stateManager.SetState<MainMenuState>());
        }

        public override void Exit(IStateManager gameManager)
        {
            _uiObject.gameObject.SetActive(false);
            _uiObject.returnBtn.onClick.RemoveAllListeners();
            _uiObject.storageBtn.onClick.RemoveAllListeners();
            _uiObject.constructionBtn.onClick.RemoveAllListeners();
            _uiObject.astrariumBtn.onClick.RemoveAllListeners();
            _uiObject.diplomacyBtn.onClick.RemoveAllListeners();
        }

        public override void Update()
        {
        }
        
        
    }
}